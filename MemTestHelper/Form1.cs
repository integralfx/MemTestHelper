using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemTestHelper
{
    public partial class Form1 : Form
    {
        private readonly int NUM_THREADS, MAX_THREADS;

        private const string MEMTEST_EXE = "memtest.exe",
                             MEMTEST_CLASSNAME = "#32770",
                             MEMTEST_BTN_START = "Button1",
                             MEMTEST_BTN_STOP = "Button2",
                             MEMTEST_EDT_RAM = "Edit1",
                             MEMTEST_STATIC_COVERAGE = "Static1",
                             // If you find this free version useful...
                             MEMTEST_STATIC_FREE_VER = "Static2",
                             CFG_FILENAME = "MemTestHelper.cfg";

        private const int MEMTEST_WIDTH = 217,
                          MEMTEST_HEIGHT = 247,
                          MEMTEST_MAX_RAM = 2048,
                          // interval (in ms) for coverage info list
                          UPDATE_INTERVAL = 200;

        private MemTest[] memtests;
        private BackgroundWorker coverageWorker;
        private DateTime startTime;
        private System.Timers.Timer timer;
        private bool isMinimised = true;

        public Form1()
        {
            InitializeComponent();

            NUM_THREADS = Convert.ToInt32(Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
            MAX_THREADS = NUM_THREADS * 4;
            memtests = new MemTest[MAX_THREADS];

            InitCboThreads();
            InitLstCoverage();
            InitCboRows();
            CentreXYOffsets();

            coverageWorker = new BackgroundWorker();
            coverageWorker.WorkerSupportsCancellation = true;
            coverageWorker.DoWork += new DoWorkEventHandler((sender, e) =>
            {
                var worker = sender as BackgroundWorker;
                while (!worker.CancellationPending)
                {
                    UpdateCoverageInfo();
                    Thread.Sleep(UPDATE_INTERVAL);
                }

                e.Cancel = true;
            });
            coverageWorker.RunWorkerCompleted += 
            new RunWorkerCompletedEventHandler((sender, e) =>
            {
                // wait for all MemTests to stop completely
                while (IsAnyMemTestStopping())
                    Thread.Sleep(100);

                // TODO: figure out why total coverage is sometimes
                // reporting as 0.0 after stopping
                UpdateCoverageInfo(false);
            });

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) =>
            {
                Invoke(new MethodInvoker(delegate
                {
                    var threads = (int)cboThreads.SelectedItem;
                    var elapsed = e.SignalTime - startTime;

                    lblElapsedTime.Text = String.Format("{0:00}h{1:00}m{2:00}s",
                                                        (int)(elapsed.TotalHours),
                                                        elapsed.Minutes,
                                                        elapsed.Seconds);

                    var total_coverage = 0.0;
                    for (var i = 1; i <= threads; i++)
                    {
                        var memtest = memtests[i - 1];
                        var info = memtest.GetCoverageInfo();
                        if (info == null) return;

                        // For some reason, this sometimes won't close.
                        memtest.ClickNagMessageBox("Message for first-time users", 1);

                        memtest.ClickNagMessageBox("Memory error detected!", 1);

                        total_coverage += info.Item1;
                    }

                    // round up to next multiple of 100
                    var cov = ((int)(total_coverage / 100) + 1) * 100;
                    var diff = elapsed.TotalMilliseconds;
                    var est = (diff / total_coverage * cov) - diff;

                    TimeSpan estimatedTime = TimeSpan.FromMilliseconds(est);
                    lblEstimatedTime.Text = String.Format("{0:00}h{1:00}m{2:00}s to {3}%",
                                                          (int)(estimatedTime.TotalHours),
                                                          estimatedTime.Minutes,
                                                          estimatedTime.Seconds,
                                                          cov);

                    var ram = Convert.ToInt32(txtRAM.Text);
                    var speed = (total_coverage / 100) * ram / (diff / 1000);
                    lblSpeedValue.Text = $"{speed:f2}MB/s";
                }));
            });
        }

        // Event Handling //

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadConfig();

            UpdateFormHeight();
            UpdateLstCoverageItems();
            cboRows.Items.Clear();
            InitCboRows();
            CentreXYOffsets();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseMemTests();
            SaveConfig();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            var threads = (int)cboThreads.SelectedItem;
            switch (WindowState)
            {
                // minimise MemTest instances
                case FormWindowState.Minimized:
                    RunInBackground(new MethodInvoker(delegate
                    {
                        for (var i = 0; i < threads; i++)
                        {
                            if (memtests[i] != null)
                            {
                                memtests[i].Minimised = true;
                                Thread.Sleep(10);
                            }
                        }
                    }));
                    break;

                // restore previous state of MemTest instances
                case FormWindowState.Normal:
                    RunInBackground(new MethodInvoker(delegate
                    {
                        /*
                         * isMinimised is true when user clicked the hide button.
                         * This means that the memtest instances should be kept minimised.
                         */
                        if (!isMinimised)
                        {
                            for (var i = 0; i < threads; i++)
                            {
                                if (memtests[i] != null)
                                {
                                    memtests[i].Minimised = false;
                                    Thread.Sleep(10);
                                }
                            }

                            // user may have changed offsets while minimised
                            LayOutMemTests();

                            // hack to bring form to top
                            TopMost = true;
                            Thread.Sleep(10);
                            TopMost = false;
                        }
                    }));
                    break;
            }

            // update the height
            if (Size.Height >= MinimumSize.Height && Size.Height <= MaximumSize.Height)
                udWinHeight.Value = Size.Height;
        }

        private void btnAutoRam_Click(object sender, EventArgs e)
        {
            txtRAM.Text = GetFreeRAM().ToString();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (!File.Exists(MEMTEST_EXE))
            {
                MessageBox.Show(MEMTEST_EXE + " not found");
                return;
            }

            if (!ValidateInput()) return;

            btnAutoRAM.Enabled = false;
            txtRAM.Enabled = false;
            cboThreads.Enabled = false;
            //cbo_rows.Enabled = false;
            btnRun.Enabled = false;
            btnStop.Enabled = true;
            chkStopAt.Enabled = false;
            txtStopAt.Enabled = false;
            chkStopAtTotal.Enabled = false;
            chkStopOnError.Enabled = false;
            chkStartMin.Enabled = false;

            // run in background as start_memtests can block
            RunInBackground(new MethodInvoker(delegate
            {
                StartMemTests();

                if (!coverageWorker.IsBusy)
                    coverageWorker.RunWorkerAsync();
                startTime = DateTime.Now;
                timer.Start();

                Activate();
            }));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Parallel.For(0, (int)cboThreads.SelectedItem, i =>
            {
                if (!memtests[i].Finished)
                    memtests[i].Stop();
            });

            coverageWorker.CancelAsync();
            timer.Stop();

            btnAutoRAM.Enabled = true;
            txtRAM.Enabled = true;
            cboThreads.Enabled = true;
            btnRun.Enabled = true;
            btnStop.Enabled = false;
            chkStopAt.Enabled = true;
            if (chkStopAt.Checked)
            {
                txtStopAt.Enabled = true;
                chkStopAtTotal.Enabled = true;
            }
            chkStopOnError.Enabled = true;
            chkStartMin.Enabled = true;

            // wait for all memtests to fully stop
            while (IsAnyMemTestStopping())
                Thread.Sleep(100);

            MessageBox.Show("Please press Ok to update coverage and errors", "MemTest finished");
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            // Run in background as Thread.Sleep can lockup the GUI.
            var threads = (int)cboThreads.SelectedItem;
            RunInBackground(new MethodInvoker(delegate
            {
                for (var i = 0; i < threads; i++)
                {
                    var memtest = memtests[i];
                    if (memtest != null)
                    {
                        memtest.Minimised = false;

                        Thread.Sleep(10);
                    }
                }

                isMinimised = false;

                // User may have changed offsets while minimised.
                LayOutMemTests();

                Activate();
            }));
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            var threads = (int)cboThreads.SelectedItem;
            RunInBackground(new MethodInvoker(delegate
            {
                for (var i = 0; i < threads; i++)
                {
                    var memtest = memtests[i];
                    if (memtest != null)
                    {
                        memtest.Minimised = true;
                        Thread.Sleep(10);
                    }
                }

                isMinimised = true;
            }));
        }

        private void offset_Changed(object sender, EventArgs e)
        {
            RunInBackground(new MethodInvoker(delegate { LayOutMemTests(); }));
        }

        private void btnCenter_Click(object sender, EventArgs e)
        {
            CentreXYOffsets();
        }

        private void cboRows_SelectionChangeCommitted(object sender, EventArgs e)
        {
            CentreXYOffsets();
        }

        private void cboThreads_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateLstCoverageItems();

            cboRows.Items.Clear();
            InitCboRows();
            CentreXYOffsets();
        }

        private void chkStopAt_CheckedChanged(object sender, EventArgs e)
        {
            if (chkStopAt.Checked)
            {
                txtStopAt.Enabled = true;
                chkStopAtTotal.Enabled = true;
            }
            else
            {
                txtStopAt.Enabled = false;
                chkStopAtTotal.Enabled = false;
            }
        }

        private void udWinHeight_ValueChanged(object sender, EventArgs e)
        {
            UpdateFormHeight();
        }

        // Helper Functions //

        // Returns free RAM in MB.
        private ulong GetFreeRAM()
        {
            /*
             * Available RAM = Free + Standby
             * https://superuser.com/a/1032481
             * 
             * Cached = sum of stuff
             * https://www.reddit.com/r/PowerShell/comments/ao59ha/cached_memory_as_it_appears_in_the_performance/efye75r/
             * 
             * Standby = Cached - Modifed
             */
            /*
            float standby = new PerformanceCounter("Memory", "Cache Bytes").NextValue() +
                            //new PerformanceCounter("Memory", "Modified Page List Bytes").NextValue() +
                            new PerformanceCounter("Memory", "Standby Cache Core Bytes").NextValue() +
                            new PerformanceCounter("Memory", "Standby Cache Normal Priority Bytes").NextValue() +
                            new PerformanceCounter("Memory", "Standby Cache Reserve Bytes").NextValue();
            */

            return new ComputerInfo().AvailablePhysicalMemory / (1024 * 1024);
        }

        // TODO: error checking
        private bool LoadConfig()
        {
            string[] validKeys = { "ram", "threads", "x_offset", "y_offset",
                                   "x_spacing", "y_spacing", "rows", "stop_at",
                                   "stop_at_value", "stop_at_total", "stop_on_error",
                                   "start_min", "win_height" };

            try
            {
                string[] lines = File.ReadAllLines(CFG_FILENAME);
                Dictionary<string, int> cfg = new Dictionary<string, int>();

                foreach (string l in lines)
                {
                    var s = l.Split('=');
                    if (s.Length != 2) continue;
                    s[0] = s[0].Trim();
                    s[1] = s[1].Trim();

                    if (validKeys.Contains(s[0]))
                    {
                        // skip blank values
                        if (s[1].Length == 0) continue;

                        int v;
                        if (Int32.TryParse(s[1], out v))
                            cfg.Add(s[0], v);
                        else return false;
                    }
                    else return false;
                }

                // input values in controls
                foreach (KeyValuePair<string, int> kv in cfg)
                {
                    switch (kv.Key)
                    {
                        case "ram":
                            txtRAM.Text = kv.Value.ToString();
                            break;
                        case "threads":
                            cboThreads.SelectedItem = kv.Value;
                            break;

                        case "x_offset":
                            udXOffset.Value = kv.Value;
                            break;
                        case "y_offset":
                            udYOffset.Value = kv.Value;
                            break;

                        case "x_spacing":
                            udXSpacing.Value = kv.Value;
                            break;
                        case "y_spacing":
                            udYSpacing.Value = kv.Value;
                            break;

                        case "stop_at":
                            chkStopAt.Checked = kv.Value != 0;
                            break;
                        case "stop_at_value":
                            txtStopAt.Text = kv.Value.ToString();
                            break;
                        case "stop_at_total":
                            chkStopAtTotal.Checked = kv.Value != 0;
                            break;

                        case "stop_on_error":
                            chkStopOnError.Checked = kv.Value != 0;
                            break;

                        case "start_min":
                            chkStartMin.Checked = kv.Value != 0;
                            break;

                        case "win_height":
                            udWinHeight.Value = kv.Value;
                            break;
                    }
                }
            }
            catch(FileNotFoundException)
            {
                return false;
            }

            return true;
        }

        private bool SaveConfig()
        {
            StreamWriter file = null;
            try {
                file = new StreamWriter(CFG_FILENAME);
                var lines = new List<string>();

                lines.Add($"ram = {txtRAM.Text}");
                lines.Add($"threads = {(int)cboThreads.SelectedItem}");

                lines.Add($"x_offset = {udXOffset.Value}");
                lines.Add($"y_offset = {udYOffset.Value}");
                lines.Add($"x_spacing = {udXSpacing.Value}");
                lines.Add($"y_spacing = {udYSpacing.Value}");
                lines.Add($"rows = {cboRows.SelectedItem}");

                lines.Add(string.Format("stop_at = {0}", chkStopAt.Checked ? 1 : 0));
                lines.Add($"stop_at_value = {txtStopAt.Text}");
                lines.Add(string.Format("stop_at_total = {0}", chkStopAtTotal.Checked ? 1 : 0));
                lines.Add(string.Format("stop_on_error = {0}", chkStopOnError.Checked ? 1 : 0));

                lines.Add(string.Format("start_min = {0}", chkStartMin.Checked ? 1 : 0));

                lines.Add($"win_height = {udWinHeight.Value}");

                foreach (var l in lines)
                    file.WriteLine(l);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                // BaseStream isn't null when open.
                if (file != null && file.BaseStream != null)
                    file.Close();
            }

            return true;
        }

        private void UpdateFormHeight()
        {
            Size = new Size(Size.Width, (int)udWinHeight.Value);
        }

        private bool ValidateInput()
        {
            var ci = new ComputerInfo();
            UInt64 totalRAM = ci.TotalPhysicalMemory / (1024 * 1024),
                   availableRAM = ci.AvailablePhysicalMemory / (1024 * 1024);

            var ramText = txtRAM.Text;
            // automatically input available ram if empty
            if (ramText.Length == 0)
            {
                ramText = GetFreeRAM().ToString();
                txtRAM.Text = ramText;
            }
            else
            {
                if (!ramText.All(char.IsDigit))
                {
                    ShowErrorMsgBox("Amount of RAM must be an integer");
                    return false;
                }
            }

            int threads = (int)cboThreads.SelectedItem,
                ram = Convert.ToInt32(ramText);
            if (ram < threads)
            {
                ShowErrorMsgBox($"Amount of RAM must be greater than {threads}");
                return false;
            }

            if (ram > MEMTEST_MAX_RAM * threads)
            {
                ShowErrorMsgBox(
                    $"Amount of RAM must be at most {MEMTEST_MAX_RAM * threads}\n" + 
                    "Try increasing the number of threads\n" + 
                    "or reducing amount of RAM"
                );
                return false;
            }

            if ((ulong)ram > totalRAM)
            {
                ShowErrorMsgBox($"Amount of RAM exceeds total RAM ({totalRAM})");
                return false;
            }

            if ((ulong)ram > availableRAM)
            {
                var res = MessageBox.Show(
                    $"Amount of RAM exceeds available RAM ({availableRAM})\n" +
                    "This will cause RAM to be paged to your storage,\n" +
                    "which may make MemTest really slow.\n" +
                    "Continue?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (res == DialogResult.No)
                    return false;
            }

            // validate stop at % and error count
            if (chkStopAt.Checked)
            {
                var stopAtText = txtStopAt.Text;

                if (stopAtText == "")
                {
                    ShowErrorMsgBox("Please enter stop at (%)");
                    return false;
                }

                if (!stopAtText.All(char.IsDigit))
                {
                    ShowErrorMsgBox("Stop at (%) must be an integer");
                    return false;
                }

                var stopAt = Convert.ToInt32(stopAtText);
                if (stopAt <= 0)
                {
                    ShowErrorMsgBox("Stop at (%) must be greater than 0");
                    return false;
                }
            }

            return true;
        }

        private void InitLstCoverage()
        {
            // Stop flickering: https://stackoverflow.com/a/15268338
            var method = typeof(ListView).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(lstCoverage, new object[] { ControlStyles.OptimizedDoubleBuffer, true });

            for (var i = 0; i <= (int)cboThreads.SelectedItem; i++)
            {
                string[] row = { i.ToString(), "-", "-" };
                // first row is total
                if (i == 0) row[0] = "T";

                lstCoverage.Items.Add(new ListViewItem(row));
            }
        }

        private void UpdateLstCoverageItems()
        {
            var threads = (int)cboThreads.SelectedItem;
            var items = lstCoverage.Items;
            if (threads < items.Count)
            {
                for (var i = items.Count - 1; i > threads; i--)
                    items.RemoveAt(i);
            }
            else
            {
                for (var i = items.Count; i <= threads; i++)
                {
                    string[] row = { i.ToString(), "-", "-" };
                    lstCoverage.Items.Add(new ListViewItem(row));
                }
            }
        }

        private void InitCboThreads()
        {
            for (var i = 0; i < MAX_THREADS; i++)
                cboThreads.Items.Add(i + 1);

            cboThreads.SelectedItem = NUM_THREADS;
        }

        private void InitCboRows()
        {
            var threads = (int)cboThreads.SelectedItem;

            for (var i = 1; i <= threads; i++)
            {
                if (threads % i == 0)
                    cboRows.Items.Add(i);
            }

            cboRows.SelectedItem = threads % 2 == 0 ? 2 : 1;
        }

        private void CentreXYOffsets()
        {
            var screen = Screen.FromControl(this).Bounds;
            int rows = (int)cboRows.SelectedItem,
                cols = (int)cboThreads.SelectedItem / rows,
                xOffset = (screen.Width - MEMTEST_WIDTH * cols) / 2,
                yOffset = (screen.Height - MEMTEST_HEIGHT * rows) / 2;

            udXOffset.Value = xOffset;
            udYOffset.Value = yOffset;
        }

        private void StartMemTests()
        {
            CloseAllMemTests();

            var threads = (int)cboThreads.SelectedItem;
            Parallel.For(0, threads, i =>
            {
                double ram = Convert.ToDouble(txtRAM.Text) / threads;
                memtests[i] = new MemTest();
                memtests[i].Start(ram, chkStartMin.Checked);
            });

            if (!chkStartMin.Checked)
                LayOutMemTests();
        }

        private void LayOutMemTests()
        {
            int xOffset = (int)udXOffset.Value,
                yOffset = (int)udYOffset.Value,
                xSpacing = (int)udXSpacing.Value - 5,
                ySpacing = (int)udYSpacing.Value - 3,
                rows = (int)cboRows.SelectedItem,
                cols = (int)cboThreads.SelectedItem / rows;

            Parallel.For(0, (int)cboThreads.SelectedItem, i =>
            {
                 var memtest = memtests[i];
                 if (memtest == null || !memtest.Started) return;

                 int r = i / cols,
                     c = i % cols,
                     x = c * MEMTEST_WIDTH + c * xSpacing + xOffset,
                     y = r * MEMTEST_HEIGHT + r * ySpacing + yOffset;

                memtest.Location = new Point(x, y);
            });
        }

        // Only close MemTests started by MemTestHelper.
        private void CloseMemTests()
        {
            Parallel.For(0, (int)cboThreads.SelectedItem, i =>
            {
                try
                {
                    memtests[i].Close();
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to close MemTest #{i}");
                }
            });
        }

        /* 
         * Close all MemTests, regardless of if they were
         * started by MemTestHelper.
         */
        private void CloseAllMemTests()
        {
            // remove the .exe
            var name = MEMTEST_EXE.Substring(0, MEMTEST_EXE.Length - 4);
            var processes = Process.GetProcessesByName(name);
            Parallel.ForEach(processes, p => { p.Kill(); });
        }

        private void UpdateCoverageInfo(bool shouldCheck = true)
        {
            lstCoverage.Invoke(new MethodInvoker(delegate
            {
                var threads = (int)cboThreads.SelectedItem;
                var totalCoverage = 0.0;
                var totalErrors = 0;

                // total is index 0
                for (var i = 1; i <= threads; i++)
                {
                    var memtest = memtests[i - 1];
                    var info = memtest.GetCoverageInfo();
                    if (info == null) continue;
                    double coverage = info.Item1;
                    int errors = info.Item2;

                    lstCoverage.Items[i].SubItems[1].Text = string.Format("{0:f1}", coverage);
                    lstCoverage.Items[i].SubItems[2].Text = errors.ToString();
                        
                    if (shouldCheck)
                    {
                        // check coverage %
                        if (chkStopAt.Checked && !chkStopAtTotal.Checked)
                        {
                            var stopAt = Convert.ToInt32(txtStopAt.Text);
                            if (coverage > stopAt)
                            {
                                if (!memtest.Finished)
                                    memtest.Stop();
                            }
                        }

                        // check error count
                        if (chkStopOnError.Checked)
                        {
                            if (errors > 0)
                            {
                                lstCoverage.Items[i].SubItems[1].ForeColor = Color.Red;
                                ClickBtnStop();
                            }
                        }
                    }

                    totalCoverage += coverage;
                    totalErrors += errors;
                }

                // update the total coverage and errors
                lstCoverage.Items[0].SubItems[1].Text = string.Format("{0:f1}", totalCoverage);
                lstCoverage.Items[0].SubItems[2].Text = totalErrors.ToString();

                if (shouldCheck)
                {
                    // check total coverage
                    if (chkStopAt.Checked && chkStopAtTotal.Checked)
                    {
                        var stopAt = Convert.ToInt32(txtStopAt.Text);
                        if (totalCoverage > stopAt)
                            ClickBtnStop();
                    }

                    if (IsAllFinished())
                        ClickBtnStop();
                }
            }));
        }

        /*
         * MemTest can take a while to stop,
         * which causes the total to return 0.
         */
        private bool IsAnyMemTestStopping()
        {
            for (var i = 0; i < (int)cboThreads.SelectedItem; i++)
            {
                if (memtests[i].Stopping)
                    return true;
            }

            return false;
        }

        /* 
         * PerformClick() only works if the button is visible
         * switch to main tab and PerformClick() then switch
         * back to the tab that the user was on.
         */
        private void ClickBtnStop()
        {
            var currTab = tabControl.SelectedTab;
            if (currTab != tab_main)
                tabControl.SelectedTab = tab_main;

            btnStop.PerformClick();
            tabControl.SelectedTab = currTab;
        }

        private void ShowErrorMsgBox(string msg)
        {
            MessageBox.Show(
                msg,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private bool IsAllFinished()
        {
            for (int i = 0; i < (int)cboThreads.SelectedItem; i++)
            {
                if (!memtests[i].Finished)
                    return false;
            }

            return true;
        }

        private void RunInBackground(Delegate method)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(delegate (object s, DoWorkEventArgs args)
            {
                Invoke(method);
            });
            bw.RunWorkerAsync();
        }
    }
}

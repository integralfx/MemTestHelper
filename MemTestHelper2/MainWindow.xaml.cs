using MahApps.Metro.Controls;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;

namespace MemTestHelper2
{
    public partial class MainWindow : MetroWindow
    {
        private const string VERSION = "2.0.3";
        private readonly int NUM_THREADS, MAX_THREADS;

        // Update interval (in ms) for coverage info list.
        private const int UPDATE_INTERVAL = 200;

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private MemTest[] memtests;
        private MemTestInfo[] memtestInfo;
        private BackgroundWorker coverageWorker;
        private DateTime startTime;
        private System.Timers.Timer timer;
        private bool isMinimised = true;

        public MainWindow()
        {
            InitializeComponent();
            lblVersion.Content = $"Version {VERSION}";

            log.Info($"Starting MemTestHelper v{VERSION}");

            NUM_THREADS = Convert.ToInt32(Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
            log.Info($"CPU Threads: {NUM_THREADS}");
            MAX_THREADS = NUM_THREADS * 4;
            memtests = new MemTest[MAX_THREADS];
            // Index 0 stores the total.
            memtestInfo = new MemTestInfo[MAX_THREADS + 1];

            InitCboThreads();
            InitLstCoverage();
            InitCboRows();
            CentreXYOffsets();
            UpdateLstCoverage();

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
                // Wait for all MemTests to stop completely.
                while (IsAnyMemTestStopping())
                    Thread.Sleep(100);

                // TODO: Figure out why total coverage is sometimes reporting 0.0 after stopping.
                UpdateCoverageInfo(false);
            });

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var threads = (int)cboThreads.SelectedItem;
                    var elapsed = e.SignalTime - startTime;

                    lblElapsedTime.Content = String.Format("{0:00}h{1:00}m{2:00}s",
                                                           (int)(elapsed.TotalHours),
                                                           elapsed.Minutes,
                                                           elapsed.Seconds);

                    // This thread only accesses element 0.
                    lock (memtestInfo)
                    {
                        var totalCoverage = memtestInfo[0].Coverage;
                        if (totalCoverage <= 0.0) return;

                        // Round up to next multiple of 100.
                        var nextCoverage = ((int)(totalCoverage / 100) + 1) * 100;
                        var elapsedms = elapsed.TotalMilliseconds;
                        var est = (elapsedms / totalCoverage * nextCoverage) - elapsedms;

                        TimeSpan estimatedTime = TimeSpan.FromMilliseconds(est);
                        lblEstimatedTime.Content = String.Format("{0:00}h{1:00}m{2:00}s to {3}%",
                                                                 (int)(estimatedTime.TotalHours),
                                                                 estimatedTime.Minutes,
                                                                 estimatedTime.Seconds,
                                                                 nextCoverage);

                        var ram = Convert.ToInt32(txtRAM.Text);
                        var speed = (totalCoverage / 100) * ram / (elapsedms / 1000);
                        lblSpeed.Content = $"{speed:f2}MB/s";
                    }
                });
            });
            
        }

        #region Event Handling

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();

            InitCboRows();
            UpdateLstCoverage();
            CentreXYOffsets();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            CloseMemTests();
            SaveConfig();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            var threads = (int)cboThreads.SelectedItem;
            switch (WindowState)
            {
                // Minimise MemTest instances.
                case WindowState.Minimized:
                    RunInBackground(() =>
                    {
                        for (var i = 0; i < threads; i++)
                        {
                            if (memtests[i] != null)
                            {
                                memtests[i].Minimised = true;
                                Thread.Sleep(10);
                            }
                        }
                    });
                    break;

                // Restore previous state of MemTest instances.
                case WindowState.Normal:
                    RunInBackground(() =>
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

                            // User may have changed offsets while minimised.
                            LayoutMemTests();

                            // Hack to bring form to top.
                            Topmost = true;
                            Thread.Sleep(10);
                            Topmost = false;
                        }
                    });
                    break;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(MemTest.EXE_NAME))
            {
                MessageBox.Show(MemTest.EXE_NAME + " not found");
                return;
            }

            if (!ValidateInput()) return;

            txtRAM.IsEnabled = false;
            cboThreads.IsEnabled = false;
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            chkStopAt.IsEnabled = false;
            txtStopAt.IsEnabled = false;
            chkStopOnError.IsEnabled = false;
            chkStartMin.IsEnabled = false;

            // Run in background as StartMemTests() can block.
            RunInBackground(() =>
            {
                StartMemTests();

                if (!coverageWorker.IsBusy)
                    coverageWorker.RunWorkerAsync();

                startTime = DateTime.Now;
                timer.Start();

                Activate();
            });
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Parallel.For(0, (int)cboThreads.SelectedItem, i =>
            {
                if (!memtests[i].Finished)
                    memtests[i].Stop();
            });

            coverageWorker.CancelAsync();
            timer.Stop();

            txtRAM.IsEnabled = true;
            cboThreads.IsEnabled = true;
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            chkStopAt.IsEnabled = true;
            if (chkStopAt.IsEnabled)
                txtStopAt.IsEnabled = true;
            chkStopOnError.IsEnabled = true;
            chkStartMin.IsEnabled = true;

            // Wait for all memtests to fully stop.
            while (IsAnyMemTestStopping())
                Thread.Sleep(100);

            // Update speed.
            var ram = Convert.ToInt32(txtRAM.Text);
            var elapsedTime = TimeSpan.ParseExact(
                (string)lblElapsedTime.Content, 
                @"hh\hmm\mss\s", 
                CultureInfo.InvariantCulture
            ).TotalSeconds;
            lock (memtestInfo)
            {
                // 0 is the total coverage.
                var speed = (memtestInfo[0].Coverage / 100) * ram / elapsedTime;
                lblSpeed.Content = $"{speed:f2}MB/s";
            }

            MessageBox.Show("Please check if there are any errors", "MemTest finished");
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            // Run in background as Thread.Sleep can lockup the GUI.
            var threads = (int)cboThreads.SelectedItem;
            RunInBackground(() =>
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
                LayoutMemTests();

                Activate();
            });
        }

        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            var threads = (int)cboThreads.SelectedItem;
            RunInBackground(() =>
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
            });
        }

        private void cboThreads_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateLstCoverage();

            cboRows.Items.Clear();
            InitCboRows();
            CentreXYOffsets();
        }

        private void Offset_Changed(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            RunInBackground(() => { LayoutMemTests(); });
        }

        private void btnCentre_Click(object sender, RoutedEventArgs e)
        {
            CentreXYOffsets();
        }

        private void cboRows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CentreXYOffsets();
        }

        private void chkStopAt_Checked(object sender, RoutedEventArgs e)
        {
            txtStopAt.IsEnabled = true;
        }

        private void chkStopAt_Unchecked(object sender, RoutedEventArgs e)
        {
            txtStopAt.IsEnabled = false;
        }

        private void txtDiscord_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            (sender as TextBox).SelectAll();
        }

        #endregion

        #region Helper Functions

        private void InitCboThreads()
        {
            cboThreads.Items.Clear();

            for (var i = 0; i < MAX_THREADS; i++)
                cboThreads.Items.Add(i + 1);

            cboThreads.SelectedItem = NUM_THREADS;
        }

        private void InitCboRows()
        {
            cboRows.Items.Clear();

            var threads = (int)cboThreads.SelectedItem;

            for (var i = 1; i <= threads; i++)
            {
                if (threads % i == 0)
                    cboRows.Items.Add(i);
            }

            cboRows.SelectedItem = threads % 2 == 0 ? 2 : 1;
        }

        private void InitLstCoverage()
        {
            for (var i = 0; i <= (int)cboThreads.SelectedItem; i++)
            {
                // First row is the total coverage.
                memtestInfo[i] = new MemTestInfo(i == 0 ? "T" : i.ToString(), 0.0, 0);
            }

            lstCoverage.ItemsSource = memtestInfo;
        }

        private void UpdateLstCoverage()
        {
            var threads = (int)cboThreads.SelectedItem;
            var items = (MemTestInfo[])lstCoverage.ItemsSource;
            if (items == null) return;

            var count = items.Count((m) => { return m != null && m.Valid; });
            // items[0] stores the total.
            if (count > threads)
            {
                for (var i = threads + 1; i < count; i++)
                    items[i].Valid = false;
            }
            else
            {
                for (var i = count; i <= threads; i++)
                    items[i] = new MemTestInfo(i.ToString(), 0.0, 0);
            }

            // Only show valid items.
            var view = CollectionViewSource.GetDefaultView(items);
            view.Filter = o =>
            {
                if (o == null) return false;

                var info = o as MemTestInfo;
                return info.Valid;
            };
            view.Refresh();
        }

        // Returns free RAM in MB.
        private ulong GetFreeRAM()
        {
            return new ComputerInfo().AvailablePhysicalMemory / (1024 * 1024);
        }

        private bool LoadConfig()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                foreach (var key in appSettings.AllKeys)
                {
                    switch (key)
                    {
                        case "ram":
                            txtRAM.Text = appSettings[key];
                            break;
                        case "threads":
                            cboThreads.SelectedItem = Int32.Parse(appSettings[key]);
                            break;

                        case "xOffset":
                            udXOffset.Value = Int32.Parse(appSettings[key]);
                            break;
                        case "yOffset":
                            udYOffset.Value = Int32.Parse(appSettings[key]);
                            break;

                        case "xSpacing":
                            udXSpacing.Value = Int32.Parse(appSettings[key]);
                            break;
                        case "ySpacing":
                            udYSpacing.Value = Int32.Parse(appSettings[key]);
                            break;

                        case "rows":
                            cboRows.SelectedItem = Int32.Parse(appSettings[key]);
                            break;

                        case "stopAt":
                            chkStopAt.IsChecked = Boolean.Parse(appSettings[key]);
                            break;
                        case "stopAtValue":
                            txtStopAt.Text = appSettings[key];
                            break;

                        case "stopOnError":
                            chkStopOnError.IsChecked = Boolean.Parse(appSettings[key]);
                            break;

                        case "startMin":
                            chkStartMin.IsChecked = Boolean.Parse(appSettings[key]);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            return true;
        }

        private bool SaveConfig()
        {
            try
            {
                var dict = new Dictionary<string, string>();
                dict.Add("ram", txtRAM.Text);
                dict.Add("threads", ((int)cboThreads.SelectedItem).ToString());
                dict.Add("xOffset", udXOffset.Value.ToString());
                dict.Add("yOffset", udYOffset.Value.ToString());
                dict.Add("xSpacing", udXSpacing.Value.ToString());
                dict.Add("ySpacing", udYSpacing.Value.ToString());
                dict.Add("rows", cboRows.SelectedItem.ToString());
                dict.Add("stopAt", chkStopAt.IsChecked.ToString());
                dict.Add("stopAtValue", txtStopAt.Text);
                dict.Add("stopOnError", chkStopOnError.IsChecked.ToString());
                dict.Add("startMin", chkStartMin.IsChecked.ToString());

                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                foreach (var pair in dict)
                {
                    if (settings[pair.Key] == null)
                        settings.Add(pair.Key, pair.Value);
                    else settings[pair.Key].Value = pair.Value;
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException e)
            {
                MessageBox.Show(e.BareMessage);
                return false;
            }

            return true;
        }

        private bool ValidateInput()
        {
            var ci = new ComputerInfo();
            UInt64 totalRAM = ci.TotalPhysicalMemory / (1024 * 1024),
                   availableRAM = ci.AvailablePhysicalMemory / (1024 * 1024);

            log.Info($"Total RAM: ${totalRAM}\nAvailable RAM: ${availableRAM}");

            var ramText = txtRAM.Text;
            log.Info($"Input RAM: ${ramText}");
            // Automatically input available RAM if empty.
            if (ramText.Length == 0)
            {
                ramText = GetFreeRAM().ToString();
                txtRAM.Text = ramText;
                log.Info($"No RAM input. Free RAM: ${ramText}");
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
            log.Info($"Selected threads: ${threads}");
            if (ram < threads)
            {
                ShowErrorMsgBox($"Amount of RAM must be greater than {threads}");
                return false;
            }

            if (ram > MemTest.MAX_RAM * threads)
            {
                ShowErrorMsgBox(
                    $"Amount of RAM must be at most {MemTest.MAX_RAM * threads}\n" +
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
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );
                if (res == MessageBoxResult.No)
                    return false;
            }

            // Validate stop at % and error count.
            if (chkStopAt.IsChecked.Value)
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

        private void CentreXYOffsets()
        {
            if (cboRows.SelectedIndex == -1 || cboThreads.SelectedIndex == -1)
                return;

            var workArea = SystemParameters.WorkArea;
            int rows = (int)cboRows.SelectedItem,
                cols = (int)cboThreads.SelectedItem / rows,
                xOffset = ((int)workArea.Width - MemTest.WIDTH * cols) / 2,
                yOffset = ((int)workArea.Height - MemTest.HEIGHT * rows) / 2;

            udXOffset.Value = xOffset;
            udYOffset.Value = yOffset;
        }

        private void StartMemTests()
        {
            CloseAllMemTests();

            var threads = (int)cboThreads.SelectedItem;
            var ram = Convert.ToDouble(txtRAM.Text) / threads;
            var startMin = chkStartMin.IsChecked.Value;
            Parallel.For(0, threads, i =>
            {
                memtests[i] = new MemTest();
                memtests[i].Start(ram, startMin);
            });

            /* 
             * Some nag message boxes won't be clicked due to concurrent execution.
             * memTestA             | memTestB
             * SetActiveWindow()    | 
             *                      | SetActiveWindow()
             * SendNotifyMessage()  | 
             *                      | SendNotifyMessage()
             *                      
             * memTestB's window will be active while calling SendNotifyMessage() for memTestA.
             */
            foreach (var hwnd in WinAPI.FindAllWindows(MemTest.MSG2))
                WinAPI.ControlClick(hwnd, MemTest.MSGBOX_OK);

            if (!chkStartMin.IsChecked.Value)
                LayoutMemTests();
        }

        private void LayoutMemTests()
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
                    x = c * MemTest.WIDTH + c * xSpacing + xOffset,
                    y = r * MemTest.HEIGHT + r * ySpacing + yOffset;

                memtest.Location = new Point(x, y);
            });
        }

        // Only close MemTests started by MemTestHelper.
        private void CloseMemTests()
        {
            Parallel.For(0, memtests.Length, i =>
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

        // Close all MemTests, regardless of if they were started by MemTestHelper.
        private void CloseAllMemTests()
        {
            // Remove the '.exe'.
            var name = MemTest.EXE_NAME.Substring(0, MemTest.EXE_NAME.Length - 4);
            var processes = Process.GetProcessesByName(name);
            Parallel.ForEach(processes, p => { p.Kill(); });
        }

        private void UpdateCoverageInfo(bool shouldCheck = true)
        {
            lstCoverage.Invoke(() =>
            {
                var threads = (int)cboThreads.SelectedItem;
                var totalCoverage = 0.0;
                var totalErrors = 0;

                for (var i = 1; i <= threads; i++)
                {
                    var memtest = memtests[i - 1];
                    var mti = memtestInfo[i];
                    if (memtest == null) return;
                    var info = memtest.GetCoverageInfo();
                    if (info == null) return;
                    double coverage = info.Item1;
                    int errors = info.Item2;

                    mti.Coverage = coverage;
                    mti.Errors = errors;

                    if (shouldCheck)
                    {
                        // Check coverage %.
                        if (chkStopAt.IsChecked.Value)
                        {
                            var stopAt = Convert.ToInt32(txtStopAt.Text);
                            if (coverage > stopAt)
                            {
                                if (!memtest.Finished) memtest.Stop();
                            }
                        }

                        // Check error count.
                        if (chkStopOnError.IsChecked.Value)
                        {
                            var item = lstCoverage.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                            if (errors > 0)
                            {
                                memtest.CloseNagMessageBox("MemTest Error");
                                item.Foreground = Brushes.Red;
                                ClickBtnStop();
                            }
                            else item.Foreground = Brushes.White;
                        }
                    }

                    totalCoverage += coverage;
                    totalErrors += errors;
                }

                // Element 0 accessed in time.Elapsed event.
                lock (memtestInfo)
                {
                    // Update the total coverage and errors.
                    memtestInfo[0].Coverage = totalCoverage / threads;
                    memtestInfo[0].Errors = totalErrors;
                }

                if (shouldCheck)
                {
                    if (IsAllFinished()) ClickBtnStop();
                }
            });
        }

        // MemTest can take a while to stop, which causes the total to return 0.
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
         * PerformClick() only works if the button is visible switch to main tab and PerformClick() then switch back to
         * the tab that the user was on.
         */
        private void ClickBtnStop()
        {
            var currTab = tabControl.SelectedItem;
            if (currTab != tabMain)
                tabControl.SelectedItem = tabMain;

            // Click the stop button.
            // https://stackoverflow.com/a/728444
            var peer = new ButtonAutomationPeer(btnStop);
            var provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            provider.Invoke();

            tabControl.SelectedItem = currTab;
        }

        private void ShowErrorMsgBox(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool IsAllFinished()
        {
            for (var i = 0; i < (int)cboThreads.SelectedItem; i++)
            {
                if (!memtests[i].Finished) return false;
            }

            return true;
        }

        private void RunInBackground(Action method)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler((sender, e) =>
            {
                Dispatcher.Invoke(method);
            });
            bw.RunWorkerAsync();
        }

        #endregion

        class MemTestInfo : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string no;
            private double coverage;
            private int errors;
            private bool valid;

            public MemTestInfo(string no, double coverage, int errors)
            {
                this.no = no;
                this.coverage = coverage;
                this.errors = errors;
                valid = true;
            }

            public string No
            {
                get { return no; }
                set { no = value; }
            }
            public double Coverage
            {
                get { return coverage; }
                set { coverage = value; NotifyPropertyChanged(); }
            }
            public int Errors
            {
                get { return errors; }
                set { errors = value; NotifyPropertyChanged(); }
            }
            public bool Valid
            {
                get { return valid; }
                set { valid = value; }
            }

            private void NotifyPropertyChanged([CallerMemberName] string property = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}

using Microsoft.VisualBasic.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace MemTestHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            init_cbo_threads();
            init_lst_coverage();
            init_cbo_rows();
            center_xy_offsets();

            bw_coverage = new BackgroundWorker();
            bw_coverage.WorkerSupportsCancellation = true;
            bw_coverage.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
            {
                BackgroundWorker worker = o as BackgroundWorker;

                while (!worker.CancellationPending)
                {
                    update_coverage_info();
                    Thread.Sleep(100);
                }

                args.Cancel = true;
            });
            bw_coverage.RunWorkerCompleted += 
            new RunWorkerCompletedEventHandler(delegate (object o, RunWorkerCompletedEventArgs args)
            {
                // wait for all MemTests to stop completely
                while (is_any_memtest_stopping())
                    Thread.Sleep(100);

                update_coverage_info();
            });

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(delegate 
            (object sender, System.Timers.ElapsedEventArgs e)
            {
                Invoke(new MethodInvoker(delegate
                {
                    int threads = (int)cbo_threads.SelectedItem;
                    var elapsed = e.SignalTime - start_time;

                    lbl_elapsed_time.Text = $"{elapsed:hh\\hmm\\mss\\s}";

                    double total_coverage = 0;
                    for (int i = 1; i <= threads; i++)
                    {
                        var info = get_coverage_info(memtest_states[i - 1].proc.MainWindowHandle);
                        if (info == null) continue;

                        total_coverage += info.Item1;
                    }

                    double diff = elapsed.TotalMilliseconds, 
                           est = 0;
                    int cov = 0;
                    // use user input coverage %
                    if (chk_stop_at.Checked)
                    {
                        cov = Convert.ToInt32(txt_stop_at.Text);

                        if (chk_stop_at_total.Checked)
                            est = (diff / total_coverage * cov) - diff;
                        else
                        {
                            // calculate average coverage and use that to estimate
                            double avg = total_coverage / threads;
                            est = (diff / avg * cov) - diff;
                        }
                    }
                    else
                    {
                        // calculate average coverage and use that to estimate
                        double avg = total_coverage / threads;
                        // round up to next multiple of 100
                        cov = ((int)(avg / 100) + 1) * 100;
                        est = (diff / avg * cov) - diff;
                    }

                    lbl_estimated_time.Text = $"{TimeSpan.FromMilliseconds(est):hh\\hmm\\mss\\s} to {cov}%";

                    int ram = Convert.ToInt32(txt_ram.Text);
                    double speed = (total_coverage / 100) * ram / (diff / 1000);
                    lbl_speed_value.Text = $"{speed:f2}MB/s";
                }));
            });

            form_layout.StartPosition = FormStartPosition.Manual;
            form_layout.Location = new Point(Location.X + Size.Width, Location.Y);
        }

        public int get_selected_num_threads()
        {
            return (int)cbo_threads.SelectedItem;
        }

        public int get_selected_num_rows()
        {
            return (int)cbo_rows.SelectedItem;
        }

        // event handling

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            form_layout.should_close = true;
            form_layout.Close();
            close_memtests();
            e.Cancel = false;
        }

        private void btn_auto_ram_Click(object sender, EventArgs e)
        {
            UInt64 free = new ComputerInfo().AvailablePhysicalMemory / (1024 * 1024);
            txt_ram.Text = free.ToString();
        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            if (!File.Exists(MEMTEST_EXE))
            {
                MessageBox.Show(MEMTEST_EXE + " not found");
                return;
            }

            if (!validate_input()) return;

            btn_auto_ram.Enabled = false;
            txt_ram.Enabled = false;
            cbo_threads.Enabled = false;
            cbo_rows.Enabled = false;
            btn_run.Enabled = false;
            btn_stop.Enabled = true;
            chk_stop_at.Enabled = false;
            txt_stop_at.Enabled = false;
            chk_stop_at_total.Enabled = false;
            chk_stop_at_err.Enabled = false;
            txt_stop_at_err.Enabled = false;
            chk_stop_at_err_total.Enabled = false;

            is_running = true;

            // run in background as start_memtests can block
            run_in_background(new MethodInvoker(delegate
            {
                start_memtests();

                if (!bw_coverage.IsBusy)
                    bw_coverage.RunWorkerAsync();
                start_time = DateTime.Now;
                timer.Start();

                Activate();
            }));
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < (int)cbo_threads.SelectedItem; i++)
            {
                if (!memtest_states[i].is_finished)
                    ControlClick(memtest_states[i].proc.MainWindowHandle, MEMTEST_BTN_STOP);
            }

            bw_coverage.CancelAsync();
            timer.Stop();

            btn_auto_ram.Enabled = true;
            txt_ram.Enabled = true;
            cbo_threads.Enabled = true;
            cbo_rows.Enabled = true;
            btn_run.Enabled = true;
            btn_stop.Enabled = false;
            chk_stop_at.Enabled = true;
            if (chk_stop_at.Checked)
            {
                txt_stop_at.Enabled = true;
                chk_stop_at_total.Enabled = true;
            }
            chk_stop_at_err.Enabled = true;
            if (chk_stop_at_err.Checked)
            {
                txt_stop_at_err.Enabled = true;
                chk_stop_at_err_total.Enabled = true;
            }

            is_running = false;

            MessageBox.Show("MemTest finished");
        }

        private void btn_show_Click(object sender, EventArgs e)
        {
            // run in background as Thread.Sleep can lockup the GUI
            int threads = (int)cbo_threads.SelectedItem;
            run_in_background(new MethodInvoker(delegate
            {
                for (int i = 0; i < threads; i++)
                {
                    if (memtest_states[i] != null)
                    {
                        SetForegroundWindow(memtest_states[i].proc.MainWindowHandle);
                        Thread.Sleep(10);
                    }
                }

                Activate();
            }));
        }

        private void offset_changed(object sender, EventArgs e)
        {
            run_in_background(new MethodInvoker(delegate { move_memtests(); }));
        }

        private void btn_center_Click(object sender, EventArgs e)
        {
            center_xy_offsets();
        }

        private void cbo_rows_SelectionChangeCommitted(object sender, EventArgs e)
        {
            center_xy_offsets();

            // recreate layout
            if (form_layout.Visible)
            {
                form_layout.Visible = false;
                form_layout.Visible = true;
            }
        }

        private void cbo_threads_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int threads = (int)cbo_threads.SelectedItem;
            var items = lst_coverage.Items;
            if (threads < items.Count)
            {
                for (int i = items.Count - 1; i > threads; i--)
                    items.RemoveAt(i);
            }
            else
            {
                for (int i = items.Count; i <= threads; i++)
                {
                    string[] row = { i.ToString(), "-", "-" };
                    lst_coverage.Items.Add(new ListViewItem(row));
                }
            }

            cbo_rows.Items.Clear();
            init_cbo_rows();
            center_xy_offsets();

            // recreate layout grid
            if (form_layout.Visible)
            {
                form_layout.Visible = false;
                form_layout.Visible = true;
            }
        }

        private void chk_stop_at_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_stop_at.Checked)
            {
                txt_stop_at.Enabled = true;
                chk_stop_at_total.Enabled = true;
            }
            else
            {
                txt_stop_at.Enabled = false;
                chk_stop_at_total.Enabled = false;
            }
        }

        private void chk_stop_at_err_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_stop_at_err.Checked)
            {
                txt_stop_at_err.Enabled = true;
                chk_stop_at_err_total.Enabled = true;
            }
            else
            {
                txt_stop_at_err.Enabled = false;
                chk_stop_at_err_total.Enabled = false;
            }
        }

        private void btn_layout_Click(object sender, EventArgs e)
        {
            if (!form_layout.Visible)
                form_layout.Show(this);
            else form_layout.Activate();
        }

        // move the layout form as well
        private void Form1_Move(object sender, EventArgs e)
        {
            form_layout.Location = new Point(Location.X + Size.Width, Location.Y);
        }

        // helper functions

        private bool validate_input()
        {
            string str_ram = txt_ram.Text;

            if (str_ram == "")
            {
                show_error_msgbox("Please enter amount of RAM");
                return false;
            }

            if (!str_ram.All(char.IsDigit))
            {
                show_error_msgbox("Amount of RAM must be an integer");
                return false;
            }

            int threads = (int)cbo_threads.SelectedItem,
                ram = Convert.ToInt32(str_ram);
            if (ram < threads)
            {
                show_error_msgbox($"Amount of RAM must be greater than {threads}");
                return false;
            }

            if (ram > MEMTEST_MAX_RAM * threads)
            {
                show_error_msgbox($"Amount of RAM must be at most {MEMTEST_MAX_RAM * threads}");
                return false;
            }

            ComputerInfo ci = new ComputerInfo();
            UInt64 total_ram = ci.TotalPhysicalMemory / (1024 * 1024),
                   avail_ram = ci.AvailablePhysicalMemory / (1024 * 1024);
            if ((UInt64)ram > total_ram)
            {
                show_error_msgbox($"Amount of RAM exceeds total RAM ({total_ram})");
                return false;
            }

            if ((UInt64)ram > avail_ram)
            {
                var res = MessageBox.Show(
                    $"Amount of RAM exceeds available RAM ({avail_ram})\n" +
                    "This will cause RAM to be paged to your storage,\n" +
                    "making MemTest really slow.\n" +
                    "Continue?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (res == DialogResult.No)
                    return false;
            }

            // validate stop at % and error count
            if (chk_stop_at.Checked)
            {
                string str_stop_at = txt_stop_at.Text;

                if (str_stop_at == "")
                {
                    show_error_msgbox("Please enter stop at (%)");
                    return false;
                }

                if (!str_stop_at.All(char.IsDigit))
                {
                    show_error_msgbox("Stop at (%) must be an integer");
                    return false;
                }

                int stop_at = Convert.ToInt32(str_stop_at);
                if (stop_at <= 0)
                {
                    show_error_msgbox("Stop at (%) must be greater than 0");
                    return false;
                }
            }

            if (chk_stop_at_err.Checked)
            {
                string str_stop_at = txt_stop_at_err.Text;

                if (str_stop_at == "")
                {
                    show_error_msgbox("Please enter stop at error count");
                    return false;
                }

                if (!str_stop_at.All(char.IsDigit))
                {
                    show_error_msgbox("Stop at error count must be an integer");
                    return false;
                }

                int stop_at = Convert.ToInt32(str_stop_at);
                if (stop_at <= 0)
                {
                    show_error_msgbox("Stop at error count must be greater than 0");
                    return false;
                }
            }



            return true;
        }

        private void init_lst_coverage()
        {
            for (int i = 0; i <= (int)cbo_threads.SelectedItem; i++)
            {
                string[] row = { i.ToString(), "-", "-" };
                // first row is total
                if (i == 0) row[0] = "T";

                lst_coverage.Items.Add(new ListViewItem(row));
            }
        }

        private void init_cbo_threads()
        {
            for (int i = 0; i < MAX_THREADS; i++)
                cbo_threads.Items.Add(i + 1);

            cbo_threads.SelectedItem = NUM_THREADS;
        }

        private void init_cbo_rows()
        {
            int threads = (int)cbo_threads.SelectedItem;

            for (int i = 1; i <= threads; i++)
            {
                if (threads % i == 0)
                    cbo_rows.Items.Add(i);
            }

            cbo_rows.SelectedItem = threads % 2 == 0 ? 2 : 1;
        }

        void center_xy_offsets()
        {
            Rectangle screen = Screen.FromControl(this).Bounds;
            int rows = (int)cbo_rows.SelectedItem,
                cols = (int)cbo_threads.SelectedItem / rows,
                x_offset = (screen.Width - MEMTEST_WIDTH * cols) / 2,
                y_offset = (screen.Height - MEMTEST_HEIGHT * rows) / 2;

            ud_x_offset.Value = x_offset;
            ud_y_offset.Value = y_offset;
        }

        private void start_memtests()
        {
            close_all_memtests();

            int threads = (int)cbo_threads.SelectedItem;
            for (int i = 0; i < threads; i++)
            {
                MemTestState state = new MemTestState();
                state.proc = Process.Start(MEMTEST_EXE);
                state.is_finished = false;
                memtest_states[i] = state;

                Thread.Sleep(20);
            }

            Thread.Sleep(20);

            move_memtests();

            for (int i = 0; i < threads; i++)
            {
                IntPtr hwnd = memtest_states[i].proc.MainWindowHandle;

                double ram = Convert.ToDouble(txt_ram.Text) / threads;
                run_in_background(new MethodInvoker(delegate
                {
                    ControlSetText(hwnd, MEMTEST_EDT_RAM, string.Format("{0:f2}", ram));

                    ControlSetText(hwnd, MEMTEST_STATIC_FREE_VER, "Modified version by ∫ntegral#7834");

                    ControlClick(hwnd, MEMTEST_BTN_START);
                }));

                Thread.Sleep(20);
            }
        }

        private void move_memtests()
        {
            if (!is_running) return;

            int x_offset = (int)ud_x_offset.Value,
                y_offset = (int)ud_y_offset.Value,
                x_spacing = (int)ud_x_spacing.Value - 5,
                y_spacing = (int)ud_y_spacing.Value - 3,
                rows = (int)cbo_rows.SelectedItem,
                cols = (int)cbo_threads.SelectedItem / rows;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    IntPtr hwnd = memtest_states[r * cols + c].proc.MainWindowHandle;
                    int x = c * MEMTEST_WIDTH + c * x_spacing + x_offset,
                        y = r * MEMTEST_HEIGHT + r * y_spacing + y_offset;

                    MoveWindow(hwnd, x, y, MEMTEST_WIDTH, MEMTEST_HEIGHT, true);
                }
            }
        }

        private void close_memtests()
        {
            foreach (var s in memtest_states)
            {
                try {
                    if (s != null) s.proc.Kill();
                }
                catch (Exception) {}
            }
        }

        private void close_all_memtests()
        {
            // remove the .exe
            string name = MEMTEST_EXE.Substring(0, MEMTEST_EXE.Length - 4);
            var procs = Process.GetProcessesByName(name);
            foreach (Process p in procs)
                p.Kill();
        }

        // returns (coverage, errors)
        private Tuple<double, int> get_coverage_info(IntPtr hwnd)
        {
            string str = ControlGetText(hwnd, MEMTEST_STATIC_COVERAGE);
            if (str == "") return null;

            // Test over. 47.3% Coverage, 0 Errors
            //            ^^^^^^^^^^^^^^^^^^^^^^^^
            int start = str.IndexOfAny("0123456789".ToCharArray());
            if (start == -1) return null;
            str = str.Substring(start);

            // 47.3% Coverage, 0 Errors
            // ^^^^
            double coverage = Convert.ToDouble(str.Split("%".ToCharArray())[0]);

            // 47.3% Coverage, 0 Errors
            //                 ^^^^^^^^
            start = str.IndexOf("Coverage, ") + "Coverage, ".Length;
            str = str.Substring(start);
            // 0 Errors
            // ^
            int errors = Convert.ToInt32(str.Substring(0, str.IndexOf(" Errors")));

            return Tuple.Create(coverage, errors);
        }

        private void update_coverage_info()
        {
            lst_coverage.Invoke(new MethodInvoker(delegate
            {
                int threads = (int)cbo_threads.SelectedItem;
                double total_coverage = 0;
                int total_errors = 0;

                // total is index 0
                for (int i = 1; i <= threads; i++)
                {
                    var hwnd = memtest_states[i - 1].proc.MainWindowHandle;
                    var info = get_coverage_info(hwnd);
                    if (info == null) continue;
                    double coverage = info.Item1;
                    int errors = info.Item2;

                    lst_coverage.Items[i].SubItems[1].Text = string.Format("{0:f1}", coverage);
                    lst_coverage.Items[i].SubItems[2].Text = errors.ToString();

                    if (errors > 0)
                        lst_coverage.Items[i].SubItems[1].ForeColor = Color.Red;

                    // check coverage %
                    if (chk_stop_at.Checked && !chk_stop_at_total.Checked)
                    {
                        int stop_at = Convert.ToInt32(txt_stop_at.Text);
                        if (coverage > stop_at)
                        {
                            if (!memtest_states[i - 1].is_finished)
                            {
                                ControlClick(memtest_states[i - 1].proc.MainWindowHandle,
                                             MEMTEST_BTN_STOP);
                                memtest_states[i - 1].is_finished = true;
                            }
                        }
                    }

                    // check error count
                    if (chk_stop_at_err.Checked && !chk_stop_at_err_total.Checked)
                    {
                        int stop_at_err = Convert.ToInt32(txt_stop_at_err.Text);
                        if (errors > stop_at_err)
                        {
                            if (!memtest_states[i].is_finished)
                            {
                                ControlClick(memtest_states[i].proc.MainWindowHandle,
                                             MEMTEST_BTN_STOP);
                                memtest_states[i].is_finished = true;
                            }
                        }
                    }

                    total_coverage += coverage;
                    total_errors += errors;
                }

                // update the total coverage and errors
                lst_coverage.Items[0].SubItems[1].Text = string.Format("{0:f1}", total_coverage);
                lst_coverage.Items[0].SubItems[2].Text = total_errors.ToString();

                // check total coverage
                if (chk_stop_at.Checked && chk_stop_at_total.Checked)
                {
                    int stop_at = Convert.ToInt32(txt_stop_at.Text);
                    if (total_coverage > stop_at)
                        click_btn_stop();
                }

                // check total errors
                if (chk_stop_at_err.Checked && chk_stop_at_err_total.Checked)
                {
                    int stop_at_err = Convert.ToInt32(txt_stop_at_err.Text);
                    if (total_errors > stop_at_err)
                        click_btn_stop();
                }

                if (is_all_finished())
                    click_btn_stop();
            }));
        }

        /*
         * MemTest can take a while to stop,
         * which causes the total to return 0
         */
        private bool is_any_memtest_stopping()
        {
            for (int i = 0; i < (int)cbo_threads.SelectedItem; i++)
            {
                IntPtr hwnd = memtest_states[i].proc.MainWindowHandle;
                string str = ControlGetText(hwnd, MEMTEST_STATIC_COVERAGE);
                if (str != "" && str.Contains("Ending")) return true;
            }

            return false;
        }

        /* 
         * PerformClick() only works if the button is visible
         * switch to main tab and PerformClick() then switch
         * back to the tab that the user was on
         */
        private void click_btn_stop()
        {
            var curr_tab = tab_control.SelectedTab;
            if (curr_tab != tab_main)
                tab_control.SelectedTab = tab_main;

            btn_stop.PerformClick();
            tab_control.SelectedTab = curr_tab;
        }

        private void show_error_msgbox(string msg)
        {
            MessageBox.Show(
                msg,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private bool is_all_finished()
        {
            for (int i = 0; i < (int)cbo_threads.SelectedItem; i++)
            {
                if (!memtest_states[i].is_finished)
                    return false;
            }

            return true;
        }

        private void run_in_background(Delegate method)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(delegate (object s, DoWorkEventArgs args)
            {
                Invoke(method);
            });
            bw.RunWorkerAsync();
        }

        /*
         * class_name should be <classname><n>
         * tries to split class_name as above
         * returns (<classname>, <n>) if possible
         * otherwise, returns null
         */
        private Tuple<string, int> split_class_name(string class_name)
        {
            Regex regex = new Regex(@"([a-zA-Z]+)(\d+)");
            Match match = regex.Match(class_name);

            if (!match.Success) return null;

            return Tuple.Create(
                match.Groups[1].Value,
                Convert.ToInt32(match.Groups[2].Value)
            );
        }

        /*
         * class_name should be <classname><n>
         * where <classname> is the name of the class to find
         *       <n>         is the nth window with that matches <classname> (1 indexed)
         * e.g. Edit1
         * returns the handle to the window if found
         * otherwise, returns IntPtr.Zero
         */
        private IntPtr find_window(IntPtr hwnd_parent, string class_name)
        {
            if (hwnd_parent == IntPtr.Zero)
                return IntPtr.Zero;

            var name = split_class_name(class_name);
            if (name == null) return IntPtr.Zero;

            IntPtr hwnd = IntPtr.Zero;
            for (int i = 0; i < name.Item2; i++)
                hwnd = FindWindowEx(hwnd_parent, hwnd, name.Item1, null);

            return hwnd;
        }

        // emulate AutoIT Control functions
        private bool ControlClick(IntPtr hwnd_parent, string class_name)
        {
            IntPtr hwnd = find_window(hwnd_parent, class_name);
            if (hwnd == IntPtr.Zero) return false;
            SendNotifyMessage(hwnd, WM_LBUTTONDOWN, IntPtr.Zero, null);
            SendNotifyMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, null);
            return true;
        }

        private bool ControlSetText(IntPtr hwnd_parent, string class_name, string text)
        {
            IntPtr hwnd = find_window(hwnd_parent, class_name);
            if (hwnd == IntPtr.Zero) return false;
            SendMessage(hwnd, WM_SETTEXT, IntPtr.Zero, text);
            return true;
        }

        private string ControlGetText(IntPtr hwnd, string class_name)
        {
            IntPtr hwnd_control = find_window(hwnd, class_name);
            if (hwnd_control == IntPtr.Zero) return null;
            int len = GetWindowTextLength(hwnd_control);
            StringBuilder str = new StringBuilder(len + 1);
            GetWindowText(hwnd_control, str, str.Capacity);
            return str.ToString();
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        // blocks
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        // doesn't block
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SendNotifyMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public static int WM_SETTEXT = 0xC, WM_LBUTTONDOWN = 0x201, WM_LBUTTONUP = 0x202;

        private static int NUM_THREADS = Convert.ToInt32(System.Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS")),
                           MAX_THREADS = NUM_THREADS * 4;
        private static string MEMTEST_EXE = "memtest_6.0_no_nag.exe",
                              MEMTEST_BTN_START = "Button1",
                              MEMTEST_BTN_STOP = "Button2",
                              MEMTEST_EDT_RAM = "Edit1",
                              MEMTEST_STATIC_COVERAGE = "Static1",
                              // If you find this free version useful...
                              MEMTEST_STATIC_FREE_VER = "Static2";

        private static int MEMTEST_WIDTH = 217,
                           MEMTEST_HEIGHT = 247,
                           MEMTEST_MAX_RAM = 2048;

        private MemTestState[] memtest_states = new MemTestState[MAX_THREADS];
        private bool is_running = false;
        private BackgroundWorker bw_coverage;
        private DateTime start_time;
        private System.Timers.Timer timer;
        // layout grid
        private FormLayout form_layout = new FormLayout();

        class MemTestState
        {
            public Process proc;
            public bool is_finished;
        }
    }
}

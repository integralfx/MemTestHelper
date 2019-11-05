using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace MemTestHelper2
{
    class MemTest
    {
        public static readonly string EXE_NAME = "memtest.exe";
        public static readonly int WIDTH = 217, HEIGHT = 247,
                                   MAX_RAM = 2048;
        public const int TIMEOUT_MS = 3000;

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        public const string CLASSNAME = "#32770",
                            BTN_START = "Button1",
                            BTN_STOP = "Button2",
                            EDT_RAM = "Edit1",
                            STATIC_COVERAGE = "Static1",
                            // If you find this free version useful...
                            STATIC_FREE_VER = "Static2",
                            MSGBOX_OK = "Button1",
                            MSGBOX_YES = "Button1",
                            MSGBOX_NO = "Button2",
                            // Welcome, New MemTest User
                            MSG1 = "Welcome",
                            // Message for first-time users
                            MSG2 = "Message";

        private Process process = null;

        public enum MsgBoxButton { OK, YES, NO }

        public bool VerboseLogging { get; set; } = false;

        public bool Started { get; private set; } = false;

        public bool Finished { get; private set; } = false;

        public bool Minimised
        {
            get { return Started ? WinAPI.IsIconic(process.MainWindowHandle) : false; }
            set
            {
                if (Started)
                {
                    var hwnd = process.MainWindowHandle;

                    if (value)
                        WinAPI.ShowWindow(hwnd, WinAPI.SW_MINIMIZE);
                    else
                    {
                        if (WinAPI.IsIconic(hwnd))
                            WinAPI.ShowWindow(hwnd, WinAPI.SW_RESTORE);
                        else
                            WinAPI.SetForegroundWindow(hwnd);
                    }
                }
            }
        }

        public Point Location
        {
            get
            {
                var rect = new WinAPI.Rect();
                WinAPI.GetWindowRect(process.MainWindowHandle, ref rect);
                return new Point(rect.Left, rect.Top);
            }
            set
            {
                if (process != null && !process.HasExited)
                    WinAPI.MoveWindow(process.MainWindowHandle, (int)value.X, (int)value.Y, WIDTH, HEIGHT, true);
            }
        }

        public bool Stopping
        {
            get
            {
                if (!Started || Finished || process == null || process.HasExited)
                    return false;

                string str = WinAPI.ControlGetText(process.MainWindowHandle, STATIC_COVERAGE);
                if (str != "" && str.Contains("Ending")) return true;

                return false;
            }
        }

        public int PID
        {
            get { return process != null ? process.Id : 0; }
        }

        public static bool IsNagMessageBox(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return false;

            var exStyles = WinAPI.GetWindowLongPtr(hwnd, WinAPI.GWL_EXSTYLE);
            var styles = WinAPI.GetWindowLongPtr(hwnd, WinAPI.GWL_STYLE);
            var expectedStyles = WinAPI.WS_CAPTION | WinAPI.WS_POPUP | WinAPI.WS_VISIBLE;
            return (styles.ToInt64() & expectedStyles) == expectedStyles && 
                   (exStyles.ToInt64() & WinAPI.WS_EX_APPWINDOW) == 0;
        }

        public void Start(double ram, bool startMinimised)
        {
            process = Process.Start(EXE_NAME);
            
            if (VerboseLogging)
            {
                log.Info(
                    $"Started MemTest {PID,5} with {ram} MB, " +
                    $"start minimised: {startMinimised}, " +
                    $"timeout: {TIMEOUT_MS}"
                );
            }

            var end = DateTime.Now + TimeSpan.FromMilliseconds(TIMEOUT_MS);
            // Wait for process to start.
            while (true)
            {
                if (DateTime.Now > end)
                {
                    if (VerboseLogging)
                        log.Error($"Process {process.Id,5}: Failed to close nag message box 1");

                    return;
                }

                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                    break;

                CloseNagMessageBox();
                Thread.Sleep(100);
                process.Refresh();
            }

            var hwnd = process.MainWindowHandle;
            WinAPI.ControlSetText(hwnd, EDT_RAM, $"{ram:f2}");
            WinAPI.ControlSetText(hwnd, STATIC_FREE_VER, "MemTestHelper by ∫ntegral#7834");
            WinAPI.ControlClick(hwnd, BTN_START);

            end = DateTime.Now + TimeSpan.FromMilliseconds(TIMEOUT_MS);
            while (true)
            {
                if (DateTime.Now > end)
                {
                    if (VerboseLogging)
                        log.Error($"Process {process.Id,5}: Failed to close nag message box 2");

                    return;
                }

                if (CloseNagMessageBox())
                    break;

                Thread.Sleep(100);
            }

            Started = true;
            Finished = false;

            if (startMinimised) Minimised = true;
        }

        public void Stop()
        {
            if (process != null && !process.HasExited && Started && !Finished)
            {
                if (VerboseLogging) log.Info($"Stopping MemTest {PID}");
                WinAPI.ControlClick(process.MainWindowHandle, BTN_STOP);
                Started = false;
                Finished = true;
            }
        }

        public void Close()
        {
            if (process != null && !process.HasExited)
                process.Kill();

            process = null;
            Started = false;
            Finished = false;
        }

        // Returns (coverage, errors).
        public Tuple<double, int> GetCoverageInfo()
        {
            if (process == null || process.HasExited)
                return null;

            var str = WinAPI.ControlGetText(process.MainWindowHandle, STATIC_COVERAGE);
            if (str.Contains("Memory allocated") || str.Contains("Ending Test")) return null;
            if (str == "" || !str.Contains("Coverage"))
            {
                if (VerboseLogging)
                    log.Error($"Invalid static coverage string: '{str}'");

                return null;
            }

            // Test over. 47.3% Coverage, 0 Errors
            //            ^^^^^^^^^^^^^^^^^^^^^^^^
            var start = str.IndexOfAny("0123456789".ToCharArray());
            if (start == -1)
            {
                if (VerboseLogging)
                    log.Error("Failed to find start of coverage number");

                return null;
            }
            str = str.Substring(start);

            // 47.3% Coverage, 0 Errors
            // ^^^^
            // some countries use a comma as the decimal point
            var coverageStr = str.Split("%".ToCharArray())[0].Replace(',', '.');
            double coverage;
            var result = Double.TryParse(coverageStr, NumberStyles.Float, CultureInfo.InvariantCulture, out coverage);
            if (!result)
            {
                if (VerboseLogging)
                    log.Error($"Failed to parse coverage % from coverage string: '{coverageStr}'");

                return null;
            }

            // 47.3% Coverage, 0 Errors
            //                 ^^^^^^^^
            start = str.IndexOf("Coverage, ") + "Coverage, ".Length;
            str = str.Substring(start);
            // 0 Errors
            // ^
            int errors;
            result = Int32.TryParse(str.Substring(0, str.IndexOf(" Errors")), out errors);
            if (!result)
            {
                if (VerboseLogging)
                    log.Error($"Failed to parse error count from error string: '{str}'");

                return null;
            }

            return Tuple.Create(coverage, errors);
        }

        public bool CloseNagMessageBox()
        {
            var end = DateTime.Now + TimeSpan.FromMilliseconds(TIMEOUT_MS);
            List<IntPtr> windows;
            do
            {
                windows = WinAPI.FindAllWindows(PID);

                if (VerboseLogging)
                {
                    for (int i = 0; i < windows.Count; i++)
                    {
                        var hwnd = windows[i];
                        var len = WinAPI.GetWindowTextLength(hwnd);
                        var sb = new StringBuilder(len + 1);
                        WinAPI.GetWindowText(hwnd, sb, sb.Capacity);
                        var exStyles = WinAPI.GetWindowLongPtr(hwnd, WinAPI.GWL_EXSTYLE);

                        log.Info(
                            $"PID {PID,5}, window {i + 1}, exstyles: 0x{exStyles.ToInt64():X16}, " +
                            $"text: '{sb.ToString()}'"
                        );
                    }
                }

                windows = windows.Where(IsNagMessageBox).ToList();

                Thread.Sleep(100);
            } while (windows.Count == 0 && DateTime.Now < end);

            if (windows.Count == 0)
            {
                if (VerboseLogging)
                    log.Error($"Failed to find nag message boxes with PID {PID}");

                return false;
            }

            foreach (var hwnd in windows)
            {
                if (WinAPI.SendNotifyMessage(hwnd, WinAPI.WM_CLOSE, IntPtr.Zero, null) != 0)
                    return true;
                else
                {
                    if (VerboseLogging)
                    {
                        log.Error(
                            $"Failed to send notify message to nag message box with PID {PID,5}. " +
                            $"Error code: {Marshal.GetLastWin32Error()}"
                        );
                    }
                    return false;
                }
            }

            return false;
        }

        public bool CloseNagMessageBox(string messageBoxCaption)
        {
            if (!Started || Finished || process == null || process.HasExited)
                return false;

            var end = DateTime.Now + TimeSpan.FromMilliseconds(TIMEOUT_MS);
            var hwnd = IntPtr.Zero;
            do
            {
                hwnd = WinAPI.GetHWNDFromPID(process.Id, messageBoxCaption);
                Thread.Sleep(10);
            } while (hwnd == IntPtr.Zero && DateTime.Now < end);

            if (hwnd == IntPtr.Zero)
            {
                if (VerboseLogging)
                    log.Error($"Failed to find nag message box with caption: '{messageBoxCaption}'");

                return false;
            }

            end = DateTime.Now + TimeSpan.FromMilliseconds(TIMEOUT_MS);
            while (true)
            {
                if (DateTime.Now > end)
                {
                    if (VerboseLogging)
                        log.Error($"Failed to close nag message box with caption: '{messageBoxCaption}'");

                    return false;
                }

                if (WinAPI.SendNotifyMessage(hwnd, WinAPI.WM_CLOSE, IntPtr.Zero, null) != 0)
                    return true;
                else
                {
                    if (VerboseLogging)
                    {
                        log.Error(
                            $"Failed to send notify message to nag message box with caption: '{messageBoxCaption}'. " +
                            $"Error code: {Marshal.GetLastWin32Error()}"
                        );
                    }
                }

                Thread.Sleep(100);
            }
        }
    }
}

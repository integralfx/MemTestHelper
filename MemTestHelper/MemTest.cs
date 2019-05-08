using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;

namespace MemTestHelper
{
    class MemTest
    {
        private const string MEMTEST_EXE = "memtest.exe",
                             MEMTEST_CLASSNAME = "#32770",
                             MEMTEST_BTN_START = "Button1",
                             MEMTEST_BTN_STOP = "Button2",
                             MEMTEST_EDT_RAM = "Edit1",
                             MEMTEST_STATIC_COVERAGE = "Static1",
                             // If you find this free version useful...
                             MEMTEST_STATIC_FREE_VER = "Static2",
                             MEMTEST_MSGBOX_OK = "Button1",
                             MEMTEST_MSGBOX_YES = "Button1",
                             MEMTEST_MSGBOX_NO = "Button2";

        private const int MEMTEST_WIDTH = 217,
                          MEMTEST_HEIGHT = 247;

        private Process process = null;
        private bool hasStarted = false, isFinished = false;

        public enum MsgBoxButton { OK, YES, NO }

        public bool Started
        {
            get { return hasStarted; }
        }

        public bool Finished
        {
            get { return isFinished; }
        }

        public bool Minimised
        {
            get { return hasStarted ? WinAPI.IsIconic(process.MainWindowHandle) : false; }
            set
            {
                if (hasStarted)
                {
                    if (value)
                    {
                        WinAPI.ShowWindow(process.MainWindowHandle, WinAPI.SW_MINIMIZE);
                    }
                    else
                    {
                        if (WinAPI.IsIconic(process.MainWindowHandle))
                            WinAPI.ShowWindow(process.MainWindowHandle, WinAPI.SW_RESTORE);
                        else
                            WinAPI.SetForegroundWindow(process.MainWindowHandle);
                    }
                }
            }
        }

        public Point Location
        {
            set
            {
                if (process != null && !process.HasExited)
                    WinAPI.MoveWindow(process.MainWindowHandle, value.X, value.Y, MEMTEST_WIDTH, MEMTEST_HEIGHT, true);
            }
        }

        public bool Stopping
        {
            get
            {
                if (!hasStarted || isFinished || process == null || process.HasExited)
                    return false;

                string str = WinAPI.ControlGetText(process.MainWindowHandle, MEMTEST_STATIC_COVERAGE);
                if (str != "" && str.Contains("Ending")) return true;

                return false;
            }
        }

        public void Start(double ram, bool startMinimised)
        {
            process = Process.Start(MEMTEST_EXE);
            hasStarted = true;
            isFinished = false;

            // Wait for process to start.
            while (string.IsNullOrEmpty(process.MainWindowTitle))
            {
                ClickNagMessageBox("Welcome, New MemTest User");
                Thread.Sleep(100);
                process.Refresh();
            }

            var hwnd = process.MainWindowHandle;
            WinAPI.ControlSetText(hwnd, MEMTEST_EDT_RAM, $"{ram:f2}");
            WinAPI.ControlSetText(hwnd, MEMTEST_STATIC_FREE_VER, "MemTestHelper by ∫ntegral#7834");
            WinAPI.ControlClick(hwnd, MEMTEST_BTN_START);

            while (!ClickNagMessageBox("Message for first-time users"))
                Thread.Sleep(100);

            if (startMinimised)
                WinAPI.ShowWindow(hwnd, WinAPI.SW_MINIMIZE);
        }

        public void Stop()
        {
            if (process != null && !process.HasExited && 
                hasStarted && !isFinished)
            {
                WinAPI.ControlClick(process.MainWindowHandle, MEMTEST_BTN_STOP);
                isFinished = true;
            }
        }

        public void Close()
        {
            if (hasStarted && !process.HasExited)
                process.Kill();

            process = null;
            hasStarted = false;
            isFinished = false;
        }

        // Returns (coverage, errors).
        public Tuple<double, int> GetCoverageInfo()
        {
            if (process == null || process.HasExited)
                return null;

            var str = WinAPI.ControlGetText(process.MainWindowHandle, MEMTEST_STATIC_COVERAGE);
            if (str == "" || !str.Contains("Coverage")) return null;

            // Test over. 47.3% Coverage, 0 Errors
            //            ^^^^^^^^^^^^^^^^^^^^^^^^
            var start = str.IndexOfAny("0123456789".ToCharArray());
            if (start == -1) return null;
            str = str.Substring(start);

            // 47.3% Coverage, 0 Errors
            // ^^^^
            // some countries use a comma as the decimal point
            var coverageStr = str.Split("%".ToCharArray())[0].Replace(',', '.');
            double coverage;
            Double.TryParse(coverageStr, NumberStyles.Any, CultureInfo.InvariantCulture, out coverage);

            // 47.3% Coverage, 0 Errors
            //                 ^^^^^^^^
            start = str.IndexOf("Coverage, ") + "Coverage, ".Length;
            str = str.Substring(start);
            // 0 Errors
            // ^
            var errors = Convert.ToInt32(str.Substring(0, str.IndexOf(" Errors")));

            return Tuple.Create(coverage, errors);
        }

        public bool ClickNagMessageBox(string messageBoxCaption, MsgBoxButton button = MsgBoxButton.OK, int maxAttempts = 10)
        {
            if (!hasStarted || isFinished || process == null || process.HasExited)
                return false;

            var hwnd = IntPtr.Zero;
            var attempts = 0;
            do
            {
                hwnd = WinAPI.GetHWNDFromPID(process.Id, messageBoxCaption);
                attempts++;
                Thread.Sleep(10);
            } while (hwnd == IntPtr.Zero && attempts < maxAttempts);

            if (hwnd == IntPtr.Zero) return false;
            else
            {
                string strBtn = "";
                switch (button)
                {
                    case MsgBoxButton.OK:
                        strBtn = MEMTEST_MSGBOX_OK;
                        break;

                    case MsgBoxButton.YES:
                        strBtn = MEMTEST_MSGBOX_YES;
                        break;

                    case MsgBoxButton.NO:
                        strBtn = MEMTEST_MSGBOX_NO;
                        break;
                }

                WinAPI.ControlClick(hwnd, strBtn);
                return true;
            }
        }

    }
}

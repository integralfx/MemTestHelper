using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MemTestHelper2
{
    class WinAPI
    {
        public const int WM_SETTEXT = 0xC, WM_LBUTTONDOWN = 0x201, WM_LBUTTONUP = 0x202, WM_SYSCOMMAND = 0x112, 
                         SC_MINIMIZE = 0xF020, SW_SHOW = 5, SW_RESTORE = 9, SW_MINIMIZE = 6, BM_CLICK = 0xF5;

        public static bool ControlClick(IntPtr hwndParent, string className)
        {
            IntPtr hwnd = FindWindow(hwndParent, className);
            if (hwnd == IntPtr.Zero) return false;
            /*
             * If the button is in a dialog box and the dialog box is not active, the BM_CLICK message might fail. 
             * To ensure success in this situation, call the SetActiveWindow function to activate the dialog box 
             * before sending the BM_CLICK message to the button.
             */
            SetActiveWindow(hwndParent);
            return SendNotifyMessage(hwnd, BM_CLICK, IntPtr.Zero, null) != 0;
        }

        public static bool ControlSetText(IntPtr hwndParent, string className, string text)
        {
            IntPtr hwnd = FindWindow(hwndParent, className);
            if (hwnd == IntPtr.Zero) return false;
            return SendMessage(hwnd, WM_SETTEXT, IntPtr.Zero, text) != IntPtr.Zero;
        }

        public static string ControlGetText(IntPtr hwndParent, string className)
        {
            IntPtr hwnd = FindWindow(hwndParent, className);
            if (hwnd == IntPtr.Zero) return null;
            int len = GetWindowTextLength(hwnd);
            StringBuilder str = new StringBuilder(len + 1);
            GetWindowText(hwnd, str, str.Capacity);
            return str.ToString();
        }

        // Finds the first window that matches pid, and if non-empty, windowTitle.
        public static IntPtr GetHWNDFromPID(int pid, String windowTitle = "")
        {
            IntPtr hwnd = IntPtr.Zero;

            EnumWindows(
                delegate (IntPtr currHwnd, IntPtr lParam)
                {
                    int len = GetWindowTextLength(currHwnd);
                    if (windowTitle.Length > 0 && len != windowTitle.Length)
                        return true;

                    StringBuilder sb = new StringBuilder(len + 1);
                    GetWindowText(currHwnd, sb, sb.Capacity);

                    uint currPid;
                    GetWindowThreadProcessId(currHwnd, out currPid);

                    if (currPid == pid)
                    {
                        if (windowTitle.Length == 0)
                            hwnd = currHwnd;
                        else
                        {
                            if (sb.ToString() == windowTitle)
                                hwnd = currHwnd;
                            else return true;
                        }
                        
                        return false;
                    }
                    else return true;
                },
                IntPtr.Zero);

            return hwnd;
        }

        public static List<IntPtr> FindAllWindows(string windowTitle)
        {
            var windows = new List<IntPtr>();

            EnumWindows(
                delegate (IntPtr hwnd, IntPtr lParam)
                {
                    int len = GetWindowTextLength(hwnd);
                    if (windowTitle.Length > 0 && len != windowTitle.Length)
                        return true;

                    StringBuilder sb = new StringBuilder(len + 1);
                    GetWindowText(hwnd, sb, sb.Capacity);

                    if (sb.ToString() == windowTitle)
                        windows.Add(hwnd);

                    return true;
                },
                IntPtr.Zero);

            return windows;
        }

        #region Imports

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        // Blocks.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        // Doesn't block.
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendNotifyMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Is minimised.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        #endregion

        /*
         * className should be <classname><n>.
         * Tries to split className as above.
         * Returns (<classname>, <n>) if possible.
         * Otherwise, returns null.
         */
        private static Tuple<string, int> SplitClassName(string className)
        {
            Regex regex = new Regex(@"([a-zA-Z]+)(\d+)");
            Match match = regex.Match(className);

            if (!match.Success) return null;

            return Tuple.Create(
                match.Groups[1].Value,
                Convert.ToInt32(match.Groups[2].Value)
            );
        }

        /*
         * className should be <classname><n>.
         * where <classname> is the name of the class to find.
         *       <n>         is the nth window with that matches <classname> (1 indexed).
         * e.g. Edit1
         * Returns the handle to the window if found.
         * Otherwise, returns IntPtr.Zero.
         */
        private static IntPtr FindWindow(IntPtr hwndParent, string className)
        {
            if (hwndParent == IntPtr.Zero)
                return IntPtr.Zero;

            var name = SplitClassName(className);
            if (name == null) return IntPtr.Zero;

            var hwnd = IntPtr.Zero;
            for (int i = 0; i < name.Item2; i++)
                hwnd = FindWindowEx(hwndParent, hwnd, name.Item1, null);

            return hwnd;
        }
    }
}

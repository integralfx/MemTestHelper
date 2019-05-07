using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MemTestHelper
{
    class WinAPI
    {
        public const int WM_SETTEXT = 0xC, WM_LBUTTONDOWN = 0x201, WM_LBUTTONUP = 0x202,
                         SW_SHOW = 5, SW_RESTORE = 9, SW_MINIMIZE = 6, BM_CLICK = 0xF5;

        // Emulate AutoIT Control functions
        public static bool ControlClick(IntPtr hwndParent, string className)
        {
            IntPtr hwnd = FindWindow(hwndParent, className);
            if (hwnd == IntPtr.Zero) return false;
            SendNotifyMessage(hwnd, BM_CLICK, IntPtr.Zero, null);
            return true;
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

        public static IntPtr GetHWNDFromPID(int pid, String window_title = "")
        {
            IntPtr hwnd = IntPtr.Zero;

            EnumWindows(
                delegate (IntPtr curr_hwnd, IntPtr lParam)
                {
                    int len = GetWindowTextLength(curr_hwnd);
                    if (len != window_title.Length) return true;
                    StringBuilder sb = new StringBuilder(len + 1);
                    GetWindowText(curr_hwnd, sb, len + 1);

                    uint proc_id;
                    GetWindowThreadProcessId(curr_hwnd, out proc_id);

                    if (proc_id == pid)
                    {
                        if (window_title.Length == 0)
                            hwnd = curr_hwnd;
                        else
                        {
                            if (sb.ToString() == window_title)
                                hwnd = curr_hwnd;
                        }
                        
                        return false;
                    }
                    else return true;
                },
                IntPtr.Zero);

            return hwnd;
        }

        // Imports

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        // blocks
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        // doesn't block
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SendNotifyMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // is minimised
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int CloseWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /*
         * className should be <classname><n>
         * tries to split class_name as above
         * returns (<classname>, <n>) if possible
         * otherwise, returns null
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
         * className should be <classname><n>
         * where <classname> is the name of the class to find
         *       <n>         is the nth window with that matches <classname> (1 indexed)
         * e.g. Edit1
         * returns the handle to the window if found
         * otherwise, returns IntPtr.Zero
         */
        private static IntPtr FindWindow(IntPtr hwndParent, string className)
        {
            if (hwndParent == IntPtr.Zero)
                return IntPtr.Zero;

            var name = SplitClassName(className);
            if (name == null) return IntPtr.Zero;

            IntPtr hwnd = IntPtr.Zero;
            for (int i = 0; i < name.Item2; i++)
                hwnd = FindWindowEx(hwndParent, hwnd, name.Item1, null);

            return hwnd;
        }
    }
}

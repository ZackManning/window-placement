using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowPlacementLib.Models;

namespace WindowPlacementLib
{
    public static class WindowHelpers
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsDelegate lpfn, IntPtr lParam);

        private delegate bool EnumDesktopWindowsDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        private const int SW_RESTORE = 9;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WindowPlacement
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        public enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        // Delegate to filter which windows to include 
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary> Get the text for the window pointed to by hWnd </summary>
        public static string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return string.Empty;
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter"> A delegate that returns true for windows
        ///    that should be returned and false for windows that should
        ///    not be returned </param>
        public static List<IntPtr> FindWindows(EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (filter == null || filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        /// <summary> Find all windows that contain the given title text </summary>
        /// <param name="titleText"> The text that the window title must contain. </param>
        public static List<IntPtr> FindWindowsWithText(string titleText)
        {
            return FindWindows(delegate (IntPtr wnd, IntPtr param)
            {
                return GetWindowText(wnd).Contains(titleText);
            });
        }

        public static List<DesktopWindow> GetDesktopWindows()
        {
            var desktopWindows = new List<DesktopWindow>();
            EnumDesktopWindowsDelegate del = delegate (IntPtr hWnd, int lParam)
            {
                var windowText = GetWindowText(hWnd);
                if (/*!string.IsNullOrWhiteSpace(windowText) && */IsWindowVisible(hWnd))
                {
                    desktopWindows.Add(new DesktopWindow(hWnd, windowText, GetWindowRect(hWnd), GetWindowPlacement(hWnd)));
                }

                return true;
            };

            EnumDesktopWindows(IntPtr.Zero, del, IntPtr.Zero);
            return desktopWindows;
        }

        public static Rect GetWindowRect(IntPtr hWnd)
        {
            var rect = new Rect();
            GetWindowRect(hWnd, ref rect);
            return rect;
        }

        //public static void SetWindowPos(IntPtr hWnd, Rect rect)
        //{
        //    var currentPos = GetWindowRect(hWnd);
        //    if (rect.Left != currentPos.Left || rect.Right != currentPos.Right || rect.Top != currentPos.Top || rect.Bottom != currentPos.Bottom)
        //    {
        //        SetWindowPos(hWnd, 0, rect.Left, rect.Top, Math.Abs(rect.Right - rect.Left), Math.Abs(rect.Bottom - rect.Top), 0);
        //    }
        //}

        public static void MoveWindowScreen(IntPtr hWnd, short direction)
        {
            if (IsIconic(hWnd))
            {
                ShowWindow(hWnd, SW_RESTORE);
            }

            var rect = GetWindowRect(hWnd);
            var screenWidth = Screen.AllScreens.First().Bounds.Width;
            var offset = screenWidth * direction;
            SetWindowPos(hWnd, 0, rect.Left + offset, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, 0);
        }

        public static WindowPlacement GetWindowPlacement(IntPtr hWnd)
        {
            WindowPlacement placement = new WindowPlacement();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            return placement;
        }

        public static bool SetWindowPlacement(IntPtr hWnd, WindowPlacement placement)
        {
            var currentPlacement = GetWindowPlacement(hWnd);
            if (currentPlacement.rcNormalPosition.Left == placement.rcNormalPosition.Left &&
                currentPlacement.rcNormalPosition.Top == placement.rcNormalPosition.Top)
            {
                return true;
            }

            return SetWindowPlacement(hWnd, ref placement);
        }
    }
}

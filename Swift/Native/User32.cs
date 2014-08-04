using System;
using System.Runtime.InteropServices;
using Hardcodet.Wpf.TaskbarNotification.Interop;

namespace Swift.Native {
    public static class User32 {
        /// <summary>
        /// Gets the screen coordinates of the current mouse position.
        /// </summary>
        [DllImport("user32.DLL", SetLastError = true)]
        public static extern bool GetPhysicalCursorPos(ref Point lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
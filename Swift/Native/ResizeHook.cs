using System;
using System.Runtime.InteropServices;

namespace Swift.Native {
    public class ResizeHook {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if (msg != Constants.WM_NCHITTEST) {
                return IntPtr.Zero;
            }

            handled = true;
            var htLocation = DefWindowProc(hwnd, msg, wParam, lParam).ToInt32();
            switch (htLocation) {
                case Constants.HTBOTTOM:
                case Constants.HTBOTTOMLEFT:
                case Constants.HTBOTTOMRIGHT:
                case Constants.HTLEFT:
                case Constants.HTRIGHT:
                case Constants.HTTOP:
                case Constants.HTTOPLEFT:
                case Constants.HTTOPRIGHT:
                    htLocation = Constants.HTBORDER;
                    break;
            }
            return new IntPtr(htLocation);
        }
    }
}
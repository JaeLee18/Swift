using System;

namespace Swift.Helpers {
    public class ResizeHook {
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if (msg != NativeMethods.WM_NCHITTEST) {
                return IntPtr.Zero;
            }

            handled = true;
            var htLocation = NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam).ToInt32();
            switch (htLocation) {
                case NativeMethods.HTBOTTOM:
                case NativeMethods.HTBOTTOMLEFT:
                case NativeMethods.HTBOTTOMRIGHT:
                case NativeMethods.HTLEFT:
                case NativeMethods.HTRIGHT:
                case NativeMethods.HTTOP:
                case NativeMethods.HTTOPLEFT:
                case NativeMethods.HTTOPRIGHT:
                    htLocation = NativeMethods.HTBORDER;
                    break;
            }
            return new IntPtr(htLocation);
        }
    }
}
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Swift.Helpers {
    public static class NativeMethods {
        #region ABEdge enum

        public enum ABEdge {
            ABE_LEFT = 0,
            ABE_TOP = 1,
            ABE_RIGHT = 2,
            ABE_BOTTOM = 3
        }

        #endregion

        #region ABMsg enum

        public enum ABMsg {
            ABM_NEW = 0,
            ABM_REMOVE = 1,
            ABM_QUERYPOS = 2,
            ABM_SETPOS = 3,
            ABM_GETSTATE = 4,
            ABM_GETTASKBARPOS = 5,
            ABM_ACTIVATE = 6,
            ABM_GETAUTOHIDEBAR = 7,
            ABM_SETAUTOHIDEBAR = 8,
            ABM_WINDOWPOSCHANGED = 9,
            ABM_SETSTATE = 10
        }

        #endregion

        public const int WM_NCHITTEST = 0x0084;
        public const int HTBORDER = 18;
        public const int HTBOTTOM = 15;
        public const int HTBOTTOMLEFT = 16;
        public const int HTBOTTOMRIGHT = 17;
        public const int HTLEFT = 10;
        public const int HTRIGHT = 11;
        public const int HTTOP = 12;
        public const int HTTOPLEFT = 13;
        public const int HTTOPRIGHT = 14;

        public const uint MONITOR_DEFAULTTONEAREST = 2;

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern int Shell_NotifyIconGetRect(ref NOTIFYICONIDENTIFIER identifier,
            out RECT iconLocation);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
            string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPhysicalCursorPos(ref POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr MonitorFromRect(ref RECT lprc, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr SHAppBarMessage(ABMsg dwMessage, ref APPBARDATA pData);

        #region Nested type: APPBARDATA

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public ABEdge uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        #endregion

        #region Nested type: MONITORINFO

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        #endregion

        #region Nested type: NOTIFYICONIDENTIFIER

        [StructLayout(LayoutKind.Sequential)]
        public struct NOTIFYICONIDENTIFIER {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uID;
            public Guid guidItem;
        }

        #endregion

        #region Nested type: POINT

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public int x;
            public int y;

            public static implicit operator Point(POINT point) {
                return new Point(point.x, point.y);
            }
        }

        #endregion

        #region Nested type: RECT

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public static implicit operator Rect(RECT rect) {
                // return a 0-width rectangle if the width or height is negative
                if (rect.right - rect.left < 0 || rect.bottom - rect.top < 0) {
                    return new Rect(rect.left, rect.top, 0, 0);
                }
                return new Rect(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            }

            public static implicit operator RECT(Rect rect) {
                return new RECT {
                    left = (int)rect.Left,
                    top = (int)rect.Top,
                    right = (int)rect.Right,
                    bottom = (int)rect.Bottom
                };
            }
        }

        #endregion
    }
}
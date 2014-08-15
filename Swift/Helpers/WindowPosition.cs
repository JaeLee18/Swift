using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Hardcodet.Wpf.TaskbarNotification.Interop;
using Point = System.Windows.Point;

namespace Swift.Helpers {
    /// <summary>
    /// Credit: https://github.com/Quppa/NotificationAreaIconSampleAppWPF
    /// Modified to work with WPF Notify Icon
    /// </summary>
    public static class WindowPosition {
        /// <summary>
        /// Gets the distance from the edge of the screen/taskbar that the window should be drawn from.
        /// This is 8 under Windows 7 with the DWM enabled, and 0 in Windows Vista and in Windows 7 when the DWM is disabled.
        /// </summary>
        private static int WindowEdgeOffset {
            get { return IsDWMEnabled ? 8 : 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the Desktop Window Manager (thus Aero) is enabled.
        /// </summary>
        /// <returns>True if the DWM is enabled, false otherwise.</returns>
        private static bool IsDWMEnabled {
            get {
                bool result;
                NativeMethods.DwmIsCompositionEnabled(out result);
                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the notification area is active.
        /// </summary>
        public static bool IsNotificationAreaActive {
            get {
                var foregroundWindow = NativeMethods.GetForegroundWindow();
                var tray = NativeMethods.FindWindow("Shell_TrayWnd", string.Empty);

                // Windows 7 notification area fly-out
                var overflowWindow = NativeMethods.FindWindow("NotifyIconOverflowWindow", string.Empty);
                return (foregroundWindow == tray || foregroundWindow == overflowWindow);
            }
        }

        /// <summary>
        /// Returns the optimum window position in relation to the specified notify icon.
        /// </summary>
        public static Point GetWindowPosition(TaskbarIcon icon, double width, double height, double dpi) {
            var taskbarInfo = GetTaskBarInfo();
            var tmpPosition = GetNotifyIconRect(icon);

            // if our functions can't find the rectangle, align it to a corner of the screen
            Rect position;
            if (tmpPosition == null) {
                switch (taskbarInfo.Alignment) {
                    case TaskBarAlignment.Top:
                        position = new Rect(taskbarInfo.Position.Right - 1, taskbarInfo.Position.Top, 1, 1);
                        break;
                    case TaskBarAlignment.Right:
                        position = new Rect(taskbarInfo.Position.Right - 1, taskbarInfo.Position.Bottom - 1, 1, 1);
                        break;
                    case TaskBarAlignment.Left:
                        position = new Rect(taskbarInfo.Position.Left, taskbarInfo.Position.Bottom - 1, 1, 1);
                        break;
                    default:
                        position = new Rect(taskbarInfo.Position.Right - 1, taskbarInfo.Position.Bottom - 1, 1, 1);
                        break;
                }
            } else {
                position = (Rect)tmpPosition;
            }

            // check if notify icon is in the fly-out
            var inFlyout = IsNotifyIconInFlyOut(position, taskbarInfo.Position);

            // determine centre of notify icon
            var iconCenter = new Point(position.Left + (position.Width / 2), position.Top + (position.Height / 2));

            // get window offset from edge
            var edgeOffset = WindowEdgeOffset * dpi;

            // get working area bounds
            var workArea = GetWorkingArea(position);

            // calculate window position
            double windowLeft, windowTop;

            switch (taskbarInfo.Alignment) {
                case TaskBarAlignment.Top:
                    // horizontally centre below icon
                    windowLeft = iconCenter.X - (width / 2);
                    if (inFlyout) {
                        windowTop = position.Bottom + edgeOffset;
                    } else {
                        windowTop = taskbarInfo.Position.Bottom + edgeOffset;
                    }
                    break;
                case TaskBarAlignment.Left:
                    // vertically centre to the right of icon (or above icon if in flyout and not pinned)
                    if (inFlyout) {
                        windowLeft = iconCenter.X - (width / 2);
                        windowTop = position.Top - height - edgeOffset;
                    } else {
                        windowLeft = taskbarInfo.Position.Right + edgeOffset;
                        windowTop = iconCenter.Y - (height / 2);
                    }
                    break;
                case TaskBarAlignment.Right:
                    // vertically centre to the left of icon (or above icon if in flyout and not pinned)
                    if (inFlyout) {
                        windowLeft = iconCenter.X - (width / 2);
                        windowTop = position.Top - height - edgeOffset;
                    } else {
                        windowLeft = taskbarInfo.Position.Left - width - edgeOffset;
                        windowTop = iconCenter.Y - (height / 2);
                    }
                    break;
                default:
                    // horizontally centre above icon
                    windowLeft = iconCenter.X - (width / 2);
                    if (inFlyout) {
                        windowTop = position.Top - height - edgeOffset;
                    } else {
                        windowTop = taskbarInfo.Position.Top - height - edgeOffset;
                    }
                    break;
            }

            // check that the window is within the working area
            // if not, put it next to the closest edge
            if (windowLeft + width + edgeOffset > workArea.Right) {
                windowLeft = workArea.Right - width - edgeOffset;
            } else if (windowLeft < workArea.Left) {
                windowLeft = workArea.Left + edgeOffset;
            }

            if (windowTop + height + edgeOffset > workArea.Bottom) {
                windowTop = workArea.Bottom - height - edgeOffset;
            }

            return new Point(windowLeft, windowTop);
        }

        /// <summary>
        /// Returns a rectangle representing the location of the specified NotifyIcon. (Windows 7+.)
        /// </summary>
        private static Rect? GetNotifyIconRect(TaskbarIcon icon) {
            // get notify icon id
            var iconDataField = icon.GetType().GetField("iconData", BindingFlags.NonPublic | BindingFlags.Instance);
            if (iconDataField == null) {
                return null;
            }
            var iconData = (NotifyIconData)iconDataField.GetValue(icon);

            NativeMethods.RECT rect;
            var notifyIconId = new NativeMethods.NOTIFYICONIDENTIFIER {
                hWnd = iconData.WindowHandle,
                uID = iconData.TaskbarIconId
            };
            notifyIconId.cbSize = (uint)Marshal.SizeOf(notifyIconId);

            var result = NativeMethods.Shell_NotifyIconGetRect(ref notifyIconId, out rect);

            // 0 means success, 1 means the notify icon is in the fly-out - either is fine
            if (result != 0 && result != 1) {
                return null;
            }

            // convert to System.Rect and return
            return rect;
        }

        /// <summary>
        /// Determines whether the specified System.Windows.Forms.NotifyIcon is contained within the Windows 7 notification area
        /// fly-out.
        /// Note that this function will return false if the fly-out is closed.
        /// </summary>
        private static bool IsNotifyIconInFlyOut(Rect iconRect, Rect taskbarRect) {
            return (iconRect.Left > taskbarRect.Right || iconRect.Right < taskbarRect.Left
                    || iconRect.Bottom < taskbarRect.Top || iconRect.Top > taskbarRect.Bottom);
        }

        /// <summary>
        /// Checks whether a point is within the bounds of the specified notify icon.
        /// </summary>
        private static bool IsPointInNotifyIcon(Point point, TaskbarIcon icon) {
            var iconRect = GetNotifyIconRect(icon);
            return iconRect != null && ((Rect)iconRect).Contains(point);
        }

        /// <summary>
        /// Returns the cursor's current position as a System.Windows.Point.
        /// </summary>
        private static Point GetCursorPosition() {
            var result = new NativeMethods.POINT();
            NativeMethods.GetPhysicalCursorPos(ref result);
            return result;
        }

        /// <summary>
        /// Returns true if the cursor is currently over the specified notify icon.
        /// </summary>
        public static bool IsCursorOverNotifyIcon(TaskbarIcon icon) {
            return IsPointInNotifyIcon(GetCursorPosition(), icon);
        }

        /// <summary>
        /// Retrieves taskbar position and alignment.
        /// </summary>
        private static TaskBarInfo GetTaskBarInfo() {
            // allocate appbardata structure
            var abdata = new NativeMethods.APPBARDATA { hWnd = IntPtr.Zero };
            abdata.cbSize = (uint)Marshal.SizeOf(abdata);

            // get task bar info
            var result = NativeMethods.SHAppBarMessage(NativeMethods.ABMsg.ABM_GETTASKBARPOS, ref abdata);

            // return null if the call failed
            if (result == IntPtr.Zero) {
                throw new ApplicationException("Could not retrieve taskbar information.");
            }

            Rect position = abdata.rc;

            TaskBarAlignment alignment;

            switch (abdata.uEdge) {
                case NativeMethods.ABEdge.ABE_BOTTOM:
                    alignment = TaskBarAlignment.Bottom;
                    break;
                case NativeMethods.ABEdge.ABE_TOP:
                    alignment = TaskBarAlignment.Top;
                    break;
                case NativeMethods.ABEdge.ABE_LEFT:
                    alignment = TaskBarAlignment.Left;
                    break;
                case NativeMethods.ABEdge.ABE_RIGHT:
                    alignment = TaskBarAlignment.Right;
                    break;
                default:
                    throw new ApplicationException("Couldn't retrieve location of taskbar.");
            }

            return new TaskBarInfo { Position = position, Alignment = alignment };
        }

        /// <summary>
        /// Returns the working area of the monitor that intersects most with the specified rectangle.
        /// If no monitor can be found, the closest monitor to the rectangle is returned.
        /// </summary>
        private static Rect GetWorkingArea(Rect rectangle) {
            var rect = (NativeMethods.RECT)rectangle;
            var monitorHandle = NativeMethods.MonitorFromRect(ref rect, NativeMethods.MONITOR_DEFAULTTONEAREST);

            var monitorInfo = new NativeMethods.MONITORINFO();
            monitorInfo.cbSize = (uint)Marshal.SizeOf(monitorInfo);

            var result = NativeMethods.GetMonitorInfo(monitorHandle, ref monitorInfo);
            if (!result) {
                throw new ApplicationException("Failed to retrieve monitor information.");
            }

            return monitorInfo.rcWork;
        }

        #region Nested type: TaskBarAlignment

        /// <summary>
        /// Represents alignment of taskbar.
        /// </summary>
        private enum TaskBarAlignment {
            Bottom,
            Top,
            Left,
            Right
        }

        #endregion

        #region Nested type: TaskBarInfo

        /// <summary>
        /// Managed structure representing taskbar position and alignment.
        /// </summary>
        private struct TaskBarInfo {
            /// <summary>
            /// Alignment of taskbar.
            /// </summary>
            public TaskBarAlignment Alignment;

            /// <summary>
            /// Rectangle of taskbar bounds.
            /// </summary>
            public Rect Position;
        }

        #endregion
    }
}
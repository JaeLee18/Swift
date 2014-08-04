using Hardcodet.Wpf.TaskbarNotification.Interop;
using Swift.Native;

namespace Swift.Helpers {
    public static class CursorPos {
        public static Point GetLocation() {
            // get mouse location
            var position = new Point();
            User32.GetPhysicalCursorPos(ref position);
            return position;
        }
    }
}
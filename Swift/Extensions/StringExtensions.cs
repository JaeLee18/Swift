using System;

namespace Swift.Extensions {
    public static class StringExtensions {
        public static bool Empty(this string src) {
            return String.IsNullOrWhiteSpace(src);
        }
    }
}
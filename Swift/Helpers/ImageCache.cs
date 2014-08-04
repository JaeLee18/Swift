using System;
using Akavache;
using Splat;

namespace Swift.Helpers {
    public static class ImageCache {
        /// <summary>
        /// Returns the image from the cache, otherwise downloads the image from the URL specified
        /// and caches it, using the url as the key
        /// </summary>
        public static IObservable<IBitmap> Get(string url, int? width = null, int? height = null) {
            return BlobCache.LocalMachine.LoadImageFromUrl(url, false, width, height, DateTime.UtcNow.AddDays(7));
        }
    }
}
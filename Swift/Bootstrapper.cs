using System.IO;
using Akavache;
using Akavache.Sqlite3;
using Swift.Helpers;
using Swift.ViewModels;

namespace Swift {
    public class Bootstrapper {
        public void AppRegistration(string dir) {
            // Initialize Akavace + Custom directories
            BlobCache.ApplicationName = App.AppName;

            Service.RegisterLazy(() =>
                new SQLiteEncryptedBlobCache(Path.Combine(dir, "SecureCache.db")), typeof(ISecureBlobCache));
            Service.RegisterLazy(() =>
                new SQLitePersistentBlobCache(Path.Combine(dir, "UserCache.db")), typeof(IBlobCache), "UserAccount");
            Service.RegisterLazy(() =>
                new SQLitePersistentBlobCache(Path.Combine(dir, "LocalCache.db")), typeof(IBlobCache), "LocalMachine");

            // app registrations
            Service.RegisterLazy(() => new MainWindowViewModel(), typeof(IMainWindowViewModel));
        }
    }
}
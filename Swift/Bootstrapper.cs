using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using Akavache.Sqlite3;
using ReactiveUI;
using Swift.API;
using Swift.Helpers;
using Swift.Models;
using Swift.ViewModels;
using Swift.Views;

namespace Swift {
    public class Bootstrapper {
        public void AppRegistration(string dir) {
            // Initialize Akavache
            BlobCache.ApplicationName = App.AppName;

            // Use a custom directory for Akavache cache store
            Service.RegisterLazy(() =>
                new SQLiteEncryptedBlobCache(Path.Combine(dir, "SecureCache.db")), typeof(ISecureBlobCache));
            Service.RegisterLazy(() =>
                new SQLitePersistentBlobCache(Path.Combine(dir, "UserCache.db")), typeof(IBlobCache), "UserAccount");
            Service.RegisterLazy(() =>
                new SQLitePersistentBlobCache(Path.Combine(dir, "LocalCache.db")), typeof(IBlobCache), "LocalMachine");

            // We want to load account data synchronously
            Service.RegisterConstant(Task.Run(async () => await new Account().UpdateFromCache()).Result, typeof(Account));

            Service.RegisterLazy(() => new HummingbirdClient(), typeof(IHummingbirdClient));

            Service.RegisterLazy(() => new MainViewModel(), typeof(MainViewModel));
            Service.RegisterLazy(() => new AuthViewModel(), typeof(AuthViewModel));
            Service.RegisterLazy(() => new MediaViewModel(), typeof(MediaViewModel));

            Service.Register(() => new AuthView(), typeof(IViewFor<AuthViewModel>));
            Service.Register(() => new MediaView(), typeof(IViewFor<MediaViewModel>));
        }
    }
}
using System;
using System.IO;
using System.Runtime;
using System.Threading;
using System.Windows;
using Akavache;

namespace Swift {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public const string AppName = "Swift";
        private static Mutex _appMutex;
        private Bootstrapper _bootstrapper;

        protected override void OnStartup(StartupEventArgs e) {
            // application mutex
            _appMutex = new Mutex(true, "Swift-C8711788-C5B5-461A-B34A-7B396937D900");
            if (!_appMutex.WaitOne(0, false)) {
                MessageBox.Show(String.Format("{0} is already running.", AppName), AppName);
                Environment.Exit(0);
            }

            // create app directory in %APPDATA%
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            // enable multicore JIT
            ProfileOptimization.SetProfileRoot(dir);
            ProfileOptimization.StartProfile("Swift.profile");

            // create cache dir
            var cacheDir = Path.Combine(dir, "Cache");
            if (!Directory.Exists(cacheDir)) {
                Directory.CreateDirectory(cacheDir);
            }

            // run registration
            _bootstrapper = new Bootstrapper();
            _bootstrapper.AppRegistration(cacheDir);

            base.OnStartup(e);
        }
    }
}
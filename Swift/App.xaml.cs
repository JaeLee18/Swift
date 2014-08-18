using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace Swift {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public const string AppName = "Swift";
        private static Mutex _appMutex;
        private Bootstrapper _bootstrapper;

        protected override void OnStartup(StartupEventArgs e) {
            // Mutex, so only one instance of Swift can be active at once
            _appMutex = new Mutex(true, "Swift-C8711788-C5B5-461A-B34A-7B396937D900");
            if (!_appMutex.WaitOne(0, false)) {
                MessageBox.Show(String.Format("{0} is already running.", AppName), AppName);
                Environment.Exit(0);
            }

            // Create our appliation directory in %APPDATA% (Usually: C:\Users\{username}\AppData\Roaming\Swift)
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            // Create `Cache` sub-directory under our %APPDATA% directory
            var cacheDir = Path.Combine(dir, "Cache");
            if (!Directory.Exists(cacheDir)) {
                Directory.CreateDirectory(cacheDir);
            }

            // Run our bootstrapper code
            _bootstrapper = new Bootstrapper();
            _bootstrapper.AppRegistration(cacheDir);

            base.OnStartup(e);
        }
    }
}
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using Splat;
using Swift.Extensions;
using Swift.Helpers;

namespace Swift.ViewModels {
    public class AuthViewModel : ReactiveObject, ISupportsActivation {
        private string _username;
        public ViewModelActivator Activator { get; private set; }
        public ReactiveCommand<IBitmap> AvatarCommand { get; private set; }

        public string Username {
            get { return _username; }
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        public AuthViewModel() {
            Activator = new ViewModelActivator();
            AvatarCommand = ReactiveCommand.CreateAsyncObservable(_ => GetAvatar());

            this.WhenActivated(d => {
            });
        }

        private IObservable<IBitmap> GetAvatar() {
            var @default = BitmapLoader.Current.LoadFromResource(
                "pack://application:,,,/Swift;component/Resources/avatar.png", 100, 100).ToObservable();

            return _username.Empty() ? @default : Observable.StartAsync(async () => {
                return "";
            }).SelectMany(url => ImageCache.Get(url, 100, 100)).Catch(@default);
        } 
    }
}
using System;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using Splat;
using Swift.API;
using Swift.API.Exceptions;
using Swift.Extensions;
using Swift.Helpers;
using Swift.Models;

namespace Swift.ViewModels {
    public class AuthViewModel : ReactiveObject, ISupportsActivation {
        private string _password;
        private string _username;

        public ReactiveCommand<IBitmap> Avatar { get; private set; }
        public ReactiveCommand<string> SignIn { get; private set; }
        public ReactiveCommand<object> Registration { get; private set; }
        public ReactiveCommand<object> ResetPassword { get; private set; }

        public string Username {
            get { return _username; }
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        public string Password {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        public string RegistrationUrl {
            get { return "http://hummingbird.me/users/sign_up"; }
        }

        public string ResetUrl {
            get { return "http://hummingbird.me/users/password/new"; }
        }

        public AuthViewModel() {
            Activator = new ViewModelActivator();
            Avatar = ReactiveCommand.CreateAsyncObservable(_ => GetAvatar());
            SignIn = ReactiveCommand.CreateAsyncObservable(this.WhenAnyValue(x => x.Username, x => x.Password,
                (u, p) => !u.Empty() && !p.Empty()), _ => Authenticate());
            Registration = ReactiveCommand.Create();
            ResetPassword = ReactiveCommand.Create();

            this.WhenActivated(d => {
                d(SignIn.Subscribe(token => {
                    var account = Service.Get<Account>();
                    account.Username = Username;
                    account.Token = token;
                    account.Save();

                    // change the content over to the app
                    var vm = Service.Get<MainViewModel>();
                    vm.Content = Service.Get<MediaViewModel>();
                }));

                d(SignIn.ThrownExceptions
                    .Select(x => {
                        if (x is ApiException) {
                            var ex = x as ApiException;
                            if (ex.StatusCode == HttpStatusCode.Unauthorized) {
                                return new UserError("Check your credentials, and try again.");
                            }
                        }
                        return new UserError("There was an issue authenticating with Hummingbird.");
                    })
                    .SelectMany(UserError.Throw)
                    .Subscribe(_ => {
                        Username = "";
                        Password = "";

                        // will set the avatar back to blank hummingbird
                        Avatar.Execute(null);
                    }));
            });
        }

        #region ISupportsActivation Members

        public ViewModelActivator Activator { get; private set; }

        #endregion

        private IObservable<IBitmap> GetAvatar() {
            var @default = BitmapLoader.Current.LoadFromResource(
                "pack://application:,,,/Swift;component/Resources/avatar.png", 100, 100).ToObservable();

            return _username.Empty()
                ? @default
                : Observable.StartAsync(async () => {
                    var client = Service.Get<IHummingbirdClient>();
                    var user = await client.Users.GetInfo(Username);
                    return user.Avatar;
                }).SelectMany(url => ImageCache.Get(url, 100, 100)).Catch(@default);
        }

        private IObservable<string> Authenticate() {
            var client = Service.Get<IHummingbirdClient>();
            return client.Users.Authenticate(Username, Password);
        }
    }
}
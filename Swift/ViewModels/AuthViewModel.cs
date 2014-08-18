using System;
using System.Diagnostics;
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

        public AuthViewModel() {
            Activator = new ViewModelActivator();
            var canSignIn = this.WhenAnyValue(x => x.Username, x => x.Password,
                (u, p) => !u.Empty() && !p.Empty());

            this.WhenActivated(d => {
                d(Avatar = ReactiveCommand.CreateAsyncObservable(_ => GetAvatar()));
                d(SignIn = ReactiveCommand.CreateAsyncObservable(canSignIn, _ => Authenticate()));
                d(Registration = ReactiveCommand.Create());
                d(ResetPassword = ReactiveCommand.Create());

                // External links
                d(Registration.Subscribe(_ => Process.Start("http://hummingbird.me/users/sign_up")));
                d(ResetPassword.Subscribe(_ => Process.Start("http://hummingbird.me/users/password/new")));

                // Handle when signing in was successful
                d(SignIn.Subscribe(token => {
                    // Save the correct user data to the secure cache
                    var account = Service.Get<Account>();
                    account.Username = Username;
                    account.Token = token;
                    account.Save();

                    // Sign in was successful, Tell the MainView to change the content view
                    MessageBus.Current.SendMessage(new MediaViewModel() as ReactiveObject, "Content");
                }));

                // Handle when an exception is thrown during the sign in process
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
                        // Error was thrown to the handler, empty out the credentials
                        Username = "";
                        Password = "";

                        // will set the avatar back to blank hummingbird
                        Avatar.Execute(null);
                    }));

                // Execute the Avatar command so that the default image is set on the view
                Avatar.Execute(null);
            });
        }

        #region ISupportsActivation Members

        public ViewModelActivator Activator { get; private set; }

        #endregion

        /// <summary>
        /// Retrieves the avatar for the username from Hummingbird
        /// </summary>
        private IObservable<IBitmap> GetAvatar() {
            // Default image that is used when an exception is thrown
            // or when `Username` is empty
            var @default = BitmapLoader.Current.LoadFromResource(
                "pack://application:,,,/Swift;component/Resources/avatar.png", 100, 100).ToObservable();

            if (Username.Empty()) {
                return @default;
            }

            // Username is "valid" at this point so attempt to grab URL to avatar from API
            var client = Service.Get<IHummingbirdClient>();
            return client.Users.GetInfo(Username)
                .SelectMany(user => ImageCache.Get(user.Avatar, 100, 100))
                .Catch(@default);
        }

        /// <summary>
        /// Authenticates the credentials with Hummingbird
        /// </summary>
        private IObservable<string> Authenticate() {
            var client = Service.Get<IHummingbirdClient>();
            return client.Users.Authenticate(Username, Password);
        }
    }
}
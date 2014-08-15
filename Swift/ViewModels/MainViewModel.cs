using System;
using ReactiveUI;
using Swift.Helpers;
using Swift.Models;

namespace Swift.ViewModels {
    public class MainViewModel : ReactiveObject {
        private readonly Account _account;
        private ReactiveObject _content;

        public string Title {
            get {
                return _account.HasCredentials ? String.Format("{0} - {1}", App.AppName, _account.Username) : App.AppName;
            }
        }

        public string ProfileUrl {
            get { return String.Format("http://hummingbird.me/users/{0}", _account.Username); }
        }

        public string DashboardUrl {
            get { return "http://hummingbird.me/dashboard"; }
        }

        public string CommunityUrl {
            get { return "http://forums.hummingbird.me/"; }
        }

        public ReactiveObject Content {
            get { return _content; }
            set { this.RaiseAndSetIfChanged(ref _content, value); }
        }

        public ReactiveCommand<object> Profile { get; private set; }
        public ReactiveCommand<object> Dashboard { get; private set; }
        public ReactiveCommand<object> Community { get; private set; }
        public ReactiveCommand<object> Exit { get; private set; }

        public MainViewModel() {
            _account = Service.Get<Account>();
            ShowInitialContent();

            var isLoggedIn = this.WhenAnyValue(x => x._account.HasCredentials);

            Profile = ReactiveCommand.Create(isLoggedIn);
            Dashboard = ReactiveCommand.Create(isLoggedIn);
            Community = ReactiveCommand.Create(isLoggedIn);
            Exit = ReactiveCommand.Create();
        }

        private void ShowInitialContent() {
            if (_account.HasCredentials) {
                Content = Service.Get<MediaViewModel>();
            } else {
                Content = Service.Get<AuthViewModel>();
            }
        }
    }
}
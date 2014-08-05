using System;
using ReactiveUI;
using Swift.Helpers;
using Swift.Models;

namespace Swift.ViewModels {
    public class MainViewModel : ReactiveObject {
        private readonly Account _account;
        private ReactiveObject _content;
        private bool _isVisible;

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

        public bool IsVisible {
            get { return _isVisible; }
            set { this.RaiseAndSetIfChanged(ref _isVisible, value); }
        }

        public ReactiveObject Content {
            get { return _content; }
            set { this.RaiseAndSetIfChanged(ref _content, value); }
        }

        public ReactiveCommand<object> VisibilityCommand { get; private set; }

        public ReactiveCommand<object> ProfileCommand { get; private set; }

        public ReactiveCommand<object> DashboardCommand { get; private set; }

        public ReactiveCommand<object> CommunityCommand { get; private set; } 

        public ReactiveCommand<object> ExitCommand { get; private set; }

        public MainViewModel() {
            _account = Service.Get<Account>();
            ShowInitialContent();

            var isLoggedIn = this.WhenAnyValue(x => x._account.HasCredentials);

            VisibilityCommand = ReactiveCommand.Create();

            ProfileCommand = ReactiveCommand.Create(isLoggedIn);
            DashboardCommand = ReactiveCommand.Create(isLoggedIn);
            CommunityCommand = ReactiveCommand.Create(isLoggedIn);

            ExitCommand = ReactiveCommand.Create();

            VisibilityCommand.Subscribe(_ => IsVisible = !IsVisible);
        }

        private void ShowInitialContent() {
            if (_account.HasCredentials) {
                Content = Service.Get<AppViewModel>();
            } else {
                Content = Service.Get<AuthViewModel>();
            }
        }
    }
}
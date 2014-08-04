using System;
using ReactiveUI;
using Swift.Helpers;
using Swift.Models;

namespace Swift.ViewModels {
    public class MainWindowViewModel : ReactiveObject {
        private ReactiveObject _content;
        private bool _isVisible;

        public string Title {
            get { return App.AppName; }
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

        public ReactiveCommand<object> ExitCommand { get; private set; }

        public MainWindowViewModel() {
            ShowInitialContent();

            VisibilityCommand = ReactiveCommand.Create();
            ExitCommand = ReactiveCommand.Create();

            VisibilityCommand.Subscribe(_ => IsVisible = !IsVisible);
        }

        private void ShowInitialContent() {
            var account = Service.Get<Account>();
            if (account.HasData()) {
                //Content = Service.Get<>()
            } else {
                Content = Service.Get<AuthViewModel>();
            }
        }
    }
}
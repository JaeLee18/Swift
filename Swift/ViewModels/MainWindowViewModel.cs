using System;
using ReactiveUI;

namespace Swift.ViewModels {
    public interface IMainWindowViewModel {
        string Title { get; }
        bool IsVisible { get; set; }
        ReactiveCommand<object> VisibilityCommand { get; }
        ReactiveCommand<object> ExitCommand { get; }
    }

    public class MainWindowViewModel : ReactiveObject, IMainWindowViewModel {
        private bool _isVisible;

        public MainWindowViewModel() {
            VisibilityCommand = ReactiveCommand.Create();
            ExitCommand = ReactiveCommand.Create();

            VisibilityCommand.Subscribe(_ => IsVisible = !IsVisible);
        }

        #region IMainWindowViewModel Members

        public string Title {
            get { return App.AppName; }
        }

        public bool IsVisible {
            get { return _isVisible; }
            set { this.RaiseAndSetIfChanged(ref _isVisible, value); }
        }

        public ReactiveCommand<object> VisibilityCommand { get; private set; }

        public ReactiveCommand<object> ExitCommand { get; private set; }

        #endregion
    }
}
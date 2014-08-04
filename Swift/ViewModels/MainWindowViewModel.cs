using ReactiveUI;

namespace Swift.ViewModels {
    public class MainWindowViewModel : ReactiveObject {
        public string Title {
            get { return App.AppName; }
        }
    }
}
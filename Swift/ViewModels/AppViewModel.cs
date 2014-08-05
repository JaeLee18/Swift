using ReactiveUI;

namespace Swift.ViewModels {
    public class AppViewModel : ReactiveObject, ISupportsActivation {
        public AppViewModel() {
            Activator = new ViewModelActivator();
        }

        #region ISupportsActivation Members

        public ViewModelActivator Activator { get; private set; }

        #endregion
    }
}
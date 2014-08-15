using ReactiveUI;

namespace Swift.ViewModels {
    public class MediaViewModel : ReactiveObject, ISupportsActivation {
        public ReactiveCommand<object> External { get; private set; }

        public string HummingbirdUrl {
            get { return "http://hummingbird.me/dashboard"; }
        }

        public MediaViewModel() {
            Activator = new ViewModelActivator();
            External = ReactiveCommand.Create();

            this.WhenActivated(d => {
                // grab the users list from HB
            });
        }

        #region ISupportsActivation Members

        public ViewModelActivator Activator { get; private set; }

        #endregion
    }
}
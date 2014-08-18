using System;
using System.Diagnostics;
using ReactiveUI;

namespace Swift.ViewModels {
    public class MediaViewModel : ReactiveObject, ISupportsActivation {
        public ReactiveCommand<object> External { get; private set; }

        public MediaViewModel() {
            Activator = new ViewModelActivator();
            
            this.WhenActivated(d => {
                d(External = ReactiveCommand.Create());

                d(External.Subscribe(_ => Process.Start("http://hummingbird.me/dashboard")));
            });
        }

        #region ISupportsActivation Members

        public ViewModelActivator Activator { get; private set; }

        #endregion
    }
}
using System;
using ReactiveUI;
using Swift.Helpers;

namespace Swift.ViewModels {
    public class AnimeViewModel : ReactiveObject, ISupportsActivation {
        public ReactiveCommand<object> ExternalCommand { get; private set; } 

        public string HummingbirdUrl {
            get { return "http://hummingbird.me/dashboard";  }
        }

        public AnimeViewModel() {
            Activator = new ViewModelActivator();

            ExternalCommand = ReactiveCommand.Create();

            this.WhenActivated(d => {
                d(ExternalCommand.Subscribe(_ => {
                    var vm = Service.Get<MainViewModel>();
                    vm.IsVisible = false;
                }));
            });
        }

        #region ISupportsActivation Members

        public ViewModelActivator Activator { get; private set; }

        #endregion
    }
}
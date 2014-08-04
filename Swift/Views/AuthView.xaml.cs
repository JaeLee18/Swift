using System;
using System.Windows.Controls;
using ReactiveUI;
using Splat;
using Swift.ViewModels;

namespace Swift.Views {
    /// <summary>
    /// Interaction logic for AuthView.xaml
    /// </summary>
    public partial class AuthView : UserControl, IViewFor<AuthViewModel> {
        public AuthView() {
            InitializeComponent();

            this.WhenActivated(d => {
                d(this.WhenAnyObservable(x => x.ViewModel.AvatarCommand)
                    .Subscribe(bitmap => Avatar.Source = bitmap.ToNative()));
            });
        }

        #region IViewFor<AuthViewModel> Members

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = value as AuthViewModel; }
        }

        public AuthViewModel ViewModel { get; set; }

        #endregion
    }
}
using System;
using System.Reactive.Linq;
using System.Windows;
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
                // Control bindings
                d(this.Bind(ViewModel, x => x.Username, x => x.Username.Text));
                d(this.Bind(ViewModel, x => x.Password, x => x.Password.Password, Password.Events().PasswordChanged));
                d(this.BindCommand(ViewModel, x => x.SignIn, x => x.SignIn));

                // Invoke ViewModel commands when the external "links" are clicked
                d(NoAccount.Events().MouseLeftButtonUp.InvokeCommand(ViewModel, x => x.Registration));
                d(ForgotPass.Events().MouseLeftButtonUp.InvokeCommand(ViewModel, x => x.ResetPassword));

                // Attempt to grab the users avatar when the Username control loses focus
                d(Username.Events().LostFocus.InvokeCommand(ViewModel, x => x.Avatar));

                // Received `IBitmap` of the users avatar
                d(this.WhenAnyObservable(x => x.ViewModel.Avatar)
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
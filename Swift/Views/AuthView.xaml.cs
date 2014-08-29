using System;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
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

                // Change the text of the SignIn control based on state
                d(this.WhenAnyObservable(x => x.ViewModel.SignIn.IsExecuting)
                    .Subscribe(state => { SignIn.Content = state ? "Signing In..." : "Sign In"; }));

                // Invoke ViewModel commands when the external "links" are clicked
                d(NoAccount.Events().MouseLeftButtonUp.InvokeCommand(ViewModel, x => x.Registration));
                d(ForgotPass.Events().MouseLeftButtonUp.InvokeCommand(ViewModel, x => x.ResetPassword));

                // Attempt to grab the users avatar when the Username control loses focus
                d(Username.Events().LostFocus.InvokeCommand(ViewModel, x => x.Avatar));

                // Received `IBitmap` of the users avatar
                d(this.WhenAnyObservable(x => x.ViewModel.Avatar)
                    .Subscribe(bitmap => Avatar.Source = bitmap.ToNative()));

                // Allow 'Enter/Return' to execute authentication
                d(this.Events().KeyUp
                    .Where(x => x.Key == Key.Enter)
                    .Where(_ => ViewModel.SignIn.CanExecute(null))
                    .Subscribe(_ => ViewModel.SignIn.Execute(null)));
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
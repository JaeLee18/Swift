﻿using System;
using System.Diagnostics;
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
                d(this.Bind(ViewModel, x => x.Username, x => x.Username.Text));
                d(this.Bind(ViewModel, x => x.Password, x => x.Password.Password, Password.Events().PasswordChanged));
                d(this.BindCommand(ViewModel, x => x.SignIn, x => x.SignIn));
                d(NoAccount.Events().MouseLeftButtonUp.InvokeCommand(ViewModel, x => x.Registration));
                d(ForgotPass.Events().MouseLeftButtonUp.InvokeCommand(ViewModel, x => x.ResetPassword));

                // Avatar handling
                d(Username.Events().LostFocus.InvokeCommand(ViewModel, x => x.Avatar));
                d(this.WhenAnyObservable(x => x.ViewModel.Avatar)
                    .Subscribe(bitmap => Avatar.Source = bitmap.ToNative()));

                // set/remove underline on links
                NoAccount.Events().MouseEnter.Merge(ForgotPass.Events().MouseEnter)
                    .Subscribe(e => { (e.Source as TextBlock).TextDecorations = TextDecorations.Underline; });
                NoAccount.Events().MouseLeave.Merge(ForgotPass.Events().MouseLeave)
                    .Subscribe(e => { (e.Source as TextBlock).TextDecorations = null; });

                // handle external links
                d(this.WhenAnyObservable(x => x.ViewModel.Registration)
                    .Subscribe(_ => Process.Start(ViewModel.RegistrationUrl)));
                d(this.WhenAnyObservable(x => x.ViewModel.ResetPassword)
                    .Subscribe(_ => Process.Start(ViewModel.ResetUrl)));
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
using System;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using Swift.ViewModels;

namespace Swift.Views {
    /// <summary>
    /// Interaction logic for AppView.xaml
    /// </summary>
    public partial class AppView : UserControl, IViewFor<AppViewModel> {
        public AppView() {
            InitializeComponent();

            this.WhenActivated(d => {
                // underline on external link hover
                d(ExternalText.Events().MouseEnter
                    .Subscribe(_ => ExternalText.TextDecorations = TextDecorations.Underline));

                d(ExternalText.Events().MouseLeave
                    .Subscribe(_ => ExternalText.TextDecorations = null));
            });
        }

        #region IViewFor<AppViewModel> Members

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = value as AppViewModel; }
        }

        public AppViewModel ViewModel { get; set; }

        #endregion
    }
}
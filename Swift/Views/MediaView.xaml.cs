using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ReactiveUI;
using Swift.ViewModels;

namespace Swift.Views {
    /// <summary>
    /// Interaction logic for AnimeView.xaml
    /// </summary>
    public partial class MediaView : UserControl, IViewFor<MediaViewModel> {
        public MediaView() {
            InitializeComponent();

            this.WhenActivated(d => {
                // begin fade animation on radio tower
                var board = TryFindResource("FadeAnimation") as Storyboard;
                if (board != null) {
                    board.Begin();
                }

                // open hummingbird when clicking link
                d(ExternalText.Events().MouseLeftButtonUp.InvokeCommand(ViewModel, x => x.External));
                d(this.WhenAnyObservable(x => x.ViewModel.External)
                    .Subscribe(_ => Process.Start(ViewModel.HummingbirdUrl)));

                // underline on external link hover
                d(ExternalText.Events().MouseEnter
                    .Subscribe(_ => ExternalText.TextDecorations = TextDecorations.Underline));

                d(ExternalText.Events().MouseLeave
                    .Subscribe(_ => ExternalText.TextDecorations = null));
            });
        }

        #region IViewFor<AnimeViewModel> Members

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = value as MediaViewModel; }
        }

        public MediaViewModel ViewModel { get; set; }

        #endregion
    }
}
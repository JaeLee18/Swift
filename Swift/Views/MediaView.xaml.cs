using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ReactiveUI;
using Swift.ViewModels;

namespace Swift.Views {
    /// <summary>
    /// Interaction logic for MediaView.xaml
    /// </summary>
    public partial class MediaView : UserControl, IViewFor<MediaViewModel> {
        public MediaView() {
            InitializeComponent();

            this.WhenActivated(d => {
                // Start the Storyboard animation when the view is Activated
                // See `MediaView.xml` for the Storyboard implementation.
                var board = TryFindResource("FadeAnimation") as Storyboard;
                if (board != null) {
                    board.Begin();
                }

                // Invoke ViewModel command when external link is clicked
                d(ExternalText.Events().MouseLeftButtonUp.InvokeCommand(ViewModel, x => x.External));
            });
        }

        #region IViewFor<MediaViewModel> Members

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = value as MediaViewModel; }
        }

        public MediaViewModel ViewModel { get; set; }

        #endregion
    }
}
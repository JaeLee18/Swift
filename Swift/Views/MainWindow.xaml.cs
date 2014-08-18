using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Interop;
using Hardcodet.Wpf.TaskbarNotification;
using ReactiveUI;
using Swift.Helpers;
using Swift.ViewModels;

namespace Swift.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainViewModel> {
        /// <summary>
        /// Boolean check to determine if the window should not be shown on a left-click as it usually is.
        /// </summary>
        private bool _shouldNotShow;

        public MainWindow() {
            InitializeComponent();
            // Set the viewmodel as this view isn't invoked by ReactiveUI
            ViewModel = Service.Get<MainViewModel>();

            this.OneWayBind(ViewModel, x => x.Title, x => x.Title);
            this.OneWayBind(ViewModel, x => x.Title, x => x.Tray.ToolTipText);

            // TaskbarIcon context menu bindings
            this.BindCommand(ViewModel, x => x.Profile, x => x.MenuProfile);
            this.BindCommand(ViewModel, x => x.Dashboard, x => x.MenuDash);
            this.BindCommand(ViewModel, x => x.Community, x => x.MenuForums);
            this.BindCommand(ViewModel, x => x.Exit, x => x.MenuExit);
            this.WhenAnyObservable(x => x.ViewModel.Exit).Subscribe(_ => Close());

            // Lowest level UserError handler, any UserError's thrown that aren't handled will hit this.
            UserError.RegisterHandler(error => {
                Tray.ShowBalloonTip("Error", error.ErrorMessage, BalloonIcon.Error);
                return Observable.Return(RecoveryOptionResult.CancelOperation);
            });

            // Change the content in the ViewModelViewHost based on the `Content` property in the ViewModel
            this.WhenAnyValue(x => x.ViewModel.Content)
                .Where(x => x != null)
                .Subscribe(model => ContentView.ViewModel = model);

            // Sets the position of the window when the height changes.
            this.WhenAnyValue(x => x.Height)
                .Subscribe(_ => SetWindowLocation());

            // TaskbarIcon left-click handler
            // Will show the window, or do nothing, based on the `_shouldNotShow` property
            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => Tray.TrayLeftMouseUp += h, h => Tray.TrayLeftMouseUp -= h)
                .Subscribe(_ => {
                    if (!_shouldNotShow) {
                        SetWindowLocation();
                        Show();
                        Activate();
                    }
                    _shouldNotShow = false;
                });

            // TaskbarIcon right-click handler
            // Sets the `_shouldNotShow` property to false so that the window will show next left-click
            // Logic here:
            //  Right-click is outside the bounds of the window so the `Deactivated` event is fired
            //  `Deactivated` event will hide the window and set `_shouldNotShow` to true
            //  This event handler is hit and sets `_shouldNotShow` to false so the application works as expected.
            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => Tray.TrayRightMouseDown += h, h => Tray.TrayRightMouseDown -= h)
                .Subscribe(_ => _shouldNotShow = false);

            // Hides the window when the user clicks anywhere outside the Window
            // Sets the `_shouldNotShow` property based on whether the cursor is within the TaskbarIcon's rect.
            this.Events().Deactivated.Subscribe(_ => {
                _shouldNotShow = WindowPosition.IsCursorOverNotifyIcon(Tray) && WindowPosition.IsNotificationAreaActive;
                Hide();
            });

            this.Events().SourceInitialized.Subscribe(_ => {
                // The following code disables the ability to resize the window
                // while still retaining the chrome border
                var windowHandle = new WindowInteropHelper(this).Handle;
                var windowSource = HwndSource.FromHwnd(windowHandle);
                if (windowSource == null) {
                    return;
                }

                var resizeHook = new ResizeHook();
                windowSource.AddHook(resizeHook.WndProc);
            });
        }

        #region IViewFor<MainViewModel> Members

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = value as MainViewModel; }
        }

        public MainViewModel ViewModel { get; set; }

        #endregion

        /// <summary>
        /// Sets the position of the window based on the position of the TaskbarIcon in the system tray
        /// </summary>
        private void SetWindowLocation() {
            // TODO: Actually use real DPI
            var position = WindowPosition.GetWindowPosition(Tray, Width, Height, 1.0);
            Top = position.Y;
            Left = position.X;
        }
    }
}
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
        private bool _shouldNotShow;

        public MainWindow() {
            InitializeComponent();
            ViewModel = Service.Get<MainViewModel>();

            this.OneWayBind(ViewModel, x => x.Title, x => x.Title);
            this.OneWayBind(ViewModel, x => x.Title, x => x.Tray.ToolTipText);

            // TaskbarIcon context menu bindings
            this.BindCommand(ViewModel, x => x.Profile, x => x.MenuProfile);
            this.BindCommand(ViewModel, x => x.Dashboard, x => x.MenuDash);
            this.BindCommand(ViewModel, x => x.Community, x => x.MenuForums);
            this.BindCommand(ViewModel, x => x.Exit, x => x.MenuExit);

            this.WhenAnyObservable(x => x.ViewModel.Exit)
                .Subscribe(_ => Close());

            // app level error handler
            UserError.RegisterHandler(error => {
                Tray.ShowBalloonTip("Error", error.ErrorMessage, BalloonIcon.Error);
                return Observable.Return(RecoveryOptionResult.CancelOperation);
            });

            // Content view handling
            this.WhenAnyValue(x => x.ViewModel.Content)
                .Where(x => x != null)
                .Subscribe(model => ContentView.ViewModel = model);

            // reset position when height changes from a content switch
            this.WhenAnyValue(x => x.Height)
                .Subscribe(_ => SetWindowLocation());

            // show/hide the window on mouse up
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

            // hide the window when opening the context menu
            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => Tray.TrayRightMouseDown += h, h => Tray.TrayRightMouseDown -= h)
                .Subscribe(_ => _shouldNotShow = false);

            // hide window when user clicks outside the window
            this.Events().Deactivated.Subscribe(_ => {
                _shouldNotShow = WindowPosition.IsCursorOverNotifyIcon(Tray) && WindowPosition.IsNotificationAreaActive;
                Hide();
            });

            this.Events().SourceInitialized.Subscribe(_ => {
                // we need to disable the resizing cursors
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

        private void SetWindowLocation() {
            // TODO: Actually use real DPI
            var position = WindowPosition.GetWindowPosition(Tray, Width, Height, 1.0);
            Top = position.Y;
            Left = position.X;
        }
    }
}
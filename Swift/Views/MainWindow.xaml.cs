using System;
using System.Diagnostics;
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
        public MainWindow() {
            InitializeComponent();
            ViewModel = Service.Get<MainViewModel>();

            this.OneWayBind(ViewModel, x => x.Title, x => x.Title);
            this.OneWayBind(ViewModel, x => x.Title, x => x.Tray.ToolTipText);

            // Tray context action
            this.BindCommand(ViewModel, x => x.ProfileCommand, x => x.MenuProfile);
            this.BindCommand(ViewModel, x => x.DashboardCommand, x => x.MenuDash);
            this.BindCommand(ViewModel, x => x.CommunityCommand, x => x.MenuForums);
            this.BindCommand(ViewModel, x => x.ExitCommand, x => x.MenuExit);

            this.WhenAnyObservable(x => x.ViewModel.ProfileCommand)
                .Subscribe(_ => Process.Start(ViewModel.ProfileUrl));
            this.WhenAnyObservable(x => x.ViewModel.DashboardCommand)
                .Subscribe(_ => Process.Start(ViewModel.DashboardUrl));
            this.WhenAnyObservable(x => x.ViewModel.CommunityCommand)
                .Subscribe(_ => Process.Start(ViewModel.CommunityUrl));
            this.WhenAnyObservable(x => x.ViewModel.ExitCommand)
                .Subscribe(_ => Close());

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
                    SetWindowLocation();
                    Show();
                    Activate();
                });

            // hide the window when opening the context menu
            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => Tray.TrayRightMouseDown += h, h => Tray.TrayRightMouseDown -= h)
                .Subscribe(_ => Hide());

            // app level error handler
            UserError.RegisterHandler(error => {
                Tray.ShowBalloonTip("Error", error.ErrorMessage, BalloonIcon.Error);
                return Observable.Return(RecoveryOptionResult.CancelOperation);
            });

            // hide window when user clicks outside the window
            this.Events().Deactivated.Subscribe(_ => Hide());

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
            var position = WindowPosition.GetWindowPosition(Tray, Width, Height, 1.0);
            Top = position.Y;
            Left = position.X;
        }
    }
}
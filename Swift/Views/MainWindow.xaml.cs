using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Interop;
using Hardcodet.Wpf.TaskbarNotification.Interop;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using ReactiveUI;
using Swift.Helpers;
using Swift.Native;
using Swift.ViewModels;
using Point = Hardcodet.Wpf.TaskbarNotification.Interop.Point;

namespace Swift.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainWindowViewModel> {
        private readonly MouseHookListener _mouseHook;
        private double _scalingFactor = double.NaN;

        public MainWindow() {
            InitializeComponent();
            ViewModel = Service.Get<MainWindowViewModel>();

            this.OneWayBind(ViewModel, x => x.Title, x => x.Title);
            this.OneWayBind(ViewModel, x => x.Title, x => x.Tray.ToolTipText);

            // bind tray events
            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => Tray.TrayLeftMouseUp += h, h => Tray.TrayLeftMouseUp -= h)
                .Subscribe(_ => ViewModel.VisibilityCommand.Execute(null));
            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => Tray.TrayRightMouseDown += h, h => Tray.TrayRightMouseDown -= h)
                .Subscribe(_ => ViewModel.IsVisible = false);

            // Tray context action
            this.BindCommand(ViewModel, x => x.ProfileCommand, x => x.MenuProfile);
            this.BindCommand(ViewModel, x => x.DashboardCommand, x => x.MenuDash);
            this.BindCommand(ViewModel, x => x.CommunityCommand, x => x.MenuForums);
            this.BindCommand(ViewModel, x => x.ExitCommand, x => x.MenuExit);

            this.WhenAnyObservable(x => x.ViewModel.ProfileCommand).Subscribe(_ => Process.Start(ViewModel.ProfileUrl));
            this.WhenAnyObservable(x => x.ViewModel.DashboardCommand)
                .Subscribe(_ => Process.Start(ViewModel.DashboardUrl));
            this.WhenAnyObservable(x => x.ViewModel.CommunityCommand)
                .Subscribe(_ => Process.Start(ViewModel.CommunityUrl));
            this.WhenAnyObservable(x => x.ViewModel.ExitCommand).Subscribe(_ => Close());

            // Content view handling
            this.WhenAnyValue(x => x.ViewModel.Content)
                .Where(x => x != null)
                .Subscribe(model => Content.ViewModel = model);

            // Readjust window location when window size changes
            this.WhenAnyValue(x => x.Width).Merge(this.WhenAnyValue(x => x.Height))
                .Subscribe(_ => SetWindowLocation());

            // Show/Hide window based on IsVisible in ViewModel
            this.WhenAnyValue(x => x.ViewModel.IsVisible).Subscribe(visible => {
                if (visible) {
                    Show();
                } else {
                    Hide();
                }
            });

            // create global mouse hook
            _mouseHook = new MouseHookListener(new GlobalHooker());
            // hide the window if user clicks outside the window
            Observable.FromEventPattern<MouseEventExtArgs>(
                h => _mouseHook.MouseDownExt += h, h => _mouseHook.MouseDownExt -= h)
                .Select(x => x.EventArgs)
                .Subscribe(e => {
                    if (e.X < Left || e.X > (Left + Width) || e.Y < Top || e.Y > (Top + Height)) {
                        ViewModel.IsVisible = false;
                    }
                });

            this.Events().Loaded.Subscribe(_ => {
                // we need to disable the resizing cursors
                var windowHandle = new WindowInteropHelper(this).Handle;
                var windowSource = HwndSource.FromHwnd(windowHandle);
                if (windowSource == null) {
                    return;
                }

                var resizeHook = new ResizeHook();
                windowSource.AddHook(resizeHook.WndProc);

                // enable mouse hook
                _mouseHook.Enabled = true;
            });
        }

        #region IViewFor<MainWindowViewModel> Members

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = value as MainWindowViewModel; }
        }

        public MainWindowViewModel ViewModel { get; set; }

        #endregion

        private void SetWindowLocation() {
            var trayLocation = GetDeviceCoords(TrayInfo.GetTrayLocation());
            var mouseLocation = GetDeviceCoords(CursorPos.GetLocation());

            Left = mouseLocation.X - (Width / 2);
            Top = trayLocation.Y - (Height + 5);

            // don't extend past the screen edge
            var width = SystemParameters.PrimaryScreenWidth;
            if ((Left + Width) > width) {
                Left -= ((Left + Width) - width) + 5;
            }
        }

        private Point GetDeviceCoords(Point point) {
            if (double.IsNaN(_scalingFactor)) {
                //calculate scaling factor in order to support non-standard DPIs
                var presentationSource = PresentationSource.FromVisual(this);
                if (presentationSource == null) {
                    _scalingFactor = 1;
                } else {
                    var transform = presentationSource.CompositionTarget.TransformToDevice;
                    _scalingFactor = 1 / transform.M11;
                }
            }

            //on standard DPI settings, just return the point
            return _scalingFactor == 1.0
                ? point : new Point { X = (int)(point.X * _scalingFactor), Y = (int)(point.Y * _scalingFactor) };
        }
    }
}
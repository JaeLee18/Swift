using System;
using System.Windows;
using System.Windows.Interop;
using ReactiveUI;
using Swift.Helpers;
using Swift.Native;
using Swift.ViewModels;

namespace Swift.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainWindowViewModel> {
        public MainWindow() {
            InitializeComponent();
            ViewModel = Service.Get<MainWindowViewModel>();

            this.OneWayBind(ViewModel, x => x.Title, x => x.Title);

            // we need to disable the resizing cursors
            this.Events().Loaded.Subscribe(_ => {
                var windowHandle = new WindowInteropHelper(this).Handle;
                var windowSource = HwndSource.FromHwnd(windowHandle);
                if (windowSource == null) {
                    return;
                }

                var resizeHook = new ResizeHook();
                windowSource.AddHook(resizeHook.WndProc);
            });
        }

        #region IViewFor<MainWindowViewModel> Members

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = value as MainWindowViewModel; }
        }

        public MainWindowViewModel ViewModel { get; set; }

        #endregion
    }
}
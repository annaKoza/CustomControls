using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Effects;
using CustomControls.Controls.WindowControl.CommandsBehaviours;

namespace CustomControls.Controls
{
    public partial class CustomWindow
    {
        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow),
                new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public CustomWindow()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow,
                OnCanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow,
                OnCanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow,
                OnCanResizeWindow));
        }

        private bool mRestoreIfMove;

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            var lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);
            var lMmi = (MINMAXINFO) Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            var lCurrentScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lCurrentScreen, lCurrentScreenInfo) == false)
                return;

            lMmi.ptMaxPosition.X = lCurrentScreenInfo.rcWork.Left - lCurrentScreenInfo.rcMonitor.Left;
            lMmi.ptMaxPosition.Y = lCurrentScreenInfo.rcWork.Top - lCurrentScreenInfo.rcMonitor.Top;
            lMmi.ptMaxSize.X = lCurrentScreenInfo.rcWork.Right - lCurrentScreenInfo.rcWork.Left;
            lMmi.ptMaxSize.Y = lCurrentScreenInfo.rcWork.Bottom - lCurrentScreenInfo.rcWork.Top;

            Marshal.StructureToPtr(lMmi, lParam, true);
        }

        private void SwitchWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                {
                    WindowState = WindowState.Maximized;
                    break;
                }
                case WindowState.Maximized:
                {
                    WindowState = WindowState.Normal;
                    break;
                }
            }
        }

        private void OnLabelOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip)
                    SwitchWindowState();
                return;
            }

            if (WindowState == WindowState.Maximized)
            {
                mRestoreIfMove = true;
                return;
            }

            DragMove();
        }

        private void OnLabelMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mRestoreIfMove = false;
        }

        private void OnLabelPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!mRestoreIfMove) return;
            mRestoreIfMove = false;

            var percentHorizontal = e.GetPosition(this).X / ActualWidth;
            var targetHorizontal = RestoreBounds.Width * percentHorizontal;

            var percentVertical = e.GetPosition(this).Y / ActualHeight;
            var targetVertical = RestoreBounds.Height * percentVertical;

            WindowState = WindowState.Normal;

            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            Left = lMousePosition.X - targetHorizontal;
            Top = lMousePosition.Y - targetVertical;

            DragMove();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        private enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Windows.Shell.SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
        }

        private void CustomWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                _maximizeButton.Visibility = Visibility.Hidden;
                _restoreButton.Visibility = Visibility.Visible;
                SetWindowMargin(true);
            }
            else if (WindowState == WindowState.Normal)
            {
                _maximizeButton.Visibility = Visibility.Visible;
                _restoreButton.Visibility = Visibility.Collapsed;
                SetWindowMargin(false);
            }
        }

        private void SetWindowMargin(bool checkState)
        {
            if (!checkState)
            {
                //get border radius to change main grid margin to show shadowEffect
                var borderEffect = _shadowBorder.Effect as DropShadowEffect;
                if (borderEffect == null) return;
                var radius = borderEffect.BlurRadius;
                _mainWindowGrid.Margin = new Thickness(radius);
            }
            else
            {
                _mainWindowGrid.Margin = new Thickness(0);
            }
        }

        private void ShowMenu()
        {
            const int additionalDistance = 5;
            var showMenuAt = PointToScreen(Mouse.GetPosition(this));
            showMenuAt.X = showMenuAt.X + additionalDistance;
            showMenuAt.Y = showMenuAt.Y + additionalDistance;
            SystemMenuManager.ShowMenu(this, showMenuAt);
        }

        public override void OnApplyTemplate()
        {
            SourceInitialized += OnCustomWindowSourceInitialized;
            base.OnApplyTemplate();
            _iconContent = Template.FindName(PART_IconContent, this) as ContentControl;
            _label = Template.FindName(PART_Label, this) as Label;
            _mainWindowGrid = Template.FindName(PART_MainWindowGrid, this) as Grid;
            _maximizeButton = Template.FindName(PART_MaximizeButton, this) as Button;
            _restoreButton = Template.FindName(PART_RestoreButton, this) as Button;
            _shadowBorder = Template.FindName(PART_ShadowBorder, this) as Border;
            _windowResizeGrip = Template.FindName(PART_WindowResizeGrip, this) as ResizeGrip;
            _resizeGrid = Template.FindName(PART_ResizeGrid, this) as Grid;
            if (_resizeGrid != null)
                foreach (UIElement element in _resizeGrid.Children)
                {
                    var resizeThumb = element as Thumb;
                    if (resizeThumb != null)
                        resizeThumb.PreviewMouseLeftButtonDown += ResizeThumb_PreviewMouseLeftButtonDown;
                }

            _windowResizeGrip.PreviewMouseLeftButtonDown += _windowResizeGrip_PreviewMouseLeftButtonDown;
            _iconContent.MouseLeftButtonDown += OnIconContentOnMouseLeftButtonDown;
            _label.MouseLeftButtonDown += OnLabelOnMouseLeftButtonDown;
            _label.MouseRightButtonDown += OnLabelOnMouseRightButtonDown;
            _label.MouseLeftButtonUp += OnLabelMouseLeftButtonUp;
            _label.MouseMove += OnLabelPreviewMouseMove;

            StateChanged += CustomWindow_StateChanged;
            CustomWindow_StateChanged(null, null);
        }

        private void ResizeThumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var thumb = sender as Thumb;
            switch (thumb.Name)
            {
                case "ThumbTop":
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "ThumbBottom":
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "ThumbLeft":
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "ThumbRight":
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "ThumbTopLeft":
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "ThumbTopRight":
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "ThumbBottomLeft":
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "ThumbBottomRight":
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
            }
        }

        private void OnCustomWindowSourceInitialized(object sender, EventArgs e)
        {
            var mWindowHandle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(WindowProc);
            _hwndSource = (HwndSource) PresentationSource.FromVisual(this);
        }

        private void OnLabelOnMouseRightButtonDown(object e, MouseButtonEventArgs i)
        {
            ShowMenu();
        }

        private void OnIconContentOnMouseLeftButtonDown(object e, MouseButtonEventArgs i)
        {
            if (i.ClickCount == 1)
                ShowMenu();
            else if (i.ClickCount == 2)
                Close();
        }

        private HwndSource _hwndSource;
        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, 0x112, (IntPtr) (61440 + direction), IntPtr.Zero);
        }

        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8
        }

        private void _windowResizeGrip_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResizeWindow(ResizeDirection.BottomRight);
        }
    }
}

using CustomControls.Controls.WindowControl.CommandsBehaviours;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace CustomControls.Controls
{
    [TemplatePart(Name = PART_MaximizeButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_RestoreButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_MainWindowGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PART_IconContent, Type = typeof(ContentControl))]
    [TemplatePart(Name = PART_Label, Type = typeof(Label))]
    [TemplatePart(Name = PART_MinimizeButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ShadowBorder, Type = typeof(Border))]
    [TemplatePart(Name = PART_WindowResizeGrip, Type = typeof(ResizeGrip))]
    public class CustomWindow : Window
    {
        const string PART_WindowResizeGrip = "PART_WindowResizeGrip";
        const string PART_CloseButton = "PART_CloseButton";
        const string PART_IconContent = "PART_IconContent";
        const string PART_Label = "PART_Label";
        const string PART_MainWindowGrid = "PART_MainWindowGrid";
        const string PART_MaximizeButton = "PART_MaximizeButton";
        const string PART_MinimizeButton = "PART_MinimizeButton";
        const string PART_RestoreButton = "PART_RestoreButton";
        const string PART_ShadowBorder = "PART_ShadowBorder";

        public static readonly DependencyProperty BarColorProperty =
            DependencyProperty.Register("BarColor", typeof(Brush), typeof(CustomWindow));

        public static readonly DependencyProperty BarHeightProperty =
            DependencyProperty.Register("BarHeight", typeof(CustomWindow), typeof(CustomWindow));

        public static readonly DependencyProperty BarLabelContentProperty =
            DependencyProperty.Register("BarLabelContent", typeof(ContentPresenter), typeof(CustomWindow));

        public static readonly DependencyProperty CloseButtonStyleProperty =
            DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(CustomWindow));
        
        public static readonly DependencyProperty IconFieldContentProperty =
            DependencyProperty.Register("IconFieldContent", typeof(DataTemplate), typeof(CustomWindow));

        public static readonly DependencyProperty InactiveBorderBrushProperty =
            DependencyProperty.Register("InactiveBorderBrush", typeof(Brush), typeof(CustomWindow));

        public static readonly DependencyProperty InactiveWindowShadowColorProperty =
            DependencyProperty.Register("InactiveWindowShadowColor", typeof(Color), typeof(CustomWindow));

        public static readonly DependencyProperty IsBarVisibleProperty =
            DependencyProperty.Register("IsBarVisible", typeof(bool), typeof(CustomWindow));

        public static readonly DependencyProperty IsIconVisibleProperty =
            DependencyProperty.Register("IsIconVisible", typeof(bool), typeof(CustomWindow));

        public static readonly DependencyProperty MaximizeButtonStyleProperty =
            DependencyProperty.Register("MaximizeButtonStyle", typeof(Style), typeof(CustomWindow));
        
        public static readonly DependencyProperty MinimizeButtonStyleProperty =
            DependencyProperty.Register("MinimizeButtonStyle", typeof(Style), typeof(CustomWindow));

        public static readonly DependencyProperty RestoreButtonStyleProperty = 
            DependencyProperty.Register("RestoreButtonStyle", typeof(Style), typeof(CustomWindow));

        public Style RestoreButtonStyle
        {
            get => (Style) GetValue(RestoreButtonStyleProperty);
            set => SetValue(RestoreButtonStyleProperty, value);
        }
        public static readonly DependencyProperty WindowShadowColorProperty =
            DependencyProperty.Register("WindowShadowColor", typeof(Color), typeof(CustomWindow));
        
        private ContentControl _iconContent;
        private Label _label;
        private Grid _mainWindowGrid;
        private Button _maximizeButton;
        private Button _restoreButton;
        private Border _shadowBorder;
        private ResizeGrip _windowResizeGrip;

        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public CustomWindow()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
        }
        private bool mRestoreIfMove = false;

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

            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);
            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            MONITORINFO lCurrentScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lCurrentScreen, lCurrentScreenInfo) == false)
            {
                return;
            }
            
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
                if ((ResizeMode == ResizeMode.CanResize) || (ResizeMode == ResizeMode.CanResizeWithGrip))
                {
                    SwitchWindowState();
                }
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

            double percentHorizontal = e.GetPosition(this).X / ActualWidth;
            double targetHorizontal = RestoreBounds.Width * percentHorizontal;

            double percentVertical = e.GetPosition(this).Y / ActualHeight;
            double targetVertical = RestoreBounds.Height * percentVertical;

            WindowState = WindowState.Normal;

            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            Left = lMousePosition.X - targetHorizontal;
            Top = lMousePosition.Y - targetVertical;

            DragMove();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }


        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);


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
        };


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
                _mainWindowGrid.Margin = new Thickness(0);
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
            PreviewMouseMove += CustomWindow_PreviewMouseMove;
            _iconContent = Template.FindName(PART_IconContent, this) as ContentControl;
            _label = Template.FindName(PART_Label, this) as Label;
            _mainWindowGrid = Template.FindName(PART_MainWindowGrid, this) as Grid;
            _maximizeButton = Template.FindName(PART_MaximizeButton, this) as Button;
            _restoreButton = Template.FindName(PART_RestoreButton, this) as Button;
            _shadowBorder = Template.FindName(PART_ShadowBorder, this) as Border;
            _windowResizeGrip = Template.FindName(PART_WindowResizeGrip, this) as ResizeGrip;

            _windowResizeGrip.PreviewMouseLeftButtonDown += _windowResizeGrip_PreviewMouseLeftButtonDown;
            _windowResizeGrip.PreviewMouseLeftButtonUp += _windowResizeGrip_PreviewMouseLeftButtonUp;
            _label.MouseDoubleClick += OnLabelOnMouseDoubleClick;

            _iconContent.MouseLeftButtonDown += OnIconContentOnMouseLeftButtonDown;
            _label.MouseLeftButtonDown += OnLabelOnMouseLeftButtonDown;
            _label.MouseRightButtonDown += OnLabelOnMouseRightButtonDown;
            _label.MouseLeftButtonUp += OnLabelMouseLeftButtonUp;
            _label.MouseMove += OnLabelPreviewMouseMove;

            StateChanged += CustomWindow_StateChanged;
            CustomWindow_StateChanged(null, null);
        }

        private void OnCustomWindowSourceInitialized(object sender, EventArgs e)
        {
            IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
        }

        private void OnLabelOnMouseDoubleClick(object e, MouseButtonEventArgs i)
        {
            if (WindowState.Equals(WindowState.Maximized))
            {
                OnRestoreWindow(this, null);
            }
            else if (WindowState.Equals(WindowState.Normal))
            {
                OnMaximizeWindow(this, null);
            }
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

        Point _startPosition;
        bool _isResizing = false;

        private void _windowResizeGrip_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Mouse.Capture(_windowResizeGrip)) return;
            _isResizing = true;
            _startPosition = Mouse.GetPosition(this);
        }

        private void CustomWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isResizing) return;
            var currentPosition = Mouse.GetPosition(this);
            var diffX = currentPosition.X - _startPosition.X;
            var diffY = currentPosition.Y - _startPosition.Y;
            Width += diffX;
            Height += diffY;
            _startPosition = currentPosition;
        }

        private void _windowResizeGrip_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isResizing) return;
            _isResizing = false;
            Mouse.Capture(null);
        }

        public Brush BarColor
        {
            get => (Brush)GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }

        public double BarHeight
        {
            get => (double)GetValue(BarHeightProperty);
            set => SetValue(BarHeightProperty, value);
        }

        public ContentPresenter BarLabelContent
        {
            get => (ContentPresenter)GetValue(BarLabelContentProperty);
            set => SetValue(BarLabelContentProperty, value);
        }

        public Style CloseButtonStyle
        {
            get => (Style)GetValue(CloseButtonStyleProperty);
            set => SetValue(CloseButtonStyleProperty, value);
        }
        
        public DataTemplate IconFieldContent
        {
            get => (DataTemplate)GetValue(IconFieldContentProperty);
            set => SetValue(IconFieldContentProperty, value);
        }

        public Brush InactiveBorderBrush
        {
            get => (Brush)GetValue(InactiveBorderBrushProperty);
            set => SetValue(InactiveBorderBrushProperty, value);
        }
        
        public Color InactiveWindowShadowColor
        {
            get => (Color)GetValue(InactiveWindowShadowColorProperty);
            set => SetValue(InactiveWindowShadowColorProperty, value);
        }

        public bool IsBarVisible
        {
            get => (bool)GetValue(IsBarVisibleProperty);
            set => SetValue(IsBarVisibleProperty, value);
        }

        public bool IsIconVisible
        {
            get => (bool)GetValue(IsIconVisibleProperty);
            set => SetValue(IsIconVisibleProperty, value);
        }

        public Style MaximizeButtonStyle
        {
            get => (Style)GetValue(MaximizeButtonStyleProperty);
            set => SetValue(MaximizeButtonStyleProperty, value);
        }
      
        public Style MinimizeButtonStyle
        {
            get => (Style)GetValue(MinimizeButtonStyleProperty);
            set => SetValue(MinimizeButtonStyleProperty, value);
        }

        public Color WindowShadowColor
        {
            get => (Color)GetValue(WindowShadowColorProperty);
            set => SetValue(WindowShadowColorProperty, value);
        }

    }
}

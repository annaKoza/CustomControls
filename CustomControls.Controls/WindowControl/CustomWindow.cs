using CustomControls.Controls.WindowControl.CommandsBehaviours;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace CustomControls.Controls
{
    [TemplatePart(Name = PART_MaximizeButton, Type = typeof(ToggleButton))]
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
        const string PART_ShadowBorder = "PART_ShadowBorder";

        public static readonly DependencyProperty BarColorProperty =
            DependencyProperty.Register("BarColor", typeof(Brush), typeof(CustomWindow));

        public static readonly DependencyProperty BarHeightProperty =
            DependencyProperty.Register("BarHeight", typeof(CustomWindow), typeof(CustomWindow));
        public static readonly DependencyProperty BarLabelContentProperty =
            DependencyProperty.Register("BarLabelContent", typeof(ContentPresenter), typeof(CustomWindow));
        public static readonly DependencyProperty brushProperty =
            DependencyProperty.Register("brush", typeof(Brush), typeof(CustomWindow));

        public static readonly DependencyProperty CloseButtonStyleProperty =
            DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(CustomWindow));

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(CustomWindow));

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

        public static readonly DependencyProperty MaximizeCommandProperty =
            DependencyProperty.Register("MaximizeCommand", typeof(ICommand), typeof(CustomWindow));

        public static readonly DependencyProperty MinimizeButtonStyleProperty =
            DependencyProperty.Register("MinimizeButtonStyle", typeof(Style), typeof(CustomWindow));

        public static readonly DependencyProperty MinimizeCommandProperty =
            DependencyProperty.Register("MinimizeCommand", typeof(ICommand), typeof(CustomWindow));

        public static readonly DependencyProperty WindowShadowColorProperty =
            DependencyProperty.Register("WindowShadowColor", typeof(Color), typeof(CustomWindow));
        
        private Button _closeButton;
        private ContentControl _iconContent;
        private Label _label;
        private Grid _mainWindowGrid;
        private ToggleButton _maximizeButton;
        private WindowMaximizeCommand _maximizeCommand = new WindowMaximizeCommand();
        private Button _minimizeButton;
        private Border _shadowBorder;
        private ResizeGrip _resizeGrip;

        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public CustomWindow()
        {
            
        }
        

        private void CustomWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                _maximizeButton.IsChecked = true;
                SetWindowMargin(true);
            }
            else if (WindowState == WindowState.Normal)
            {
                _maximizeButton.IsChecked = false;
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
            base.OnApplyTemplate();

            _closeButton = this.Template.FindName(PART_CloseButton, this) as Button;
            _iconContent = Template.FindName(PART_IconContent, this) as ContentControl;
            _label = Template.FindName(PART_Label, this) as Label;
            _mainWindowGrid = Template.FindName(PART_MainWindowGrid, this) as Grid;
            _maximizeButton = Template.FindName(PART_MaximizeButton, this) as ToggleButton;
            _minimizeButton = Template.FindName(PART_MinimizeButton, this) as Button;
            _shadowBorder = Template.FindName(PART_ShadowBorder, this) as Border;

            _label.MouseDoubleClick += (e, i) => _maximizeCommand.Execute(this);
            _iconContent.MouseLeftButtonDown += (e, i) =>
            {
                if (i.ClickCount == 1)
                    ShowMenu();
                else if (i.ClickCount == 2)
                    Close();
            };
            _label.MouseLeftButtonDown += (e, i) => DragMove();
            _label.MouseRightButtonDown += (e, i) => ShowMenu();

            StateChanged += CustomWindow_StateChanged;
            CustomWindow_StateChanged(null, null);
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

        public Brush brush
        {
            get => (Brush)GetValue(brushProperty);
            set => SetValue(brushProperty, value);
        }

        public Style CloseButtonStyle
        {
            get => (Style)GetValue(CloseButtonStyleProperty);
            set => SetValue(CloseButtonStyleProperty, value);
        }

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
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

        public ICommand MaximizeCommand
        {
            get => (ICommand)GetValue(MaximizeCommandProperty);
            set => SetValue(MaximizeCommandProperty, value);
        }

        public Style MinimizeButtonStyle
        {
            get => (Style)GetValue(MinimizeButtonStyleProperty);
            set => SetValue(MinimizeButtonStyleProperty, value);
        }

        public ICommand MinimizeCommand
        {
            get => (ICommand)GetValue(MinimizeCommandProperty);
            set => SetValue(MinimizeCommandProperty, value);
        }

        public Color WindowShadowColor
        {
            get => (Color)GetValue(WindowShadowColorProperty);
            set => SetValue(WindowShadowColorProperty, value);
        }

    }
}

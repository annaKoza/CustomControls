using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

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
    [TemplatePart(Name = PART_ResizeGrid, Type = typeof(Grid))]
    public partial class CustomWindow : Window
    {
        private const string PART_WindowResizeGrip = "PART_WindowResizeGrip";
        private const string PART_CloseButton = "PART_CloseButton";
        private const string PART_IconContent = "PART_IconContent";
        private const string PART_Label = "PART_Label";
        private const string PART_MainWindowGrid = "PART_MainWindowGrid";
        private const string PART_MaximizeButton = "PART_MaximizeButton";
        private const string PART_MinimizeButton = "PART_MinimizeButton";
        private const string PART_RestoreButton = "PART_RestoreButton";
        private const string PART_ShadowBorder = "PART_ShadowBorder";
        private const string PART_ResizeGrid = "PART_ResizeGrid";

        public static readonly DependencyProperty BarColorProperty =
            DependencyProperty.Register("BarColor", typeof(Brush), typeof(CustomWindow));

        public static readonly DependencyProperty BarHeightProperty =
            DependencyProperty.Register("BarHeight", typeof(double), typeof(CustomWindow));

        public static readonly DependencyProperty BarLabelContentProperty =
            DependencyProperty.Register("BarLabelContent", typeof(object), typeof(CustomWindow));

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
        private Grid _resizeGrid;

        public Brush BarColor
        {
            get => (Brush) GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }

        public double BarHeight
        {
            get => (double) GetValue(BarHeightProperty);
            set => SetValue(BarHeightProperty, value);
        }

        public object BarLabelContent
        {
            get => GetValue(BarLabelContentProperty);
            set => SetValue(BarLabelContentProperty, value);
        }

        public Style CloseButtonStyle
        {
            get => (Style) GetValue(CloseButtonStyleProperty);
            set => SetValue(CloseButtonStyleProperty, value);
        }

        public DataTemplate IconFieldContent
        {
            get => (DataTemplate) GetValue(IconFieldContentProperty);
            set => SetValue(IconFieldContentProperty, value);
        }

        public Brush InactiveBorderBrush
        {
            get => (Brush) GetValue(InactiveBorderBrushProperty);
            set => SetValue(InactiveBorderBrushProperty, value);
        }

        public Color InactiveWindowShadowColor
        {
            get => (Color) GetValue(InactiveWindowShadowColorProperty);
            set => SetValue(InactiveWindowShadowColorProperty, value);
        }

        public bool IsBarVisible
        {
            get => (bool) GetValue(IsBarVisibleProperty);
            set => SetValue(IsBarVisibleProperty, value);
        }

        public bool IsIconVisible
        {
            get => (bool) GetValue(IsIconVisibleProperty);
            set => SetValue(IsIconVisibleProperty, value);
        }

        public Style MaximizeButtonStyle
        {
            get => (Style) GetValue(MaximizeButtonStyleProperty);
            set => SetValue(MaximizeButtonStyleProperty, value);
        }

        public Style MinimizeButtonStyle
        {
            get => (Style) GetValue(MinimizeButtonStyleProperty);
            set => SetValue(MinimizeButtonStyleProperty, value);
        }

        public Color WindowShadowColor
        {
            get => (Color) GetValue(WindowShadowColorProperty);
            set => SetValue(WindowShadowColorProperty, value);
        }
    }
}

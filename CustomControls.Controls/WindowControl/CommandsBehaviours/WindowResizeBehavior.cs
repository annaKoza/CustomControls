using System.Windows;
using System.Windows.Controls.Primitives;

namespace CustomControls.Controls.WindowControl.CommandsBehaviours
{
    public static class WindowResizeBehavior
    {

        public static readonly DependencyProperty BottomLeftResize = DependencyProperty.RegisterAttached("BottomLeftResize",
            typeof(Window), typeof(WindowResizeBehavior),
            new UIPropertyMetadata(null, OnBottomLeftResizeChanged));

        public static readonly DependencyProperty BottomResize = DependencyProperty.RegisterAttached("BottomResize",
            typeof(Window), typeof(WindowResizeBehavior),
            new UIPropertyMetadata(null, OnBottomResizeChanged));

        public static readonly DependencyProperty BottomRightResize = DependencyProperty.RegisterAttached("BottomRightResize",
            typeof(Window), typeof(WindowResizeBehavior),
            new UIPropertyMetadata(null, OnBottomRightResizeChanged));

        public static readonly DependencyProperty LeftResize = DependencyProperty.RegisterAttached("LeftResize",
            typeof(Window), typeof(WindowResizeBehavior),
            new UIPropertyMetadata(null, OnLeftResizeChanged));

        public static readonly DependencyProperty RightResize = DependencyProperty.RegisterAttached("RightResize",
            typeof(Window), typeof(WindowResizeBehavior),
            new UIPropertyMetadata(null, OnRightResizeChanged));

        public static readonly DependencyProperty TopLeftResize = DependencyProperty.RegisterAttached("TopLeftResize",
            typeof(Window), typeof(WindowResizeBehavior),
            new UIPropertyMetadata(null, OnTopLeftResizeChanged));

        public static readonly DependencyProperty TopResize = DependencyProperty.RegisterAttached("TopResize",
            typeof(Window), typeof(WindowResizeBehavior),
            new UIPropertyMetadata(null, OnTopResizeChanged));

        public static readonly DependencyProperty TopRightResize = DependencyProperty.RegisterAttached("TopRightResize",
            typeof(Window), typeof(WindowResizeBehavior),
            new UIPropertyMetadata(null, OnTopRightResizeChanged));

        private static void DragBottom(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;

            if (!(thumb?.GetValue(BottomResize) is Window window)) return;
            var verticalChange = window.SafeHeightChange(e.VerticalChange);
            window.Height += verticalChange;
        }

        private static void DragBottomLeft(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;

            if (!(thumb?.GetValue(BottomLeftResize) is Window window)) return;
            var verticalChange = window.SafeHeightChange(e.VerticalChange);
            var horizontalChange = window.SafeWidthChange(e.HorizontalChange, false);

            window.Width -= horizontalChange;
            window.Left += horizontalChange;
            window.Height += verticalChange;
        }

        private static void DragBottomRight(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;

            if (!(thumb?.GetValue(BottomRightResize) is Window window)) return;
            var verticalChange = window.SafeHeightChange(e.VerticalChange);
            var horizontalChange = window.SafeWidthChange(e.HorizontalChange);

            window.Width += horizontalChange;
            window.Height += verticalChange;
        }

        private static void DragLeft(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;

            if (!(thumb?.GetValue(LeftResize) is Window window)) return;
            var horizontalChange = window.SafeWidthChange(e.HorizontalChange, false);
            window.Width -= horizontalChange;
            window.Left += horizontalChange;
        }

        private static void DragRight(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;

            if (!(thumb?.GetValue(RightResize) is Window window)) return;
            var horizontalChange = window.SafeWidthChange(e.HorizontalChange);
            window.Width += horizontalChange;
        }

        private static void DragTop(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;

            if (!(thumb?.GetValue(TopResize) is Window window)) return;
            var verticalChange = window.SafeHeightChange(e.VerticalChange, false);
            window.Height -= verticalChange;
            window.Top += verticalChange;
        }

        private static void DragTopLeft(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            
            if (!(thumb?.GetValue(TopLeftResize) is Window window)) return;
            var verticalChange = window.SafeHeightChange(e.VerticalChange, false);
            var horizontalChange = window.SafeWidthChange(e.HorizontalChange, false);

            window.Width -= horizontalChange;
            window.Left += horizontalChange;
            window.Height -= verticalChange;
            window.Top += verticalChange;
        }

        private static void DragTopRight(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;

            if (!(thumb?.GetValue(TopRightResize) is Window window)) return;
            var verticalChange = window.SafeHeightChange(e.VerticalChange, false);
            var horizontalChange = window.SafeWidthChange(e.HorizontalChange);

            window.Width += horizontalChange;
            window.Height -= verticalChange;
            window.Top += verticalChange;
        }

        private static void OnBottomLeftResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                thumb.DragDelta += DragBottomLeft;
            }
        }

        private static void OnBottomResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                thumb.DragDelta += DragBottom;
            }
        }

        private static void OnBottomRightResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Thumb thumb)
                thumb.DragDelta += DragBottomRight;
        }

        private static void OnLeftResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                thumb.DragDelta += DragLeft;
            }
        }

        private static void OnRightResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                thumb.DragDelta += DragRight;
            }
        }

        private static void OnTopLeftResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Thumb thumb)
                thumb.DragDelta += DragTopLeft;
        }

        private static void OnTopResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                thumb.DragDelta += DragTop;
            }
        }

        private static void OnTopRightResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Thumb thumb)
                thumb.DragDelta += DragTopRight;
        }

        private static double SafeHeightChange(this Window window, double change, bool positive = true)
        {
            var result = positive ? window.Height + change : window.Height - change;
            return result <= window.MinHeight ? 0 : (result >= window.MaxHeight ? 0 : (result < 0 ? 0 : change));
        }

        private static double SafeWidthChange(this Window window, double change, bool positive = true)
        {
            var result = positive ? window.Width + change : window.Width - change;
            return result <= window.MinWidth ? 0 : (result >= window.MaxWidth ? 0 : (result < 0 ? 0 : change));
        }

        public static Window GetBottomLeftResize(DependencyObject obj)
        {
            return (Window)obj.GetValue(BottomLeftResize);
        }

        public static Window GetBottomResize(DependencyObject obj)
        {
            return (Window)obj.GetValue(BottomResize);
        }

        public static Window GetBottomRightResize(DependencyObject obj)
        {
            return (Window)obj.GetValue(BottomRightResize);
        }

        public static Window GetLeftResize(DependencyObject obj)
        {
            return (Window)obj.GetValue(LeftResize);
        }

        public static Window GetRightResize(DependencyObject obj)
        {
            return (Window)obj.GetValue(RightResize);
        }
        public static Window GetTopLeftResize(DependencyObject obj)
        {
            return (Window)obj.GetValue(TopLeftResize);
        }

        public static Window GetTopResize(DependencyObject obj)
        {
            return (Window)obj.GetValue(TopResize);
        }

        public static Window GetTopRightResize(DependencyObject obj)
        {
            return (Window)obj.GetValue(TopRightResize);
        }

        public static void SetBottomLeftResize(DependencyObject obj, Window window)
        {
            obj.SetValue(BottomLeftResize, window);
        }

        public static void SetBottomResize(DependencyObject obj, Window window)
        {
            obj.SetValue(BottomResize, window);
        }

        public static void SetBottomRightResize(DependencyObject obj, Window window)
        {
            obj.SetValue(BottomRightResize, window);
        }

        public static void SetLeftResize(DependencyObject obj, Window window)
        {
            obj.SetValue(LeftResize, window);
        }

        public static void SetRightResize(DependencyObject obj, Window window)
        {
            obj.SetValue(RightResize, window);
        }

        public static void SetTopLeftResize(DependencyObject obj, Window window)
        {
            obj.SetValue(TopLeftResize, window);
        }

        public static void SetTopResize(DependencyObject obj, Window window)
        {
            obj.SetValue(TopResize, window);
        }

        public static void SetTopRightResize(DependencyObject obj, Window window)
        {
            obj.SetValue(TopRightResize, window);
        }
    }
}

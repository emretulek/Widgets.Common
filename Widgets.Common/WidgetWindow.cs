using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;

namespace Widgets.Common
{
    public partial class WidgetWindow
    {
        public Window Window;
        public WidgetDefaultStruct Default;

        public WidgetWindow(Window window, WidgetDefaultStruct defaults)
        {

            Window = window;
            Default = defaults;

            SetWidgetStruct(Default);
            Window.Loaded += WidgetWindow_Loaded;
        }

        public void SetWidgetStruct(WidgetDefaultStruct widgetStruct)
        {
            Width = widgetStruct.Width;
            Height = widgetStruct.Height;
            MaxWidth = widgetStruct.MaxWidth;
            MaxHeight = widgetStruct.MaxHeight;
            MinWidth = widgetStruct.MinWidth;
            MinHeight = widgetStruct.MinHeight;
            Left = widgetStruct.Left;
            Top = widgetStruct.Top;
            Background = widgetStruct.Background;
            BorderThickness = widgetStruct.BorderThickness;
            Margin = widgetStruct.Margin;
            Padding = widgetStruct.Padding;
            BorderBrush = widgetStruct.BorderBrush;
            AllowsTransparency = widgetStruct.AllowsTransparency;
            WindowStyle = widgetStruct.WindowStyle;
            ShowInTaskbar = widgetStruct.ShowInTaskbar;
            ResizeMode = widgetStruct.ResizeMode;
            SizeToContent = widgetStruct.SizeToContent;
            IsHitTestVisible = widgetStruct.IsHitTestVisible;
        }

        public double Width
        {
            get { return Window.Width; }
            set
            {
                Window.Width = value;
            }
        }

        public double Height
        {
            get { return Window.Height; }
            set => Window.Height = value;
        }

        public double MaxWidth
        {
            get { return Window.MaxWidth; }
            set => Window.MaxWidth = value;
        }

        public double MaxHeight
        {
            get { return Window.MaxHeight; }
            set => Window.MaxHeight = value;
        }

        public double MinWidth
        {
            get { return Window.MinWidth; }
            set => Window.MinWidth = value;
        }

        public double MinHeight
        {
            get { return Window.MinHeight; }
            set => Window.MinHeight = value;
        }

        public double Left
        {
            get { return Window.Left; }
            set => Window.Left = value;
        }

        public double Top
        {
            get { return Window.Top; }
            set => Window.Top = value;
        }

        public Brush Background
        {
            get { return Window.Background; }
            set => Window.Background = value;
        }

        public Brush BorderBrush
        {
            get { return Window.BorderBrush; }
            set => Window.BorderBrush = value;
        }

        public Thickness BorderThickness
        {
            get { return Window.BorderThickness; }
            set => Window.BorderThickness = value;
        }

        public bool AllowsTransparency
        {
            get { return Window.AllowsTransparency; }
            set => Window.AllowsTransparency = value;
        }

        public WindowStyle WindowStyle
        {
            get { return Window.WindowStyle; }
            set => Window.WindowStyle = value;
        }

        public bool ShowInTaskbar
        {
            get { return Window.ShowInTaskbar; }
            set => Window.ShowInTaskbar = value;
        }

        public ResizeMode ResizeMode
        {
            get { return Window.ResizeMode; }
            set => Window.ResizeMode = value;
        }

        public bool IsHitTestVisible
        {
            get { return Window.IsHitTestVisible; }
            set => Window.IsHitTestVisible = value;
        }

        public SizeToContent SizeToContent
        {
            get { return Window.SizeToContent; }
            set => Window.SizeToContent = value;
        }

        public Thickness Margin
        {
            get
            {
                if (Window.Content is FrameworkElement element)
                {
                    return element.Margin;
                }
                else
                {
                    return Window.Margin;
                }
            }
            set
            {
                if (Window.Content is FrameworkElement element)
                {
                    element.Margin = value;
                }
                else
                {
                    Window.Margin = value;
                }
            }
        }

        public Thickness Padding
        {
            get
            {
                if (Window.Content is Border element)
                {
                    return element.Padding;
                }
                else
                {
                    return Window.Padding;
                }
            }
            set
            {
                if (Window.Content is Border element)
                {
                    element.Padding = value;
                }
                else
                {
                    Window.Padding = value;
                }
            }
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        //private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDesktopWindow();

        //const uint SWP_NOACTIVATE = 0x0010;
        //const uint SWP_NOZORDER = 0x0004;

        private void WidgetWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(Window).Handle;
            IntPtr exStyle = GetWindowLongPtr(hWnd, GWL_EXSTYLE);
            //IntPtr desktopHandle = GetDesktopWindow();
            SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(exStyle.ToInt64() | WS_EX_TOOLWINDOW));
            //SetWindowPos(hWnd, desktopHandle, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOZORDER);
        }
    }
}

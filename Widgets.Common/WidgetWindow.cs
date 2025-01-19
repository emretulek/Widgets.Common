using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows;
using System.Drawing;

namespace Widgets.Common
{
    public partial class WidgetWindow
    {
        public Window Window;
        private IntPtr _wpfHwnd;

        public WidgetWindow(Window window)
        {
            Window = window;
            Window.SourceInitialized += WidgetWindow_SourceInitialized;
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
            //ResizeMode = widgetStruct.ResizeMode; //Applies widget manager after widget window is loaded
            SizeToContent = widgetStruct.SizeToContent;
            IsHitTestVisible = widgetStruct.IsHitTestVisible;
            //DesktopIntegration = widgetStruct.DesktopIntegration; //Applies widget manager after widget window is loaded
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

        public System.Windows.Media.Brush Background
        {
            get { return Window.Background; }
            set => Window.Background = value;
        }

        public System.Windows.Media.Brush BorderBrush
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

        public bool DesktopIntegration
        {
            get
            {
                return DeskTopHelper.GetParent(_wpfHwnd) > 0;
            }
            set
            {
                if (value)
                {
                    DesktopIntegrate();
                }
                else
                {
                    DesktopUnIntegrate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async void DesktopIntegrate()
        {
            var formHwnd = await DeskTopHelper.GetWorkerwContainer();
            var bounds = new Rectangle((int)Window.Left, (int)Window.Top, (int)Window.Width, (int)Window.Height);
            DeskTopHelper.SetParentExtended(_wpfHwnd, formHwnd, bounds);
        }

        /// <summary>
        /// 
        /// </summary>
        private void DesktopUnIntegrate()
        {
            DeskTopHelper.SetParent(_wpfHwnd, IntPtr.Zero);
        }

       
        /// <summary>
        /// Turn window into toolwindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WidgetWindow_SourceInitialized(object? sender, EventArgs e)
        {
            _wpfHwnd = new WindowInteropHelper(Window).Handle;
            IntPtr exStyle = DeskTopHelper.GetWindowLongPtr(_wpfHwnd, DeskTopHelper.GWL_EXSTYLE);
            DeskTopHelper.SetWindowLongPtr(_wpfHwnd, DeskTopHelper.GWL_EXSTYLE, new IntPtr(exStyle.ToInt64() | DeskTopHelper.WS_EX_TOOLWINDOW));
        }
    }
}

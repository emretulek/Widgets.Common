using System.Windows;
using System.Windows.Media;

namespace Widgets.Common
{
    public class WidgetDefaultStruct
    {
        public bool IsActive { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double MaxWidth { get; set; }
        public double MaxHeight { get; set; }
        public double MinWidth { get; set; }
        public double MinHeight { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public SolidColorBrush Background { get; set; }
        public SolidColorBrush BorderBrush { get; set; }
        public Thickness BorderThickness { get; set; }
        public Thickness Margin { get; set; }
        public Thickness Padding { get; set; }
        public bool AllowsTransparency { get; set; }
        public WindowStyle WindowStyle { get; set; }
        public ResizeMode ResizeMode { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public bool ShowInTaskbar { get; set; }
        public bool Dragable { get; set; }
        public bool IsHitTestVisible { get; set; }
        public bool DesktopIntegration { get; set; }

        public WidgetDefaultStruct()
        {
            IsActive = false;
            Width = 400;
            Height = 200;
            MaxWidth = 99999;
            MaxHeight = 99999;
            MinWidth = 0;
            MinHeight = 0;
            Top = 200;
            Left = 200;
            Background = PropertyParser.ToColorBrush("#20000000");
            BorderBrush = PropertyParser.ToColorBrush("#FF000000");
            BorderThickness = new Thickness(1);
            Margin = new Thickness(0);
            Padding = new Thickness(0);
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.CanResizeWithGrip;
            ShowInTaskbar = false;
            SizeToContent = SizeToContent.Manual;
            IsHitTestVisible = true;
            Dragable = true;
            DesktopIntegration = false;
        }
    };
}

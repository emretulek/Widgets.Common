namespace Widgets.Common
{
    public interface IPlugin
    {
        string Name { get; }
        string? ConfigFile { get;}
        WidgetDefaultStruct WidgetDefaultStruct();
        WidgetWindow WidgetWindow();
    }
}

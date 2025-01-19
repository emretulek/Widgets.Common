# Widgets.Common

IPlugin interface and helper classes for creating widgets plugins.

### IPlugin

```csharp
public interface IPlugin
{
    string Name { get; }
    string? ConfigFile { get;}
    WidgetDefaultStruct WidgetDefaultStruct();
    WidgetWindow WidgetWindow();
}
```

### IWidgetWindow

```csharp
public interface IWidgetWindow
{
    WidgetWindow WidgetWindow();
    static abstract WidgetDefaultStruct WidgetDefaultStruct();
}
```

### WidgetWindow

The class that gives the Widget properties to the created Window object. [WidgetWindow](https://github.com/emretulek/Widgets.Common/blob/master/Widgets.Common/WidgetWindow.cs) 

```csharp
//WidgetWindow instance
public WidgetWindow WidgetWindow()
{
    return new WidgetWindow(this);
}

// Default widget settings
public static WidgetDefaultStruct WidgetDefaultStruct()
{
    return new()
    {
        Padding = new Thickness(10)
    };
}
```

### Logger

This class is used to send and log exceptions caught in the widget to the widget manager.

```csharp
public static void Info(object message, string? pluginName = null, 
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = 0)

public static void Error(object message, string? pluginName = null,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = 0)

public static void Warning(object message, string? pluginName = null,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = 0)
```

```csharp
try{
 //code
}catch(Exception e){
  Logger.warning(e.Message)
}
```

### ProperyParser

It can be used for type conversion of properties of windows objects saved as string.

```csharp
public static bool ToBool(object? value)
public static float ToFloat(object? value, float _default = 0)
public static int ToInt(object? value, int _default = 0)
public static string ToString(object? value, string _default = "")
public static SolidColorBrush ToColorBrush(object? value, string _default = "#FFFFFF")
public static Thickness ToThickness(object? property, int _default = 1)
```

```csharp
 UsageText.FontSize = PropertyParser.ToFloat(config.GetValue("usage_font_size"));
 UsageText.Foreground = PropertyParser.ToColorBrush(config.GetValue("usage_foreground"));
```

### Config

You can create a simple json type config file to save the widgets' own settings.

```csharp
public readonly static string SettingsFile = "settings.widgetname.json";
private readonly Config config = new(SettingsFile);

public void LoadSettings()
{
    try
    {
        Settings.ReceivedColor = PropertyParser.ToString(config.GetValue("received_color"), Settings.ReceivedColor);
        Settings.SentColor = PropertyParser.ToString(config.GetValue("sent_color"), Settings.SentColor);
        Settings.TimeLine = PropertyParser.ToFloat(config.GetValue("graphic_timeline"), Settings.TimeLine);
        UsageText.FontSize = PropertyParser.ToFloat(config.GetValue("usage_font_size"));
        UsageText.Foreground = PropertyParser.ToColorBrush(config.GetValue("usage_foreground"));
    }
    catch (Exception)
    {
        config.Add("usage_font_size", UsageText.FontSize);
        config.Add("usage_foreground", UsageText.Foreground);
        config.Add("received_color", Settings.ReceivedColor);
        config.Add("sent_color", Settings.SentColor);
        config.Add("graphic_timeline", Settings.TimeLine);

        config.Save();
    }
}

```

### Schedule

Since many widgets consist of schedules and loops, a simple schedule class can be helpful.

```csharp
public string Secondly(Action action, int intervalInSeconds, DateTime? startTime = null)
public string Minutely(Action action, int intervalInMinutes, DateTime? startTime = null)
public string Hourly(Action action, int intervalInHours, DateTime? startTime = null)
public string Daily(Action action, int intervalInDays, DateTime? startTime = null)
public string Weekly(Action action, int intervalInWeeks, DateTime? startTime = null)
public string Monthly(Action action, int intervalInMonths, DateTime? startTime = null)
public string Yearly(Action action, int intervalInYears, DateTime? startTime = null)
```

```csharp
//start schedule
var scheduleId =schedule.Secondly(() => {
  Debug.WriteLine("after each 10 seconds")
}, 10);

// stop schedule
schedule.Stop(scheduleId)
```

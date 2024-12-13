namespace Widgets.Common
{
    public class LogMessage
    {
        public LogLevel Level { get; set; } = LogLevel.Info;
        public object Message { get; set; } = "";
        public string? PluginName { get; set; } = null;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string MemberName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int LineNumber { get; set; } = 0;
        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-ddTHH:mm:ss.fffZ}] [{Level}] [{PluginName}] {Message}  |   {MemberName} {FilePath} {LineNumber}";
        }
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
}

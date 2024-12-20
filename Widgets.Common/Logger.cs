using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Widgets.Common
{
    public class Logger
    {
        public static event EventHandler<LogMessage>? LogEvent;
        private static readonly ConcurrentQueue<string> _logQueue = new();
        private static readonly Schedule schedule = new();
        public readonly static string LogFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private  static string scheduleID = string.Empty;

        public static void Log(object message, 
            LogLevel level, 
            string? pluginName = null, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string filePath = "", 
            [CallerLineNumber] int lineNumber = 0)
        {
            var logMessage = new LogMessage
            {
                Message = message,
                Level = level,
                PluginName = pluginName ?? Helper.GetCallingAssemblyName(),
                Timestamp = DateTime.UtcNow,
                MemberName = memberName,
                FilePath = filePath,
                LineNumber = lineNumber
            };

            LogEvent?.Invoke(Helper.GetCallingAssemblyName(), logMessage);
        }

        public static void Info(object message, string? pluginName = null, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Log(message, LogLevel.Info, pluginName, memberName, filePath, lineNumber);
        }

        public static void Error(object message, string? pluginName = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Log(message, LogLevel.Error, pluginName, memberName, filePath, lineNumber);
        }

        public static void Warning(object message, string? pluginName = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Log(message, LogLevel.Warning, pluginName, memberName, filePath, lineNumber);
        }

        public static void BufferLog(string log)
        {
            _logQueue.Enqueue(log);
        }

        public static void WriteLogSchedule(int flushLogIntervalSec = 5)
        {
            scheduleID = schedule.Secondly(async() => await WriteFromBufferToFile(), flushLogIntervalSec);
        }

        private static async Task WriteFromBufferToFile()
        {
            try
            {
                if (_logQueue.IsEmpty) return;

                string date = DateTime.Now.ToString("yyyy_MM_dd");
                string fileName = $"{LogFileDir}/{date}_widgets.log";

                if (!Directory.Exists(LogFileDir))
                {
                    Directory.CreateDirectory(LogFileDir);
                }

                var logBuilder = new StringBuilder();
                while (_logQueue.TryDequeue(out var log))
                {
                    logBuilder.AppendLine(log);
                }

                await File.AppendAllLinesAsync(fileName, [logBuilder.ToString().TrimEnd()]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static async Task CloseWithFlush()
        {
            schedule.Stop(scheduleID);
            await WriteFromBufferToFile();
            _logQueue.Clear();
        }
    }
}

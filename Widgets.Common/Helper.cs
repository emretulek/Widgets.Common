using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Widgets.Common
{
    internal class Helper
    {
        // Name of the dll file called
        public static string? GetCallingAssemblyName()
        {
            var stackTrace = new StackTrace();
            foreach (var frame in stackTrace.GetFrames())
            {
                var assembly = frame.GetMethod()?.DeclaringType?.Assembly;
                if (assembly != null && assembly != Assembly.GetExecutingAssembly())
                {
                    return assembly.GetName().Name ?? null;
                }
            }

            return null;
        }

        // Path of the dll file called
        public static string? GetCallingAssemblyPath()
        {
            var stackTrace = new StackTrace();
            foreach (var frame in stackTrace.GetFrames())
            {
                var assembly = frame.GetMethod()?.DeclaringType?.Assembly;
                if (assembly != null && assembly != Assembly.GetExecutingAssembly())
                {
                    return Path.GetDirectoryName(assembly.Location) ?? null;
                }
            }

            return null;
        }
    }
}

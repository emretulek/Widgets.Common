using System.IO;

namespace Widgets.Common
{
    public class Config
    {
        public readonly static string PluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        private readonly string FilePath = Helper.GetCallingAssemblyPath() ?? "";
        public Dictionary<string, object> KeyValuePairs = [];

        /// <summary>
        /// 
        /// </summary>
        public Config(string fileName)
        {
            FilePath = Path.Combine(FilePath, fileName);
            KeyValuePairs = JsonFile.Read(FilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            KeyValuePairs[key] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            return JsonFile.Write(FilePath, KeyValuePairs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public object GetValue(string key)
        {
            if (KeyValuePairs.TryGetValue(key, out object? value))
            {
                return value;
            }

            throw new FileNotFoundException($"{key} Not Found in {FilePath}");
        }
    }
}

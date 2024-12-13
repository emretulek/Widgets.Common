using Newtonsoft.Json;
using System.IO;

namespace Widgets.Common
{
    internal class JsonFile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Dictionary<string,object> Read(string filePath)
        {
            var widgetConfig = new Dictionary<string, object>();
            try
            {
                string jsonString = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Dictionary<string,object>>(jsonString) ?? [];
            }
            catch (Exception ex)
            {
                Logger.Error($"Can not read config file. Reason: {ex.Message}");
            }

            return widgetConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="keyValuePairs"></param>
        public static bool Write(string filePath, Dictionary<string,object> keyValuePairs)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(keyValuePairs, Formatting.Indented);
                File.WriteAllText(filePath, jsonString);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Can not write config file. Reason: {ex.Message}");
                return false;
            }
        }
    }
}

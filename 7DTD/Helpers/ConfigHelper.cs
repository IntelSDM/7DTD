using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Cheat.Helpers
{
    class ConfigHelper
    {
        public static string SelectedConfig = "1";
        private static string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Hag\\");
        private static string GetConfigPath(string name = "1") { return ConfigPath + name + ".cfg"; }
        public static void CreateEnvironment()
        {
            if (!Directory.Exists(ConfigPath))
            {
                Directory.CreateDirectory(ConfigPath);
            }
            if (!File.Exists(GetConfigPath()))
                SaveConfig();
            else
                LoadConfig();
        }
        public static void SaveConfig(string name = "1", bool setconfig = false)
        {
            string path = GetConfigPath(name);
            string json = JsonConvert.SerializeObject(Globals.Config, Formatting.Indented);
            File.WriteAllText(path, json);
            if (setconfig)
                SelectedConfig = name;
            // draw something after this to show the user something happened or do a beep boop :triumph:

        }
        public static void LoadConfig(string name = "1")
        {
            if (File.Exists(GetConfigPath(name)))
            {
                string json = File.ReadAllText(GetConfigPath(name));
                Configs.Config s = JsonConvert.DeserializeObject<Configs.Config>(json);
                Globals.Config = s;
                SelectedConfig = name;

              //  ColourHandler.AddColours();

            }
        }
        public static List<string> GetConfigs()
        {
            List<string> files = new List<string>();
            DirectoryInfo d = new DirectoryInfo(ConfigPath);
            FileInfo[] Files = d.GetFiles("*.cfg");
            foreach (FileInfo file in Files)
            {
                    files.Add(file.Name.Substring(0, file.Name.Length));
              
            }
            return files;
        }
    }
}

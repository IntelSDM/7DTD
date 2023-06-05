using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;

namespace Cheat.Helpers
{
    class ConfigHelper
    {
        private static string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "7DaysCheat\\");
        private static string GetConfigPath(string name = "Default")
        {
            return ConfigPath + name + ".cfg"; 
        }
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
       
        private static readonly string Hash = "sgdsdgsdg32g9343hg973gb39ug34bibsdvbw2u2bv2u9v2v2n9bwv2gb29esdvs2q"; // cant make this dynamic or people cant share configs
        private static string DecryptStatic(string _cipherText)
        {

            try
            {
                byte[] _cipherBytes = Convert.FromBase64String(_cipherText);
                using (Aes _encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes _pdb = new Rfc2898DeriveBytes(Hash, new byte[] { 0xfd, 0xef, 0x32, 0x4f, 0xfa, 0x66, 0xa7, 0x42, 0x57, 0x95, 0x64, 0x75, 0x76 });
                    _encryptor.Key = _pdb.GetBytes(32);
                    _encryptor.IV = _pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, _encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(_cipherBytes, 0, _cipherBytes.Length);
                            cs.Close();
                        }
                        _cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return _cipherText;
        }
        private static string EncryptStatic(string _clearText)
        {


            byte[] _clearBytes = Encoding.Unicode.GetBytes(_clearText);
            using (Aes _encryptor = Aes.Create())
            {

                Rfc2898DeriveBytes _pdb = new Rfc2898DeriveBytes(Hash, new byte[] { 0xfd, 0xef, 0x32, 0x4f, 0xfa, 0x66, 0xa7, 0x42, 0x57, 0x95, 0x64, 0x75, 0x76 });
                _encryptor.Key = _pdb.GetBytes(32);
                _encryptor.IV = _pdb.GetBytes(16);
                using (MemoryStream _ms = new MemoryStream())
                {
                    using (CryptoStream _cs = new CryptoStream(_ms, _encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        _cs.Write(_clearBytes, 0, _clearBytes.Length);
                        _cs.Close();
                    }
                    _clearText = Convert.ToBase64String(_ms.ToArray());
                }
            }
            return _clearText;
        }
        public static void SaveConfig(string name = "Default")
        {
            string path = GetConfigPath(name);
            string json = JsonConvert.SerializeObject(Globals.Config, Formatting.Indented); // serialize it to the config format
            File.WriteAllText(path, EncryptStatic(json)); // save it encrypted by base64, helps against sigs

        }
        public static void LoadConfig(string name = "Default")
        {
            if (File.Exists(GetConfigPath(name)))
            {
                string json = DecryptStatic(File.ReadAllText(GetConfigPath(name))); // decrypt the config and read the contents
                Configs.Config s = JsonConvert.DeserializeObject<Configs.Config>(json);  // deserialize and cast it
                Globals.Config = s;

            }
        }
        public static List<string> GetConfigs()
        {
            List<string> files = new List<string>();
            DirectoryInfo d = new DirectoryInfo(ConfigPath);
            FileInfo[] Files = d.GetFiles("*.cfg"); // get an array of all files ending in .cfg
            foreach (FileInfo file in Files)
            {
                    files.Add(file.Name.Substring(0, file.Name.Length));
              
            }
            return files;
        }
    }
}

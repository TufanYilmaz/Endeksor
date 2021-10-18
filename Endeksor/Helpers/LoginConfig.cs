using System;
using System.IO;

namespace App4.Helpers
{
    public static class LoginConfig
    {
        static readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "AbysisHandLoginConfig.con");
        public static bool ConfigFileExist()
        {
            return File.Exists(filePath);
        }
        public static bool SetLoginInfo(string LoginInformation="|")
        {
            bool result = false;
            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.Write(LoginInformation);
                    }
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public static bool CreteConfigFile()
        {
            bool result = false;
            try
            {
                if (!File.Exists(filePath))
                {
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.Write("|");
                    }
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public static string GetLoginInfo()
        {
            string content = "";
            try
            {

                using (var streamReader = new StreamReader(filePath)) // with and without file://
                {
                    content = streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return content;
            }
            return content;
        }
    }
}
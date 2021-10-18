using System;
using System.IO;

namespace App4.Helpers
{
    public static class SerialConfig
    {
        static readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "AbysisHandSerialConfig.con");
        public static bool ConfigFileExist
        {
            get
            {
                return File.Exists(filePath);
            }
        }

        public static bool CreteSerialConfigFile()
        {
            bool result = false;
            try
            {
                string serial = string.Empty;
                //Random serialRand = new Random();
                serial = new Random().Next(100000, 999999).ToString();
                if (!ConfigFileExist)
                {
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.Write(serial);
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
        public static string GetSerialConfigInfo()
        {
            string content = "";
            try
            {

                using (var streamReader = new StreamReader(filePath))
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
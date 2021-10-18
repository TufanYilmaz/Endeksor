using System;
using System.IO;
using System.Text.RegularExpressions;

namespace App4.Helpers
{
    public static class ServiceConfig
    {
        static readonly string URLIPValidatorRegex =
            @"^http(s)?://(([01]?[0-9][0-9]?|2[0-4][0-9]|25[0-5])(\.([01]?[0-9][0-9]?|2[0-4][0-9]|25[0-5])){3})+(:?[0-9][0-9]?)+(/?)+([0-9a-zA-Z]*)+(/?)$";
        static readonly string URLValidatorRegex = @"^http(s)?://(([\w-]+.)+[\w-]+(/[\w- ./?%&=])?)+((:?[0-9][0-9]?)?)+(/?)+([0-9a-zA-Z]*)+(/?)$";

        //regex for link --^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?(:?[0-9][0-9]?)+(/?)+([0-9a-zA-Z]*)+(/?)$
        //regex for linkv2 -- ^http(s)?://(([\w-]+.)+[\w-]+(/[\w- ./?%&=])?)+(:?[0-9][0-9]?)+(/?)+([0-9a-zA-Z]*)+(/?)$
        //regex for linkv3 -- ^http(s)?://(([\w-]+.)+[\w-]+(/[\w- ./?%&=])?)+((:?[0-9][0-9]?)?)+(/?)+([0-9a-zA-Z]*)+(/?)$
        static readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "AbysisHandConfig.con");
        //public static readonly string defaultServiceURL = "http://213.74.195.138:90/Services";//Kosbi
        //public static readonly string defaultServiceURL = "http://elk.mimarsinanosb.org.tr:90/Services";//Mosb
        public static readonly string defaultServiceURL = "http://95.6.50.139:90/Services";//Akhisar

        //95.6.50.139

        public static bool ConfigFileExist()
        {
            return File.Exists(filePath);
        }
        public static bool SetServiceUrl(string EndPointAdress)
        {
            bool result = false;
            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.Write(EndPointAdress); 
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
                        writer.Write(defaultServiceURL);
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
        public static string GetServiceURL()
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
        public static bool URLValidator(string endpointaddress)
        {
            return ((new Regex(URLValidatorRegex)).IsMatch(endpointaddress) || (new Regex(URLIPValidatorRegex)).IsMatch(endpointaddress));
        }
        
    }
}
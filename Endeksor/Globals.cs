using App4.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace App4
{
    public static class Globals
    {
        public static bool isLogin = false;
        public const string DateTimeFormat = "dd/MM/yyyy HH:mm";
        public static string SessionId = new Guid().ToString();
        public static string DepartmentCode = "";
        public static Subscribers.department Department = new Subscribers.department();
        public static Authentication.period Period = new Authentication.period();
        public static Authentication.readgroup SelectedReadGroup = new Authentication.readgroup();
        public static List<Authentication.HandTerminal> AuthorizedTerminals = new List<Authentication.HandTerminal>();
        public static Authentication.HandTerminal SelectedHandTerminal = new Authentication.HandTerminal();
        //public static DataTable Indexes;
        public static List<Indexes.baseSubscription> Indexes;
        public static int DepartmentId = -1;
        
        public static int TerminalId = 1;
        //public static List<Subscriber> Subscribers;

        public static void AssignTo(this object source, object dest)
        {
            Type typeB = dest.GetType();
            foreach (PropertyInfo property in source.GetType().GetProperties())
            {
                if (!property.CanRead || (property.GetIndexParameters().Length > 0))
                {
                    continue;
                }
                PropertyInfo other = typeB.GetProperty(property.Name);
                if ((property.PropertyType.IsClass) && (property.PropertyType != typeof(string)))
                {
                    object src = property.GetValue(source, null);
                    if (src != null)
                    {
                        object trg = other.GetValue(dest, null);
                        src.AssignTo(trg);
                    }
                }
                else
                if ((other != null) && (other.CanWrite))
                {
                    other.SetValue(dest, property.GetValue(source, null), null);
                }
            }
        }
        public static string Right(this string sValue, int length)
        {
            if (string.IsNullOrEmpty(sValue))
            {
                sValue = string.Empty;
            }
            else if (sValue.Length > length)
            {
                sValue = sValue.Substring(sValue.Length - length, length);
            }
            return sValue;
        }
        public static string Left(this string sValue, int length)
        {
            if (string.IsNullOrEmpty(sValue))
            {
                sValue = string.Empty;
            }
            else if (sValue.Length > length)
            {
                sValue = sValue.Substring(0, length);
            }
            return sValue;
        }
    }
}
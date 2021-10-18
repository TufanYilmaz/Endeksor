using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using App4.Models;

namespace App4.Helpers
{
    static class SubscriberHelper
    {
        public static List<Subscriber> GetSubscribers(DataTable dataSubscribers)
        {
            List<Subscriber> result = new List<Subscriber>();
            foreach (DataRow item in dataSubscribers.Rows)
            {
                result.Add(
                    new Subscriber()
                    {
                        Id = Convert.ToInt32(item["SUB_ID"]),
                        Code = item["CODE"].ToString(),
                        Info = item["INFO"].ToString(),
                        NewValue = item["NEWVALUE"] != DBNull.Value ? Convert.ToInt32(item["NEWVALUE"]) :-2
                    });
            }
            List<Subscriber> disResult = result.GroupBy(s => s.Id).Select(S => S.First()).ToList();
            return disResult;
        }
        //public static 
    }
}
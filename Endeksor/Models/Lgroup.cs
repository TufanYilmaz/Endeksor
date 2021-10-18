using System;

using App4.Indexes;

namespace App4.Models
{
    public class Lgroup
    {
        public readgroup group;

        public Lgroup(readgroup item)
        {
            group = item;
        }

        public override string ToString()
        {
            return string.Format("{0:" + Globals.DateTimeFormat + "}", group.DateTime);
        }
    }
}
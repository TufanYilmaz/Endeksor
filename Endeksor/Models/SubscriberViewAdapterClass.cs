using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App4.Models
{
    public class SubscriberViewAdapterClass
    {
        public int LINE_ID { get; set; }
        public string METER_CODE { get; set; }
        public int METER_ID { get; set; }
        public int ID { get; set; }
        public int INDEXLINES_ID { get; set; }
        public string INDEXLINES_CODE { get; set; }
        public string INDEXLINES_INFO { get; set; }
        public double PRIORVALUE { get; set; }
        public double NEWVALUE { get; set; }
        public bool IsOrdered { get; set; }
        public bool IsEdited { get; set; }

        public bool IsSelected { get; set; } = false;
    }
}
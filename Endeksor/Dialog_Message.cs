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

namespace App4
{
    class Dialog_Message:DialogFragment
    {
        Button btnClose;
        TextView tvAdr1;
        TextView tvAdr2;
        private string Adr1 { get; set; }
        private string Adr2 { get; set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_subscriber,container, false);

            btnClose=view.FindViewById<Button>(Resource.Id.btnFragmentSubscriberClose);
            btnClose.Click += delegate { Dismiss(); };

            if (Arguments != null)
            {
                Adr1 = string.IsNullOrEmpty(Arguments.GetString("Adr1")) ? "Bilgi Yok" : Arguments.GetString("Adr1");
                Adr2 =string.IsNullOrEmpty( Arguments.GetString("Adr2")) ? "Bilgi Yok": Arguments.GetString("Adr2");
            }

            tvAdr1  =view.FindViewById<TextView>(Resource.Id.Adress);
            tvAdr1.Text = Adr1;
            tvAdr2 = view.FindViewById<TextView>(Resource.Id.AdressInfo);
            tvAdr2.SetTextColor(new Android.Graphics.Color(255, 50, 50));
            tvAdr2.Text = Adr2;

            return view;
        }
    }
}
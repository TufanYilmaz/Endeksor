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
    class Dialog_Subsciber: DialogFragment
    {

        private TextView txtAdress;
        private Button btnClose;
        public string Adress { get; set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_subscriber, container, false);
            if(Arguments!=null)
            {
                Adress = Arguments.GetString("Adress");
            }
            btnClose = view.FindViewById<Button>(Resource.Id.btnFragmentSubscriberClose);
            btnClose.Click += (s, e) => {
                Dismiss();
            };
            txtAdress = view.FindViewById<TextView>(Resource.Id.AdressInfo);
            txtAdress.Text = Adress;


            return view;
        }
    }
}
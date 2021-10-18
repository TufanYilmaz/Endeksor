using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App4.Helpers;

namespace App4
{
    class Dialog_ServiceConfig : DialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.serviceEndPointLayout, container, false);
            ManageUIs(view);
            ManageData();

            if (!SerialConfig.ConfigFileExist)
            {
                SerialConfig.CreteSerialConfigFile();
            }
            string serial = SerialConfig.GetSerialConfigInfo();


            //((TextView)view.FindViewById(Resource.Id.tvDevicesSerialNo)).Text = Build.Serial;
            ((TextView)view.FindViewById(Resource.Id.tvDevicesSerialNo)).Text = serial;
            return view;
        }



        EditText etServiceEndPoint;
        Button btnClose;
        Button btnSaveClose;
        TextView tvServiceInfo;

        private void ManageUIs(View view)
        {
            etServiceEndPoint = view.FindViewById<EditText>(Resource.Id.etServiceEndPoint);
            btnClose = view.FindViewById<Button>(Resource.Id.btnFragmentURLConfigClose);
            btnClose.Click += delegate { Dismiss(); };
            btnSaveClose = view.FindViewById<Button>(Resource.Id.btnFragmentURLConfigSaveClose);
            btnSaveClose.Click += BtnSaveClose_Click;
            tvServiceInfo = view.FindViewById<TextView>(Resource.Id.tvServiceInfo);
        }
        int iTry = 0;
        private void BtnSaveClose_Click(object sender, EventArgs e)
        {
            if (Helpers.ServiceConfig.URLValidator(etServiceEndPoint.Text))
            {
                Helpers.ServiceConfig.SetServiceUrl(etServiceEndPoint.Text);
                Dismiss();
            }
            else
                tvServiceInfo.Text = $"Geçerli Bir Servis Adresi Giriniz ({iTry++}.deneme)";
        }
        private void ManageData()
        {
            etServiceEndPoint.Text = Helpers.ServiceConfig.GetServiceURL();
        }
    }
}
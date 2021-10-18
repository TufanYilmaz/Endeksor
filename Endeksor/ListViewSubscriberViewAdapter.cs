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
using App4.Models;

namespace App4
{
    class ListViewSubscriberViewAdapter : BaseAdapter<SubscriberViewAdapterClass>
    {
        private List<SubscriberViewAdapterClass> subscriberViews;
        private Context sContext;

        public ListViewSubscriberViewAdapter(Context context,List<SubscriberViewAdapterClass> subscriberViews)
        {
            SContext = context;
            this.subscriberViews = subscriberViews;
        }

        public override SubscriberViewAdapterClass this[int position] { get { return subscriberViews[position]; } }

        public override int Count { get { return subscriberViews.Count; } }

        public Context SContext { get => sContext; set => sContext = value; }

        public override long GetItemId(int position)
        {
            //Toast.MakeText(SContext, "(Method Eklenecek" + position, ToastLength.Short).Show();
            return position;
        }
        public long GetRowNum(int position)
        {
            return Convert.ToInt32(subscriberViews[position].ID);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row==null)
            {
                row = LayoutInflater.From(sContext).Inflate(Resource.Layout.listViewSubscriberViewAdapter, null, false);
            }
            TextView tvROW = row.FindViewById<TextView>(Resource.Id.tvROW);
            TextView tvMETER_CODE = row.FindViewById<TextView>(Resource.Id.tvMETER_CODE);
            TextView tvINDEXLINES_CODE = row.FindViewById<TextView>(Resource.Id.tvINDEXLINES_CODE);
            TextView tvINDEXLINES_INFO = row.FindViewById<TextView>(Resource.Id.tvINDEXLINES_INFO);
            TextView tvNEWVALUE = row.FindViewById<TextView>(Resource.Id.tvNEWVALUE);
            TextView tvROWNumber = row.FindViewById<TextView>(Resource.Id.tvROWNumber);
            tvROWNumber.Text = subscriberViews[position].LINE_ID.ToString();
            tvROW.Text = subscriberViews[position].ID.ToString();
            tvMETER_CODE.Text = subscriberViews[position].METER_CODE;
            tvINDEXLINES_CODE.Text = subscriberViews[position].INDEXLINES_CODE;
            tvINDEXLINES_INFO.Text = subscriberViews[position].INDEXLINES_INFO;
            tvNEWVALUE.Text = subscriberViews[position].NEWVALUE == -1 || 
                double.IsNaN(subscriberViews[position].NEWVALUE) ? "" : 
                subscriberViews[position].NEWVALUE.ToString();
            if (subscriberViews[position].IsSelected)
                row.SetBackgroundColor(new Android.Graphics.Color(50, 250, 255));
            else
                row.SetBackgroundColor(new Android.Graphics.Color(255, 255, 255));

            return row;
        }

    }
}
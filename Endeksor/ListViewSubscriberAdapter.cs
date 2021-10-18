using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;

namespace App4
{
    class ListViewSubscriberAdapter : BaseAdapter<Indexes.baseSubscription>
    {
        private List<Indexes.baseSubscription> Subs;
        private Context sContext;
        
        public ListViewSubscriberAdapter(Context context, List<Indexes.baseSubscription> subs)
        {
            Subs = subs;
            sContext = context;
        }

        public override int Count
        {
            get { return Subs.Count; }
        }

        public Context SContext { get => sContext; set => sContext = value; }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override Indexes.baseSubscription this[int position]
        {
            get { return Subs[position]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
            {
                row = LayoutInflater.From(sContext).Inflate(Resource.Layout.listViewSubscriberRow, null, false);
            }


            TextView txtCode = row.FindViewById<TextView>(Resource.Id.txtCode);
            txtCode.Text = Subs[position].Code;

            TextView txtInfo = row.FindViewById<TextView>(Resource.Id.txtInfo);
            txtInfo.Text =Subs[position].Info;

            TextView txtId = row.FindViewById<TextView>(Resource.Id.txtId);
            txtId.Text = Subs[position].Id.ToString();
            //var bgColor = new Android.Graphics.Color(240,240,240);
            var bgColor = new Android.Graphics.Color(255, 50, 50);//Kırmızı
            //if (Subs[position].Meters.Count() <= 1)
            //{
            //    var validForOneAll = Subs[position].Meters[0].Values.All(x => !double.IsNaN(x.NewValue.RawValue) || x.NewValue.RawValue != -1);
            //    var validForOneAny = Subs[position].Meters[0].Values.Any(x => x.NewValue.RawValue >0);
            //    if (validForOneAll)
            //        bgColor = new Android.Graphics.Color(50, 180, 50);//koyu yeşil
            //    if(validForOneAny)
            //        bgColor = new Android.Graphics.Color(50, 255, 50);//Yeşil
            //    //else
            //    //{
            //    //    if (validForOneAny)
            //    //        bgColor = new Android.Graphics.Color(255, 50, 50);//Kırmızı
            //    //    else
            //    //        bgColor = new Android.Graphics.Color(50, 255, 50);//Yeşil
            //    //}
                
            //}
            //else
            //{
            var validForAllMeterAllLines= Subs[position].Meters.All(x => x.Values.All(m => double.IsNaN(m.NewValue.RawValue) || m.NewValue.RawValue == -1));
            var validForMultiAny= Subs[position].Meters.All(x => x.Values.Any(m => double.IsNaN(m.NewValue.RawValue) || m.NewValue.RawValue == -1));
            //var validForMultiAll = Subs[position].Meters.All(x => x.Values.Any(m => !double.IsNaN(m.NewValue.RawValue) || m.NewValue.RawValue != -1));
            if(validForAllMeterAllLines)
                bgColor = new Android.Graphics.Color(255, 50, 50);//Kırmızı
            else if(validForMultiAny)
                bgColor = new Android.Graphics.Color(50, 255, 50);//Yeşil
            else 
                bgColor = new Android.Graphics.Color(50, 180, 50);//Koyu Yeşil
            //bgColor = new Android.Graphics.Color(240, 180, 50);//Koyu Turuncu
            //}
            //var valid = (from m in Subs[position].Meters
            //             group m by m.Id into L
            //             select new
            //             {
            //                 SumOfNewValue = (from l in L
            //                                  select l.Values.Sum(x => x.NewValue.RawValue)).First()
            //             }).Select(x => x.SumOfNewValue).ToList();

            row.SetBackgroundColor(bgColor);
           
            return row;
        }
    }
}
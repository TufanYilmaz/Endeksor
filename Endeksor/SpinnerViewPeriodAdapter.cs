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
using App4.Invoices;
using App4.Indexes;
using Java.Lang;
using App4.Models;

namespace App4
{
    class SpinnerViewPeriodAdapter : BaseAdapter
    {
        private Context sContext;
        private List<Lperiod> lperiods;
        private List<Lgroup> lgroups;
        private bool isLperiod = false;
        private bool isLgroup = false;

        public SpinnerViewPeriodAdapter(Context sContext, List<Lperiod> lperiods)
        {
            this.sContext = sContext;
            this.lperiods = lperiods;
            isLperiod = true;
        }

        public SpinnerViewPeriodAdapter(Context sContext, List<Lgroup> lgroups)
        {
            this.sContext = sContext;
            this.lgroups = lgroups;
            isLgroup = true;
        }


        public override int Count
        {
            get
            {
                if(isLperiod)
                    return lperiods.Count;
                if (isLgroup)
                    return lgroups.Count;
                return 0;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
            {
                row = LayoutInflater.From(sContext).Inflate(Resource.Layout.spinnerViewPeriodAdapter, null, false);
            }
            TextView txtPeriodInfo = row.FindViewById<TextView>(Resource.Id.txtPeriod);
            TextView txtPeriodId = row.FindViewById<TextView>(Resource.Id.txtPeriodId);
            if(isLgroup)
            {
                txtPeriodInfo.Text = lgroups[position].group.DateTime.ToString();
                txtPeriodId.Text = lgroups[position].group.Id.ToString();
            }
            if (isLperiod)
            {
                txtPeriodInfo.Text = lperiods[position].Period.Info;
                txtPeriodId.Text = lperiods[position].Period.Id.ToString();
            }


                return row;
        }
        public Lperiod GetSelectedPeriod(int position)
        {
            return lperiods[position];
           
        }
    }
}
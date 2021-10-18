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
using App4.Authentication;
using Java.Lang;

namespace App4
{
    class HandTerminalSpinnerAdapter : BaseAdapter<HandTerminal>
    {
        Context sContext;
        List<HandTerminal> Terminals;
        public override HandTerminal this[int position] => Terminals[position];

        public override int Count => Terminals.Count;

        public override long GetItemId(int position)
        {
            return Terminals[position].Id;
        }
        public HandTerminalSpinnerAdapter(Context context,List<HandTerminal> terminals)
        {
            sContext = context;
            Terminals = terminals;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
            {
                row = LayoutInflater.From(sContext).Inflate(Resource.Layout.spinnerHandTerminalAdapter, null, false);
            }
            TextView tvHandTerminalId = row.FindViewById<TextView>(Resource.Id.tvSpinnerHandTerminalID);
            TextView tvHandTerminalCode = row.FindViewById<TextView>(Resource.Id.tvSpinnerHandTerminalCode);

            tvHandTerminalId.Text = Terminals[position].Id.ToString();
            tvHandTerminalCode.Text = Terminals[position].Code;
            
            return row;
        }
    }
}
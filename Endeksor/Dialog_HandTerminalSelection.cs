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

namespace App4
{
    public class OnTerminalSelectedEventArgs : EventArgs
    {
        //private string username;
        //private string password;
        //private string department;
        public int Id { get; set; }
        
        public OnTerminalSelectedEventArgs(int Id)
        {
            this.Id = Id;
        }
    }
    class Dialog_HandTerminalSelection: DialogFragment
    {
        Spinner spAuthHandTerminals;
        Button btnSelectHandTerminal;
        public event EventHandler<OnTerminalSelectedEventArgs> TerminalSelectedComplate;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_selectHandTerminal, container, false);
            spAuthHandTerminals = view.FindViewById<Spinner>(Resource.Id.spnAuthTerminals);

            HandTerminalSpinnerAdapter terminalSpinnerAdapter = new HandTerminalSpinnerAdapter(Context, Globals.AuthorizedTerminals);
            spAuthHandTerminals.Adapter = terminalSpinnerAdapter;
            
            btnSelectHandTerminal = view.FindViewById<Button>(Resource.Id.btnSelectHandTerminal);
            btnSelectHandTerminal.Click += BtnSelectHandTerminal_Click;
            return view;
        }

        private void BtnSelectHandTerminal_Click(object sender, EventArgs e)
        {

            TerminalSelectedComplate.Invoke(this, new OnTerminalSelectedEventArgs((int)spAuthHandTerminals.SelectedItemId));

            Dismiss();
        }

    }
    
}
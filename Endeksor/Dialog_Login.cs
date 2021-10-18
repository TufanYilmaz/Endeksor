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
    public class OnLoginEventArgs : EventArgs
    {
        //private string username;
        //private string password;
        //private string department;
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Department { get; set; }
        public bool PasswordSave { get; set; }
        public OnLoginEventArgs(string user, string pass, int dep, bool passwordSave = false)
        {
            UserName = user;
            Password = pass;
            Department = dep;
            PasswordSave = passwordSave;
        }
    }
    class Dialog_Login : DialogFragment
    {
        Button btnLogin;
        EditText txtUsername;
        EditText txtPassword;
        Spinner cmbDepartment;
        CheckBox chkPasswordSave;
        public event EventHandler<OnLoginEventArgs> SignUpComplate;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.login_fragment, container, false);
            btnLogin = view.FindViewById<Button>(Resource.Id.btnLogin);
            txtUsername = view.FindViewById<EditText>(Resource.Id.txtUserName);
            txtPassword = view.FindViewById<EditText>(Resource.Id.txtPassword);
            cmbDepartment = view.FindViewById<Spinner>(Resource.Id.cmbDepartmentId);
            //TO DO: departmanları adapter ile bağla
            chkPasswordSave = view.FindViewById<CheckBox>(Resource.Id.chkPasswordSave);
            btnLogin.Click += BtnLogin_Click;
            MangeSavedPassword();
            return view;
        }

        private void MangeSavedPassword()
        {
            //Helpers.LoginConfig.SetLoginInfo("|");
            if (Helpers.LoginConfig.ConfigFileExist())
            {
                var content = Helpers.LoginConfig.GetLoginInfo().Split('|');
                if (content.Length > 1)
                {
                    if (!string.IsNullOrWhiteSpace(content[0]))
                        txtUsername.Text = content[0];
                    if (!string.IsNullOrWhiteSpace(content[1]))
                        txtPassword.Text = content[1];
                }
            }
            else
                Helpers.LoginConfig.CreteConfigFile();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Toast.MakeText(this,txt)
            //Toast.MakeText(null, cmbDepartment.SelectedItem.ToString() + "-"+ txtUsername.Text + "-" + txtPassword.Text, ToastLength.Long);
            //Globals.sDepartment = cmbDepartment.SelectedItem.ToString();
            //Globals.sUserName = txtUsername.Text;
            //Globals.sPassword = txtPassword.Text;
            //#pragma warning disable CS0618
            //            ProgressDialog loginProgressDialog = new ProgressDialog(this.Activity);
            //            loginProgressDialog.SetMessage(GetString(Resource.String.loginMessage));
            //            loginProgressDialog.Show();
            //#pragma warning restore CS0618
            if (cmbDepartment.SelectedItemPosition <= 0)
            {
                Toast.MakeText(Context, "Departman Seçiniz", ToastLength.Short).Show();
                return;
            }
            string deviceInfo = "~|" + Helpers.SerialConfig.GetSerialConfigInfo() + "|";
            //string deviceInfo = "~|" + "U8WSVW6SKVP7VGEQ" + "|";
            //string denemeSerial = Build.GetSerial();
            SignUpComplate.Invoke(this, new OnLoginEventArgs(txtUsername.Text + deviceInfo, txtPassword.Text, cmbDepartment.SelectedItemPosition, chkPasswordSave.Checked));

            //loginProgressDialog.Dismiss();
            //MainActivity.
            this.Dismiss();
        }
        public override void Dismiss()
        {
            base.Dismiss();
        }
    }
}
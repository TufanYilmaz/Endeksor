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
using App4.Models;

namespace App4
{
    //public class OnPeriodSelectedEventArgs : EventArgs
    //{
    //    //private string username;
    //    //private string password;
    //    //private string department;

    //    public OnPeriodSelectedEventArgs()
    //    {        }
    //}

    [Activity(Label = "PeriodActivity")]
    public class PeriodActivity : Activity
    {
        //public class OnTerminalSelectedEventArgs : EventArgs
        //{

        //    public int Id { get; set; }

        //    public OnTerminalSelectedEventArgs(int Id)
        //    {
        //        this.Id = Id;
        //    }
        //}


        private List<Lperiod> Periods = new List<Lperiod>();
        private List<Lgroup> Groups = new List<Lgroup>();
        private Spinner cmbActivePeriod;
        private Spinner cmbIndexTableDate;
        private Button btnCreateNewTable;
        private Button btnCancel;
        private Button btnChoose;
        SpinnerViewPeriodAdapter periodAdapter;
        SpinnerViewPeriodAdapter groupAdapter;
        Invoices.Invoices InvoicesClient = new Invoices.Invoices();
        Indexes.Indexes IndexesClient = new Indexes.Indexes();
        private Lperiod SelectedPeriod = null;
        private Lgroup SelectedGroup = null;
        //public event EventHandler<OnPeriodSelectedEventArgs> PeriodSelectedComplate;
        private void InitializeUIComponents()
        {
            cmbActivePeriod = FindViewById<Spinner>(Resource.Id.cmbActivePeriod);
            cmbActivePeriod.ItemSelected += CmbActivePeriod_ItemSelected;

            cmbIndexTableDate = FindViewById<Spinner>(Resource.Id.cmbIndexTableDate);
            cmbIndexTableDate.ItemSelected += CmbIndexTableDate_ItemSelected;

            btnCreateNewTable = FindViewById<Button>(Resource.Id.btnCreateNewTable);
            btnCreateNewTable.Click += BtnCreateNewTable_Click;

            btnCancel = FindViewById<Button>(Resource.Id.btnPeriodCancel);
            btnCancel.Click += (s, e) => { Finish(); };

            btnChoose = FindViewById<Button>(Resource.Id.btnPeriodChoose);
            btnChoose.Click += BtnChoose_Click;
        }

        private void BtnCreateNewTable_Click(object sender, EventArgs e)
        {
            var result = IndexesClient.CreateDefaultReadGroup(Globals.SessionId,Globals.DepartmentId, SelectedPeriod.Period.Id);
            if (result.Result.Id == 0) LoadGroups(Periods[cmbActivePeriod.SelectedItemPosition]);
            else Toast.MakeText(this,result.Result.Info,ToastLength.Short).Show();
        }

        private void CmbIndexTableDate_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SelectedGroup = Groups[e.Position];
        }

        private void CmbActivePeriod_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Lperiod tmpLperiod = Periods[e.Position];
            
            if (SelectedPeriod != tmpLperiod )
                LoadGroups(tmpLperiod);
            if(cmbIndexTableDate.Count>0)
            {
                btnCreateNewTable.Enabled = false;
            }
        }

        private void BtnChoose_Click(object sender, EventArgs e)
        {
            if ((SelectedPeriod != null) && (SelectedGroup != null))
            {
                SelectedPeriod.Period.AssignTo(Globals.Period);
                SelectedGroup.group.AssignTo(Globals.SelectedReadGroup);
                //PeriodSelectedComplate.Invoke(this, new OnPeriodSelectedEventArgs());
                Finish();
            }
            else
                Toast.MakeText(this, "Geçerli bir dönem ve tablo tarihi seçiniz.", ToastLength.Long).Show();
                //MessageBox.Show("Geçerli bir dönem ve tablo tarihi seçiniz.");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_period);
            InitializeUIComponents();
            GetPeriods();
            //Toast.MakeText(this, "Periyot seçim bitti", ToastLength.Short).Show();
        }
        private void GetPeriods()
        {
            cmbActivePeriod.Adapter = null;
            Periods.Clear();
            var result = InvoicesClient.GetPeriods(Globals.SessionId,Globals.DepartmentId, "");
            if(result.Result.Id==0)
            {
                foreach (var item in result.Value)
                {
                    Periods.Insert(0, new Lperiod(item));
                }
                int c = 0;
                int index = -1;
                foreach (var item in Periods)
                {
                    if (index < 0)
                    {
                        if (item.Period.Id == Globals.Period.Id)
                            index = c;
                    }
                   // ePeriods.Items.Add(item);
                    c++;
                }
                //ePeriods.SelectedIndex = index;
                periodAdapter = new SpinnerViewPeriodAdapter(this, Periods);
                cmbActivePeriod.Adapter = periodAdapter;
                cmbActivePeriod.SetSelection(index);

            }
        }
        private void LoadGroups(Lperiod lperiod)
        {
            SelectedPeriod = lperiod;
            cmbIndexTableDate.Adapter = null;
            Groups.Clear();
            var result = IndexesClient.GetReadGroups(Globals.SessionId,Globals.DepartmentId, SelectedPeriod.Period.Id);
            SelectGroup(result);
        }
        private void SelectGroup(ReadGroupResult result)
        {
            if (result.Result.Id == 0)
            {
                foreach (var item in result.Value)
                {
                    Groups.Insert(0, new Lgroup(item));
                }
                int c = 0;
                int index = -1;
                foreach (var item in Groups)
                {
                    if (index < 0)
                    {
                        if (Globals.SelectedReadGroup != null)
                            if (item.group.Id == Globals.SelectedReadGroup.Id)
                                index = c;
                    }
                    //eGroups.Items.Add(item);
                    c++;
                }
                groupAdapter = new SpinnerViewPeriodAdapter(this, Groups);
                cmbIndexTableDate.Adapter = groupAdapter;
                if (index == -1 && (cmbActivePeriod.Count > 0)) index = 0;
                cmbIndexTableDate.SetSelection(index);
                btnCreateNewTable.Visibility = cmbIndexTableDate.SelectedItemPosition == -1 ? ViewStates.Invisible : ViewStates.Visible;
                //if ((index == -1) && (eGroups.Items.Count > 0)) index = 0;
                //eGroups.SelectedIndex = index;
                //btnNewGroup.Visible = eGroups.SelectedIndex == -1;
            }
        }
    }
}
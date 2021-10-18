using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App4.Authentication;
using App4.Indexes;
using App4.Invoices;
using App4.Models;

namespace App4
{
    public class OnPeriodSelectedEventArgs : EventArgs
    {
        public int Id { get; set; }
        public string Msg1 { get; set; }
        public string Msg2 { get; set; }

        public OnPeriodSelectedEventArgs()
        { }
    }
    class Dialog_Period :DialogFragment
    {
        private List<Lperiod> Periods = new List<Lperiod>();
        private List<Lgroup> Groups = new List<Lgroup>();
        private Spinner cmbActivePeriod;
        private Spinner cmbIndexTableDate;
        private Button btnCreateNewTable;
        private Button btnCancel;
        private Button btnChoose;
        SpinnerViewPeriodAdapter periodAdapter;
        SpinnerViewPeriodAdapter groupAdapter;
        //Invoices.Invoices InvoicesClient = new Invoices.Invoices();
        //Indexes.Indexes IndexesClient = new Indexes.Indexes();
        private Lperiod SelectedPeriod = null;
        private Lgroup SelectedGroup = null;
        public event EventHandler<OnPeriodSelectedEventArgs> PeriodSelectedComplate;
        private void InitializeUIComponents(View view)
        {
            cmbActivePeriod = view.FindViewById<Spinner>(Resource.Id.cmbfActivePeriod);
            cmbActivePeriod.ItemSelected += CmbActivePeriod_ItemSelected;

            cmbIndexTableDate = view.FindViewById<Spinner>(Resource.Id.cmbfIndexTableDate);
            cmbIndexTableDate.ItemSelected += CmbIndexTableDate_ItemSelected;

            btnCreateNewTable =view. FindViewById<Button>(Resource.Id.btnfCreateNewTable);
            btnCreateNewTable.Click += BtnCreateNewTable_Click;

            btnCancel = view.FindViewById<Button>(Resource.Id.btnfPeriodCancel);
            btnCancel.Click += (s, e) => { Dismiss(); };

            btnChoose = view.FindViewById<Button>(Resource.Id.btnfPeriodChoose);
            btnChoose.Click += BtnChoose_Click;
            btnChoose.Enabled = false;
        }

        private void BtnCreateNewTable_Click(object sender, EventArgs e)
        {
            Indexes.BaseResult result = null;
            try
            {
                 result = Services.indexesClient.CreateDefaultReadGroup(Globals.SessionId, Globals.DepartmentId, SelectedPeriod.Period.Id);

            }
            catch (Exception ex)
            {
                PeriodSelectedComplate.Invoke(this, new OnPeriodSelectedEventArgs() { Id=99,Msg1="Bağlantınızı Kontrol Ediniz",Msg2=ex.Message });
                Dismiss();
            }
            if (result.Result.Id == 0) LoadGroups(Periods[cmbActivePeriod.SelectedItemPosition]);
            else Toast.MakeText(Context, result.Result.Info, ToastLength.Short).Show();
        }

        private void CmbIndexTableDate_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SelectedGroup = Groups[e.Position];
        }

        private void CmbActivePeriod_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Lperiod tmpLperiod = Periods[e.Position];

            if (SelectedPeriod != tmpLperiod)
            {
                LoadGroups(tmpLperiod);
                btnChoose.Enabled = true;
            }
            else
            {
                btnChoose.Enabled = false;
            }
            if (cmbIndexTableDate.Count > 0)
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

                PeriodSelectedComplate.Invoke(this, new OnPeriodSelectedEventArgs() { Id=0, Msg1="Success" , Msg2="Success" });
                Dismiss();
            }
            else
                Toast.MakeText(Context, "Geçerli bir dönem ve tablo tarihi seçiniz.", ToastLength.Long).Show();
            //MessageBox.Show("Geçerli bir dönem ve tablo tarihi seçiniz.");
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_period, container, false);
            InitializeUIComponents(view);
           
            return view;
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                if (!GetPeriods())
                {
                    Toast.MakeText(Context, "Bağlantınızı Kontrol ediniz!!", ToastLength.Long).Show();
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private bool GetPeriods()
        {
            bool pResult = false;
            cmbActivePeriod.Adapter = null;
            PeriodListResult result = null;
            Periods.Clear();
            try
            {
                result = Services.invoicesClient.GetPeriods(Globals.SessionId, Globals.DepartmentId, "");
            }
            catch (Exception ex)
            {
                btnChoose.Enabled = false;
                btnChoose.SetTextColor(new Android.Graphics.Color(255, 50, 50));//Buton yazı rengi kırmızı
                btnChoose.Text = "Bağlantınızı Kontrol Ediniz"+ex.Message;
                return pResult;
            }
            if (result.Result.Id == 0)
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
                periodAdapter = new SpinnerViewPeriodAdapter(Context, Periods);
                cmbActivePeriod.Adapter = periodAdapter;
                cmbActivePeriod.SetSelection(index);
                SelectedPeriod = Periods[cmbActivePeriod.SelectedItemPosition];
                if(LoadGroups(SelectedPeriod))
                    pResult = true;
               
            }
            return pResult;
        }
        private bool LoadGroups(Lperiod lperiod)
        {
            bool pResult = false;
            SelectedPeriod = lperiod;
            cmbIndexTableDate.Adapter = null;
            Groups.Clear();
            ReadGroupResult result = null;
            try
            {
                result = Services.indexesClient.GetReadGroups(Globals.SessionId, Globals.DepartmentId, SelectedPeriod.Period.Id);
                pResult = true;
            }
            catch (Exception ex)
            {
                PeriodSelectedComplate.Invoke(this, new OnPeriodSelectedEventArgs() { Id = 99, Msg1 = "Bağlantınızı Kontrol Ediniz", Msg2 = ex.Message });
                Dismiss();
                return pResult;
            }
            SelectGroup(result);
            return pResult;
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
                groupAdapter = new SpinnerViewPeriodAdapter(Context, Groups);
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
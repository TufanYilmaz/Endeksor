using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using App4.Indexes;
using App4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace App4
{
    public class OnSubscriberDetailEndEventArgs : EventArgs
    {
        public int Id { get; set; }
        public string Msg1 { get; set; }
        public string Msg2 { get; set; }

        public OnSubscriberDetailEndEventArgs()
        { }
    }
    class Dialog_SubscriberDetail : DialogFragment
    {
        //Indexes.Indexes IndexesClient = new Indexes.Indexes();
        //Subscribers.Subscribers SubscribersClient = new Subscribers.Subscribers();
        ListViewSubscriberViewAdapter viewAdapter;
        private baseSubscription Subscriber;
        private bool editing = false;
        List<Indexes.baseMeter> subscriberMeters = new List<baseMeter>();
        //List<Indexes.baseMeter> tmpSubscriberMeters=new List<baseMeter>();
        TextView tvCaption1;
        TextView tvCaption2;
        TextView tvCaption3;
        TextView tvSubscriberName;
        TextView tvLineInfo;
        ListView lvIndexView;
        EditText txtPriorValue;
        EditText txtNewValue;
        CheckBox chkMeterLap;
        Button btnCancel;
        Button btnSaveClose;
        List<SubscriberViewAdapterClass> subscriberViews;
        public event EventHandler<OnSubscriberDetailEndEventArgs> OnSubcriberIndexesEnd;
        int ID;
        InputMethodManager inputManger;
        private void InitializeUIComponents(View view)
        {
            tvCaption1 = view.FindViewById<TextView>(Resource.Id.tvfCaption1);
            tvCaption2 = view.FindViewById<TextView>(Resource.Id.tvfCaption2);
            tvCaption3 = view.FindViewById<TextView>(Resource.Id.tvfCaption3);
            tvSubscriberName = view.FindViewById<TextView>(Resource.Id.tvfSubscriberName);
            tvLineInfo = view.FindViewById<TextView>(Resource.Id.tvfSubscriberLineInfo);
            lvIndexView = view.FindViewById<ListView>(Resource.Id.lvfIndexView);
            lvIndexView.ItemClick += LvIndexView_RowSelectItemClick;
            //lvIndexView.ItemSelected += LvIndexView_ItemSelected;
            txtPriorValue = view.FindViewById<EditText>(Resource.Id.txtfPriorValue);
            txtPriorValue.Enabled = false;
            txtNewValue = view.FindViewById<EditText>(Resource.Id.txtfNewValue);
            txtNewValue.FocusChange += TxtNewValue_FocusChange;
            chkMeterLap = view.FindViewById<CheckBox>(Resource.Id.chkfMeterLap);
            btnCancel = view.FindViewById<Button>(Resource.Id.btnfCancel);
            btnCancel.Click += (s, e) => { Dismiss(); };
            btnSaveClose = view.FindViewById<Button>(Resource.Id.btnfSaveClose);
            btnSaveClose.Click += BtnSaveClose_Click;
            inputManger = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            lvIndexView.Focusable = true;

        }



        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_subscriberdetail, container, false);
            if (Arguments != null)
            {
                ID = Arguments.GetInt("ID");
                InitializeSubscriber(ID);
            }

            InitializeUIComponents(view);
            LoadFormData();
            return view;
        }
        private void InitializeSubscriber(int id)
        {
            try
            {
                this.Subscriber = (from s in Globals.Indexes where s.Id == id select s).FirstOrDefault();

                subscriberMeters = (from m in Globals.Indexes where m.Id == id select m.Meters.ToList()).FirstOrDefault().ToList();

            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, ex.Message, ToastLength.Short).Show();
                Dismiss();
            }
        }
        private void LoadFormData()
        {
            tvSubscriberName.Text = Subscriber.Code + " - " + Subscriber.Info;
            tvCaption1.Text = Globals.DepartmentCode.Left(10);
            tvCaption2.Text = Globals.Period.Info.Left(10);
            tvCaption3.Text = string.Format("{0:" + Globals.DateTimeFormat + "}", Globals.SelectedReadGroup.DateTime);
            GetSubscriberIndexes();
        }
        private void GetSubscriberIndexes(bool isEditing = false, bool scroll = false)
        {
            if (!isEditing)
                subscriberViews = GetIndexLines(subscriberMeters);
            viewAdapter = new ListViewSubscriberViewAdapter(Context, subscriberViews);
            lvIndexView.Adapter = null;
            lvIndexView.Adapter = viewAdapter;
            int selected = subscriberViews.Where(x => x.IsSelected).Count();
            if (selected == 1 && scroll)
            {
                int lastPosition = subscriberViews.Count;
                int position = subscriberViews.Where(x => x.IsSelected).Select(x => x.LINE_ID).FirstOrDefault();
                int scroolTo = position;
                if (position != lastPosition && position != 0)
                    scroolTo -= 1;
                lvIndexView.SetSelection(scroolTo);
                lvIndexView.SmoothScrollToPosition(scroolTo);
            }
        }

        SubscriberViewAdapterClass selectedLine;
        private void LvIndexView_RowSelectItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            txtNewValue.ClearFocus();
            selectedLine = (from l in subscriberViews
                            where l.LINE_ID == subscriberViews[e.Position].LINE_ID
                            select l).FirstOrDefault();
            lvIndexView.SetSelection(e.Position);
            txtPriorValue.Text = string.Format("{0}", double.IsNaN(selectedLine.PRIORVALUE) || selectedLine.PRIORVALUE == -1 ? "" : selectedLine.PRIORVALUE.ToString());
            txtNewValue.Text = string.Format("{0}", double.IsNaN(selectedLine.NEWVALUE) || selectedLine.NEWVALUE == -1 ? "" : selectedLine.NEWVALUE.ToString());
            txtNewValue.RequestFocus(FocusSearchDirection.Right);

        }

        private void BtnSaveClose_Click(object sender, EventArgs e)
        {
            bool result = true;
            if (editing) result = UpdateLine();
            if (result)
                if (SaveValues())
                {
                    OnSubcriberIndexesEnd.Invoke(this, new OnSubscriberDetailEndEventArgs() { Id = 0 });
                    Dismiss();
                }
        }
        private bool SaveValues()
        {
            bool result = true;
            foreach (var item in subscriberViews)
            {
                if (!item.IsEdited)
                    continue;
                result = SaveLine(item);
                if (!result) break;
                subscriberMeters.Where(x => x.Id == item.METER_ID)
                   .FirstOrDefault().Values
                   .Where(x => x.Id == item.INDEXLINES_ID)
                   .FirstOrDefault().NewValue.RawValue = item.NEWVALUE;

            }
            //subscriberMeters = tmpSubscriberMeters;
            return result;
        }
        private bool SaveLine(SubscriberViewAdapterClass item)
        {
            bool result = false;
            //string exeption = "";
            try
            {
                int meterId = Convert.ToInt32(item.METER_ID);
                if (Convert.ToInt32(item.ID) > 0)
                {

                    IndexValueResult r = null;
                    if (!double.IsNaN(item.NEWVALUE))
                    {
                        r = Services.indexesClient.SetIndexValue(
                               Globals.SessionId,
                               Globals.DepartmentId,
                               Convert.ToInt32(item.ID),
                               Globals.SelectedReadGroup.Id,
                               Globals.SelectedReadGroup.DateTime,
                               Subscriber.Id,
                               meterId,
                               Convert.ToInt32(item.INDEXLINES_ID),
                               Convert.ToDouble(item.NEWVALUE));
                        //RegisterClient.RegisterDbClient client = new RegisterClient.RegisterDbClient();
                        //client.
                    }
                    else
                    {
                        r = Services.indexesClient.DropIndexValue(Globals.SessionId, Globals.DepartmentId, Convert.ToInt32(item.ID), 0);
                        item.ID = -1;
                        subscriberViews.Where(x => x.LINE_ID == item.LINE_ID).FirstOrDefault().ID = -1;
                        subscriberMeters.Where(x => x.Id == item.METER_ID)
                  .FirstOrDefault().Values
                  .Where(x => x.Id == item.INDEXLINES_ID)
                  .FirstOrDefault().NewValue.Id = -1;
                    }
                    result = r.Result.Id == 0;
                    if (!result) throw new Exception(r.Result.Info);

                }
                else
                {
                    if (!double.IsNaN(item.NEWVALUE))
                    {
                        var r = Services.indexesClient.AddIndexValue(
                               Globals.SessionId,
                               Globals.DepartmentId,
                               Globals.SelectedReadGroup.DateTime,
                               Subscriber.Id,
                               meterId,
                               Convert.ToInt32(item.INDEXLINES_ID),
                               Convert.ToDouble(item.NEWVALUE),
                               Globals.SelectedReadGroup.Id);
                        result = r.Result.Id == 0;
                        if (!result) throw new Exception(r.Result.Info);
                        item.ID = r.Value;
                        subscriberViews.Where(x => x.LINE_ID == item.LINE_ID).FirstOrDefault().ID = r.Value;
                        subscriberMeters.Where(x => x.Id == item.METER_ID)
                  .FirstOrDefault().Values
                  .Where(x => x.Id == item.INDEXLINES_ID)
                  .FirstOrDefault().NewValue.Id = r.Value;
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                //OnSubcriberIndexesEnd.Invoke(this, new OnSubscriberDetailEndEventArgs() { Id = 99, Msg1 =item.INDEXLINES_CODE, Msg2 = ex.Message });
                Toast.MakeText(Context, "Bağlantınızı Kontrol Ediniz" + ex.Message, ToastLength.Long).Show();
            }
            return result;
        }

        private void TxtNewValue_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (selectedLine == null)
                return;
            if (e.HasFocus) //GotFocus
            {
                editing = true;
                inputManger.ShowSoftInput(txtNewValue, ShowFlags.Implicit);
                ColorSelectedListItem();
                GetSubscriberIndexes(true, true);
            }
            else //LostFocus
            {
                if (!UpdateLine())
                {
                    txtNewValue.RequestFocus();
                }
                else
                    editing = false;
            }
        }
        private void ColorSelectedListItem()
        {
            //collection.ToList().ForEach(c => c.PropertyToSet = value);
            subscriberViews.ForEach(c => c.IsSelected = false);
            subscriberViews.Where(x => x.LINE_ID == selectedLine.LINE_ID).FirstOrDefault().IsSelected = true;
            //lvIndexView.GetChildAt(position).SetBackgroundColor(new Android.Graphics.Color(100, 100, 100));
            //lvIndexView.
        }
        private bool UpdateLine()
        {
            bool result = true;
            var NewValue = txtNewValue.Text == "" ? -1 : Convert.ToDouble((txtNewValue.Text).Replace('.', ','));
            var PriorValue = double.IsNaN(selectedLine.PRIORVALUE) ? -1 : Convert.ToDouble(selectedLine.PRIORVALUE);
            if (selectedLine == null)
            {
                Toast.MakeText(Context, "Beklenmeyen bir hata oluştu:\nLütfen Tekrar Deneyiniz", ToastLength.Long).Show();
                return false;
            }
            if (selectedLine.IsOrdered && (!chkMeterLap.Checked))
            {
                if (txtNewValue.Text != "")
                {

                    if (NewValue < PriorValue)
                    {
                        result = false;
                        Toast.MakeText(Context, "Yeni değer dönem başından küçük olamaz.", ToastLength.Long).Show();
                    }
                }

            }
            if (result)
            {
                if (selectedLine.NEWVALUE != NewValue)
                {
                    if (NewValue < 0)
                        selectedLine.NEWVALUE = double.NaN;
                    else
                        selectedLine.NEWVALUE = NewValue;
                    selectedLine.IsEdited = true;
                    if (!selectedLine.IsOrdered && PriorValue != NewValue && !double.IsNaN(NewValue) && NewValue > -1)
                        tvLineInfo.Text = "Bilgi :" + selectedLine.METER_CODE + " | " + selectedLine.INDEXLINES_INFO + " Önceki değerinden farklı";


                    //subscriberMeters.Where(x => x.Id == Convert.ToInt32(selectedLine.METER_ID))
                    //    .FirstOrDefault().Values
                    //    .Where(x => x.Id == Convert.ToInt32(selectedLine.INDEXLINES_ID))
                    //    .FirstOrDefault().NewValue.RawValue = NewValue;
                    //subscriberViews.Where(x => x.LINE_ID == selectedLine.LINE_ID)
                    //    .FirstOrDefault().NEWVALUE = NewValue;
                    //(from l in subscriberViews where l.LINE_ID == selectedLine.LINE_ID select l).FirstOrDefault().NEWVALUE=NewValue;


                }
            }
            GetSubscriberIndexes(true);
            return result;
        }


        public List<SubscriberViewAdapterClass> GetIndexLines(IEnumerable<Indexes.baseMeter> meters)
        {
            List<SubscriberViewAdapterClass> viewSubscriber = new List<SubscriberViewAdapterClass>();
            int lineId = 0;
            foreach (var meter in meters)
            {
                foreach (var lines in meter.Values)
                {
                    viewSubscriber.Add(
                        new SubscriberViewAdapterClass()
                        {
                            LINE_ID = lineId++,
                            ID = lines.NewValue.Id,
                            METER_ID = meter.Id,
                            METER_CODE = meter.Code,
                            INDEXLINES_CODE = lines.Code,
                            INDEXLINES_ID = lines.Id,
                            INDEXLINES_INFO = lines.Info,
                            NEWVALUE = lines.NewValue.RawValue,
                            PRIORVALUE = lines.PriorValue.RawValue,
                            IsOrdered = lines.IsOrdered,
                            IsEdited = false,
                            IsSelected = false
                        });
                }
            }
            return viewSubscriber;
        }
    }
}
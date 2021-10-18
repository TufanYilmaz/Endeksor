using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using App4.Subscribers;
using System.Linq;
using System.Web.Services.Protocols;
using System.Threading;
using System.Data;
using App4.Models;
using System.Threading.Tasks;
using App4.Helpers;

namespace App4
{
    //LaunchMode = Android.Content.PM.LaunchMode.SingleTop
    [Activity(Label = "@string/app_name", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@drawable/Terminal",ScreenOrientation =Android.Content.PM.ScreenOrientation.Portrait )]
    public class MainActivity : Activity
    {
        private Button btnLogin;
        private TextView txtResult;
        private ListView lvSubscribers;
        private LinearLayout layoutActionBar;
        private LinearLayout layoutLogin;
        private LinearLayout layoutSubsctibers;
        private EditText txtSearch;
        private List<Indexes.baseSubscription> subscribersList;
        private List<Indexes.baseSubscription> subscribersListForSearch;
        private ListViewSubscriberAdapter subscriberAdapter;
        private ProgressBar loginProgressBar;
        private Button btnSearchSubscriber;
        private Button btnChoosePeriod;
        //------------TerminalSelection----------------
        private Button btnSelectTerminal;
        private TextView tvTerminalInfo;
        //-------------------------------------------
        //private Android.Support.V7.App.ActionBar actionBar;
        //Authentication.Authentication authenticationClient = new Authentication.Authentication();
        //Subscribers.Subscribers subscribersClient = new Subscribers.Subscribers();
        //Indexes.Indexes indexClient = new Indexes.Indexes();
        
        public InputMethodManager inputMethodManager;
        private bool isAnimetedDown;
        private bool isAnimating;
        private int animeDuration;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Set your main view here
            //indexClient.Timeout = 50000;
            //LoginConfig.SetLoginInfo("|");
            if(!ServiceConfig.ConfigFileExist())
            {
                ServiceConfig.CreteConfigFile();
                ServiceConfig.SetServiceUrl(ServiceConfig.defaultServiceURL);
            }
            if(!LoginConfig.ConfigFileExist())
            {
                LoginConfig.CreteConfigFile();
                LoginConfig.SetLoginInfo("|");
            }
            //Services.indexesClient.Timeout = 15000;
            //Services.subscribersClient.Timeout = 6000;
            SetContentView(Resource.Layout.activity_main);
            layoutActionBar = FindViewById<LinearLayout>(Resource.Id.layoutActActionBar);
            layoutLogin = FindViewById<LinearLayout>(Resource.Id.layoutLogin);
            layoutSubsctibers = FindViewById<LinearLayout>(Resource.Id.layoutSubscribers);
            lvSubscribers = FindViewById<ListView>(Resource.Id.lvSubscribers);
            lvSubscribers.ItemClick += LvSubscribers_ItemClick;
            lvSubscribers.ItemLongClick += LvSubscribers_ItemLongClick;
            txtResult = FindViewById<TextView>(Resource.Id.txtResult);
            btnLogin = FindViewById<Button>(Resource.Id.loginButton);
            txtSearch = FindViewById<EditText>(Resource.Id.txtSearch);
            loginProgressBar=FindViewById<ProgressBar>(Resource.Id.loginProgressBar);
            animeDuration =Resources.GetInteger(Resource.Integer.animeDuration);
            btnSearchSubscriber = FindViewById<Button>(Resource.Id.btnSubscriberSearch);
            btnChoosePeriod = FindViewById<Button>(Resource.Id.btnSelectPeriod);
            //Terminal Views
            btnSelectTerminal = FindViewById<Button>(Resource.Id.btnDoSelectTerminal);
            btnSelectTerminal.Click += (s, e) => DoSelectTerminal();
            tvTerminalInfo = FindViewById<TextView>(Resource.Id.tvHandTerminalInfo);
            //FindViewById<TextView>(Resource.Id.lblAppName).Text = Build.Serial;
            btnChoosePeriod.Click += (s, e) => {
                DoSelectPeriod();
            };
            
            btnSearchSubscriber.Click += BtnSearchSubscriber_Click;
            txtSearch.Alpha = 0;
            txtSearch.Enabled = false;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtResult.Text =string.Empty;
            inputMethodManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);//klavye gizlemek için
            btnLogin.Click += (s, e) =>
            {
                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                Dialog_Login Dialog_Login = new Dialog_Login();
                Dialog_Login.Show(fragmentTransaction, "Giriş Yap");
                Dialog_Login.SignUpComplate += Dialog_Login_SignUpComplate;
            };
            
            if (Globals.isLogin)
            {
                OnLoginCustomize();
            }
            else
            {
                layoutSubsctibers.Visibility = ViewStates.Invisible;
                layoutLogin.Visibility = ViewStates.Visible;
                layoutActionBar.Visibility = ViewStates.Invisible;
                //ActionBar.Hide();
            }


        }

        private void BtnSearchSubscriber_Click(object sender, EventArgs e)
        {
            if (Globals.isLogin)
            {
                if (isAnimating)
                    return;
                //Arama butonu Tıklandığında
                if (!isAnimetedDown)
                {
                    //subscriber listesi yukarıda
                    MainAnimation anim = new MainAnimation(lvSubscribers, lvSubscribers.Height - txtSearch.Height)
                    {
                        Duration = animeDuration
                    };
                    lvSubscribers.StartAnimation(anim);
                    anim.AnimationStart += Anim_AnimationStartDown;
                    anim.AnimationEnd += Anim_AnimationDownEnd;
                    layoutSubsctibers.Animate().TranslationYBy(txtSearch.Height).SetDuration(animeDuration).Start();
                }
                else
                {
                    //subscriber listesi aşağıda
                    MainAnimation anim = new MainAnimation(lvSubscribers, lvSubscribers.Height + txtSearch.Height)
                    {
                        Duration = 500
                    };
                    lvSubscribers.StartAnimation(anim);
                    anim.AnimationStart += Anim_AnimationStartUp;
                    anim.AnimationEnd += Anim_AnimationEndUp;
                    layoutSubsctibers.Animate().TranslationYBy(-txtSearch.Height).SetDuration(animeDuration).Start();
                }
                isAnimetedDown = !isAnimetedDown;

            }
        }

        #region  ARAMA
        private void TxtSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)=>
            SearchTextForSubscriberList(txtSearch.Text);
            //List<subscriber> searchedSubscribers = 
            //lvSubscribers.ItemClick += (s, le) =>
            //{

            //    //Listedeki elemana basıldığında
            //    Toast.MakeText(this, subscribersList[le.Position].Code, ToastLength.Short).Show();
            //};
            //lvSubscribers.ItemLongClick += (s, le) =>
            //{
            //    //listedeki elemana uzun basıldığında
            //    Toast.MakeText(this, subscribersList[le.Position].Info, ToastLength.Short).Show();
            //};
        
        private void SearchTextForSubscriberList(string searchText)
        {
            try
            {
                subscribersList = (from subs in subscribersListForSearch
                                   where (subs.Info.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                                   || subs.Code.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                                   select subs).ToList();


                subscriberAdapter = new ListViewSubscriberAdapter(this, subscribersList);
                lvSubscribers.Adapter.Dispose();
                lvSubscribers.Adapter = subscriberAdapter;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "SearchTextForSubscriberList\n" + ex.Message, ToastLength.Short).Show();
            }
        }

        #endregion

        #region Default Overrides
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.mainMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override void CloseOptionsMenu()
        {
            base.CloseOptionsMenu();
        }
        public override void OnBackPressed()
        {
           
            Android.App.AlertDialog.Builder alertDialogBuilder = new Android.App.AlertDialog.Builder(this);
            alertDialogBuilder.SetMessage(Resource.String.AskClose);
            alertDialogBuilder.SetCancelable(true);
            alertDialogBuilder.SetPositiveButton(Resource.String.Close, delegate
            {
                Globals.isLogin = false;
                Globals.SessionId = string.Empty;
                base.OnBackPressed();
            });
            alertDialogBuilder.SetNegativeButton(Resource.String.Cancel, delegate
            {
                alertDialogBuilder.Dispose();
            });
            alertDialogBuilder.Show();
        }

        #endregion

        #region HandTerminalSelect Section

        #endregion

        #region LOGIN Giriş Yapma Kısmı 
        private void Dialog_Login_SignUpComplate(object sender, OnLoginEventArgs e)
        {
            //inputMethodManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
            if(e.Password.StartsWith("admin") && e.UserName.StartsWith("admin"))
            {
                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                var dialog_ServiceConfig = new Dialog_ServiceConfig();
                dialog_ServiceConfig.Show(fragmentTransaction, "Servis Ayarları");
            }
            else
            {
                loginProgressBar.Visibility = ViewStates.Visible;
                Thread loginThread = new Thread(() => LoginRequest(e));
                loginThread.Start();
                if (e.PasswordSave)
                {
                    LoginConfig.SetLoginInfo(e.UserName.Split("~|")[0] + "|" + e.Password);
                }
            }

        }
        private void LoginRequest( OnLoginEventArgs e=null)
        {
            try
            {
                RunOnUiThread(() => { btnLogin.Enabled = false; txtResult.Text = ""; });
                var auth =Services.authenticationClient.Login(e.UserName, e.Password, e.Department.ToString());
                if (auth.Result.Id > 0)
                {
                    RunOnUiThread(() => {
                        txtResult.Text = "Başarısız!" + auth.Result.Info + "\n" + auth.Result.ResultType;
                        loginProgressBar.Visibility = ViewStates.Visible;
                        btnLogin.Enabled = true;
                    });
                }
                else
                {
                    //Giriş yapıldığında
                    
                    RunOnUiThread(() => { txtResult.Text = "Başarılı\n" + auth.Result.Info + "\n Endex Verileri Yükleniyor Lütfen Bekleyiniz"; });
                    Globals.isLogin = true;
                    LogonValues(auth);
                    if (Globals.isLogin && auth.AuthorizedTerminals.Length>0)
                    {
                        RunOnUiThread(() => 
                        {
                            //DoSelectTerminal
                            OnLoginCustomize();
                        });
                        //ActionBar.Show();
                    }
                    else
                    {
                        RunOnUiThread(() => {
                            ShowLoginLayout();
                            txtResult.Text = "Bu makinaya kayıtlı el terminali bulunamadı";
                            btnLogin.Enabled = true;
                        });
                    }

                }
                RunOnUiThread(() => { loginProgressBar.Visibility = ViewStates.Invisible; });
            }
            catch (SoapException ex)
            {
                RunOnUiThread(()=>
                {
                    txtResult.Text = "Sunucuya erişilemedi!\n" + ex.Message;
                    loginProgressBar.Visibility = ViewStates.Invisible;
                    btnLogin.Enabled = true;
                });
                
            }
            catch (Exception ex)
            {
                RunOnUiThread(() =>
                {
                    txtResult.Text = "Bilinmeyen Hata" + ex.Message.ToString();
                    loginProgressBar.Visibility = ViewStates.Invisible;
                    btnLogin.Enabled = true;
                });
                
            }
        }

        private void ShowLoginLayout()
        {
            layoutLogin.Visibility = ViewStates.Visible; layoutSubsctibers.Visibility = ViewStates.Invisible;
        }

        public void OnLoginCustomize()
        {
            ShowMainLayout();
            if (Globals.SelectedReadGroup == null || Globals.SelectedReadGroup.Id == 0)
                DoSelectPeriod();
            if (Globals.SelectedReadGroup != null)
            {
                if (Globals.AuthorizedTerminals.Count == 1)
                {
                    btnSelectTerminal.Visibility = ViewStates.Gone;
                    Globals.SelectedHandTerminal = Globals.AuthorizedTerminals[0];
                    tvTerminalInfo.Text = Globals.SelectedHandTerminal.Code;
                    LoadForm();
                }
                else
                {
                    tvTerminalInfo.Visibility = ViewStates.Gone;
                    DoSelectTerminal();
                }
            }
        }

        private void DoSelectTerminal()
        {
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            Dialog_HandTerminalSelection Dialog_HandTerminal = new Dialog_HandTerminalSelection();
            Dialog_HandTerminal.Show(fragmentTransaction, "Giriş Yap");
            Dialog_HandTerminal.TerminalSelectedComplate += Dialog_HandTerminal_TerminalSelectedComplate;
            
        }

        private void Dialog_HandTerminal_TerminalSelectedComplate(object sender, OnTerminalSelectedEventArgs e)
        {
            Authentication.HandTerminal tmpHandTerminal = Globals.SelectedHandTerminal;
            if (e != null)
            {
                Globals.SelectedHandTerminal = (from terms in Globals.AuthorizedTerminals
                                                where terms.Id == e.Id
                                                select terms).First();
               
            }
            //try
            //{

            if (LoadForm())
                btnSelectTerminal.Text = Globals.SelectedHandTerminal.Code;
            else
            {
                Globals.SelectedHandTerminal = tmpHandTerminal;
            }
             
            }

        private void ShowMainLayout()
        {
            layoutSubsctibers.Visibility = ViewStates.Visible;
            layoutLogin.Visibility = ViewStates.Invisible;
            layoutActionBar.Visibility = ViewStates.Visible;
        }
        #endregion

        #region SUBSCRIBER LIST OPTIONS
        private void LvSubscribers_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {//listedeki elemana uzun basıldığında
            SubscriberResult sresult;
            try
            {

                sresult =Services.subscribersClient.GetSubscriber(Globals.SessionId, Globals.DepartmentId, subscribersList[e.Position].Id);

                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                Dialog_Subsciber dialog_Subsciber = new Dialog_Subsciber();
                Bundle fragmentBundle = new Bundle();
                fragmentBundle.PutString("Adress", sresult.Value.Address.Line1 + sresult.Value.Address.Line2 + sresult.Value.Address.Line3);
                dialog_Subsciber.Arguments = fragmentBundle;
                dialog_Subsciber.Show(fragmentTransaction, "Subsciber");
            }
            catch (Exception ex)
            {
                MakeDialogMessage("GetSubscibersDetail", ex.Message);

            }
            
            //Toast.MakeText(this, subscribersList[e.Position].Info, ToastLength.Short).Show();
        }
        //Liste Elemanına Basıldığında
        private void LvSubscribers_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Toast.MakeText(this, subscribersList.Find(s => s.Id == e.Id).Info, ToastLength.Short).Show(); //hem ilk yükleme hemde aramada kullanmak için Find kullanıldı
            //Toast.MakeText(this, subscribersList[e.Position].Code, ToastLength.Short).Show();
            //StartActivity(new Intent(this, typeof(SubscriberActivity)).PutExtra("subscriberId", subscribersList[e.Position].Id));
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            position = e.Position;
            var dialog_Subsciber = new Dialog_SubscriberDetail();
            Bundle fragmentBundle = new Bundle();
            fragmentBundle.PutInt("ID", subscribersList[e.Position].Id);
            dialog_Subsciber.Arguments = fragmentBundle;
            dialog_Subsciber.SetStyle(DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeMaterialLightNoActionBarFullscreen);
            dialog_Subsciber.Show(fragmentTransaction, "SubsciberDetail");
            dialog_Subsciber.OnSubcriberIndexesEnd += Dialog_Subsciber_OnSubcriberIndexesEnd;
            
        }
        int position = 0;
        private void Dialog_Subsciber_OnSubcriberIndexesEnd(object sender, OnSubscriberDetailEndEventArgs e)
        {
            if(e.Id!=99)
            {
                //if (!string.IsNullOrEmpty(txtSearch.Text))
                    SearchTextForSubscriberList(txtSearch.Text);
                //RunOnUiThread(() =>lvSubscribers.Invalidate());
                lvSubscribers.SetSelection(position);
                lvSubscribers.SmoothScrollToPosition(position);
                //lvSubscribers.PostInvalidate();
            }
        }
        #endregion

        #region Data
        private void DoSelectPeriod()
        {
            //Intent intentPeriod = new Intent(this, typeof(PeriodActivity));
            
            //StartActivity(intentPeriod);
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            var Dialog_period = new Dialog_Period();
            Dialog_period.SetStyle(DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeMaterialLightNoActionBarFullscreen);//Android.Resource.Style.ThemeBlackNoTitleBarFullScreen
            Dialog_period.PeriodSelectedComplate += Dialog_period_PeriodSelectedComplate;
            Dialog_period.Show(fragmentTransaction, "Period Seç");

        }

        private void Dialog_period_PeriodSelectedComplate(object sender, OnPeriodSelectedEventArgs e)
        {
            if(e.Id!=99)
                LoadForm();
            else
            {
                MakeDialogMessage(e.Msg1, e.Msg2);
                //Toast.MakeText(this, "Bağlantınızı Kontrol Ediniz !!", ToastLength.Long).Show();
            }

        }

        private void GetSubscribers()
        {
            subscriberAdapter = new ListViewSubscriberAdapter(this, subscribersList);
            lvSubscribers.Adapter = subscriberAdapter;
            SearchTextForSubscriberList(txtSearch.Text);

        }
        private void LogonValues(Authentication.AuthenticationResult AuthRes)
        {
            Globals.SessionId = AuthRes.Value.ToString();
            Globals.DepartmentCode = AuthRes.Department.Code;
            Globals.DepartmentId = AuthRes.Department.Id;
            Globals.Period = AuthRes.Period;
            Globals.AuthorizedTerminals = AuthRes.AuthorizedTerminals.ToList();
            //AuthRes.Period.AssignTo(Globals.Period);
            int GroupCount = AuthRes.ReadGroups.Length;
            if (GroupCount > 0)
                Globals.SelectedReadGroup =
                    new Authentication.readgroup()
                    {
                        Id = AuthRes.ReadGroups[GroupCount - 1].Id,
                        DateTime = AuthRes.ReadGroups[GroupCount - 1].DateTime
                    };
        }
        //Get Subscribers for Terminal
        private bool  LoadForm(bool isUpdate=false)
        {
            bool result = true;
            //subscribersClient = new Subscribers.Subscribers();
            //var Subscribers = subscribersClient.GetSubscribersByTerminal(Globals.SessionId,Globals.DepartmentId, Globals.SelectedHandTerminal.Id);
            if (!isUpdate)
            {
                try
                {
                    var indexes = Services.indexesClient.GetIndexesByReadGroup(Globals.SessionId, Globals.SelectedReadGroup.Id, Globals.SelectedHandTerminal.Id, 0);
                    //DataTable indexTable = indexes.Value;
                    Globals.Indexes = indexes.Value.ToList();

                    subscribersList = Globals.Indexes;
                    subscribersListForSearch = subscribersList;

                    GetSubscribers();
                }
                catch (Exception ex)
                {
                    MakeDialogMessage("LoadForm",ex.Message);

                    result = false;
                }

            }
            //List<Subscriber> list = Helpers.SubscriberHelper.GetSubscribers(indexTable);
            //Toast.MakeText(this, indexes.Result.Info, ToastLength.Short).Show();
            //Globals.Subscribers = Helpers.SubscriberHelper.GetSubscribers(Globals.Indexes); //Subscribers.Value.ToList();
            //Globals.Subscribers.Sort(
            //    delegate (subscriber x, subscriber y)
            //    {
            //        int compare = x.Order.CompareTo(y.Order);
            //        if (compare != 0)
            //            return compare;
            //        return x.Info.CompareTo(y.Info);
            //    });


            return result;
        }

        private void MakeDialogMessage(string m1,string m2)
        {
            
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            Dialog_Message message = new Dialog_Message();
            Bundle fragmentBundle = new Bundle();
            fragmentBundle.PutString("Adr1", m1);
            fragmentBundle.PutString("Adr2", m2);
            message.Arguments = fragmentBundle;
            message.Show(fragmentTransaction, "Error");
        }
        #endregion

        #region Action Bar Kısmı
        //override option
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            
            if (Globals.isLogin)
            {
                //return base.OnOptionsItemSelected(item);
                switch (item.ItemId)
                {
                    case Resource.Id.action_search:
                        if (isAnimating)
                            return true;
                        //Arama butonu Tıklandığında
                        if (!isAnimetedDown)
                        {
                            //subscriber listesi yukarıda
                            MainAnimation anim = new MainAnimation(lvSubscribers, lvSubscribers.Height - txtSearch.Height)
                            {
                                Duration = animeDuration
                            };
                            lvSubscribers.StartAnimation(anim);
                            anim.AnimationStart += Anim_AnimationStartDown;
                            anim.AnimationEnd += Anim_AnimationDownEnd;
                            layoutSubsctibers.Animate().TranslationYBy(txtSearch.Height).SetDuration(animeDuration).Start();
                        }
                        else
                        {
                            //subscriber listesi aşağıda
                            MainAnimation anim = new MainAnimation(lvSubscribers, lvSubscribers.Height + txtSearch.Height)
                            {
                                Duration = 500
                            };
                            lvSubscribers.StartAnimation(anim);
                            anim.AnimationStart += Anim_AnimationStartUp;
                            anim.AnimationEnd += Anim_AnimationEndUp;
                            layoutSubsctibers.Animate().TranslationYBy(-txtSearch.Height).SetDuration(animeDuration).Start();
                        }
                        isAnimetedDown = !isAnimetedDown;
                        return true;
                    default:
                        return base.OnOptionsItemSelected(item);
                }
            }
            return true;
        }

        private void Anim_AnimationEndUp(object sender, Android.Views.Animations.Animation.AnimationEndEventArgs e)
        {
            isAnimating = false;
            txtSearch.Enabled = false;
        }

        private void Anim_AnimationDownEnd(object sender, Android.Views.Animations.Animation.AnimationEndEventArgs e)
        {
            txtSearch.Enabled = true;
            isAnimating = false;
        }

        private void Anim_AnimationStartDown(object sender, Android.Views.Animations.Animation.AnimationStartEventArgs e)
        {
            isAnimating = true;
            txtSearch.Animate().AlphaBy(1.0f).SetDuration(animeDuration).Start();
        }
        private void Anim_AnimationStartUp(object sender, Android.Views.Animations.Animation.AnimationStartEventArgs e)
        {
            isAnimating = true;
            txtSearch.Animate().AlphaBy(-1.0f).SetDuration(animeDuration).Start();
        }
        #endregion
    }
}


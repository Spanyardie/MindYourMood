using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Wizards;
using Android.Speech.Tts;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class ThoughtRecordsActivity : AppCompatActivity, TextToSpeech.IOnInitListener
    {
        public static string TAG = "M:ThoughtRecordsActivity";

        private TextView _selectedDate;
        private TextView _progressFromDate;
        private ImageView _dateSelectButton;
        private ImageView _dateProgressFrom;
        private Toolbar _toolbar;
        private ImageView _enterThought;
        private ImageView _viewThoughts;
        private LinearLayout _showProgress;
        private TextView _enterThoughtRecord;
        private TextView _viewThoughtRecords;
        private TextView _showProgressTextView;

        private TextView _selectedFromDateLabel;
        private TextView _selectedToDateLabel;

        private TextToSpeech _spokenWord;

        private DateTime _fromDate;
        private DateTime _toDate;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutString("fromDate", _fromDate.ToString());
                outState.PutString("toDate", _toDate.ToString());
            }

            base.OnSaveInstanceState(outState);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if(savedInstanceState != null)
                {
                    _fromDate = Convert.ToDateTime(savedInstanceState.GetString("fromDate"));
                    _toDate = Convert.ToDateTime(savedInstanceState.GetString("toDate"));
                }
                SetContentView(Resource.Layout.ThoughtRecordMainMenu);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.thoughtRecordMainMenuToolbar, Resource.String.ThoughtRecordsActionBarTitle, Color.White);

                SetupButtonHooks();

                if (_spokenWord == null)
                    _spokenWord = new TextToSpeech(this, this);

                _fromDate = DateTime.Now.AddDays(-7);
                _toDate = DateTime.Now;

                if (_selectedToDateLabel != null)
                    _selectedToDateLabel.Text = _toDate.ToShortDateString();
                if (_selectedFromDateLabel != null)
                    _selectedFromDateLabel.Text = _fromDate.ToShortDateString();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingThoughtRecordActivity), "ThoughtRecordActivity.OnCreate");
            }
        }

        protected override void OnDestroy()
        {
            if (_spokenWord != null)
                _spokenWord.Shutdown();
            base.OnDestroy();
        }

        void DateSelect_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                DatePickerFragment datePicker = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    if (time.CompareTo(_fromDate) > 0)
                    {
                        _selectedToDateLabel.Text = time.ToShortDateString();
                        _toDate = time;
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.ThoughtRecordsProgressToDateInvalidToast, ToastLength.Long).Show();
                    }
                });
                datePicker.Show(FragmentManager, DatePickerFragment.TAG);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "DateSelect_OnClick: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingDatePicker), "ThoughtRecordsActivity.DateSelect_OnClick");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ThoughtRecordsMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    if(_spokenWord != null)
                    {
                        if (_spokenWord.IsSpeaking)
                            _spokenWord.Stop();
                    }
                    Finish();
                    return true;
                }
                if(item.ItemId == Resource.Id.thoughtRecordsActionHelp)
                {
                    Intent intent = new Intent(this, typeof(ThoughtsHelpActivity));
                    StartActivity(intent);
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            try
            {
                _selectedDate = FindViewById<TextView>(Resource.Id.txtSelectedDate);
                _progressFromDate = FindViewById<TextView>(Resource.Id.txtProgressFromDateLabel);
                _dateSelectButton = FindViewById<ImageView>(Resource.Id.imgbtnSelectDate);
                _dateProgressFrom = FindViewById<ImageView>(Resource.Id.imgbtnProgressFromDate);
                _enterThought = FindViewById<ImageView>(Resource.Id.imgbtnEnterThoughtRecord);
                _viewThoughts = FindViewById<ImageView>(Resource.Id.imgbtnViewThoughtRecord);
                _showProgress = FindViewById<LinearLayout>(Resource.Id.imgbtnShowProgress);
                _enterThoughtRecord = FindViewById<TextView>(Resource.Id.txtEnterThoughtRecord);
                _viewThoughtRecords = FindViewById<TextView>(Resource.Id.txtViewThoughtRecord);
                _showProgressTextView = FindViewById<TextView>(Resource.Id.txtShowProgress);

                _selectedFromDateLabel = FindViewById<TextView>(Resource.Id.txtSelectedFromDateLabel);
                _selectedToDateLabel = FindViewById<TextView>(Resource.Id.txtSelectedToDateLabel);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorGettingFieldComponents), "ThoughtRecordsActivity.GetFieldComponents");
            }
        }

        private void SetupButtonHooks()
        {
            try
            {
                if (_enterThought != null)
                    _enterThought.Click += EnterThought_Click;
                if (_viewThoughts != null)
                    _viewThoughts.Click += ViewThoughts_Click;
                if (_showProgress != null)
                    _showProgress.Click += ShowProgress_Click;
                if (_dateSelectButton != null)
                    _dateSelectButton.Click += DateSelect_OnClick;
                if (_dateProgressFrom != null)
                    _dateProgressFrom.Click += DateProgressFrom_Click;
                if (_selectedDate != null)
                    _selectedDate.Click += DateSelect_OnClick;
                if (_progressFromDate != null)
                    _progressFromDate.Click += DateProgressFrom_Click;
                if (_enterThoughtRecord != null)
                    _enterThoughtRecord.Click += EnterThought_Click;
                if (_viewThoughtRecords != null)
                    _viewThoughtRecords.Click += ViewThoughts_Click;
                if (_showProgressTextView != null)
                    _showProgressTextView.Click += ShowProgress_Click;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupButtonHooks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSettingUpButtonHooks), "ThoughtRecordsActivity.SetupButtonHooks");
            }
        }

        private void DateProgressFrom_Click(object sender, EventArgs e)
        {
            try
            {
                DatePickerFragment datePicker = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    if (time.CompareTo(_toDate) < 0)
                    {
                        _selectedFromDateLabel.Text = time.ToShortDateString();
                        _fromDate = time;
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.ThoughtRecordsProgressFromDateInvalidToast, ToastLength.Long).Show();
                    }
                });
                datePicker.Show(FragmentManager, DatePickerFragment.TAG);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "DateProgressFrom_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorCreatingDatePicker), "ThoughtRecordsActivity.DateProgressFrom_Click");
            }
        }

        private void ShowProgress_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MyProgressActivity));
            intent.PutExtra("EndDate", _selectedToDateLabel.Text.Trim());
            intent.PutExtra("StartDate", _selectedFromDateLabel.Text.Trim());
            StartActivity(intent);
        }

        private void ViewThoughts_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(ViewThoughtsActivity));
                intent.PutExtra("RecordDate", _selectedToDateLabel.Text.Trim());
                StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ViewThoughts_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorViewingThoughtsActivity), "ThoughtRecordsActivity.ViewThoughts_Click");
            }
        }

        private void EnterThought_Click(object sender, EventArgs e)
        {
            Globals dbHelp = new Globals();
            try
            {
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();

                //Create an initial ThoughtRecord so we have an Id when stepping through the wizard
                GlobalData.SetupDataFields();
                GlobalData.ThoughtRecordItem.RecordDateTime = Convert.ToDateTime(_selectedToDateLabel.Text.Trim());
                GlobalData.ThoughtRecordItem.Save(sqlDatabase);
                GlobalData.ThoughtRecordId = dbHelp.GetAutoIndexID(sqlDatabase);
                dbHelp.CloseDatabase();
                Log.Info(TAG, "New global Thought Record created - Date " + GlobalData.ThoughtRecordItem.RecordDateTime.ToShortDateString() + ", record ID - " + GlobalData.ThoughtRecordItem.ThoughtRecordId.ToString());
                Intent intent = new Intent(this, typeof(SituationMainActivity));
                StartActivity(intent);
            }
            catch(Exception eEnter)
            {
                Log.Error(TAG, "Exception - " + eEnter.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, eEnter, GetString(Resource.String.ErrorCreatingDefaultThoughtRecord), "ThoughtRecordsActivity.EnterThought_Click");
                if (dbHelp != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void OnInit([GeneratedEnum] OperationResult status)
        {
            
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHelp = menu.FindItem(Resource.Id.thoughtRecordsActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SummaryActivity.SetActionIcons");
            }
        }
    }
}
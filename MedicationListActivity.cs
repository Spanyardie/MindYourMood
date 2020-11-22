using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using V7Sup = Android.Support.V7.App;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Helpers;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V4.App;
using Android.Speech;
using Android.Runtime;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment;
using Android.Content.PM;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class MedicationListActivity : V7Sup.AppCompatActivity, IMedicationTime, IMedicationReminder, IAlertCallback
    {
        public const string TAG = "M:MedicationListActivity";

        private Toolbar _toolbar;

        private Medication _medication;

        private EditText _medName;
        private EditText _dailyDose;
        private Spinner _prescriptionType;
        private TextView _prescriptionWeekDayLabel;
        private Spinner _prescriptionWeekDay;
        private TextView _prescriptionMonthDayLabel;
        private EditText _prescriptionMonthDay;

        private ListView _medicationTimeList;
        private ImageButton _medicationTimeAdd;
        private ImageButton _medicationTimeRemove;

        private ListView _medicationReminderList;
        private ImageButton _medicationReminderAdd;
        private ImageButton _medicationReminderRemove;

        private LinearLayout _linTimeList;
        private LinearLayout _linReminderList;
        private LinearLayout _linSep;

        private List<MedicationSpread> _medicationSpreadListItems;

        private bool _isNew = true;
        private bool _isDirty = false;
        private int _medicationID = -1;

        private int _selectedMedicationTimeID = -1;

        private bool _firstTimeView = false;

        private string _headerTitleText;

        private int _selectedTimeItemIndex = -1;
        private int _selectedReminderItemIndex = -1;


        private string _activityTitle = "";

        private ImageButton _speakMedicationName;
        private ImageButton _speakDailyDose;
        private ImageButton _speakMonthDay;

        private LinearLayout _medListSep3;

        private ConstantsAndTypes.MedicationSpeakType _currentSpeakType = ConstantsAndTypes.MedicationSpeakType.Name;

        private void Initialise()
        {
            try
            {
                _medication = new Medication();
                _firstTimeView = true;


                if (_medicationID != -1)
                {
                    _headerTitleText = GetString(Resource.String.MedListFragHeaderTitleTextEdit);
                    //existing medication
                    GetMedicationData();
                }
                else
                {
                    _headerTitleText = GetString(Resource.String.MedListFragHeaderTitleTextAdd);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "InitialiseFragment: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedicationListActivityInitialise), "MedicationListActivity.InitialiseFragment");
            }
        }

        public int GetSelectedTimeItemIndex()
        {
            return _selectedTimeItemIndex;
        }

        public int GetSelectedReminderItemIndex()
        {
            return _selectedReminderItemIndex;
        }

        private void UpdateFields()
        {
            try
            {
                if (_medication != null)
                {
                    if (_medName != null)
                        _medName.Text = _medication.MedicationName;
                    if (_dailyDose != null)
                        _dailyDose.Text = _medication.TotalDailyDosage.ToString();
                    //if (_prescriptionMonthDay != null)
                    //    _prescriptionMonthDay.Text = _medication.PrescriptionType.MonthlyDay.ToString();
                    UpdateReminderAdapter();
                    UpdateTimeAdapter();
                    if (_prescriptionType != null)
                        _prescriptionType.SetSelection((int)_medication.PrescriptionType.PrescriptionType);
                    if (_prescriptionWeekDay != null)
                        _prescriptionWeekDay.SetSelection((int)_medication.PrescriptionType.WeeklyDay);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateFields: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedicationListUpdatingFields), "MedicationListActivity.UpdateFields");
            }
        }

        private void UpdateReminderAdapter()
        {
            try
            {
                var adapter = new MedicationReminderListAdapter(this, _medicationID);
                Log.Info(TAG, "UpdateReminderAdapter: Parent - " + ComponentName + ", Medication ID - " + _medicationID.ToString());
                if (_medicationReminderList != null)
                {
                    _medicationReminderList.Adapter = adapter;
                    Log.Info(TAG, "UpdateReminderAdapter: Successfully updated Reminder List adapter");
                }
                else
                {
                    Log.Error(TAG, "UpdateReminderAdapter: _medicationReminderList is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateReminderAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListFragUpdateRemindList), "MedicationListActivity.UpdateReminderAdapter");
            }
        }

        private void UpdateTimeAdapter()
        {
            try
            {
                //var adapter = new MedicationTimeListAdapter(this, _medicationID);
                //Log.Info(TAG, "UpdateTimeAdapter: Parent - " + ComponentName + ", Medication ID - " + _medicationID.ToString());
                //if (_medicationTimeList != null)
                //{
                //    _medicationTimeList.Adapter = adapter;
                //    Log.Info(TAG, "UpdateReminderAdapter: Successfully updated Time List adapter");
                //}
                //else
                //{
                //    Log.Error(TAG, "UpdateTimeAdapter: _medicationTimeList is NULL!");
                //}
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateTimeAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListFragUpdateTimeList), "MedicationListActivity.UpdateTimeAdapter");
            }
        }

        private void GetMedicationData()
        {
            try
            {
                _medication = GlobalData.MedicationItems.Find(med => med.ID == _medicationID);

                if (_medication != null)
                {
                    _medicationSpreadListItems = _medication.MedicationSpread;
                    if (_medicationSpreadListItems != null)
                    {
                        Log.Info(TAG, "GetMedicationData: Medication Spread List has " + _medicationSpreadListItems.Count.ToString() + " items");
                    }
                    else
                    {
                        Log.Error(TAG, "GetMedicationData: _medicationSpreadListItems is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetMedicationData: Medication with ID " + _medicationID.ToString() + " could not be found!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetMedicationData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListFragGetData), "MedicationListActivity.GetMedicationData");
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            try
            {
                if(outState != null)
                {
                    SaveValues(outState);
                }
                base.OnSaveInstanceState(outState);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "");
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedicationListActivitySaveState), "MedicationListActivity.OnSaveInstanceState");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MedicationListMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if(Intent != null)
            {
                if(Intent.Extras != null)
                {
                    _activityTitle = Intent.GetStringExtra("title");
                    _isNew = Intent.GetBooleanExtra("isNew", true);
                    _medicationID = Intent.GetIntExtra("medicationID", -1);
                }
            }

            if (savedInstanceState != null)
            {
                RestoreValues(savedInstanceState);
            }
            else
            {
                Initialise();
            }

            try
            {
                SetContentView(Resource.Layout.MedicationListActivityLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.medicationListToolbar, Resource.String.MedicationActionBarTitle, Color.White);

                GetFieldComponents();
                CheckMicPermission();

                SetupCallbacks();

                GetMedicationData();

                _toolbar.Title = _headerTitleText.Trim();

                SetupSpinners();

                if (_firstTimeView)
                {
                    if (_isNew)
                    {
                        if (_linTimeList != null)
                        {
                            _linTimeList.Visibility = ViewStates.Invisible;
                            _linTimeList.LayoutParameters.Height = 0;
                        }
                        if (_linReminderList != null)
                        {
                            _linReminderList.Visibility = ViewStates.Invisible;
                            _linReminderList.LayoutParameters.Height = 0;
                        }
                        if (_linSep != null)
                        {
                            _linSep.Visibility = ViewStates.Invisible;
                            _linSep.LayoutParameters.Height = 0;
                        }
                        if (_medListSep3 != null)
                            _medListSep3.Visibility = ViewStates.Invisible;
                    }
                    UpdateFields();
                    _firstTimeView = false;
                }

                Log.Info(TAG, "OnCreate: Successfully created View");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListFragCreateView), "MedicationListActivity.OnCreate");
            }
        }

        private void RestoreValues(Bundle savedInstanceState)
        {
            try
            {
                _isNew = savedInstanceState.GetBoolean("IsNew");
                _isDirty = savedInstanceState.GetBoolean("IsDirty");
                _medicationID = savedInstanceState.GetInt("medicationID");
                _selectedMedicationTimeID = savedInstanceState.GetInt("SelectedMedicationTimeID");
                _firstTimeView = savedInstanceState.GetBoolean("FirstTimeView");
                _headerTitleText = savedInstanceState.GetString("HeaderTitleText");
                _selectedTimeItemIndex = savedInstanceState.GetInt("SelectedTimeItemIndex");
                _selectedReminderItemIndex = savedInstanceState.GetInt("SelectedReminderItemIndex");
                _activityTitle = savedInstanceState.GetString("dialogTitle");
                _currentSpeakType = (ConstantsAndTypes.MedicationSpeakType)savedInstanceState.GetInt("currentSpeakType");

                if (_medication == null)
                    _medication = new Medication();

                Log.Info(TAG, "RestoreValues: Restored value IsNew - " + (_isNew ? "True" : "False"));
                Log.Info(TAG, "RestoreValues: Restored value IsDirty - " + (_isDirty ? "True" : "False"));
                Log.Info(TAG, "RestoreValues: Restored value medicationID - " + _medicationID.ToString());
                Log.Info(TAG, "RestoreValues: Restored value SelectedMedicationTimeID - " + _selectedMedicationTimeID.ToString());
                Log.Info(TAG, "RestoreValues: Restored value FirstTimeView - " + (_firstTimeView ? "True" : "False"));
                Log.Info(TAG, "RestoreValues: Restored value HeaderTitleText - " + _headerTitleText);
                Log.Info(TAG, "RestoreValues: Restored value SelectedTimeItemIndex - " + _selectedTimeItemIndex.ToString());
                Log.Info(TAG, "RestoreValues: Restored value SelectedReminderItemIndex - " + _selectedReminderItemIndex.ToString());
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RestoreValues: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedicationListActivityRestoreValues), "MedicationListActivity.RestoreValues");
            }
        }

        private void SaveValues(Bundle outState)
        {
            try
            {
                outState.PutBoolean("IsNew", _isNew);
                outState.PutBoolean("IsDirty", _isDirty);
                outState.PutInt("medicationID", _medicationID);
                outState.PutInt("SelectedMedicationTimeID", _selectedMedicationTimeID);
                outState.PutBoolean("FirstTimeView", true); //special case here for when the orientation changes we want it to make the correct views visible
                outState.PutString("HeaderTitleText", _headerTitleText);
                outState.PutInt("SelectedTimeItemIndex", _selectedTimeItemIndex);
                outState.PutInt("SelectedReminderItemIndex", _selectedReminderItemIndex);
                outState.PutString("dialogTitle", _activityTitle);
                outState.PutInt("currentSpeakType", (int)_currentSpeakType);

                Log.Info(TAG, "SaveValues: Saved value IsNew - " + (_isNew ? "True" : "False"));
                Log.Info(TAG, "SaveValues: Saved value IsDirty - " + (_isDirty ? "True" : "False"));
                Log.Info(TAG, "SaveValues: Saved value medicationID - " + _medicationID.ToString());
                Log.Info(TAG, "SaveValues: Saved value SelectedMedicationTimeID - " + _selectedMedicationTimeID.ToString());
                Log.Info(TAG, "SaveValues: Saved value FirstTimeView - " + (_firstTimeView ? "True" : "False"));
                Log.Info(TAG, "SaveValues: Saved value HeaderTitleText - " + _headerTitleText);
                Log.Info(TAG, "SaveValues: Saved value SelectedTimeItemIndex - " + _selectedTimeItemIndex.ToString());
                Log.Info(TAG, "SaveValues: Saved value SelectedReminderItemIndex - " + _selectedReminderItemIndex.ToString());
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SaveValues: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedicationListActivitySaveValues), "MedicationListActivity.SaveValues");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _medName = FindViewById<EditText>(Resource.Id.edtMedListMedName);
                _dailyDose = FindViewById<EditText>(Resource.Id.edtMedListMainDailyDose);
                _prescriptionType = FindViewById<Spinner>(Resource.Id.spnMedListPrescType);
                _prescriptionWeekDayLabel = FindViewById<TextView>(Resource.Id.txtMedListPrescWeekDayLabel);
                _prescriptionWeekDay = FindViewById<Spinner>(Resource.Id.spnMedListPrescWeekDay);
                _prescriptionMonthDayLabel = FindViewById<TextView>(Resource.Id.txtMedListPrescMonthDayLabel);
                _prescriptionMonthDay = FindViewById<EditText>(Resource.Id.edtMedListPrescMonthDay);
                _medicationTimeList = FindViewById<ListView>(Resource.Id.lstMedListTime);
                _medicationTimeAdd = FindViewById<ImageButton>(Resource.Id.imgbtnMedListTimeAdd);
                _medicationTimeRemove = FindViewById<ImageButton>(Resource.Id.imgbtnMedListTimeRemove);
                _medicationReminderList = FindViewById<ListView>(Resource.Id.lstMedListReminder);
                _medicationReminderAdd = FindViewById<ImageButton>(Resource.Id.imgbtnMedListRemindAdd);
                _medicationReminderRemove = FindViewById<ImageButton>(Resource.Id.imgbtnMedListRemindRemove);

                _linTimeList = FindViewById<LinearLayout>(Resource.Id.linMedListTimeList);
                _linReminderList = FindViewById<LinearLayout>(Resource.Id.linMedListReminderList);
                _linSep = FindViewById<LinearLayout>(Resource.Id.linMedListSep3);

                _speakMedicationName = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakMedicationName);
                _speakDailyDose = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakDailyDose);
                _speakMonthDay = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakMonthDay);

                _medListSep3 = FindViewById<LinearLayout>(Resource.Id.linMedListSep3);

                Log.Info(TAG, "GetFieldComponents: Successfully retrieved field components");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListAdapterGetComponents), "MedicationListActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_medicationTimeAdd != null)
                {
                    _medicationTimeAdd.Click += MedicationTimeAdd_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _medicationTimeAdd is NULL!");
                }
                if(_medicationTimeRemove != null)
                {
                    _medicationTimeRemove.Click += MedicationTimeRemove_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _medicationTimeRemove is NULL!");
                }
                if(_medicationReminderAdd != null)
                {
                    _medicationReminderAdd.Click += MedicationReminderAdd_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _medicationReminderAdd is NULL!");
                }
                if(_medicationReminderRemove != null)
                {
                    _medicationReminderRemove.Click += MedicationReminderRemove_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _medicationReminderRemove is NULL!");
                }
                if(_medName != null)
                {
                    _medName.AfterTextChanged += MedName_AfterTextChanged;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _medName is NULL!");
                }
                if(_dailyDose != null)
                {
                    _dailyDose.AfterTextChanged += DailyDose_AfterTextChanged;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _dailyDose is NULL!");
                }
                if(_prescriptionType != null)
                {
                    _prescriptionType.ItemSelected += PrescriptionType_ItemSelected;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _prescriptionType is NULL!");
                }
                if(_prescriptionWeekDay != null)
                {
                    _prescriptionWeekDay.ItemSelected += PrescriptionWeekDay_ItemSelected;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _prescriptionWeekDay is NULL");
                }
                if(_prescriptionMonthDay != null)
                {
                    _prescriptionMonthDay.AfterTextChanged += PrescriptionMonthDay_AfterTextChanged;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _prescriptionMonthDay is NULL!");
                }
                if(_medicationTimeList != null)
                {
                    _medicationTimeList.ItemClick += MedicationTimeList_ItemClick;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _medicationTimeList is NULL!");
                }
                if(_medicationReminderList != null)
                {
                    _medicationReminderList.ItemClick += MedicationReminderList_ItemClick;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _medicationReminderList is NULL!");
                }
                if(_speakDailyDose != null)
                    _speakDailyDose.Click += SpeakDailyDose_Click;
                if(_speakMedicationName != null)
                    _speakMedicationName.Click += SpeakMedicationName_Click;
                if(_speakMonthDay != null)
                    _speakMonthDay.Click += SpeakMonthDay_Click;
                Log.Info(TAG, "SetupCallbacks: Successfully set up Callbacks");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedActivitySetupCallbacks), "MedicationListActivity.SetupCallbacks");
            }
        }

        private void SpeakMonthDay_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.MedicationMonthDaySpeakPrompt));
                _currentSpeakType = ConstantsAndTypes.MedicationSpeakType.MonthDay;
                Log.Info(TAG, "SpeakMedicationName_Click: Created intent, sending request...");
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "SpeakMonthDay_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListSpeakMonthDay), "MedicationListActivity.SpeakMonthDay_Click");
            }
        }

        private void SpeakMedicationName_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.MedicationNameSpeakPrompt));
                _currentSpeakType = ConstantsAndTypes.MedicationSpeakType.Name;
                Log.Info(TAG, "SpeakMedicationName_Click: Created intent, sending request...");
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "SpeakMedicationName_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListSpeakMedName), "MedicationListActivity.SpeakMedicationName_Click");
            }
        }

        private void SpeakDailyDose_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.MedicationDailyDoseSpeakPrompt));
                _currentSpeakType = ConstantsAndTypes.MedicationSpeakType.DailyDose;
                Log.Info(TAG, "SpeakMedicationName_Click: Created intent, sending request...");
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "SpeakDailyDose_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListSpeakDailyDose), "MedicationListActivity.SpeakDailyDose_Click");
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == ConstantsAndTypes.VOICE_RECOGNITION_REQUEST)
                {
                    if (resultCode == Result.Ok && data != null)
                    {
                        IList<string> matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                        if (matches != null)
                        {
                            switch (_currentSpeakType)
                            {
                                case ConstantsAndTypes.MedicationSpeakType.Name:
                                    if (_medName != null)
                                        _medName.Text = matches[0];
                                    break;
                                case ConstantsAndTypes.MedicationSpeakType.DailyDose:
                                    if (_dailyDose != null)
                                        _dailyDose.Text = matches[0];
                                    break;
                                case ConstantsAndTypes.MedicationSpeakType.MonthDay:
                                    if (_prescriptionMonthDay != null)
                                        _prescriptionMonthDay.Text = matches[0];
                                    break;
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedicationListProcessResponse), "MedicationListActivity.OnActivityResult");
            }
        }

        private void MedicationReminderList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                Log.Info(TAG, "MedicationReminderList_ItemClick: Selected item at position - " + e.Position.ToString());
                _selectedReminderItemIndex = e.Position;
                UpdateReminderAdapter();
                _medicationReminderList.SetSelection(_selectedReminderItemIndex);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "MedicationReminderList_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListActivitySelectReminder), "MedicationListActivity.MedicationReminderList_ItemClick");
            }
        }

        private void MedicationTimeList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedMedicationTimeID = e.Position;
                Log.Info(TAG, "MedicationTimeList_ItemClick: Selected item at position - " + e.Position.ToString());
                _selectedTimeItemIndex = e.Position;
                UpdateTimeAdapter();
                _medicationTimeList.SetSelection(_selectedMedicationTimeID);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "MedicationTimeList_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListActivitySelectTime), "MedicationListActivity.MedicationTimeList_ItemClick");
            }
        }

        private void PrescriptionMonthDay_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            if (_medication != null)
            {
                _medication.PrescriptionType.IsDirty = true;
            }
            _isDirty = true;
            Log.Info(TAG, "PrescriptionMonthDay_AfterTextChanged: Text changed, set IsDirty to TRUE");
        }

        private void PrescriptionWeekDay_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (_medication != null)
            {
                _medication.PrescriptionType.IsDirty = true;
            }
            _isDirty = true;
            Log.Info(TAG, "PrescriptionWeekDay_ItemSelected: Week day item selected, index - " + e.Position.ToString() + ", set IsDirty to TRUE");
        }

        private void PrescriptionType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                if (_medication != null)
                {
                    _medication.PrescriptionType.IsDirty = true;
                }
                _isDirty = true;
                SetPrescriptionItemVisibility();
                Log.Info(TAG, "PrescriptionType_ItemSelected: Prescription type item selected, index - " + e.Position.ToString() + ", set IsDirty to TRUE");
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "PrescriptionType_ItemSelected: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListActivitySelectPresc), "MedicationListActivity.PrescriptionType_ItemSelected");
            }
        }

        private void DailyDose_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            _isDirty = true;
            Log.Info(TAG, "DailyDose_AfterTextChanged: Text changed, set IsDirty to TRUE");
        }

        private void MedName_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            _isDirty = true;
            Log.Info(TAG, "MedName_AfterTextChanged: Text changed, set IsDirty to TRUE");
        }

        private void Add()
        {
            try
            {
                if (!Validate())
                {
                    Toast.MakeText(this, Resource.String.MedListFragAddClickToast, ToastLength.Long).Show();
                    return;
                }

                Log.Info(TAG, "Add_Click: Passed validation");
                if (_isNew)
                    _medication = new Medication();
                if (_isDirty || _isNew)
                {
                    if (_medication != null)
                    {
                        Log.Info(TAG, "Add_Click: Updating data in model");
                        SetDataToMedicationModel();
                        Log.Info(TAG, "Add_Click: Saving Medication item");
                        SaveMedication();
                        SetResult(Result.Ok);
                        Finish();
                    }
                    else
                    {
                        Log.Error(TAG, "Add_Click: _medication is NULL!");
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListActivityAddingMedication), "MedicationListActivity.Add");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.medicationlistActionAdd);
                var itemHelp = menu.FindItem(Resource.Id.medicationlistActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "MedicationListActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            try
            {
                SetResult(Result.Canceled);
                Finish();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "GoBack_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListActivityPrevAct), "MedicationListActivity.GoBack_Click");
            }

            Finish();
        }

        private void MedicationReminderRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedReminderItemIndex == -1)
                {
                    Toast.MakeText(this, GetString(Resource.String.MedListFragReminderRemoveNoSelectToast), ToastLength.Short).Show();
                    return;
                }

                ConfirmReminderDeletion();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "MedicationReminderRemove_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedListFragRemoveRemind), "MedicationListActivity.MedicationReminderRemove_Click");
            }
        }

        private void MedicationReminderAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validate())
                {
                    Log.Info(TAG, "MedicationReminderAdd_Click: Validation failed, name or dose missing!");
                    Toast.MakeText(this, Resource.String.MedListFragRemindAddToast, ToastLength.Long).Show();
                    return;
                }

                if (_selectedMedicationTimeID != -1 && _selectedTimeItemIndex != -1)
                {
                    Log.Info(TAG, "MedicationReminderAdd_Click: _selectedMedicationTimeID - " + _selectedMedicationTimeID.ToString());
                    if (_isNew)
                    {
                        Log.Info(TAG, "MedicationReminderAdd_Click: IsNew is TRUE - Calling SaveMedication");
                        SaveMedication();
                        _isNew = false;
                    }
                    MedicationReminderDialogFragment reminderAddFragment = new MedicationReminderDialogFragment(this, this, _medication.PrescriptionType.PrescriptionType,  (ConstantsAndTypes.DAYS_OF_THE_WEEK)_prescriptionWeekDay.SelectedItemPosition, _medicationSpreadListItems[_selectedMedicationTimeID].ID, _medication.MedicationSpread[_selectedTimeItemIndex].MedicationTakeTime.TakenTime, "Add Medication Reminder");

                    if (reminderAddFragment != null)
                    {
                        Log.Info(TAG, "MedicationReminderAdd_Click: Successfully created Reminder Dialog Fragment");
                        var fragmentTransaction = FragmentManager.BeginTransaction();
                        if (fragmentTransaction != null)
                        {
                            Log.Info(TAG, "MedicationReminderAdd_Click: Created fragmentTransaction");
                            reminderAddFragment.Show(fragmentTransaction, reminderAddFragment.Tag);
                        }
                        else
                        {
                            Log.Error(TAG, "MedicationReminderAdd_Click: Failed to create fragment Transaction!");
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "MedicationReminderAdd_Click: Failed to create Medication Reminder Dialog Fragment!");
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "MedicationReminderAdd_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedListFragAddRemind), "MedicationListActivity.MedicationReminderAdd_Click");
            }
        }

        private void MedicationTimeRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedTimeItemIndex == -1)
                {
                    Toast.MakeText(this, Resource.String.MedListFragRemoveTimeToast, ToastLength.Short).Show();
                    return;
                }

                ConfirmTimeDeletion();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "MedicationTimeRemove_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedListFragRemoveTime), "MedicationListActivity.MedicationTimeRemove_Click");
            }
        }

        private void ConfirmTimeDeletion()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertTitle = GetString(Resource.String.MedListFragConfirmDeleteDialogTitleTime);
                alertHelper.AlertMessage = GetString(Resource.String.MedListFragConfirmDeleteDialogQuestionTime);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.InstanceId = "removeTime";
                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ConfirmTimeDeletion: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Removing Time", "MedicationListActivity.ConfirmTimeDeletion");
            }
        }

        private void ConfirmReminderDeletion()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertTitle = GetString(Resource.String.MedListFragConfirmDeleteDialogTitleRemind);
                alertHelper.AlertMessage = GetString(Resource.String.MedListFragConfirmDeleteDialogQuestionRemind);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.InstanceId = "removeReminder";
                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ConfirmReminderDeletion: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Removing Reminder", "MedicationListActivity.ConfirmReminderDeletion");
            }
        }

        private void MedicationTimeAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validate())
                {
                    Log.Info(TAG, "MedicationTimeAdd_Click: Validation failed, name or dose missing");
                    Toast.MakeText(this, Resource.String.MedListFragRemindAddToast, ToastLength.Long).Show();
                    return;
                }

                //we made it to here meaning validation checks have passed and
                //this is a new medication which needs to have a basic save
                //before attempting to add any list items for medication spread
                //this ensures we have a MedicationID to pass forward
                if (_selectedMedicationTimeID == -1)
                {
                    Log.Info(TAG, "MedicationTimeAdd_Click: _selectedMedicationTimeID - " + _selectedMedicationTimeID.ToString());
                    Log.Info(TAG, "MedicationTimeAdd_Click: Setting data to medication model...");
                    SetDataToMedicationModel();
                    _medication.IsNew = true;
                    Log.Info(TAG, "MedicationTimeAdd_Click: Saving Medication...");
                    SaveMedication();
                    _selectedMedicationTimeID = _medication.ID;
                    _medicationID = _medication.ID;
                    Log.Info(TAG, "MedicationTimeAdd_Click: Added medication, new ID - " + _selectedMedicationTimeID.ToString());
                }

                //MedicationTimeDialogFragment timeAddFragment = new MedicationTimeDialogFragment(this, this, "Add Medication Time");

                //if (timeAddFragment != null)
                //{
                //    var fragmentTransaction = FragmentManager.BeginTransaction();
                //    if (fragmentTransaction != null)
                //    {
                //        timeAddFragment.Show(fragmentTransaction, timeAddFragment.Tag);
                //    }
                //    else
                //    {
                //        Log.Error(TAG, "MedicationTimeAdd_Click: Failed to create fragment Transaction!");
                //    }
                //}
                //else
                //{
                //    Log.Error(TAG, "MedicationTimeAdd_Click: Failed to create Medication Time Dialog Fragment");
                //}
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "MedicationTimeAdd_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedListFragAddTime), "MedicationListActivity.MedicationTimeAdd_Click");
            }
        }

        private bool Validate()
        {
            //we want at the very least the medication name and daily dose before we can save
            if(_medName != null)
            {
                if (_medName.Text.Trim() == "")
                {
                    Log.Info(TAG, "Validate: _medName is empty!");
                    return false;
                }
            }
            else
            {
                Log.Error(TAG, "Validate: _medName is NULL");
                return false;
            }
            if(_dailyDose != null)
            {
                if (_dailyDose.Text.Trim() == "")
                {
                    Log.Info(TAG, "Validate: _dailyDose is empty!");
                    return false;
                }
            }
            else
            {
                Log.Error(TAG, "Validate: _dailDose is NULL!");
                return false;
            }

            return true;
        }

        private void SetDataToMedicationModel()
        {
            try
            {
                if (_medication != null)
                {
                    if (_medName != null)
                    {
                        _medication.MedicationName = _medName.Text.Trim();
                        Log.Info(TAG, "SetDataToMedicationModel: Stored Medication Name - " + _medication.MedicationName);
                    }
                    else
                    {
                        Log.Error(TAG, "SetDataToMedicationModel: _medName is NULL!");
                    }
                    if (_dailyDose != null)
                    {
                        _medication.TotalDailyDosage = Convert.ToInt32(_dailyDose.Text.Trim());
                        Log.Info(TAG, "SetDataToMedicationModel: Stored Total daily dosage - " + _medication.TotalDailyDosage.ToString());
                    }
                    else
                    {
                        Log.Error(TAG, "SetDataToMedicationModel: _dailyDose is NULL!");
                    }
                    if (_prescriptionType != null)
                    {
                        _medication.PrescriptionType.PrescriptionType = (ConstantsAndTypes.PRESCRIPTION_TYPE)_prescriptionType.SelectedItemPosition;
                        Log.Info(TAG, "SetDataToMedicationModel: Stored Prescription type - " + StringHelper.PrescriptionStringForConstant(_medication.PrescriptionType.PrescriptionType));
                    }
                    else
                    {
                        Log.Error(TAG, "SetDataToMedicationModel: _prescriptionType is NULL!");
                    }
                    if (_prescriptionWeekDay != null)
                    {
                        _medication.PrescriptionType.WeeklyDay = (ConstantsAndTypes.DAYS_OF_THE_WEEK)_prescriptionWeekDay.SelectedItemPosition;
                        Log.Info(TAG, "SetDataToMedicationModel: Stored Prescription Weekly Day - " + StringHelper.DayStringForConstant(_medication.PrescriptionType.WeeklyDay));
                    }
                    else
                    {
                        Log.Error(TAG, "SetDataToMedicationModel: _prescriptionWeekDay is NULL!");
                    }
                    //if (_prescriptionMonthDay != null)
                    //{
                    //    _medication.PrescriptionType.MonthlyDay = Convert.ToInt32(_prescriptionMonthDay.Text.Trim());
                    //    Log.Info(TAG, "SetDataToMedicationModel: Stored Prescription Monthly Day - " + _medication.PrescriptionType.MonthlyDay.ToString());
                    //}
                    //else
                    //{
                    //    Log.Error(TAG, "SetDataToMedicationModel: _prescriptionMonthDay is NULL!");
                    //}
                }
                else
                {
                    Log.Error(TAG, "SetDataToMedicationModel: _medication is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetDataToMedicationModel: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListFragSetDataModel), "MedicationListActivity.SetDataToMedicationModel");
            }
        }

        private void SaveMedication()
        {
            try
            {
                if(_medication != null)
                {
                    Log.Info(TAG, "SaveMedication: medication object exists!");
                    if (Validate())
                    {
                        Log.Info(TAG, "SaveMedication: medication passed validation! IsNew - " + (_isNew?"TRUE":"FALSE") + ", IsDirty - " + (_isDirty?"TRUE":"FALSE"));
                        _medication.IsNew = _isNew;
                        _medication.IsDirty = _isDirty;
                        Log.Info(TAG, "SaveMedication: calling Medication Save...");
                        _medication.Save();
                        _medicationID = _medication.ID;
                        Log.Info(TAG, "SaveMedication: Saved Medication with ID - " + _medicationID.ToString());

                        if (_isNew)
                        {
                            GlobalData.MedicationItems.Add(_medication);
                            Log.Info(TAG, "SaveMedication: Added medication to Global cache, ID - " + _medication.ID.ToString());
                        }
                        _isNew = false;
                        _isDirty = false;
                        Log.Info(TAG, "SaveMedication: IsNew and IsDirty now FALSE");
                    }
                }
                else
                {
                    Log.Error(TAG, "SaveMedication: _medication is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SaveMedication: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListFragSaveMedication), "MedicationListActivity.SaveMedication");
            }
        }

        public void MedicationTimeAdded(int dose, ConstantsAndTypes.MEDICATION_FOOD medFood, ConstantsAndTypes.MEDICATION_TIME medTime, DateTime taken)
        {
            try
            {
                //we cant add this spread if the dosage will take us over the total daily dosage
                var totalSpreadDosage = _medication.GetTotalSpreadDosage();
                if(totalSpreadDosage + dose > _medication.TotalDailyDosage)
                {
                    OverDose();
                    return;
                }

                MedicationSpread spread = new MedicationSpread();
                if (spread != null)
                {
                    spread.IsNew = true;
                    Log.Info(TAG, "MedicationTimeAdded: spread IsNew - TRUE");
                    Log.Info(TAG, "MedicationTimeAdded: Medication ID - " + _medicationID.ToString());
                    spread.Dosage = dose;
                    spread.FoodRelevance = medFood;
                    Log.Info(TAG, "MedicationTimeAdded: Stored Dosage - " + spread.Dosage.ToString() + ", Food Relevance - " + StringHelper.MedicationFoodForConstant(medFood));
                    spread.Save(-1, _medicationID);
                    MedicationTime timeTaken = new MedicationTime();
                    if (timeTaken != null)
                    {
                        Log.Info(TAG, "MedicationTimeAdded: Created Medication Time");
                        timeTaken.IsNew = true;
                        timeTaken.MedicationSpreadID = spread.ID;
                        timeTaken.MedicationTime = medTime;
                        timeTaken.TakenTime = taken;
                        Log.Info(TAG, "MedicationTimeAdded: Stored Spread ID - " + timeTaken.MedicationSpreadID.ToString() + ", Medication Time - " + StringHelper.MedicationTimeForConstant(medTime) + ", Taken Time - " + taken.ToShortTimeString());
                        timeTaken.Save(spread.ID);
                        spread.MedicationTakeTime = timeTaken;
                        _medication.MedicationSpread.Add(spread);
                        Log.Info(TAG, "MedicationTimeAdded: Add spread to Medication Spread list");
                        _isDirty = true;
                        UpdateTimeAdapter();
                        Log.Info(TAG, "MedicationTimeAdded: Updated Time Adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "MedicationTimeAdded: Failed to create Medication Time");
                    }
                }
                else
                {
                    Log.Error(TAG, "MedicationTimeAdded: spread is NULL!");
                }
                DumpGlobal.Medication();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "MedicationTimeAdded: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListFragAddTime), "MedicationListActivity.MedicationTimeAdded");
            }
        }

        private void OverDose()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertTitle = GetString(Resource.String.MedListFragConfirmDeleteDialogTitleTime);
                alertHelper.AlertMessage = GetString(Resource.String.MedListFragConfirmDeleteDialogQuestionTime);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.InstanceId = "overdose";
                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OverDose: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Showing dialog", "MedicationListActivity.OverDose");
            }
        }

        public void MedicationReminderAdded(ConstantsAndTypes.DAYS_OF_THE_WEEK day, DateTime reminderTime, int medicationTimeID)
        {
            try
            {
                Log.Info(TAG, "MedicationReminderAdded: medicationTimeID - " + medicationTimeID.ToString());
                //there must be an existing Medication Time before having a reminder
                MedicationReminder reminder = new MedicationReminder();
                if (reminder != null)
                {
                    reminder.MedicationDay = day;
                    reminder.MedicationTime = reminderTime;
                    reminder.IsSet = true;
                    Log.Info(TAG, "MedicationReminderAdded: Stored Medication Day - " + StringHelper.DayStringForConstant(day) + ", Medication Time - " + reminderTime.ToShortTimeString());
                    var spreadHelp = _medication.MedicationSpread.Find(spread => spread.ID == medicationTimeID);
                    reminder.MedicationSpreadID = medicationTimeID;
                    reminder.Save(medicationTimeID);
                    spreadHelp.MedicationTakeReminder = reminder;
                    Log.Info(TAG, "MedicationReminderAdded: Stored Reminder in Spread");
                    _isDirty = true;
                    UpdateReminderAdapter();
                    Log.Info(TAG, "MedicationReminderAdded: Updated Reminder Adapter");
                    //now set the reminder
                    AlarmHelper alarm = new AlarmHelper(this);

                    var ofText = "of ";
                    switch(GlobalData.CurrentIsoLanguageCode.ToLower())
                    {
                        case "eng":
                            ofText = "of ";
                            break;
                        case "spa":
                            ofText = "de ";
                            break;
                    }
                    switch(_medication.PrescriptionType.PrescriptionType)
                    {
                        case ConstantsAndTypes.PRESCRIPTION_TYPE.Daily:
                            alarm.SetAlarm(
                                this, 
                                reminder.ID, 
                                reminderTime, 
                                spreadHelp.Dosage.ToString() + "mg " + ofText + _medication.MedicationName, 
                                ConstantsAndTypes.ALARM_INTERVALS.Daily, 
                                ConstantsAndTypes.DAYS_OF_THE_WEEK.Undefined, 
                                true
                            );
                            break;
                        case ConstantsAndTypes.PRESCRIPTION_TYPE.Weekly:
                            alarm.SetAlarm(
                                this,
                                reminder.ID,
                                reminderTime,
                                spreadHelp.Dosage.ToString() + "mg " + ofText + _medication.MedicationName,
                                ConstantsAndTypes.ALARM_INTERVALS.Weekly,
                                reminder.MedicationDay,
                                true
                            );
                            break;
                        default:
                            alarm.SetAlarm(
                                this,
                                reminder.ID,
                                reminderTime,
                                spreadHelp.Dosage.ToString() + "mg " + ofText + _medication.MedicationName,
                                ConstantsAndTypes.ALARM_INTERVALS.EveryMinute,
                                ConstantsAndTypes.DAYS_OF_THE_WEEK.Undefined,
                                true
                            );
                            Log.Info(TAG, "MedicationReminderAdded: Set alarm with ID " + reminder.ID.ToString() + ", Dosage - " + spreadHelp.Dosage.ToString() + ", Medication name - " + _medication.MedicationName);
                            break;
                    }
                }
                else
                {
                    Log.Error(TAG, "MedicationReminderAdded: reminder is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "MedicationReminderAdded: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListFragAddRemind), "MedicationListActivity.MedicationReminderAdded");
            }
        }

        private void SetupSpinners()
        {
            SetupMedicationTypeSpinner();
            SetupMedicationWeekDaySpinner();
        }

        private void SetupMedicationTypeSpinner()
        {
            try
            {
                if (_prescriptionType != null)
                {
                    string[] medTypes = StringHelper.PrescriptionTypes();

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, medTypes);

                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _prescriptionType.Adapter = adapter;
                        Log.Info(TAG, "SetupMedicationTypeSpinner: Set Prescription Type Adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupMedicationTypeSpinner: Failed to create Adapter");
                    }
                }
                else
                {
                    Log.Error(TAG, "SetupMedicationTypeSpinner: _prescriptionType is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupMedicationTypeSpinner: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListFragTypeSpin), "MedicationListActivity.SetupMedicationTypeSpinner");
            }
        }

        private void SetupMedicationWeekDaySpinner()
        {
            try
            {
                if (_prescriptionWeekDay != null)
                {
                    string[] weekDays = StringHelper.DaysOfTheWeek();

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, weekDays);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _prescriptionWeekDay.Adapter = adapter;
                        Log.Info(TAG, "SetupMedicationTypeSpinner: Set Prescription Week Day Adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupMedicationTypeSpinner: Failed to create Adapter");
                    }
                }
                else
                {
                    Log.Error(TAG, "SetupMedicationWeekDaySpinner: _prescriptionWeekDay = is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupMedicationWeekDaySpinner: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedListFragWeekDaySpin), "MedicationListActivity.SetupMedicationWeekDaySpinner");
            }
        }

        private void SetPrescriptionItemVisibility()
        {
            switch(((ConstantsAndTypes.PRESCRIPTION_TYPE)_prescriptionType.SelectedItemPosition))
            {
                case ConstantsAndTypes.PRESCRIPTION_TYPE.Daily:
                    if (_prescriptionWeekDayLabel != null)
                        _prescriptionWeekDayLabel.Visibility = ViewStates.Invisible;
                    if (_prescriptionWeekDay != null)
                        _prescriptionWeekDay.Visibility = ViewStates.Invisible;
                    if (_prescriptionMonthDayLabel != null)
                        _prescriptionMonthDayLabel.Visibility = ViewStates.Invisible;
                    if (_prescriptionMonthDay != null)
                        _prescriptionMonthDay.Visibility = ViewStates.Invisible;
                    break;
                //case ConstantsAndTypes.PRESCRIPTION_TYPE.Monthly:
                //    if (_prescriptionWeekDayLabel != null)
                //        _prescriptionWeekDayLabel.Visibility = ViewStates.Invisible;
                //    if (_prescriptionWeekDay != null)
                //        _prescriptionWeekDay.Visibility = ViewStates.Invisible;
                //    if (_prescriptionMonthDayLabel != null)
                //        _prescriptionMonthDayLabel.Visibility = ViewStates.Visible;
                //    if (_prescriptionMonthDay != null)
                //        _prescriptionMonthDay.Visibility = ViewStates.Visible;
                //    break;
                case ConstantsAndTypes.PRESCRIPTION_TYPE.Weekly:
                    if (_prescriptionWeekDayLabel != null)
                        _prescriptionWeekDayLabel.Visibility = ViewStates.Visible;
                    if (_prescriptionWeekDay != null)
                        _prescriptionWeekDay.Visibility = ViewStates.Visible;
                    if (_prescriptionMonthDayLabel != null)
                        _prescriptionMonthDayLabel.Visibility = ViewStates.Invisible;
                    if (_prescriptionMonthDay != null)
                        _prescriptionMonthDay.Visibility = ViewStates.Invisible;
                    break;
            }
        }

        private void CancelAlarm(int reminderID)
        {
            new AlarmHelper(this).CancelAlarm(reminderID);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    GoBack();
                    return true;
                }

                switch (item.ItemId)
                {
                    case Resource.Id.medicationlistActionAdd:
                        Add();
                        return true;
                    case Resource.Id.medicationlistActionHelp:
                        Intent intent = new Intent(this, typeof(TreatmentMedicationHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            try
            {
                if (instanceId == "removeTime")
                {
                    //start with getting the spread id of the selected item
                    var spreadID = _medicationSpreadListItems[_selectedTimeItemIndex].ID;

                    //is there a reminder set?
                    bool isSet = false;
                    if (_medicationSpreadListItems[_selectedTimeItemIndex].MedicationTakeReminder != null)
                    {
                        isSet = _medicationSpreadListItems[_selectedTimeItemIndex].MedicationTakeReminder.IsSet;
                    }
                    if (isSet)
                    {

                        if (_medicationSpreadListItems[_selectedTimeItemIndex].MedicationTakeReminder != null)
                        {
                            CancelAlarm(_medicationSpreadListItems[_selectedTimeItemIndex].MedicationTakeReminder.ID);
                            _medicationSpreadListItems[_selectedTimeItemIndex].MedicationTakeReminder.Remove();
                        }
                    }

                    if (_medicationSpreadListItems[_selectedTimeItemIndex].MedicationTakeTime != null)
                        _medicationSpreadListItems[_selectedTimeItemIndex].MedicationTakeTime.Remove();

                    //finally, remove the spread
                    _medicationSpreadListItems[_selectedTimeItemIndex].Remove();
                    _medicationSpreadListItems.Remove(_medicationSpreadListItems[_selectedTimeItemIndex]);

                    if (_medicationSpreadListItems.Count == 0)
                    {
                        _selectedMedicationTimeID = -1;
                        _selectedReminderItemIndex = -1;
                        _selectedTimeItemIndex = -1;
                    }
                    var globalMedication = GlobalData.MedicationItems.Find(med => med.ID == _medicationID);

                    if (globalMedication != null)
                    {
                        var globalSpread = globalMedication.MedicationSpread.Find(spread => spread.ID == spreadID);
                        if (globalSpread != null)
                            globalMedication.MedicationSpread.Remove(globalSpread);
                        UpdateTimeAdapter();
                        UpdateReminderAdapter();
                        _isDirty = true;
                    }
                }

                if(instanceId == "removeReminder")
                {
                    try
                    {
                        //grab the list set
                        List<MedicationSpread> medSpread = new List<MedicationSpread>();
                        foreach (var spread in _medication.MedicationSpread)
                        {
                            if (spread.MedicationTakeReminder != null)
                            {
                                if (spread.MedicationTakeReminder.IsSet)
                                    medSpread.Add(spread);
                            }
                        }
                        if (medSpread.Count > 0)
                        {
                            //now get the spread
                            var medicationSpread = medSpread[_selectedReminderItemIndex];
                            var spreadID = medicationSpread.ID;
                            if (medicationSpread != null)
                            {
                                Log.Info(TAG, "AlertPositiveButtonSelect: Found reminder with ID " + spreadID.ToString());
                                var medication = GlobalData.MedicationItems.Find(med => med.ID == _medicationID);
                                if (medication != null)
                                {
                                    var rmd = medication.MedicationSpread.Find(spread => spread.ID == spreadID).MedicationTakeReminder;
                                    if (rmd != null)
                                    {
                                        rmd.IsSet = false;
                                        CancelAlarm(rmd.ID);
                                        Log.Info(TAG, "AlertPositiveButtonSelect: Removed alarm with ID " + rmd.ID.ToString() + ", Dosage - " + medicationSpread.Dosage.ToString() + ", Medication name - " + _medication.MedicationName);
                                    }
                                    else
                                    {
                                        Log.Info(TAG, "AlertPositiveButtonSelect: Could not find reminder with Spread ID " + spreadID.ToString());
                                    }
                                }
                                else
                                {
                                    Log.Info(TAG, "AlertPositiveButtonSelect: Could not find Medication with ID " + _medicationID.ToString() + ", Local medication ID is " + _medication.ID.ToString());
                                }
                                medicationSpread.MedicationTakeReminder.Remove();
                                medicationSpread.MedicationTakeReminder = null;
                                UpdateReminderAdapter();
                            }
                        }
                        _selectedReminderItemIndex = -1;
                        _isDirty = true;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                        if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedListFragRemoveRemind), "MedicationListActivity.AlertPositiveButtonSelect");
                    }
                }

                if(instanceId == "overdose")
                {
                    try
                    {
                        Toast.MakeText(this, Resource.String.MedListFragOverdoseAbandonToast, ToastLength.Long).Show();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                        if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedListFragAddTime), "MedicationListActivity.AlertPositiveButtonSelect");
                    }
                }

                if (instanceId == "useMic")
                {
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedListFragRemoveTime), "MedicationListActivity.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                PermissionResultUpdate(Permission.Denied);
            }
        }

        private void CheckMicPermission()
        {
            try
            {
                if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.UseMicrophone)))
                {
                    AttemptPermissionRequest();
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "CheckMicPermission: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCheckingApplicationPermission), "MedicationListActivity.CheckMicPermission");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            try
            {
                if (requestCode == ConstantsAndTypes.REQUEST_CODE_PERMISSION_USE_MICROPHONE)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //now update the global permission
                        if (GlobalData.ApplicationPermissions == null)
                        {
                            //if null then we can go get permissions
                            PermissionsHelper.SetupDefaultPermissionList(this);
                        }
                        else
                        {
                            //we need to update the existing permission
                            if (PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone))
                            {
                                GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == ConstantsAndTypes.AppPermission.UseMicrophone).PermissionGranted = Permission.Granted;
                            }
                        }
                        PermissionResultUpdate(Permission.Granted);
                    }
                    else
                    {
                        PermissionResultUpdate(Permission.Denied);
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRequestingApplicationPermission), "MedicationListActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                if(_speakMedicationName != null)
                    _speakMedicationName.SetImageResource(Resource.Drawable.micgreyscale);
                if(_speakDailyDose != null)
                    _speakDailyDose.SetImageResource(Resource.Drawable.micgreyscale);
                if(_speakMonthDay != null)
                    _speakMonthDay.SetImageResource(Resource.Drawable.micgreyscale);

                if(_speakMedicationName != null)
                    _speakMedicationName.Enabled = false;
                if(_speakDailyDose != null)
                    _speakDailyDose.Enabled = false;
                if(_speakMonthDay != null)
                    _speakMonthDay.Enabled = false;
            }
        }

        private void ShowPermissionRationale()
        {
            try
            {
                if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagMic").SettingValue == "True")
                {
                    if(!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.UseMicrophone) == true))
                    {
                        PermissionResultUpdate(Permission.Denied);
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                    return;
                }

                AlertHelper alertHelper = new AlertHelper(this);

                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolInformation;
                alertHelper.AlertMessage = GetString(Resource.String.RequestPermissionUseMicrophoneAlertMessage);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertTitle = GetString(Resource.String.RequestPermissionUseMicrophoneAlertTitle);
                alertHelper.InstanceId = "useMic";
                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ShowPermissionRationale: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "MedicationListActivity.ShowPermissionRationale");
            }
        }

        public void AttemptPermissionRequest()
        {
            try
            {
                if (PermissionsHelper.ShouldShowPermissionRationale(this, ConstantsAndTypes.AppPermission.UseMicrophone))
                {
                    ShowPermissionRationale();
                    return;
                }
                else
                {
                    //just request the permission
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "AttemptPermissionRequest: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "MedicationListActivity.AttemptPermissionRequest");
            }
        }
    }
}
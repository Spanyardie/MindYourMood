using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Speech;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Util;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Views;
using Android.Graphics;
using Android.Runtime;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Widget;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class MedicationMainActivity : AppCompatActivity, ViewPager.IOnPageChangeListener, IMedicationSpeakCallback, IAlertCallback
    {
        public const string TAG = "M:MedicationMainActivity";

        private ViewPager _viewPager;
        private PagerTitleStrip _titleStrip;

        private Toolbar _toolbar;

        private ConstantsAndTypes.MedicationSpeakType _currentSpeakType;

        private Medication _medication;
        private bool _speakPermission = false;
        private MedicationPagerAdapter.MedicationView _currentView = MedicationPagerAdapter.MedicationView.MedicationName;

        private Button _doneButton;
        private Button _cancelButton;

        private const int FIRST_PAGE = 0;
        private const int SECOND_PAGE = 1;
        private const int THIRD_PAGE = 2;
        private int _currentPage = -1;

        private int _medicationID = -1;
        private bool _isNew = true;
        private string _title = "";

        private ImageLoader _imageLoader = null;

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            //TODO:
            return base.DispatchKeyEvent(e);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            if(outState != null)
            {
                var adapter = (MedicationPagerAdapter)_viewPager.Adapter;
                if (adapter != null)
                    _medication = adapter.Medication;
            }
        }

        public void SpeakMedicationDosage()
        {
            try
            {
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.MedicationDailyDoseSpeakPrompt));
                _currentSpeakType = ConstantsAndTypes.MedicationSpeakType.DailyDose;
                Log.Info(TAG, "SpeakMedicationDosage: Created intent, sending request...");
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SpeakMedicationDosage: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListSpeakDailyDose), "MedicationListActivity.SpeakMedicationDosage");
            }
        }

        public void SpeakMedicationName()
        {
            try
            {
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.MedicationNameSpeakPrompt));
                _currentSpeakType = ConstantsAndTypes.MedicationSpeakType.Name;
                Log.Info(TAG, "SpeakMedicationName: Created intent, sending request...");
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SpeakMedicationName: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListSpeakMedName), "MedicationListActivity.SpeakMedicationName");
            }
        }

        public void SpeakMonthDay()
        {
            try
            {
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.MedicationMonthDaySpeakPrompt));
                _currentSpeakType = ConstantsAndTypes.MedicationSpeakType.MonthDay;
                Log.Info(TAG, "SpeakMonthDay: Created intent, sending request...");
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SpeakMonthDay: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationListSpeakMonthDay), "MedicationListActivity.SpeakMonthDay");
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.MedicationMainLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.medicationMainToolbar, Resource.String.MedicationToolbarTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.medication,
                    new ImageLoadingListener
                    (
                        loadingComplete: (imageUri, view, loadedImage) =>
                        {
                            var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                            ImageLoader_LoadingComplete(null, args);
                        }
                    )
                );

                SetupCallbacks();

                CheckMicPermission();

                if (Intent != null)
                {
                    if (Intent.HasExtra("title"))
                        _title = Intent.GetStringExtra("title");

                    if (Intent.HasExtra("isNew"))
                        _isNew = Intent.GetBooleanExtra("isNew", true);

                    if (Intent.HasExtra("medicationID"))
                        _medicationID = Intent.GetIntExtra("medicationID", -1);

                    if (_medicationID != -1)
                    {
                        _medication = GlobalData.MedicationItems.Find(med => med.ID == _medicationID);
                    }
                }

                if (_medication == null)
                {
                    _medication = new Medication();
                }

                UpdateAdapter();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)this, e, "Failed creating Medication Main Activity", "MedicationPagerAdapter.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_viewPager != null)
                _viewPager.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void CheckMicPermission()
        {
            try
            {
                if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.UseMicrophone)))
                {
                    AttemptPermissionRequest();
                }
                else
                {
                    _speakPermission = true;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "CheckMicPermission: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "MedicationMainActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "MedicationMainActivity.AttemptPermissionRequest");
            }
        }

        private void ShowPermissionRationale()
        {
            try
            {
                if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagMic").SettingValue == "True") return;

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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "MedicationMainActivity.ShowPermissionRationale");
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            string spokenText;
            try
            {
                if (requestCode == ConstantsAndTypes.VOICE_RECOGNITION_REQUEST && resultCode == Result.Ok)
                {
                    IList<string> matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches != null)
                    {
                        spokenText = matches[0];
                        switch(_currentSpeakType)
                        {
                            case ConstantsAndTypes.MedicationSpeakType.Name:
                                _medication.MedicationName = spokenText.Trim();
                                break;
                            case ConstantsAndTypes.MedicationSpeakType.DailyDose:
                                _medication.TotalDailyDosage = Convert.ToInt32(spokenText.Trim());
                                break;
                            //case ConstantsAndTypes.MedicationSpeakType.MonthDay:
                            //    _medication.PrescriptionType.MonthlyDay = Convert.ToInt32(spokenText.Trim());
                            //    break;
                        }
                        UpdateAdapter();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCheckingVoiceRecognition), "MedicationMainActivity.OnActivityResult");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MedicationMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.medicationActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.medicationActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.medicationActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "MedicationMainActivity.SetActionIcons");
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
                        _speakPermission = true; ;
                    }
                    else
                    {
                        _speakPermission = false;
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                    var item = _currentPage;
                    UpdateAdapter();
                    _viewPager.SetCurrentItem(item, true);
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "MedicationMainActivity.OnRequestPermissionsResult");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_viewPager != null)
                    _viewPager.SetOnPageChangeListener(this);
                if (_doneButton != null)
                    _doneButton.Click += DoneButton_Click;
                if (_cancelButton != null)
                    _cancelButton.Click += CancelButton_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)this, e, "Failed setting up callbacks", "MedicationPagerAdapter.SetupCallbacks");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_medication != null)
                {
                    //if we have a medication Id, then we probably want to cancel any changes
                    if (_medication.ID != -1)
                    {
                        _medication = null;
                        SetResult(Result.Canceled);
                        Finish();
                        return;
                    }
                    else
                    {
                        //safe to just go back to medication list
                        SetResult(Result.Canceled);
                        Finish();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "CancelButton_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)this, ex, "Failed during cancel operation", "MedicationPagerAdapter.CancelButton_Click");
            }
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            //if we have a new or dirty medication object, then save it
            //this needs to be grabbed from the adapter
            try
            {
                bool isNew, isDirty;

                var adapter = (MedicationPagerAdapter)_viewPager.Adapter;
                if (adapter != null)
                {
                    var medication = adapter.Medication;
                    if (medication != null)
                    {
                        isNew = medication.IsNew;
                        isDirty = medication.IsDirty;
                        if ((isNew || isDirty) && Validate(medication))
                        {
                            medication.Save();
                            if (isNew)
                            {
                                GlobalData.MedicationItems.Add(medication);
                            }
                            else if (isDirty)
                            {
                                var existingMed = GlobalData.MedicationItems.FindIndex(med => med.ID == medication.ID);
                                GlobalData.MedicationItems[existingMed] = medication;
                            }
                            SetResult(Result.Ok);
                        }
                        else
                        {
                            SetResult(Result.Canceled);
                        }
                        Finish();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "DoneButton_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)this, ex, "Failed during attempt to save medication", "MedicationPagerAdapter.DoneButton_Click");
            }
        }

        private bool Validate(Medication medication)
        {
            //we want at the very least the medication name and daily dose before we can save
            if (medication != null)
            {
                if (medication.MedicationName.Trim() == "")
                {
                    Log.Info(TAG, "Validate: Medication Name is empty!");
                    return false;
                }
            }
            else
            {
                Log.Error(TAG, "Validate: Medication object is NULL");
                return false;
            }
            if (medication != null)
            {
                if (medication.TotalDailyDosage == 0)
                {
                    Log.Info(TAG, "Validate: TotalDailyDosage is zero!");
                    return false;
                }
            }
            else
            {
                Log.Error(TAG, "Validate: Medication object is NULL!");
                return false;
            }

            return true;
        }

        private void GetFieldComponents()
        {
            try
            {
                _viewPager = FindViewById<ViewPager>(Resource.Id.pagerMedicationMain);
                _titleStrip = FindViewById<PagerTitleStrip>(Resource.Id.pagerTitleMedicationMain);

                _doneButton = FindViewById<Button>(Resource.Id.btnDone);
                _cancelButton = FindViewById<Button>(Resource.Id.btnCancel);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)this, e, "Failed to retrieve field components", "MedicationPagerAdapter.GetFieldComponents");
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //TODO:
            return base.OnOptionsItemSelected(item);
        }

        private void UpdateAdapter(string errorMessage = "")
        {
            try
            {
                MedicationPagerAdapter adapter = new MedicationPagerAdapter(this, _medication, _speakPermission);

                _viewPager.Adapter = adapter;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)this, e, "Failed updating pager adapter", "MedicationPagerAdapter.UpdateAdapter");
            }
        }

        public void OnPageScrollStateChanged(int state)
        {

        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {

        }

        public void OnPageSelected(int position)
        {
            int picResource = -1;

            switch(position)
            {
                case FIRST_PAGE:
                    picResource = Resource.Drawable.MedicationNameAndDose;
                    _currentView = MedicationPagerAdapter.MedicationView.MedicationName;
                    break;
                case SECOND_PAGE:
                    picResource = Resource.Drawable.MedicationInterval;
                    _currentView = MedicationPagerAdapter.MedicationView.MedicationInterval;
                    break;
                case THIRD_PAGE:
                    picResource = Resource.Drawable.MedicationTimes;
                    _currentView = MedicationPagerAdapter.MedicationView.MedicationTimes;
                    break;
            }

            if(picResource != -1)
            {
                if(_viewPager != null)
                {
                    _viewPager.SetBackgroundResource(picResource);
                }
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            try
            {
                if (instanceId == "medicationCancel")
                {
                }

                MedicationPagerAdapter adapter = (MedicationPagerAdapter)_viewPager.Adapter;
                if (adapter == null) return;
                _medication = adapter.Medication;
                if (_medication == null) return;

                var selectedItemIndex = adapter.SelectedItemIndex;
                if (instanceId == "removeTime")
                {
                    //start with getting the spread id of the selected item
                    var spreadID = _medication.MedicationSpread[selectedItemIndex].ID;

                    //is there a reminder set?
                    bool isSet = false;
                    if (_medication.MedicationSpread[selectedItemIndex].MedicationTakeReminder != null)
                    {
                        isSet = _medication.MedicationSpread[selectedItemIndex].MedicationTakeReminder.IsSet;
                    }
                    if (isSet)
                    {

                        if (_medication.MedicationSpread[selectedItemIndex].MedicationTakeReminder != null)
                        {
                            CancelAlarm(_medication.MedicationSpread[selectedItemIndex].MedicationTakeReminder.ID);
                            _medication.MedicationSpread[selectedItemIndex].MedicationTakeReminder.Remove();
                        }
                    }

                    if (_medication.MedicationSpread[selectedItemIndex].MedicationTakeTime != null)
                        _medication.MedicationSpread[selectedItemIndex].MedicationTakeTime.Remove();

                    //finally, remove the spread
                    _medication.MedicationSpread[selectedItemIndex].Remove();
                    _medication.MedicationSpread.Remove(_medication.MedicationSpread[selectedItemIndex]);

                    //if (_medication.MedicationSpread.Count == 0)
                    //{
                    //    _selectedMedicationTimeID = -1;
                    //    _selectedItemIndex = -1;
                    //}
                    var globalMedication = GlobalData.MedicationItems.Find(med => med.ID == _medication.ID);

                    if (globalMedication != null)
                    {
                        var globalSpread = globalMedication.MedicationSpread.Find(spread => spread.ID == spreadID);
                        if (globalSpread != null)
                            globalMedication.MedicationSpread.Remove(globalSpread);
                        UpdateAdapter();
                        _viewPager.SetCurrentItem((int)_currentView, false);
                        //IsDirty = true;
                    }
                }
                if (instanceId == "removeReminder")
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
                            var medicationSpread = medSpread[selectedItemIndex];
                            var spreadID = medicationSpread.ID;
                            if (medicationSpread != null)
                            {
                                Log.Info(TAG, "AlertPositiveButtonSelect: Found reminder with ID " + spreadID.ToString());
                                var medication = GlobalData.MedicationItems.Find(med => med.ID == _medication.ID);
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
                                    Log.Info(TAG, "AlertPositiveButtonSelect: Could not find Medication with ID " + _medication.ID.ToString() + ", Local medication ID is " + _medication.ID.ToString());
                                }
                                medicationSpread.MedicationTakeReminder.Remove();
                                medicationSpread.MedicationTakeReminder = null;
                                UpdateAdapter();
                                _viewPager.SetCurrentItem((int)_currentView, false);
                            }
                        }
                        //IsDirty = true;
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                        if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedListFragRemoveRemind), "MedicationMainActivity.AlertPositiveButtonSelect");
                    }
                }
                if (instanceId == "overdose")
                {
                    try
                    {
                        Toast.MakeText(this, Resource.String.MedListFragOverdoseAbandonToast, ToastLength.Long).Show();
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                        if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedListFragAddTime), "MedicationMainActivity.AlertPositiveButtonSelect");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)this, ex, GetString(Resource.String.ErrorMedListFragAddTime), "MedicationMainActivity.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            //do nothing
        }

        private void CancelAlarm(int reminderID)
        {
            new AlarmHelper(this).CancelAlarm(reminderID);
        }
    }
}
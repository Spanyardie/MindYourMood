using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using V7Sup = Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Speech;
using Android.Graphics;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.SubActivities.Help.ThoughtsHelp;
using Android.Content.PM;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.Wizards
{
    [Activity]
    public class ThoughtRecordWizardEvidenceForHotThoughtStep : V7Sup.AppCompatActivity, IAlertCallback
    {
        public static string TAG = "M:ThoughtRecordWizardEvidenceForHotThoughtStep";

        private ListView _evidenceForHotThoughtList;
        private EditText _evidenceText;
        private TextView _hotThoughtText;

        private bool _validated;

        private bool _setupCallbacksComplete;
        private int _selectedItemIndex = -1;
        private int _hotThoughtId = -1;

        private ImageButton _speakEvidenceFor;
        private bool _spokenEvidenceFor = false;
        private string _spokenText = "";
        private Toolbar _toolbar;

        private Button _continue;

        private LinearLayout _evidenceForThoughtMain;
        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutInt("selectedItemIndex", +_selectedItemIndex);
                outState.PutInt("hotThoughtID", _hotThoughtId);
            }
            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.EvidenceForHotThoughtMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.EvidenceForHotThoughtActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.EvidenceForHotThoughtActionRemove);
                var itemCancel = menu.FindItem(Resource.Id.EvidenceForHotThoughtActionCancel);
                var itemHelp = menu.FindItem(Resource.Id.EvidenceForHotThoughtActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if(itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
                        if (itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ThoughtRecordWizardEvidenceForHotThoughtStep.SetActionIcons");
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    Previous();
                    return true;
                }

                switch (item.ItemId)
                {
                    case Resource.Id.EvidenceForHotThoughtActionAdd:
                        AddEvidenceForThought();
                        return true;
                    case Resource.Id.EvidenceForHotThoughtActionRemove:
                        RemoveEvidenceForThought();
                        return true;

                    case Resource.Id.EvidenceForHotThoughtActionCancel:
                        Cancel();
                        return true;

                    case Resource.Id.EvidenceForHotThoughtActionHelp:
                        Intent intent = new Intent(this, typeof(EvidenceForHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                {
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");
                    _hotThoughtId = savedInstanceState.GetInt("hotThoughtID");
                }

                SetContentView(Resource.Layout.EvidenceForHotThought);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.evidenceForToolbar, Resource.String.evidenceForHeading, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.evidence,
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

                GetHotThoughtText();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateForHotThought), "ThoughtRecordWizardEvidenceForHotThoughtStep.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_evidenceForThoughtMain != null)
                _evidenceForThoughtMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            try
            {
                if (e.KeyCode == Keycode.Back)
                {
                    GlobalData.RemoveEvidenceForHotThought();
                    GlobalData.EvidenceForHotThoughtItems.Clear();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "DispatchKeyEvent: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorForHotThoughtDispatchkeyEvent), "ThoughtRecordWizardEvidenceForHotThoughtStep.DispatchKeyEvent");
            }
            return base.DispatchKeyEvent(e);
        }

        private void GetHotThoughtText()
        {
            try
            {
                string thoughtText = GetString(Resource.String.evidenceForHotThoughtNoHotThoughtDefined);
                if (_hotThoughtText != null)
                {
                    foreach (var thought in GlobalData.AutomaticThoughtsItems)
                    {
                        if (thought.IsHotThought)
                        {
                            thoughtText = thought.Thought.Trim();
                            _hotThoughtId = thought.AutomaticThoughtsId;
                            break;
                        }
                    }
                    _hotThoughtText.Text = thoughtText.Trim();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetHotThoughtText: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorGettingHotThoughtText), "ThoughtRecordWizardEvidenceForHotThoughtStep.GetHotThoughtText");
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                GetFieldComponents();
                SetupCallbacks();

                if (_spokenEvidenceFor)
                {
                    if (_evidenceText != null)
                        _evidenceText.Text = _spokenText;
                    _spokenEvidenceFor = false;
                }

                UpdateAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnResume: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorResumeForHotThought), "ThoughtRecordWizardEvidenceForHotThoughtStep.OnResume");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _evidenceForHotThoughtList = FindViewById<ListView>(Resource.Id.lstEvidenceForHotThought);
                _evidenceText = FindViewById<EditText>(Resource.Id.edtEvidenceForThought);
                _hotThoughtText = FindViewById<TextView>(Resource.Id.txtHotThoughtText);
                _speakEvidenceFor = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakEvidenceFor);
                _continue = FindViewById<Button>(Resource.Id.btnContinue);
                _evidenceForThoughtMain = FindViewById<LinearLayout>(Resource.Id.linEvidenceForThoughtMain);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorForHotThoughtGetComponents), "ThoughtRecordWizardEvidenceForHotThoughtStep.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (!_setupCallbacksComplete)
                {
                    if (_evidenceForHotThoughtList != null)
                        _evidenceForHotThoughtList.ItemClick += EvidenceForHotThoughtList_ItemClick;
                    if(_speakEvidenceFor != null)
                        _speakEvidenceFor.Click += SpeakEvidenceFor_Click;
                    if(_continue != null)
                        _continue.Click += Continue_Click;
                    _setupCallbacksComplete = true;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorForHotThoughtSetupCallbacks), "ThoughtRecordWizardEvidenceForHotThoughtStep.SetupCallbacks");
            }
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void SpeakEvidenceFor_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.EvidenceForSpeakWhatPrompt));

            Log.Info(TAG, "SpeakEvidenceFor_Click: Created intent, sending request...");
            StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == ConstantsAndTypes.VOICE_RECOGNITION_REQUEST && resultCode == Result.Ok)
                {
                    IList<string> matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches != null)
                    {
                        _spokenEvidenceFor = true;
                        _spokenText = matches[0];
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Checking Voice Recognition result", "ThoughtRecordWizardEvidenceForHotThoughtStep.OnActivityResult");
            }
        }

        private void Previous()
        {
            try
            {
                GlobalData.RemoveEvidenceForHotThought();
                GlobalData.EvidenceForHotThoughtItems.Clear();
                Intent intent = new Intent(this, typeof(ThoughtRecordWizardAutomaticThoughtsStep));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                Finish();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Previous: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorForHotThoughtPreviousButton), "ThoughtRecordWizardEvidenceForHotThoughtStep.Previous");
            }
        }

        private void Cancel()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertMessage = GetString(Resource.String.ThoughtRecordWizardAutomaticThoughtConfirm);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonCancelCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonOKCaption);
                alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardAutomaticThoughtCancel);
                alertHelper.InstanceId = "evidenceForCancel";

                alertHelper.ShowAlert();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "CancelButton_Click: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCancelForHotThoughtAddition), "ThoughtRecordWizardEvidenceForHotThoughtStep.CancelButton_Click");
            }
        }

        private void Next()
        {
            try
            {
                Validate();
                if (!_validated)
                    return;

                StoreEvidenceForHotThought();

                Intent intent = new Intent(this, typeof(ThoughtRecordWizardEvidenceAgainstHotThoughtStep));
                StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Next: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorNextButtonForHot), "ThoughtRecordWizardEvidenceForHotThoughtStep.Next");
            }
        }

        private void StoreEvidenceForHotThought()
        {
            Globals dbHelp = new Globals();

            try
            {
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var thought in GlobalData.EvidenceForHotThoughtItems)
                    {
                        if (thought.EvidenceForHotThoughtId == 0)
                        {
                            thought.AutomaticThoughtsId = _hotThoughtId;
                            thought.Save(sqlDatabase);
                        }
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "StoreEvidenceForHotThought: Exception - " + e.Message);
                if (dbHelp != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStoringEvidenceFor), "ThoughtRecordWizardEvidenceForHotThoughtStep.StoreEvidenceForHotThought");
            }
        }

        private void AddEvidenceForThought()
        {
            try
            {
                if (_evidenceText == null)
                    return;

                if (string.IsNullOrWhiteSpace(_evidenceText.Text.Trim()))
                {
                    var resourceString = GetString(Resource.String.evidenceForHotThoughtNoEvidence);
                    _evidenceText.Error = resourceString;
                    return;
                }

                EvidenceForHotThought thought = new EvidenceForHotThought();
                thought.ThoughtRecordId = GlobalData.ThoughtRecordId;
                thought.Evidence = _evidenceText.Text.Trim();
                if (GlobalData.EvidenceForHotThoughtItems == null)
                {
                    GlobalData.EvidenceForHotThoughtItems = new List<EvidenceForHotThought>();
                }
                GlobalData.EvidenceForHotThoughtItems.Add(thought);

                UpdateAdapter();

                _evidenceText.Text = "";
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AddEvidenceForThought_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAddingFor), "ThoughtRecordWizardEvidenceForHotThoughtStep.AddEvidenceForThought_Click");
            }
        }

        private void UpdateAdapter()
        {
            try
            {
                EvidenceForHotThoughtItemsAdapter thoughtAdapter = new EvidenceForHotThoughtItemsAdapter(this);
                if (_evidenceForHotThoughtList != null)
                    _evidenceForHotThoughtList.Adapter = thoughtAdapter;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorForUpdateAdapter), "ThoughtRecordWizardEvidenceForHotThoughtStep.UpdateAdapter");
            }
        }

        private void RemoveEvidenceForThought()
        {
            try
            {
                if (_selectedItemIndex > -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertMessage = GetString(Resource.String.ThoughtRecordWizardEvidenceForDeleteConfirm);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonCancelCaption);
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonOKCaption);
                    alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardEvidenceForDeleteTitle);
                    alertHelper.InstanceId = "evidenceForRemove";

                    alertHelper.ShowAlert();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RemoveEvidenceForThought: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRemovingFor), "ThoughtRecordWizardEvidenceForHotThoughtStep.RemoveEvidenceForThought");
            }
        }

        private void EvidenceForHotThoughtList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;
                UpdateAdapter();
                _evidenceForHotThoughtList.SetSelection(_selectedItemIndex);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "EvidenceForHotThoughtList_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSelectingFor), "ThoughtRecordWizardEvidenceForHotThoughtStep.EvidenceForHotThoughtList_ItemClick");
            }
        }

        private void Validate()
        {
            _validated = true;

            try
            {
                if (_evidenceForHotThoughtList != null)
                {
                    if (_evidenceForHotThoughtList.Adapter != null)
                    {
                        if (_evidenceForHotThoughtList.Adapter.Count == 0)
                        {
                            var resourceString = GetString(Resource.String.evidenceForHotThoughtNoHotThoughtDefinedOnNext);
                            Toast.MakeText(this, resourceString, ToastLength.Short).Show();
                            _validated = false;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Validate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorValidatingFor), "ThoughtRecordWizardEvidenceForHotThoughtStep.Validate");
            }
        }

        public int GetSelectedItem()
        {
            return _selectedItemIndex;
        }

        private void Cleanup()
        {
            try
            {
                GlobalData.RemoveThoughtRecord();
                GlobalData.SituationItem.What = "";
                GlobalData.SituationItem.When = "";
                GlobalData.SituationItem.Where = "";
                GlobalData.SituationItem.Who = "";
                GlobalData.RemoveSituation();
                GlobalData.RemoveMoods();
                GlobalData.RemoveAutomaticThoughts();
                GlobalData.RemoveEvidenceForHotThought();
                GlobalData.MoodItems.Clear();
                GlobalData.AutomaticThoughtsItems.Clear();
                GlobalData.EvidenceForHotThoughtItems.Clear();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Cleanup: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorForCleanup), "ThoughtRecordWizardEvidenceForHotThoughtStep.Cleanup");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if(instanceId == "evidenceForCancel")
            {
                Cleanup();
                Intent intent = new Intent(this, typeof(ThoughtRecordsActivity));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
            }
            if(instanceId == "evidenceForRemove")
            {
                var item = GlobalData.EvidenceForHotThoughtItems[_selectedItemIndex];
                GlobalData.EvidenceForHotThoughtItems.Remove(item);
                UpdateAdapter();
                _selectedItemIndex = -1;
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ThoughtRecordWizardEvidenceForHotThoughtStep.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ThoughtRecordWizardEvidenceForHotThoughtStep.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                _speakEvidenceFor.SetImageResource(Resource.Drawable.micgreyscale);

                _speakEvidenceFor.Enabled = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ThoughtRecordWizardEvidenceForHotThoughtStep.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "ThoughtRecordWizardEvidenceForHotThoughtStep.AttemptPermissionRequest");
            }
        }
    }
}
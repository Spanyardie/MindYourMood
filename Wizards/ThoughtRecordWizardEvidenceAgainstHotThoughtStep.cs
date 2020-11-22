using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using V7Sup = Android.Support.V7.App;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Speech;
using Toolbar = Android.Support.V7.Widget.Toolbar;
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
    public class ThoughtRecordWizardEvidenceAgainstHotThoughtStep : V7Sup.AppCompatActivity, IAlertCallback
    {
        public static string TAG = "M:ThoughtRecordWizardEvidenceAgainstHotThoughtStep";

        private Toolbar _toolbar;
        private ListView _evidenceAgainstHotThoughtList;
        private EditText _evidenceText;
        private TextView _hotThoughtText;
        private LinearLayout _linEvidenceAgainstMain;
        private bool _validated;

        private bool _setupCallbacksComplete;
        private int _selectedItemIndex = -1;
        private int _hotThoughtId = -1;

        private ImageButton _speakEvidenceAgainst;
        private bool _spokenEvidenceAgainst = false;
        private string _spokenText = "";

        private Button _continue;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("selectedItemIndex", +_selectedItemIndex);
                outState.PutInt("hotThoughtID", _hotThoughtId);
            }
            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.EvidenceAgainstHotThoughtMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if(savedInstanceState != null)
                {
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");
                    _hotThoughtId = savedInstanceState.GetInt("hotThoughtID");
                }

                SetContentView(Resource.Layout.EvidenceAgainstHotThought);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.evidenceAgainstToolbar, Resource.String.evidenceAgainstHeading, Color.White);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateAgainstHotThought), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linEvidenceAgainstMain != null)
                _linEvidenceAgainstMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            try
            {
                if (e.KeyCode == Keycode.Back)
                {
                    GlobalData.RemoveEvidenceAgainstHotThought();
                    GlobalData.EvidenceAgainstHotThoughtItems.Clear();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "DispatchKeyEvent: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAgainstHotThoughtDispatchkeyEvent), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.DispatchKeyEvent");
            }
            return base.DispatchKeyEvent(e);
        }

        private void GetHotThoughtText()
        {
            try
            {
                string thoughtText = GetString(Resource.String.evidenceAgainstHotThoughtNoHotThoughtDefined);
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorGettingHotThoughtText), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.GetHotThoughtText");
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                GetFieldComponents();
                SetupCallbacks();

                if (_spokenEvidenceAgainst)
                {
                    if (_evidenceText != null)
                        _evidenceText.Text = _spokenText;
                    _spokenEvidenceAgainst = false;
                }

                UpdateAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnResume: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorResumeAgainstHotThought), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.OnResume");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _evidenceAgainstHotThoughtList = FindViewById<ListView>(Resource.Id.lstEvidenceAgainstHotThought);
                _evidenceText = FindViewById<EditText>(Resource.Id.edtEvidenceAgainstThought);
                _hotThoughtText = FindViewById<TextView>(Resource.Id.txtHotThoughtText);
                _speakEvidenceAgainst = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakEvidenceAgainst);
                _continue = FindViewById<Button>(Resource.Id.btnContinue);
                _linEvidenceAgainstMain = FindViewById<LinearLayout>(Resource.Id.linEvidenceAgainstHotThoughtListMain);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAgainstHotThoughtGetComponents), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (!_setupCallbacksComplete)
                {
                    if (_evidenceAgainstHotThoughtList != null)
                        _evidenceAgainstHotThoughtList.ItemClick += EvidenceAgainstHotThoughtList_ItemClick;
                    if(_speakEvidenceAgainst != null)
                        _speakEvidenceAgainst.Click += SpeakEvidenceAgainst_Click;
                    if(_continue != null)
                        _continue.Click += Continue_Click;
                    _setupCallbacksComplete = true;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                _setupCallbacksComplete = false;
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAgainstHotThoughtSetupCallbacks), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.SetupCallbacks");
            }
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void SpeakEvidenceAgainst_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.EvidenceAgainstSpeakWhatPrompt));

            Log.Info(TAG, "SpeakEvidenceAgainst_Click: Created intent, sending request...");
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
                        _spokenEvidenceAgainst = true;
                        _spokenText = matches[0];
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Checking Voice Recognition result", "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.OnActivityResult");
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
                    case Resource.Id.evidenceagainsthotthoughtActionAdd:
                        AddEvidenceAgainstThought();
                        return true;

                    case Resource.Id.evidenceagainsthotthoughtActionRemove:
                        RemoveEvidenceAgainstThought();
                        return true;

                    case Resource.Id.evidenceagainsthotthoughtActionCancel:
                        Cancel();
                        return true;

                    case Resource.Id.evidenceAgainstHotThoughtActionHelp:
                        Intent intent = new Intent(this, typeof(EvidenceAgainstHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.evidenceagainsthotthoughtActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.evidenceagainsthotthoughtActionRemove);
                var itemCancel = menu.FindItem(Resource.Id.evidenceagainsthotthoughtActionCancel);
                var itemHelp = menu.FindItem(Resource.Id.evidenceAgainstHotThoughtActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.SetActionIcons");
            }
        }

        private void Previous()
        {
            try
            {
                GlobalData.RemoveEvidenceAgainstHotThought();
                GlobalData.EvidenceAgainstHotThoughtItems.Clear();
                Intent intent = new Intent(this, typeof(ThoughtRecordWizardEvidenceForHotThoughtStep));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                Finish();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "PreviousButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAgainstHotThoughtPreviousButton), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.PreviousButton_Click");
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
                alertHelper.InstanceId = "evidenceAgainstCancel";

                alertHelper.ShowAlert();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "CancelButton_Click: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCancelAgainstHotThoughtAddition), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.CancelButton_Click");
            }
        }

        private void Next()
        {
            try
            {
                Validate();
                if (!_validated)
                    return;

                StoreEvidenceAgainstHotThought();
                Intent intent = new Intent(this, typeof(ThoughtRecordWizardAlternativeThoughtStep));
                StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "NextButton_Click: exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorNextButtonAgainstHot), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.NextButton_Click");
            }
        }

        private void StoreEvidenceAgainstHotThought()
        {
            Globals dbHelp = new Globals();
            try
            {
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var thought in GlobalData.EvidenceAgainstHotThoughtItems)
                    {
                        if (thought.EvidenceAgainstHotThoughtId == 0)
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
                Log.Error(TAG, "");
                if (dbHelp != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStoringEvidenceAgainst), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.StoreEvidenceAgainstHotThought");
            }
        }

        private void AddEvidenceAgainstThought()
        {
            try
            {
                if (_evidenceText == null)
                    return;

                if (string.IsNullOrWhiteSpace(_evidenceText.Text.Trim()))
                {
                    var resourceString = GetString(Resource.String.evidenceAgainstHotThoughtNoEvidence);
                    _evidenceText.Error = resourceString;
                    return;
                }

                EvidenceAgainstHotThought thought = new EvidenceAgainstHotThought();
                thought.ThoughtRecordId = GlobalData.ThoughtRecordId;
                thought.Evidence = _evidenceText.Text.Trim();
                if (GlobalData.EvidenceAgainstHotThoughtItems == null)
                {
                    GlobalData.EvidenceAgainstHotThoughtItems = new List<EvidenceAgainstHotThought>();
                }
                GlobalData.EvidenceAgainstHotThoughtItems.Add(thought);

                UpdateAdapter();

                _evidenceText.Text = "";
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AddEvidenceAgainstThought_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAddingAgainst), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.AddEvidenceAgainstThought_Click");
            }
        }

        private void UpdateAdapter()
        {
            try
            {
                EvidenceAgainstHotThoughtItemsAdapter thoughtAdapter = new EvidenceAgainstHotThoughtItemsAdapter(this);
                if (_evidenceAgainstHotThoughtList != null)
                    _evidenceAgainstHotThoughtList.Adapter = thoughtAdapter;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAgainstUpdateAdapter), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.UpdateAdapter");
            }
        }

        private void RemoveEvidenceAgainstThought()
        {
            try
            {
                if (_selectedItemIndex > -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertMessage = GetString(Resource.String.ThoughtRecordWizardEvidenceAgainstDeleteConfirm);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonCancelCaption);
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonOKCaption);
                    alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardEvidenceAgainstDeleteTitle);
                    alertHelper.InstanceId = "evidenceAgainstRemove";

                    alertHelper.ShowAlert();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RemoveEvidenceAgainstThought: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRemovingAgainst), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.RemoveEvidenceAgainstThought");
            }
        }

        private void EvidenceAgainstHotThoughtList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;
                UpdateAdapter();
                _evidenceAgainstHotThoughtList.SetSelection(_selectedItemIndex);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "EvidenceAgainstHotThoughtList_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSelectingAgainst), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.EvidenceAgainstHotThoughtList_ItemClick");
            }
        }

        private void Validate()
        {
            try
            {
                _validated = true;

                if (_evidenceAgainstHotThoughtList != null)
                {
                    if (_evidenceAgainstHotThoughtList.Adapter != null)
                    {
                        if (_evidenceAgainstHotThoughtList.Adapter.Count == 0)
                        {
                            var resourceString = GetString(Resource.String.evidenceAgainstHotThoughtNoHotThoughtDefinedOnNext);
                            Toast.MakeText(this, resourceString, ToastLength.Short).Show();
                            _validated = false;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Validate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorValidatingAgainst), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.Validate");
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
                GlobalData.RemoveEvidenceAgainstHotThought();
                GlobalData.MoodItems.Clear();
                GlobalData.AutomaticThoughtsItems.Clear();
                GlobalData.EvidenceForHotThoughtItems.Clear();
                GlobalData.EvidenceAgainstHotThoughtItems.Clear();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Cleanup: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAgainstCleanup), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.Cleanup");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "evidenceAgainstCancel")
            {
                Cleanup();
                Intent intent = new Intent(this, typeof(ThoughtRecordsActivity));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
            }
            if(instanceId == "evidenceAgainstRemove")
            {
                var item = GlobalData.EvidenceAgainstHotThoughtItems[_selectedItemIndex];
                GlobalData.EvidenceAgainstHotThoughtItems.Remove(item);
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                _speakEvidenceAgainst.SetImageResource(Resource.Drawable.micgreyscale);

                _speakEvidenceAgainst.Enabled = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "ThoughtRecordWizardEvidenceAgainstHotThoughtStep.AttemptPermissionRequest");
            }
        }
    }
}
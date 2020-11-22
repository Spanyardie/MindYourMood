using System.Collections.Generic;
using Android.OS;
using Android.Widget;

using V7Sup = Android.Support.V7.App;
using Android.Content;
using Android.App;
using com.spanyardie.MindYourMood.Model;
using Android.Runtime;
using Android.Views;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Speech;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.Model.Interfaces;
using System;
using com.spanyardie.MindYourMood.SubActivities.Help.ThoughtsHelp;
using Android.Content.PM;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.Wizards
{
    [Activity]
    public class ThoughtRecordWizardAutomaticThoughtsStep : V7Sup.AppCompatActivity, IAlertCallback
    {
        public static string TAG = "M:ThoughtRecordWizardAutomaticThoughtsStep";

        private ListView _automaticThoughtsList;
        private EditText _automaticThought;
        private int _selectedItemIndex = -1;

        private bool _validated;
        private bool _setupCallbacksComplete;

        private ImageButton _speakWhatThought;
        private bool _spokenThought = false;
        private string _spokenText = "";
        private Toolbar _toolbar;

        private Button _continue;

        private LinearLayout _automaticThoughtsRoot;
        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemIndex", _selectedItemIndex);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.AutomaticThoughtsMenu, menu);

            SetActionIcons(menu);

            return true;
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
                    case Resource.Id.automaticthoughtsActionAdd:
                        AddThoughtButton();
                        return true;

                    case Resource.Id.automaticthoughtsActionRemove:
                        RemoveThoughtButton();
                        return true;

                    case Resource.Id.automaticthoughtsActionHotThought:
                        HotThoughtSelect();
                        return true;

                    case Resource.Id.automaticthoughtsActionCancel:
                        Cancel();
                        return true;

                    case Resource.Id.automaticThoughtsActionHelp:
                        Intent intent = new Intent(this, typeof(AutomaticThoughtsHelpActivity));
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
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");

                SetContentView(Resource.Layout.AutomaticThoughts);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.automaticThoughtsToolbar, Resource.String.automaticHeading, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.thoughtbubbleswoman,
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
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingAutoThoughtActivity), "ThoughtRecordWizardAutomaticThoughtsStep.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_automaticThoughtsRoot != null)
                _automaticThoughtsRoot.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                GetFieldComponents();
                SetupCallbacks();

                if(_spokenThought)
                {
                    if (_automaticThought != null)
                        _automaticThought.Text = _spokenText;
                    _spokenThought = false;
                }

                UpdateAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnResume: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorResumingAutoThought), "ThoughtRecordWizardAutomaticThoughtsStep.OnResume");
            }
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            try
            {
                if (e.KeyCode == Keycode.Back)
                {
                    GlobalData.RemoveAutomaticThoughts();
                    GlobalData.AutomaticThoughtsItems.Clear();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "DispatchKeyEvent: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorDispatchKeyEventAutoThought), "ThoughtRecordWizardAutomaticThoughtsStep.DispatchKeyEvent");
            }
            return base.DispatchKeyEvent(e);
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.automaticthoughtsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.automaticthoughtsActionRemove);
                var itemHot = menu.FindItem(Resource.Id.automaticthoughtsActionHotThought);
                var itemCancel = menu.FindItem(Resource.Id.automaticthoughtsActionCancel);
                var itemHelp = menu.FindItem(Resource.Id.automaticThoughtsActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if(itemHot != null)
                            itemHot.SetIcon(Resource.Drawable.ic_flash_on_white_24dp);
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
                        if (itemHot != null)
                            itemHot.SetIcon(Resource.Drawable.ic_flash_on_white_36dp);
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
                        if (itemHot != null)
                            itemHot.SetIcon(Resource.Drawable.ic_flash_on_white_48dp);
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ThoughtRecordWizardAutomaticThoughtsStep.SetActionIcons");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _automaticThoughtsList = FindViewById<ListView>(Resource.Id.lstAutomaticThoughts);
                _automaticThought = FindViewById<EditText>(Resource.Id.edtWhatThought);
                _speakWhatThought = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWhatThought);
                _continue = FindViewById<Button>(Resource.Id.btnContinue);
                _automaticThoughtsRoot = FindViewById<LinearLayout>(Resource.Id.linAutomaticThoughtsMainRoot);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAutoThoughtGetComponents), "ThoughtRecordWizardAutomaticThoughtsStep.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (!_setupCallbacksComplete)
                {
                    if (_automaticThoughtsList != null)
                        _automaticThoughtsList.ItemClick += AutomaticThoughtsList_ItemClick;
                    if(_speakWhatThought != null)
                        _speakWhatThought.Click += SpeakWhatThought_Click;
                    if(_continue != null)
                        _continue.Click += Continue_Click;
                    _setupCallbacksComplete = true;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                _setupCallbacksComplete = false;
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAutoThoughtSetupCallbacks), "ThoughtRecordWizardAutomaticThoughtsStep.SetupCallbacks");
            }
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void SpeakWhatThought_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.AutomaticThoughtSpeakWhatPrompt));

            Log.Info(TAG, "SpeakWhatThought_Click: Created intent, sending request...");
            StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if(requestCode == ConstantsAndTypes.VOICE_RECOGNITION_REQUEST && resultCode == Result.Ok)
                {
                    IList<string> matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if(matches != null)
                    {
                        _spokenThought = true;
                        _spokenText = matches[0];
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Checking Voice Recognition result", "ThoughtRecordWizardAutomaticThoughtsStep.OnActivityResult");
            }
        }
        private void RemoveThoughtButton()
        {
            try
            {
                if (_selectedItemIndex > -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertMessage = GetString(Resource.String.ThoughtRecordWizardAutomaticDeleteConfirm);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonCancelCaption);
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonOKCaption);
                    alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardAutomaticDeleteTitle);
                    alertHelper.InstanceId = "automaticRemove";

                    alertHelper.ShowAlert();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RemoveThoughtButton_Click: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRemovingAutoThought), "ThoughtRecordWizardAutomaticThoughtsStep.RemoveThoughtButton_Click");
            }
        }

        private void HotThoughtSelect()
        {
            try
            {
                ClearHotThoughts();
                if (_automaticThoughtsList != null)
                {
                    if (_selectedItemIndex > -1)
                    {
                        GlobalData.AutomaticThoughtsItems[_selectedItemIndex].IsHotThought = true;
                        UpdateAdapter();
                        _automaticThoughtsList.SetSelection(_selectedItemIndex);
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "HotThoughtSelect_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSettingHotThought), "ThoughtRecordWizardAutomaticThoughtsStep.HotThoughtSelect_Click");
            }
        }

        private void Previous()
        {
            try
            {
                //if we are heading back to Moods then clear the Automatic Thoughts
                GlobalData.RemoveAutomaticThoughts();
                GlobalData.AutomaticThoughtsItems.Clear();

                Intent intent = new Intent(this, typeof(ThoughtRecordWizardMoodStep));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                Finish();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "PreviousButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorGoBackMoodsAutoThought), "ThoughtRecordWizardAutomaticThoughtsStep.PreviousButton_Click");
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
                alertHelper.InstanceId = "automaticCancel";

                alertHelper.ShowAlert();

            }
            catch(Exception e)
            {
                Log.Error(TAG, "CancelButton_Click: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAutoThoughtRemoveDialog), "ThoughtRecordWizardAutomaticThoughtsStep.CancelButton_Click");
            }
        }

        private void Next()
        {
            try
            {
                Validate();
                if (!_validated)
                    return;

                StoreAutomaticThoughts();

                Intent intent = new Intent(this, typeof(ThoughtRecordWizardEvidenceForHotThoughtStep));
                StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "NextButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAutoThoughtNextClick), "ThoughtRecordWizardAutomaticThoughtsStep.CancelButton_Click");
            }
        }

        public void StoreAutomaticThoughts()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var thought in GlobalData.AutomaticThoughtsItems)
                    {
                        if (thought.AutomaticThoughtsId == 0)
                            thought.Save(sqlDatabase);
                    }
                    dbHelp.CloseDatabase();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "StoreAutomaticThoughts: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStoringAutoThoughts), "ThoughtRecordWizardAutomaticThoughtsStep.StoreAutomaticThoughts");
            }
        }

        private void AddThoughtButton()
        {
            try
            {
                if (_automaticThought == null)
                    return;

                if (string.IsNullOrWhiteSpace(_automaticThought.Text.Trim()))
                {
                    var resourceString = GetString(Resource.String.automaticThoughtNoEvidence);
                    _automaticThought.Error = resourceString;
                    return;
                }

                AutomaticThoughts thought = new AutomaticThoughts();
                thought.ThoughtRecordId = GlobalData.ThoughtRecordId;
                thought.Thought = _automaticThought.Text.Trim();
                if (GlobalData.AutomaticThoughtsItems == null)
                {
                    GlobalData.AutomaticThoughtsItems = new List<AutomaticThoughts>();
                }
                GlobalData.AutomaticThoughtsItems.Add(thought);

                UpdateAdapter();

                _automaticThought.Text = "";
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AddThoughtButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAddingAutomaticThought), "ThoughtRecordWizardAutomaticThoughtsStep.AddThoughtButton_Click");
            }
        }

        private void UpdateAdapter()
        {
            AutomaticThoughtItemsAdapter thoughtAdapter = new AutomaticThoughtItemsAdapter(this);
            if(_automaticThoughtsList != null)
                _automaticThoughtsList.Adapter = thoughtAdapter;
        }

        private void AutomaticThoughtsList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateAdapter();
            _automaticThoughtsList.SetSelection(_selectedItemIndex);
        }

        public int GetSelectedItem()
        {
            return _selectedItemIndex;
        }

        private void ClearHotThoughts()
        {
            foreach(var thought in GlobalData.AutomaticThoughtsItems)
            {
                thought.IsHotThought = false;
            }
        }

        private void Validate()
        {
            _validated = true;

            if (_automaticThoughtsList.Adapter != null)
            {
                if (_automaticThoughtsList.Adapter.Count == 0)
                {
                    var resourceString = GetString(Resource.String.automaticThoughtNotDefined);
                    Toast.MakeText(this, resourceString, ToastLength.Short).Show();
                    _validated = false;
                }
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "automaticCancel")
            {
                GlobalData.RemoveThoughtRecord();
                //cancelling so clear up globals
                GlobalData.SituationItem.What = "";
                GlobalData.SituationItem.When = "";
                GlobalData.SituationItem.Where = "";
                GlobalData.SituationItem.Who = "";
                GlobalData.RemoveSituation();
                GlobalData.RemoveMoods();
                GlobalData.MoodItems.Clear();
                GlobalData.RemoveAutomaticThoughts();
                GlobalData.AutomaticThoughtsItems.Clear();
                Intent intent = new Intent(this, typeof(ThoughtRecordsActivity));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
            }
            if(instanceId == "automaticRemove")
            {
                var item = GlobalData.AutomaticThoughtsItems[_selectedItemIndex];
                GlobalData.AutomaticThoughtsItems.Remove(item);
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ThoughtRecordWizardAutomaticThoughtsStep.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ThoughtRecordWizardAutomaticThoughtsStep.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                _speakWhatThought.SetImageResource(Resource.Drawable.micgreyscale);

                _speakWhatThought.Enabled = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ThoughtRecordWizardAutomaticThoughtsStep.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "ThoughtRecordWizardAlternativeThoughtStep.AttemptPermissionRequest");
            }
        }
    }
}
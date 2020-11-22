using System.Collections.Generic;
using Android.OS;
using Android.Widget;
using V7Sup = Android.Support.V7.App;
using Android.Support.V7.App;
using Android.Content;
using Android.App;
using com.spanyardie.MindYourMood.Model;
using System;
using Android.Runtime;
using Android.Views;
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
    public class ThoughtRecordWizardAlternativeThoughtStep : AppCompatActivity, IAlertCallback
    {
        public static string TAG = "M:ThoughtRecordWizardAlternativeThoughtStep";

        private ListView _alternativeThoughtsList;
        private EditText _alternativeThought;
        private TextView _percentageLabel;
        private SeekBar _rateAlternativeMood;
        private LinearLayout _alternativeThoughtsMain;
        private int _selectedItemIndex = -1;

        private bool _validated;
        private bool _setupCallbacksComplete;

        private ImageButton _speakAlternative;
        private bool _spokenAlternative = false;
        private string _spokenText = "";
        private Toolbar _toolbar;

        private Button _continue;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemIndex", _selectedItemIndex);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.AlternativeThoughtsMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");

                SetContentView(Resource.Layout.AlternativeThoughts);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.alternativeThoughtsToolbar, Resource.String.alternativeHeading, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.alternative,
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingAltThoughtStepActivity), "ThoughtRecordWizardAlternativeThoughtStep.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_alternativeThoughtsMain != null)
                _alternativeThoughtsMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                GetFieldComponents();

                if (_spokenAlternative)
                {
                    if (_alternativeThought != null)
                        _alternativeThought.Text = _spokenText;
                    _spokenAlternative = false;
                }

                SetupCallbacks();
                UpdateAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnResume: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorResumingAltThoughtStep), "ThoughtRecordWizardAlternativeThoughtStep.OnResume");
            }
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            try
            {
                if (e.KeyCode == Keycode.Back)
                {
                    GlobalData.RemoveAlternativeThoughts();
                    GlobalData.AlternativeThoughtsItems.Clear();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "DispathKeyEvent: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorDispatchAltThoughtStepKeyEvent), "ThoughtRecordWizardAlternativeThoughtStep.DispatchKeyEvent");
            }
            return base.DispatchKeyEvent(e);
        }

        private void GetFieldComponents()
        {
            _alternativeThoughtsList = FindViewById<ListView>(Resource.Id.lstAlternativeThoughts);
            _alternativeThought = FindViewById<EditText>(Resource.Id.edtAlternativeThought);
            _rateAlternativeMood = FindViewById<SeekBar>(Resource.Id.skbRateAlternativeThoughts);
            _percentageLabel = FindViewById<TextView>(Resource.Id.txtPercentageLabel);
            _speakAlternative = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakAlternativeThought);
            _continue = FindViewById<Button>(Resource.Id.btnContinue);
            _alternativeThoughtsMain = FindViewById<LinearLayout>(Resource.Id.linAlternativeThoughtsMain);
        }

        private void SetupCallbacks()
        {
            if (!_setupCallbacksComplete)
            {
                if (_alternativeThoughtsList != null)
                    _alternativeThoughtsList.ItemClick += AlternativeThoughtsList_ItemClick;
                if(_rateAlternativeMood != null)
                    _rateAlternativeMood.ProgressChanged += RateAlternativeMood_ProgressChanged;
                if(_speakAlternative != null)
                    _speakAlternative.Click += SpeakAlternative_Click;
                if(_continue != null)
                    _continue.Click += Continue_Click;
                _setupCallbacksComplete = true;
            }
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void SpeakAlternative_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.AlternativeThoughtSpeakWhatPrompt));

            Log.Info(TAG, "SpeakAlternative_Click: Created intent, sending request...");
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
                        _spokenAlternative = true;
                        _spokenText = matches[0];
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Checking Voice Recognition result", "ThoughtRecordWizardAlternativeThoughtStep.OnActivityResult");
            }
        }

        private void RateAlternativeMood_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            var val = e.Progress;
            if (_percentageLabel != null)
                _percentageLabel.Text = val.ToString() + "%";
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
                    case Resource.Id.AlternativeThoughtActionAdd:
                        AddThoughtButton();
                        return true;
                    case Resource.Id.AlternativeThoughtActionRemove:
                        RemoveThoughtButton();
                        return true;
                    case Resource.Id.AlternativeThoughtActionCancel:
                        Cancel();
                        return true;
                    case Resource.Id.AlternativeThoughtActionNext:
                        Next();
                        return true;

                    case Resource.Id.AlternativeThoughtsActionHelp:
                        Intent intent = new Intent(this, typeof(AlternativeThoughtsHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void RemoveThoughtButton()
        {
            try
            {
                if (_selectedItemIndex > -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardAlternativeDeleteTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.ThoughtRecordWizardAlternativeThoughtDeleteConfirm);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "RemoveThoughtButton: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRemovingAltThoughtItem), "ThoughtRecordWizardAlternativeThoughtStep.RemoveThoughtButton");
            }
        }

        private void Previous()
        {
            try
            {
                //if we are heading back to Evidence Against then clear the Alternative Thoughts
                GlobalData.RemoveAlternativeThoughts();
                GlobalData.AlternativeThoughtsItems.Clear();
                Intent intent = new Intent(this, typeof(ThoughtRecordWizardEvidenceAgainstHotThoughtStep));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                Finish();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "PreviousButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSteppingBackAltThoughtActivity), "ThoughtRecordWizardAlternativeThoughtStep.PreviousButton_Click");
            }
        }

        private void Cancel()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertMessage = GetString(Resource.String.ThoughtRecordWizardAlternativeThoughtConfirm);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonCancelCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonOKCaption);
                alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardAlternativeThoughtCancel);
                alertHelper.InstanceId = "alternativeCancel";

                alertHelper.ShowAlert();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Cancel: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorCancellingAltThoughtAddition), "ThoughtRecordWizardAlternativeThoughtStep.Cancel");
            }
        }

        private void Next()
        {
            try
            {
                Validate();
                if (!_validated)
                    return;

                StoreAlternativeThoughts();

                Intent intent = new Intent(this, typeof(ThoughtRecordWizardRerateMoodStep));
                StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "NextButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMoveToRerateStep), "ThoughtRecordWizardAlternativeThoughtStep.NextButton_Click");
            }
        }

        private void StoreAlternativeThoughts()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var thought in GlobalData.AlternativeThoughtsItems)
                    {
                        if (thought.AlternativeThoughtsId == 0)
                            thought.Save(sqlDatabase);
                    }
                    dbHelp.CloseDatabase();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "StoreAlternativeThoughts: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStoreAltThoughtItems), "ThoughtRecordWizardAlternativeThoughtStep.StoreAlternativeThoughts");
            }
        }

        private void AddThoughtButton()
        {
            if (_alternativeThought == null)
                return;

            try
            {
                if (string.IsNullOrWhiteSpace(_alternativeThought.Text.Trim()))
                {
                    var resourceString = GetString(Resource.String.alternativeThoughtNoEvidence);
                    _alternativeThought.Error = resourceString;
                    return;
                }

                AlternativeThoughts thought = new AlternativeThoughts();
                thought.ThoughtRecordId = GlobalData.ThoughtRecordId;
                thought.Alternative = _alternativeThought.Text.Trim();
                thought.BeliefRating = Convert.ToInt32(_percentageLabel.Text.Trim().Replace("%", ""));
                if (GlobalData.AlternativeThoughtsItems == null)
                {
                    GlobalData.AlternativeThoughtsItems = new List<AlternativeThoughts>();
                }
                GlobalData.AlternativeThoughtsItems.Add(thought);

                UpdateAdapter();

                _alternativeThought.Text = "";
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AddThoughtButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAddingAltThought), "ThoughtRecordWizardAlternativeThoughtStep.AddThoughtButton_Click");
            }
        }

        private void UpdateAdapter()
        {
            try
            {
                AlternativeThoughtItemsAdapter thoughtAdapter = new AlternativeThoughtItemsAdapter(this);
                if (_alternativeThoughtsList != null)
                    _alternativeThoughtsList.Adapter = thoughtAdapter;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorUpdatingAltThoughtDisplay), "ThoughtRecordWizardAlternativeThoughtStep.UpdateAdapter");
            }
        }

        private void AlternativeThoughtsList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;
                UpdateAdapter();
                _alternativeThoughtsList.SetSelection(_selectedItemIndex);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AlternativeThoughtsList_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSelectingAltThought), "ThoughtRecordWizardAlternativeThoughtStep.AlternativeThoughtList_Click");
            }
        }

        public int GetSelectedItem()
        {
            return _selectedItemIndex;
        }

        private void Validate()
        {
            try
            {
                _validated = true;

                if (_alternativeThoughtsList.Adapter != null)
                {
                    if (_alternativeThoughtsList.Adapter.Count == 0)
                    {
                        var resourceString = GetString(Resource.String.alternativeThoughtNotDefined);
                        Toast.MakeText(this, resourceString, ToastLength.Short).Show();
                        _validated = false;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Validate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorValidatingAltThought), "ThoughtRecordWizardAlternativeThoughtStep.Validate");
                _validated = false;
            }
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
                GlobalData.RemoveAlternativeThoughts();
                GlobalData.MoodItems.Clear();
                GlobalData.AutomaticThoughtsItems.Clear();
                GlobalData.EvidenceForHotThoughtItems.Clear();
                GlobalData.EvidenceAgainstHotThoughtItems.Clear();
                GlobalData.AlternativeThoughtsItems.Clear();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Cleanup: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCleanupAltThought), "ThoughtRecordWizardAlternativeThoughtStep.Cleanup");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.AlternativeThoughtActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.AlternativeThoughtActionRemove);
                var itemCancel = menu.FindItem(Resource.Id.AlternativeThoughtActionCancel);
                var itemHelp = menu.FindItem(Resource.Id.AlternativeThoughtsActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if (itemCancel != null)
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ThoughtRecordWizardAlternativeThoughtStep.SetActionIcons");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            try
            {
                if (instanceId == "useMic")
                {
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                    return;
                }

                if (instanceId != null && instanceId == "remove")
                {
                    var item = GlobalData.AlternativeThoughtsItems[_selectedItemIndex];
                    GlobalData.AlternativeThoughtsItems.Remove(item);
                    UpdateAdapter();
                    _selectedItemIndex = -1;
                }
                else
                {
                    GlobalData.RemoveThoughtRecord();
                    //cancelling so clear up globals
                    GlobalData.SituationItem.What = "";
                    GlobalData.SituationItem.When = "";
                    GlobalData.SituationItem.Where = "";
                    GlobalData.SituationItem.Who = "";
                    GlobalData.RemoveSituation();
                    GlobalData.RemoveMoods();
                    GlobalData.RemoveAutomaticThoughts();
                    GlobalData.RemoveEvidenceForHotThought();
                    GlobalData.RemoveEvidenceAgainstHotThought();
                    GlobalData.RemoveAlternativeThoughts();
                    GlobalData.MoodItems.Clear();
                    GlobalData.AutomaticThoughtsItems.Clear();
                    GlobalData.EvidenceForHotThoughtItems.Clear();
                    GlobalData.EvidenceAgainstHotThoughtItems.Clear();
                    GlobalData.AlternativeThoughtsItems.Clear();
                    Intent intent = new Intent(this, typeof(ThoughtRecordsActivity));
                    Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Alert positive selection", "ThoughtRecordWizardAlternativeThoughtStep.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ThoughtRecordWizardAlternativeThoughtStep.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ThoughtRecordWizardAlternativeThoughtStep.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                _speakAlternative.SetImageResource(Resource.Drawable.micgreyscale);

                _speakAlternative.Enabled = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ThoughtRecordWizardAlternativeThoughtStep.ShowPermissionRationale");
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
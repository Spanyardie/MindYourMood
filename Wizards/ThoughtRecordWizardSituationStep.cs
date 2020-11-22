using Android.OS;
using Android.Widget;
using Android.App;
using V7Sup = Android.Support.V7.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Speech;
using System.Collections.Generic;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.SubActivities.Help.ThoughtsHelp;
using Android.Content.PM;
using System;

namespace com.spanyardie.MindYourMood.Wizards
{
    [Activity]
    public class ThoughtRecordWizardSituationStep : V7Sup.AppCompatActivity, IAlertCallback
    {
        public static string TAG = "M:ThoughtRecordWizardSituationStep";

        private EditText _situationWhat;
        private EditText _situationWho;
        private EditText _situationWhere;
        private EditText _situationWhen;

        private bool _validated = false;
        private bool _setupCallbacksComplete = false;

        //speech recognition elements
        private ImageButton _speakWhat;
        private ImageButton _speakWho;
        private ImageButton _speakWhere;
        private ImageButton _speakWhen;
        private ConstantsAndTypes.SituationCategories _speakCategory;

        private Toolbar _toolbar;

        private Button _continue;

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SituationMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.situation);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.situationToolbar, Resource.String.situationHeading, Color.White);

                SetupCallbacks();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateSituationActivity), "ThoughtRecordWizardSituationStep.OnCreate");
            }
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            try
            {
                if (e.KeyCode == Keycode.Back)
                {
                    Log.Info(TAG, "DispatchKeyEvent removing Situation and ThoughtRecord (ID - " + GlobalData.ThoughtRecordId.ToString() + ")");
                    GlobalData.RemoveSituation();
                    GlobalData.RemoveThoughtRecord();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "DispatchKeyEvent: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSituationDispatchkeyEvent), "ThoughtRecordWizardSituationStep.DispatchKeyEvent");
            }
            return base.DispatchKeyEvent(e);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    Cancel();
                    return true;
                }

                switch(item.ItemId)
                {
                    case Resource.Id.situationActionCancel:
                        Cancel();
                        return true;

                    case Resource.Id.situationActionHelp:
                        Intent intent = new Intent(this, typeof(SituationHelpActivity));
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
                var itemCancel = menu.FindItem(Resource.Id.situationActionCancel);
                var itemHelp = menu.FindItem(Resource.Id.situationActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ThoughtRecordWizardSituationStep.SetActionIcons");
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                if (GlobalData.SituationItem == null)
                    return;

                GetFieldComponents();

                if (_situationWhat != null)
                    _situationWhat.Text = string.IsNullOrEmpty(GlobalData.SituationItem.What.Trim()) ? GetString(Resource.String.ThoughtRecordDefaultRatingText) : GlobalData.SituationItem.What.Trim();
                if (_situationWhen != null)
                    _situationWhen.Text = string.IsNullOrEmpty(GlobalData.SituationItem.When.Trim()) ? GetString(Resource.String.ThoughtRecordDefaultRatingText) : GlobalData.SituationItem.When.Trim();
                if (_situationWhere != null)
                    _situationWhere.Text = string.IsNullOrEmpty(GlobalData.SituationItem.Where.Trim()) ? GetString(Resource.String.ThoughtRecordDefaultRatingText) : GlobalData.SituationItem.Where.Trim();
                if (_situationWho != null)
                    _situationWho.Text = string.IsNullOrEmpty(GlobalData.SituationItem.Who.Trim()) ? GetString(Resource.String.ThoughtRecordDefaultRatingText) : GlobalData.SituationItem.Who.Trim();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnResume: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorResumeSituationActivity), "ThoughtRecordWizardSituationStep.OnResume");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _situationWhat = FindViewById<EditText>(Resource.Id.edtSituationWhat);
                _speakWhat = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWhat);

                _situationWhen = FindViewById<EditText>(Resource.Id.edtSituationWhen);
                _speakWhen = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWhen);

                _situationWhere = FindViewById<EditText>(Resource.Id.edtSituationWhere);
                _speakWhere = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWhere);

                _situationWho = FindViewById<EditText>(Resource.Id.edtSituationWho);
                _speakWho = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWho);

                _continue = FindViewById<Button>(Resource.Id.btnContinue);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSituationGetComponents), "ThoughtRecordWizardSituationStep.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (!_setupCallbacksComplete)
                {
                    if(_speakWhat != null)
                        _speakWhat.Click += SpeakWhat_Click;
                    if(_speakWhen != null)
                        _speakWhen.Click += SpeakWhen_Click;
                    if(_speakWhere != null)
                        _speakWhere.Click += SpeakWhere_Click;
                    if(_speakWho != null)
                        _speakWho.Click += SpeakWho_Click;

                    if(_continue != null)
                        _continue.Click += Continue_Click;

                    _setupCallbacksComplete = true;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSituationSetupCallbacks), "ThoughtRecordWizardSituationStep.SetupCallbacks");
            }
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void SpeakWho_Click(object sender, System.EventArgs e)
        {
            Log.Info(TAG, "SpeakWho_Click: Attempting SpeakTo with message - " + GetString(Resource.String.SituationSpeakWhoPrompt));
            SpeakToMYM(ConstantsAndTypes.SituationCategories.Who, GetString(Resource.String.SituationSpeakWhoPrompt));
        }

        private void SpeakWhere_Click(object sender, System.EventArgs e)
        {
            Log.Info(TAG, "SpeakWhere_Click: Attempting SpeakTo with message - " + GetString(Resource.String.SituationSpeakWherePrompt));
            SpeakToMYM(ConstantsAndTypes.SituationCategories.Where, GetString(Resource.String.SituationSpeakWherePrompt));
        }

        private void SpeakWhen_Click(object sender, System.EventArgs e)
        {
            Log.Info(TAG, "SpeakWhen_Click: Attempting SpeakTo with message - " + GetString(Resource.String.SituationSpeakWhenPrompt));
            SpeakToMYM(ConstantsAndTypes.SituationCategories.When, GetString(Resource.String.SituationSpeakWhenPrompt));
        }

        private void SpeakWhat_Click(object sender, System.EventArgs e)
        {
            Log.Info(TAG, "SpeakWhat_Click: Attempting SpeakTo with message - " + GetString(Resource.String.SituationSpeakWhatPrompt));
            SpeakToMYM(ConstantsAndTypes.SituationCategories.What, GetString(Resource.String.SituationSpeakWhatPrompt));
        }

        private void SpeakToMYM(ConstantsAndTypes.SituationCategories category, string message)
        {
            try
            {
                _speakCategory = category;
                Log.Info(TAG, "SpeakToMYM: _speakCategory - " + category.ToString());

                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, message);

                Log.Info(TAG, "SpeakToMYM: Created intent, sending request...");
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SpeakToMYM: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Attempting Voice Recognition", "ThoughtRecordWizardSituationStep.SpeakToMYM");
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                Log.Info(TAG, "OnActivityResult: requestCode - " + requestCode.ToString() + ", resultCode - " + resultCode.ToString());
                if (requestCode == ConstantsAndTypes.VOICE_RECOGNITION_REQUEST && resultCode == Result.Ok)
                {
                    IList<string> matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches != null)
                    {
                        //NOTE: Instead of updating the EditText here we defer to the OnResume which uses the GlobalData version
                        //of the property changed - this method gets called PRIOR to OnResume so all we have to do is to set
                        //the GlobalData version and it will update via OnResume
                        switch (_speakCategory)
                        {
                            case ConstantsAndTypes.SituationCategories.What:
                                Log.Info(TAG, "OnActivityResult: Category 'What', text is - " + matches[0]);
                                GlobalData.SituationItem.What = matches[0];
                                break;

                            case ConstantsAndTypes.SituationCategories.When:
                                GlobalData.SituationItem.When = matches[0];
                                break;

                            case ConstantsAndTypes.SituationCategories.Where:
                                GlobalData.SituationItem.Where = matches[0];
                                break;

                            case ConstantsAndTypes.SituationCategories.Who:
                                GlobalData.SituationItem.Who = matches[0];
                                break;
                        }
                    }
                }
                base.OnActivityResult(requestCode, resultCode, data);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Checking Voice Recognition result", "ThoughtRecordWizardSituationStep.OnActivityResult");
            }
        }

        private void Next()
        {
            try
            {
                Validate();

                if (!_validated)
                    return;

                StoreSituation();

                //on to Automatic Thoughts
                Intent intent = new Intent(this, typeof(ThoughtRecordWizardMoodStep));
                StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Next: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorNextButtonSituation), "ThoughtRecordWizardSituationStep.Next");
            }
        }

        private void Cancel()
        {
            ShowCancelDialog();
        }

        private void ShowCancelDialog()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertMessage = GetString(Resource.String.ThoughtRecordWizardSituationConfirm);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonCancelCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonOKCaption);
                alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardSituationCancel);
                alertHelper.InstanceId = "situationCancel";

                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ShowCancelDialog: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCancelSituationAddition), "ThoughtRecordWizardSituationStep.ShowCancelDialog");
            }
        }

        private void Validate()
        {
            try
            {
                _validated = true;

                if (_situationWhat != null)
                {
                    if (string.IsNullOrWhiteSpace(_situationWhat.Text))
                    {
                        var errorString = GetString(Resource.String.situationWhatIncompleteError);
                        _situationWhat.Error = errorString;
                        _validated = false;
                        return;
                    }
                    else
                    {
                        _situationWhat.Error = null;
                    }
                }

                if (_situationWhen != null)
                {
                    if (string.IsNullOrWhiteSpace(_situationWhen.Text))
                    {
                        var errorString = GetString(Resource.String.situationWhenIncompleteError);
                        _situationWhen.Error = errorString;
                        _validated = false;
                        return;
                    }
                    else
                    {
                        _situationWhen.Error = null;
                    }
                }

                if (_situationWhere != null)
                {
                    if (string.IsNullOrWhiteSpace(_situationWhere.Text))
                    {
                        var errorString = GetString(Resource.String.situationWhereIncompleteError);
                        _situationWhere.Error = errorString;
                        _validated = false;
                        return;
                    }
                    else
                    {
                        _situationWhere.Error = null;
                    }
                }

                if (_situationWho != null)
                {
                    if (string.IsNullOrWhiteSpace(_situationWho.Text))
                    {
                        var errorString = GetString(Resource.String.situationWhoIncompleteError);
                        _situationWho.Error = errorString;
                        _validated = false;
                        return;
                    }
                    else
                    {
                        _situationWho.Error = null;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Validate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorValidatingSituation), "ThoughtRecordWizardSituationStep.Validate");
            }
        }

        private void StoreSituation()
        {
            try
            {
                if (GlobalData.SituationItem == null)
                {
                    GlobalData.SituationItem = new Model.Situation();
                }

                GlobalData.SituationItem.ThoughtRecordId = GlobalData.ThoughtRecordId;

                if (_situationWhat != null)
                    GlobalData.SituationItem.What = _situationWhat.Text.Trim();
                if (_situationWhen != null)
                    GlobalData.SituationItem.When = _situationWhen.Text.Trim();
                if (_situationWhere != null)
                    GlobalData.SituationItem.Where = _situationWhere.Text.Trim();
                if (_situationWho != null)
                    GlobalData.SituationItem.Who = _situationWho.Text.Trim();

                if (GlobalData.SituationItem.SituationId == 0) // 0 ID means we haven't saved yet
                {
                    Globals dbHelp = new Globals();
                    dbHelp.OpenDatabase();
                    var sqlDatabase = dbHelp.GetSQLiteDatabase();
                    if (sqlDatabase != null)
                    {
                        GlobalData.SituationItem.Save(sqlDatabase);
                    }
                    dbHelp.CloseDatabase();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "StoreSituation: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStoringSituation), "ThoughtRecordWizardSituationStep.StoreSituation");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "situationCancel")
            {
                try
                {
                    //Go back to the Thought Record screen
                    GlobalData.RemoveSituation();
                    GlobalData.RemoveThoughtRecord();

                    Intent intent = new Intent(this, typeof(ThoughtRecordsActivity));
                    Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                    if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRemovingSituation), "ThoughtRecordWizardSituationStep.AlertPositiveButtonSelect");
                }
            }

            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if(instanceId == "useMic")
            {
                PermissionResultUpdate(Permission.Denied);
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
            catch(Exception e)
            {
                Log.Error(TAG, "CheckMicPermission: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ThoughtRecordWizardSituationStep.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ThoughtRecordWizardSituationStep.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if(permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                _speakWhat.SetImageResource(Resource.Drawable.micgreyscale);
                _speakWho.SetImageResource(Resource.Drawable.micgreyscale);
                _speakWhere.SetImageResource(Resource.Drawable.micgreyscale);
                _speakWhen.SetImageResource(Resource.Drawable.micgreyscale);

                _speakWhat.Enabled = false;
                _speakWho.Enabled = false;
                _speakWhere.Enabled = false;
                _speakWhen.Enabled = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ThoughtRecordWizardSituationStep.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "ThoughtRecordWizardSituationStep.AttemptPermissionRequest");
            }
        }
    }
}
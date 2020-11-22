using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Content;
using Android.Support.V7.App;
using Android.Speech;
using Android.Runtime;
using System.Collections.Generic;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using Android.Views;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment.StructuredPlan;
using Android.Content.PM;
using com.spanyardie.MindYourMood.Model.Interfaces;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.Helpers
{
    [Activity(Label = "Feelings")]
    public class StructuredPlanFeelingsDialogActivity : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanFeelingsDialogFragment";

        private Feelings _feeling;

        private EditText _about;
        private SeekBar _strength;
        private TextView _strengthPercent;
        private Spinner _reaction;
        private Spinner _intention;
        private EditText _actionOf;

        private ImageButton _speakAbout;
        private ImageButton _speakActionOf;

        private Button _cancel;
        private Button _done;

        private LinearLayout _linStructuredPlanFeelingsMain;

        private int _currentSpeakType;

        private bool _firstTimeView = true;
        private int _feelingID;

        private string _dialogTitle = "";

        private Toolbar _toolbar;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutBoolean("firstTimeView", true);
                outState.PutInt("feelingID", _feelingID);
                outState.PutString("dialogTitle", _dialogTitle);
                outState.PutInt("currentSpeakType", _currentSpeakType);
            }

            base.OnSaveInstanceState(outState);
        }

        private void GetFeelingData()
        {
            try
            {
                if (_feelingID != -1)
                {
                    Log.Info(TAG, "GetFeelingData: Attempting to find Feeling with ID - " + _feelingID.ToString());
                    _feeling = GlobalData.StructuredPlanFeelings.Find(feel => feel.FeelingsID == _feelingID);
                    if (_feeling == null)
                        Log.Error(TAG, "GetFeelingData: _feeling is NULL");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFeelingData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFeelingsDialogGetData), "StructuredPlanFeelingsDialogActivity.GetFeelingData");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.StructuredPlanFeelingsMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                {
                    _firstTimeView = savedInstanceState.GetBoolean("firstTimeView");
                    _feelingID = savedInstanceState.GetInt("feelingID");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                    _currentSpeakType = savedInstanceState.GetInt("currentSpeakType");
                }
                if(Intent != null)
                {
                    _feelingID = Intent.GetIntExtra("feelingsID", -1);
                    _dialogTitle = Intent.GetStringExtra("activityTitle");
                }

                SetContentView(Resource.Layout.StructuredPlanFeelingsDialogActivityLayout);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.structuredplanfeelingsdialogactivityToolbar, Resource.String.StructuredPlanFeelingsActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanfeelingspager,
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

                GetFeelingData();

                SetupSpinners();

                if (_feelingID != -1 && _feeling != null)
                {
                    if (_firstTimeView)
                    {
                        //existing item
                        if (_about != null)
                            _about.Text = _feeling.AboutWhat.Trim();
                        if (_strength != null)
                            _strength.Progress = _feeling.Strength;
                        if (_reaction != null)
                            _reaction.SetSelection((int)_feeling.Type);
                        if (_intention != null)
                            _intention.SetSelection((int)_feeling.Action);
                        if (_actionOf != null)
                            _actionOf.Text = _feeling.ActionOf.Trim();
                        _firstTimeView = false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFeelingsDialogCreateView), "StructuredPlanFeelingsDialogActivity.OnCreateView");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linStructuredPlanFeelingsMain != null)
                _linStructuredPlanFeelingsMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
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
                    case Resource.Id.StructuredPlanFeelingsDialogActivityActionAdd:
                        Add();
                        return true;

                    case Resource.Id.StructuredPlanFeelingsDialogActivityActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanFeelingsHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void SetupCallbacks()
        {
            if(_speakAbout != null)
                _speakAbout.Click += SpeakAbout_Click;
            if(_speakActionOf != null)
                _speakActionOf.Click += SpeakActionOf_Click;
            if(_strength != null)
                _strength.ProgressChanged += Strength_ProgressChanged;
            if (_cancel != null)
                _cancel.Click += Cancel_Click;
            if (_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Add();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            GoBack();
        }

        private void Strength_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (_strengthPercent != null)
                _strengthPercent.Text = _strength.Progress.ToString() + "%";
        }

        private void SpeakActionOf_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_ACTION_OF, "What do you intend to do?");
        }

        private void SpeakAbout_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_ABOUT, "Feelings About...");
        }

        private void SpeakToMYM(int category, string message)
        {
            try
            {
                _currentSpeakType = category;
                Log.Info(TAG, "SpeakToMYM: _speakCategory - " + category.ToString());

                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, message);

                Log.Info(TAG, "SpeakToMYM: Created intent, sending request...");
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SpeakToMYM: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Attempting Voice Recognition", "StructuredPlanFeelingsDialogActivity.SpeakToMYM");
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == ConstantsAndTypes.VOICE_RECOGNITION_REQUEST && resultCode == Result.Ok)
            {
                IList<string> matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);

                if (matches != null)
                {
                    switch (_currentSpeakType)
                    {
                        case ConstantsAndTypes.SPEAK_ABOUT:
                            if (_about != null)
                                _about.Text = matches[0];
                            break;
                        case ConstantsAndTypes.SPEAK_ACTION_OF:
                            if (_actionOf != null)
                                _actionOf.Text = matches[0];
                            break;
                    }
                }
            }
        }

        private void Add()
        {
            try
            {
                var about = _about.Text.Trim();
                if(string.IsNullOrEmpty(about))
                {
                    _about.Error = GetString(Resource.String.ErrorStructuredPlanFeelingsDialogAbout);
                    return;
                }

                var strength = _strength.Progress;
                var reaction = (ConstantsAndTypes.REACTION_TYPE)_reaction.SelectedItemPosition;
                var intention = (ConstantsAndTypes.ACTION_TYPE)_intention.SelectedItemPosition;
                var actionOf = _actionOf.Text.Trim();
                if (string.IsNullOrEmpty(actionOf))
                {
                    _actionOf.Error = GetString(Resource.String.ErrorStructuredPlanFeelingsDialogAction);
                    return;
                }

                Intent intent = new Intent();
                intent
                    .PutExtra("feelingsID", _feelingID)
                    .PutExtra("about", about)
                    .PutExtra("strength", strength)
                    .PutExtra("reaction", (int)reaction)
                    .PutExtra("intention", (int)intention)
                    .PutExtra("actionOf", actionOf);

                SetResult(Result.Ok, intent);
                Finish();

            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanFeelingsDialogAdd), "StructuredPlanFeelingsDialogActivity.Add_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.StructuredPlanFeelingsDialogActivityActionAdd);
                var itemHelp = menu.FindItem(Resource.Id.StructuredPlanFeelingsDialogActivityActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanFeelingsDialogActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            SetResult(Result.Canceled);
            Finish();
        }

        private void GetFieldComponents()
        {
            try
            {
                _about = FindViewById<EditText>(Resource.Id.edtStructuredPlanFeelingDialogAboutText);
                _strength = FindViewById<SeekBar>(Resource.Id.skbStructuredPlanFeelingDialogStrength);
                _strengthPercent = FindViewById<TextView>(Resource.Id.txtStructuredPlanFeelingsStrengthPercent);
                _reaction = FindViewById<Spinner>(Resource.Id.spnStructuredPlanFeelingDialogReaction);
                _intention = FindViewById<Spinner>(Resource.Id.spnStructuredPlanFeelingDialogIntention);
                _actionOf = FindViewById<EditText>(Resource.Id.edtStructuredPlanFeelingDialogActionOfText);
                _speakAbout = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakFeelingsAbout);
                _speakActionOf = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakFeelingsOf);
                _cancel = FindViewById<Button>(Resource.Id.btnCancel);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _linStructuredPlanFeelingsMain = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanFeelingsMain);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFeelingsDialogGetComponents), "StructuredPlanFeelingsDialogActivity.GetFieldComponents");
            }
        }

        private void SetupSpinners()
        {
            SetupReactionSpinner();
            SetupIntentionSpinner();
        }

        private void SetupReactionSpinner()
        {
            if(_reaction != null)
            {
                try
                {
                    string[] reactions = StringHelper.ReactionList();

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, reactions);
                    if(adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _reaction.Adapter = adapter;
                        Log.Info(TAG, "SetupRectionSpinner: Set Reaction type adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupRectionSpinner: Failed to create adapter");
                    }
                }
                catch(Exception e)
                {
                    Log.Error(TAG, "SetupRectionSpinner: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFeelingsDialogSetReactionSpin), "StructuredPlanFeelingsDialogActivity.SetupRectionSpinner");
                }
            }
            else
            {
                Log.Error(TAG, "SetupRectionSpinner: _reaction is NULL!");
            }
        }

        private void SetupIntentionSpinner()
        {
            if (_intention != null)
            {
                try
                {
                    string[] intentions = StringHelper.ActionList();

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, intentions);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _intention.Adapter = adapter;
                        Log.Info(TAG, "SetupIntentionSpinner: Set Intentions type adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupIntentionSpinner: Failed to create adapter");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "SetupIntentionSpinner: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFeelingsDialogSetIntentSpin), "StructuredPlanFeelingsDialogActivity.SetupIntentionSpinner");
                }
            }
            else
            {
                Log.Error(TAG, "SetupRectionSpinner: _reaction is NULL!");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "StructuredPlanFeelingsDialogActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "StructuredPlanFeelingsDialogActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                if(_speakAbout != null)
                    _speakAbout.SetImageResource(Resource.Drawable.micgreyscale);
                if(_speakActionOf != null)
                    _speakActionOf.SetImageResource(Resource.Drawable.micgreyscale);

                if(_speakAbout != null)
                    _speakAbout.Enabled = false;
                if(_speakActionOf != null)
                    _speakActionOf.Enabled = false;
            }
        }

        private void ShowPermissionRationale()
        {
            try
            {
                if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagMic").SettingValue == "True")
                {
                    if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.UseMicrophone) == true))
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "StructuredPlanFeelingsDialogActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "StructuredPlanFeelingsDialogActivity.AttemptPermissionRequest");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                PermissionResultUpdate(Permission.Denied);
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
            }
        }
    }
}
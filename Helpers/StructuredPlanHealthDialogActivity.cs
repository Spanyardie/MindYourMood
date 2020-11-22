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
    [Activity(Label = "Health")]
    public class StructuredPlanHealthDialogActivity : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanHealthDialogActivity";

        private Health _health;

        private EditText _aspect;
        private SeekBar _importance;
        private TextView _importancePercent;
        private Spinner _reaction;
        private Spinner _intention;
        private EditText _actionOf;

        private ImageButton _speakAspect;
        private ImageButton _speakActionOf;

        private LinearLayout _linStructuredPlanHealthMain;

        private Button _cancel;
        private Button _done;

        private int _currentSpeakType;

        private bool _firstTimeView = true;
        private int _healthID;

        private string _dialogTitle = "";

        private Toolbar _toolbar;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutBoolean("firstTimeView", true);
                outState.PutInt("healthID", _healthID);
                outState.PutString("dialogTitle", _dialogTitle);
                outState.PutInt("currentSpeakType", _currentSpeakType);
            }

            base.OnSaveInstanceState(outState);
        }

        private void GetHealthData()
        {
            try
            {
                if (_healthID != -1)
                {
                    Log.Info(TAG, "GetHealthData: Attempting to find Health with ID - " + _healthID.ToString());
                    _health = GlobalData.StructuredPlanHealth.Find(feel => feel.HealthID == _healthID);
                    if (_health == null)
                        Log.Error(TAG, "GetHealthData: _health is NULL");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetHealthData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanHealthDialogGetData), "StructuredPlanHealthDialogActivity.GetHealthData");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.StructuredPlanHealthMenu, menu);

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
                    _healthID = savedInstanceState.GetInt("healthID");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                    _currentSpeakType = savedInstanceState.GetInt("currentSpeakType");
                }
                if(Intent != null)
                {
                    _healthID = Intent.GetIntExtra("healthID", -1);
                    _dialogTitle = Intent.GetStringExtra("activityTitle");
                }

                SetContentView(Resource.Layout.StructuredPlanHealthDialogActivityLayout);

                GetFieldComponents();
                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.structuredplanhealthdialogactivityToolbar, Resource.String.StructuredPlanHealthActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanhealthpager,
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

                GetHealthData();

                SetupSpinners();

                if (_healthID != -1 && _health != null)
                {
                    if (_firstTimeView)
                    {
                        //existing item
                        if (_aspect != null)
                            _aspect.Text = _health.Aspect.Trim();
                        if (_importance != null)
                            _importance.Progress = _health.Importance;
                        if (_reaction != null)
                            _reaction.SetSelection((int)_health.Type);
                        if (_intention != null)
                            _intention.SetSelection((int)_health.Action);
                        if (_actionOf != null)
                            _actionOf.Text = _health.ActionOf.Trim();
                        _firstTimeView = false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanHealthDialogCreateView), "StructuredPlanHealthDialogActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linStructuredPlanHealthMain != null)
                _linStructuredPlanHealthMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
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
                    case Resource.Id.StructuredPlanHealthDialogActivityActionAdd:
                        Add();
                        return true;
                    case Resource.Id.StructuredPlanHealthDialogActivityActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanHealthHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void SetupCallbacks()
        {
            if(_speakActionOf != null)
                _speakActionOf.Click += SpeakActionOf_Click;
            if(_speakAspect != null)
                _speakAspect.Click += SpeakAspect_Click;
            if(_importance != null)
                _importance.ProgressChanged += Importance_ProgressChanged;
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

        private void Importance_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (_importancePercent != null)
                _importancePercent.Text = _importance.Progress.ToString() + "%";
        }

        private void SpeakAspect_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_ASPECT, "What aspect of your Health?");
        }

        private void SpeakActionOf_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_ACTION_OF, "What do you intend to do?");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Attempting Voice Recognition", "StructuredPlanHealthDialogActivity.SpeakToMYM");
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
                        case ConstantsAndTypes.SPEAK_ASPECT:
                            if (_aspect != null)
                                _aspect.Text = matches[0];
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
                var aspect = _aspect.Text.Trim();
                if (string.IsNullOrEmpty(aspect))
                {
                    _aspect.Error = GetString(Resource.String.ErrorStructuredPlanHealthDialogAspect);
                    return;
                }

                var importance = _importance.Progress;
                var reaction = (ConstantsAndTypes.REACTION_TYPE)_reaction.SelectedItemPosition;
                var intention = (ConstantsAndTypes.ACTION_TYPE)_intention.SelectedItemPosition;
                var actionOf = _actionOf.Text.Trim();
                if (string.IsNullOrEmpty(actionOf))
                {
                    _actionOf.Error = GetString(Resource.String.ErrorStructuredPlanHealthDialogAction);
                    return;
                }

                Intent intent = new Intent();
                intent
                    .PutExtra("healthID", _healthID)
                    .PutExtra("aspect", aspect)
                    .PutExtra("importance", importance)
                    .PutExtra("reaction", (int)reaction)
                    .PutExtra("intention", (int)intention)
                    .PutExtra("actionOf", actionOf);

                SetResult(Result.Ok, intent);

                Finish();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanHealthDialogAdd), "StructuredPlanHealthDialogActivity.Add");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.StructuredPlanHealthDialogActivityActionAdd);
                var itemHelp = menu.FindItem(Resource.Id.StructuredPlanHealthDialogActivityActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanHealthDialogActivity.SetActionIcons");
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
                _aspect = FindViewById<EditText>(Resource.Id.edtStructuredPlanHealthDialogAboutText);
                _importance = FindViewById<SeekBar>(Resource.Id.skbStructuredPlanHealthDialogStrength);
                _importancePercent = FindViewById<TextView>(Resource.Id.txtStructuredPlanHealthImportancePercent);
                _reaction = FindViewById<Spinner>(Resource.Id.spnStructuredPlanHealthDialogReaction);
                _intention = FindViewById<Spinner>(Resource.Id.spnStructuredPlanHealthDialogIntention);
                _actionOf = FindViewById<EditText>(Resource.Id.edtStructuredPlanHealthDialogActionOfText);
                _speakActionOf = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakHealthOf);
                _speakAspect = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakHealthAspect);
                _cancel = FindViewById<Button>(Resource.Id.btnCancel);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _linStructuredPlanHealthMain = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanHealthMain);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanHealthDialogGetComponents), "StructuredPlanHealthDialogActivity.GetFieldComponents");
            }
        }

        private void SetupSpinners()
        {
            SetupReactionSpinner();
            SetupIntentionSpinner();
        }

        private void SetupReactionSpinner()
        {
            if (_reaction != null)
            {
                try
                {
                    string[] reactions = StringHelper.ReactionList();

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, reactions);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _reaction.Adapter = adapter;
                        Log.Info(TAG, "SetupReactionSpinner: Set Reaction type adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupReactionSpinner: Failed to create adapter");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "SetupRectionSpinner: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanHealthDialogSetReactSpin), "StructuredPlanHealthDialogActivity.SetupReactionSpinner");
                }
            }
            else
            {
                Log.Error(TAG, "SetupReactionSpinner: _reaction is NULL!");
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
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanHealthDialogSetIntentSpin), "StructuredPlanHealthDialogActivity.SetupIntentionSpinner");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "StructuredPlanHealthDialogActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "StructuredPlanHealthDialogActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                if(_speakAspect != null)
                    _speakAspect.SetImageResource(Resource.Drawable.micgreyscale);
                if(_speakActionOf != null)
                    _speakActionOf.SetImageResource(Resource.Drawable.micgreyscale);

                if(_speakAspect != null)
                    _speakAspect.Enabled = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "StructuredPlanHealthDialogActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "StructuredPlanHealthDialogActivity.AttemptPermissionRequest");
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
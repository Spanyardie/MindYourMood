using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Support.V7.App;
using Android.Content;
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
    [Activity(Label = "Fantasies")]
    public class StructuredPlanFantasiesDialogActivity : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanFantasiesDialogActivity";

        private Fantasies _fantasies;

        private EditText _ofWhat;
        private SeekBar _strength;
        private TextView _strengthPercent;
        private Spinner _reaction;
        private Spinner _intention;
        private EditText _actionOf;

        private ImageButton _speakOfWhat;
        private ImageButton _speakActionOf;

        private Button _cancel;
        private Button _done;
        private LinearLayout _linStructuredPlanFantasiesMain;

        private int _currentSpeakType;

        private bool _firstTimeView = true;
        private int _fantasiesID;

        private string _dialogTitle = "";

        private Toolbar _toolbar;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutBoolean("firstTimeView", true);
                outState.PutInt("fantasiesID", _fantasiesID);
                outState.PutString("dialogTitle", _dialogTitle);
                outState.PutInt("currentSpeakType", _currentSpeakType);
            }

            base.OnSaveInstanceState(outState);
        }

        private void GetFantasiesData()
        {
            try
            {
                if (_fantasiesID != -1)
                {
                    Log.Info(TAG, "GetFantasiesData: Attempting to find Fantasies with ID - " + _fantasiesID.ToString());
                    _fantasies = GlobalData.StructuredPlanFantasies.Find(feel => feel.FantasiesID == _fantasiesID);
                    if (_fantasies == null)
                        Log.Error(TAG, "GetFantasiesData: _fantasies is NULL");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFantasiesData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFantasiesDialogGetData), "StructuredPlanFantasiesDialogActivity.GetFantasiesData");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.StructuredPlanFantasiesMenu, menu);

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
                    _fantasiesID = savedInstanceState.GetInt("fantasiesID");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                    _currentSpeakType = savedInstanceState.GetInt("currentSpeakType");
                }
                if(Intent != null)
                {
                    _fantasiesID = Intent.GetIntExtra("fantasiesID", -1);
                    _dialogTitle = Intent.GetStringExtra("activityTitle");
                }

                SetContentView(Resource.Layout.StructuredPlanFantasiesDialogActivityLayout);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.structuredplanfantasiesdialogactivityToolbar, Resource.String.StructuredPlanFantasiesActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanfantasiespager,
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

                SetupSpinners();

                GetFantasiesData();

                if (_fantasiesID != -1 && _fantasies != null)
                {
                    if (_firstTimeView)
                    {
                        //existing item
                        if (_ofWhat != null)
                            _ofWhat.Text = _fantasies.OfWhat.Trim();
                        if (_strength != null)
                            _strength.Progress = _fantasies.Strength;
                        if (_reaction != null)
                            _reaction.SetSelection((int)_fantasies.Type);
                        if (_intention != null)
                            _intention.SetSelection((int)_fantasies.Action);
                        if (_actionOf != null)
                            _actionOf.Text = _fantasies.ActionOf.Trim();
                        _firstTimeView = false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFantasiesDialogCreateView), "StructuredPlanFantasiesDialogActivity.OnCreateView");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linStructuredPlanFantasiesMain != null)
                _linStructuredPlanFantasiesMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
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
                    case Resource.Id.StructuredPlanFantasiesDialogActivityActionAdd:
                        Add();
                        return true;
                    case Resource.Id.StructuredPlanFantasiesDialogActivityActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanFantasiesHelpActivity));
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
                var itemAdd = menu.FindItem(Resource.Id.StructuredPlanFantasiesDialogActivityActionAdd);
                var itemHelp = menu.FindItem(Resource.Id.StructuredPlanFantasiesDialogActivityActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanFantasiesDialogActivity.SetActionIcons");
            }
        }

        private void SetupCallbacks()
        {
            if(_speakActionOf != null)
                _speakActionOf.Click += SpeakActionOf_Click;
            if(_speakOfWhat != null)
                _speakOfWhat.Click += SpeakOfWhat_Click;
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

        private void SpeakOfWhat_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_OF, "Your Fantasy is...");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Attempting Voice Recognition", "StructuredPlanFantasiesDialogActivity.SpeakToMYM");
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
                        case ConstantsAndTypes.SPEAK_OF:
                            if (_ofWhat != null)
                                _ofWhat.Text = matches[0];
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
                var ofWhat = _ofWhat.Text.Trim();
                if (string.IsNullOrEmpty(ofWhat))
                {
                    _ofWhat.Error = GetString(Resource.String.ErrorStructuredPlanFantasiesDialogOfWhat);
                    return;
                }

                var strength = _strength.Progress;
                var reaction = (ConstantsAndTypes.REACTION_TYPE)_reaction.SelectedItemPosition;
                var intention = (ConstantsAndTypes.ACTION_TYPE)_intention.SelectedItemPosition;
                var actionOf = _actionOf.Text.Trim();
                if (string.IsNullOrEmpty(actionOf))
                {
                    _actionOf.Error = GetString(Resource.String.ErrorStructuredPlanFantasiesDialogAction);
                    return;
                }

                Intent intent = new Intent();
                intent
                    .PutExtra("fantasiesID", _fantasiesID)
                    .PutExtra("ofWhat", ofWhat)
                    .PutExtra("strength", strength)
                    .PutExtra("reaction", (int)reaction)
                    .PutExtra("intention", (int)intention)
                    .PutExtra("actionOf", actionOf);

                SetResult(Result.Ok, intent);
                Finish();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanFantasiesDialogAdd), "StructuredPlanFantasiesDialogActivity.Add_Click");
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
                _ofWhat = FindViewById<EditText>(Resource.Id.edtStructuredPlanFantasiesDialogAboutText);
                _strength = FindViewById<SeekBar>(Resource.Id.skbStructuredPlanFantasiesDialogStrength);
                _strengthPercent = FindViewById<TextView>(Resource.Id.txtStructuredPlanFantasiesStrengthPercent);
                _reaction = FindViewById<Spinner>(Resource.Id.spnStructuredPlanFantasiesDialogReaction);
                _intention = FindViewById<Spinner>(Resource.Id.spnStructuredPlanFantasiesDialogIntention);
                _actionOf = FindViewById<EditText>(Resource.Id.edtStructuredPlanFantasiesDialogActionOfText);
                _speakActionOf = FindViewById<ImageButton>(Resource.Id.imgbtnFantasiesSpeakOfWhat);
                _speakOfWhat = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakFantasiesOf);
                _cancel = FindViewById<Button>(Resource.Id.btnCancel);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _linStructuredPlanFantasiesMain = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanFantasiesMain);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFantasiesDialogGetComponents), "StructuredPlanFantasiesDialogActivity.GetFieldComponents");
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
                        Log.Info(TAG, "SetupRectionSpinner: Set Reaction type adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupRectionSpinner: Failed to create adapter");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "SetupRectionSpinner: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFantasiesDialogSetReactSpinner), "StructuredPlanFantasiesDialogActivity.SetupReactionSpinner");
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
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFantasiesDialogSetIntentSpinner), "StructuredPlanFantasiesDialogActivity.SetupIntentionSpinner");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "StructuredPlanFantasiesDialogActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "StructuredPlanFantasiesDialogActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                if(_speakOfWhat != null)
                    _speakOfWhat.SetImageResource(Resource.Drawable.micgreyscale);
                if(_speakActionOf != null)
                    _speakActionOf.SetImageResource(Resource.Drawable.micgreyscale);

                if(_speakOfWhat != null)
                    _speakOfWhat.Enabled = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "StructuredPlanFantasiesDialogActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "StructuredPlanFantasiesDialogActivity.AttemptPermissionRequest");
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
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Support.V7.App;
using Android.Speech;
using Android.Runtime;
using System.Collections.Generic;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Views;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment.StructuredPlan;
using Android.Content.PM;
using com.spanyardie.MindYourMood.Model.Interfaces;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.Helpers
{
    [Activity(Label = "Attitudes")]
    public class StructuredPlanAttitudesDialogActivity : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanAttitudesDialogActivity";

        private Attitudes _attitudes;

        private EditText _toWhat;
        private Spinner _type;
        private SeekBar _belief;
        private TextView _beliefPercent;
        private Spinner _feeling;
        private Spinner _action;
        private EditText _actionOf;
        private ImageButton _speakToWhat;
        private ImageButton _speakOf;
        private LinearLayout _linStructuredPlanAttitudeMain;

        private Button _cancel;
        private Button _done;

        private bool _firstTimeView = true;
        private int _attitudesID;

        private string _dialogTitle = "";

        private int _currentSpeakType;

        private Toolbar _toolbar;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutBoolean("firstTimeView", true);
                outState.PutInt("attitudesID", _attitudesID);
                outState.PutString("dialogTitle", _dialogTitle);
                outState.PutInt("currentSpeakType", _currentSpeakType);
            }

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.StructuredPlanAttitudesMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.StructuredPlanAttitudesDialogActivityActionAdd);
                var itemHelp = menu.FindItem(Resource.Id.structuredPlanAttitudesDialogActivityActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanAttitudesDialogActivity.SetActionIcons");
            }
        }

        private void GetAttitudesData()
        {
            try
            {
                if (_attitudesID != -1)
                {
                    Log.Info(TAG, "GetAttitudesData: Attempting to find Attitudes with ID - " + _attitudesID.ToString());
                    _attitudes = GlobalData.StructuredPlanAttitudes.Find(attitude => attitude.AttitudesID == _attitudesID);
                    if (_attitudes == null)
                        Log.Error(TAG, "GetAttitudesData: _attitudes is NULL");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAttitudesData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudeDialogGetData), "StructuredPlanAttitudesDialogActivity.GetAttitudesData");
            }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                {
                    _firstTimeView = savedInstanceState.GetBoolean("firstTimeView");
                    _attitudesID = savedInstanceState.GetInt("attitudesID");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                    _currentSpeakType = savedInstanceState.GetInt("currentSpeakType");
                }
                if(Intent != null)
                {
                    _attitudesID = Intent.GetIntExtra("attitudesID", -1);
                    _dialogTitle = Intent.GetStringExtra("activityTitle");
                }

                SetContentView(Resource.Layout.StructuredPlanAttitudesDialogActivityLayout);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.structuredplanattitudesdialogToolbar, Resource.String.StructuredPlanAttitudesActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanattitudespager,
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

                GetAttitudesData();

                SetupSpinners();

                if (_attitudesID != -1 && _attitudes != null)
                {
                    if (_firstTimeView)
                    {
                        //existing item
                        if (_toWhat != null)
                            _toWhat.Text = _attitudes.ToWhat.Trim();
                        if (_type != null)
                            _type.SetSelection((int)_attitudes.TypeOf);
                        if (_belief != null)
                            _belief.Progress = _attitudes.Belief;
                        if (_feeling != null)
                            _feeling.SetSelection(_attitudes.Feeling);
                        if (_action != null)
                            _action.SetSelection((int)_attitudes.Action);
                        if (_actionOf != null)
                            _actionOf.Text = _attitudes.ActionOf.Trim();
                        _firstTimeView = false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudeDialogCreateView), "StructuredPlanAttitudesDialogActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linStructuredPlanAttitudeMain != null)
                _linStructuredPlanAttitudeMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            if (_speakOf != null)
                _speakOf.Click += SpeakOf_Click;
            if(_speakToWhat != null)
                _speakToWhat.Click += SpeakToWhat_Click;
            if(_belief != null)
                _belief.ProgressChanged += Belief_ProgressChanged;
            if(_cancel != null)
                _cancel.Click += Cancel_Click;
            if(_done != null)
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

        private void Belief_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (_beliefPercent != null)
                _beliefPercent.Text = _belief.Progress.ToString() + "%";
        }

        private void SpeakToWhat_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_TO_WHAT, "Your Attitude to...");
        }

        private void SpeakOf_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_OF, "What do you intend to do?");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Attempting Voice Recognition", "StructuredPlanAttitudesDialogActivity.SpeakToMYM");
            }
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
                    case Resource.Id.StructuredPlanAttitudesDialogActivityActionAdd:
                        Add();
                        return true;
                    case Resource.Id.structuredPlanAttitudesDialogActivityActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanAttitudesHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
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
                            if (_actionOf != null)
                                _actionOf.Text = matches[0];
                            break;
                        case ConstantsAndTypes.SPEAK_TO_WHAT:
                            if (_toWhat != null)
                                _toWhat.Text = matches[0];
                            break;
                    }
                }
            }
        }

        private void Add()
        {
            try
            {
                var toWhat = _toWhat.Text.Trim();
                if (string.IsNullOrEmpty(toWhat))
                {
                    _toWhat.Error = GetString(Resource.String.ErrorStructuredPlanAttitudeDialogToWhat);
                    return;
                }

                var type = (ConstantsAndTypes.ATTITUDE_TYPES)_type.SelectedItemPosition;
                var belief = _belief.Progress;
                var feeling = _feeling.SelectedItemPosition;
                var action = (ConstantsAndTypes.ACTION_TYPE)_action.SelectedItemPosition;
                var actionOf = _actionOf.Text.Trim();
                if (string.IsNullOrEmpty(actionOf))
                {
                    _actionOf.Error = GetString(Resource.String.ErrorStructuredPlanAttitudeDialogActionOf);
                    return;
                }

                Intent intent = new Intent();
                intent
                    .PutExtra("attitudesID", _attitudesID)
                    .PutExtra("toWhat", toWhat)
                    .PutExtra("type", (int)type)
                    .PutExtra("belief", belief)
                    .PutExtra("feeling", feeling)
                    .PutExtra("action", (int)action)
                    .PutExtra("actionOf", actionOf);

                SetResult(Result.Ok, intent);

                Finish();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanAttitudeDialogAdd), "StructuredPlanAttitudesDialogActivity.Add_Click");
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
                _toWhat = FindViewById<EditText>(Resource.Id.edtStructuredPlanAttitudesDialogAboutText);
                _type = FindViewById<Spinner>(Resource.Id.spnStructuredPlanAttitudesDialogType);
                _belief = FindViewById<SeekBar>(Resource.Id.skbStructuredPlanAttitudesDialogStrength);
                _beliefPercent = FindViewById<TextView>(Resource.Id.txtStructuredPlanAttitudesBeliefPercent);
                _feeling = FindViewById<Spinner>(Resource.Id.spnStructuredPlanAttitudesDialogFeeling);
                _action = FindViewById<Spinner>(Resource.Id.spnStructuredPlanAttitudesDialogAction);
                _actionOf = FindViewById<EditText>(Resource.Id.edtStructuredPlanAttitudesDialogActionOfText);
                _speakToWhat = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakAttitudeToWhat);
                _speakOf = FindViewById<ImageButton>(Resource.Id.imgbtnAttitudesSpeakOf);
                _cancel = FindViewById<Button>(Resource.Id.btnCancel);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _linStructuredPlanAttitudeMain = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanAttitudesMain);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudeDialogGetComponents), "StructuredPlanAttitudesDialogActivity.GetFieldComponents");
            }
        }

        private void SetupSpinners()
        {
            SetupTypeSpinner();
            SetupFeelingSpinner();
            SetupActionSpinner();
        }

        private void SetupTypeSpinner()
        {
            if (_type != null)
            {
                try
                {
                    string[] types = StringHelper.AttitudeList();

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, types);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _type.Adapter = adapter;
                        Log.Info(TAG, "SetupTypeSpinner: Set Type adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupTypeSpinner: Failed to create adapter");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "SetupTypeSpinner: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudeDialogSetTypeSpin), "StructuredPlanAttitudesDialogActivity.SetupTypeSpinner");
                }
            }
            else
            {
                Log.Error(TAG, "SetupTypeSpinner: _type is NULL!");
            }
        }

        private void SetupFeelingSpinner()
        {
            if (_feeling != null)
            {
                try
                {
                    Globals dataHelper = new Globals();

                    var feelings = dataHelper.GetAllMoodsForAdapter();
                    string[] feelingsArray = feelings.ToArray();

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, feelingsArray);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _feeling.Adapter = adapter;
                        Log.Info(TAG, "SetupFeelingSpinner: Set Feelings adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupFeelingSpinner: Failed to create adapter");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "SetupFeelingSpinner: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudeDialogSetFeelSpin), "StructuredPlanAttitudesDialogActivity.SetupFeelingSpinner");
                }
            }
            else
            {
                Log.Error(TAG, "SetupFeelingSpinner: _reaction is NULL!");
            }
        }

        private void SetupActionSpinner()
        {
            if (_action != null)
            {
                try
                {
                    string[] actions = StringHelper.ActionList();

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, actions);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _action.Adapter = adapter;
                        Log.Info(TAG, "SetupActionSpinner: Set Action type adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupActionSpinner: Failed to create adapter");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "SetupActionSpinner: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudeDialogSetActSpin), "StructuredPlanAttitudesDialogActivity.SetupActionSpinner");
                }
            }
            else
            {
                Log.Error(TAG, "SetupActionSpinner: _action is NULL!");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "StructuredPlanAttitudesDialogActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "StructuredPlanAttitudesDialogActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                if(_speakToWhat != null)
                    _speakToWhat.SetImageResource(Resource.Drawable.micgreyscale);
                if(_speakOf != null)
                    _speakOf.SetImageResource(Resource.Drawable.micgreyscale);

                if(_speakToWhat != null)
                    _speakToWhat.Enabled = false;
                if(_speakOf != null)
                    _speakOf.Enabled = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "StructuredPlanAttitudesDialogActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "StructuredPlanAttitudesDialogActivity.AttemptPermissionRequest");
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
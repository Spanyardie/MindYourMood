using System;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Content;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Speech;
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
    [Activity(Label = "Relationships")]
    public class StructuredPlanRelationshipsDialogActivity : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanRelationshipsDialogActivity";

        private Relationships _relationships;

        private EditText _withWhom;
        private Spinner _type;
        private SeekBar _strength;
        private TextView _strengthPercent;
        private Spinner _feeling;
        private Spinner _action;
        private EditText _actionOf;

        private ImageButton _speakWith;
        private ImageButton _speakOf;

        private Button _cancel;
        private Button _done;

        private LinearLayout _linStructuredPlanRelationshipsMain;

        private int _currentSpeakType;

        private bool _firstTimeView = true;
        private int _relationshipsID;

        private string _dialogTitle = "";

        private Toolbar _toolbar;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutBoolean("firstTimeView", true);
                outState.PutInt("relationshipsID", _relationshipsID);
                outState.PutString("dialogTitle", _dialogTitle);
                outState.PutInt("currentSpeakType", _currentSpeakType);
            }

            base.OnSaveInstanceState(outState);
        }

        private void GetRelationshipsData()
        {
            try
            {
                if (_relationshipsID != -1)
                {
                    Log.Info(TAG, "GetRelationshipsData: Attempting to find Relationships with ID - " + _relationshipsID.ToString());
                    _relationships = GlobalData.StructuredPlanRelationships.Find(attitude => attitude.RelationshipsID == _relationshipsID);
                    if (_relationships == null)
                        Log.Error(TAG, "GetRelationshipsData: _relationships is NULL");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetRelationshipsData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsDialogGetData), "StructuredPlanRelationshipsDialogActivity.GetRelationshipsData");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.StructuredPlanRelationshipsMenu, menu);

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
                    _relationshipsID = savedInstanceState.GetInt("relationshipsID");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                    _currentSpeakType = savedInstanceState.GetInt("currentSpeakType");
                }
                if(Intent != null)
                {
                    _relationshipsID = Intent.GetIntExtra("relationshipsID", -1);
                    _dialogTitle = Intent.GetStringExtra("activityTitle");
                }

                SetContentView(Resource.Layout.StructuredPlanRelationshipsDialogActivityLayout);

                GetFieldComponents();
                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.structuredplanrelationshipsdialogactivitylayoutToolbar, Resource.String.StructuredPlanRelationshipsActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanrelationshipspager,
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

                GetRelationshipsData();

                SetupSpinners();

                if (_relationshipsID != -1 && _relationships != null)
                {
                    if (_firstTimeView)
                    {
                        //existing item
                        if (_withWhom != null)
                            _withWhom.Text = _relationships.WithWhom.Trim();
                        if (_type != null)
                            _type.SetSelection((int)_relationships.Type);
                        if (_strength != null)
                            _strength.Progress = _relationships.Strength;
                        if (_feeling != null)
                            _feeling.SetSelection(_relationships.Feeling);
                        if (_action != null)
                            _action.SetSelection((int)_relationships.Action);
                        if (_actionOf != null)
                            _actionOf.Text = _relationships.ActionOf.Trim();
                        _firstTimeView = false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsDialogCreateView), "StructuredPlanRelationshipsDialogActivity.OnCreateView");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linStructuredPlanRelationshipsMain != null)
                _linStructuredPlanRelationshipsMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
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
                    case Resource.Id.StructuredPlanRelationshipsDialogActivityActionAdd:
                        Add();
                        return true;
                    case Resource.Id.StructuredPlanRelationshipsDialogActivityActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanRelationshipsHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void SetupCallbacks()
        {
            if(_speakWith != null)
                _speakWith.Click += SpeakWith_Click;
            if(_speakOf != null)
                _speakOf.Click += SpeakOf_Click;
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

        private void SpeakOf_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_OF, "What Action are you taking?");
        }

        private void SpeakWith_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_WITH, "Who is the Relationship with?");
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
                        case ConstantsAndTypes.SPEAK_WITH:
                            if (_withWhom != null)
                                _withWhom.Text = matches[0];
                            break;
                    }
                }
            }
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Attempting Voice Recognition", "StructuredPlanRelationshipsDialogActivity.SpeakToMYM");
            }
        }

        private void Add()
        {
            try
            {
                var withWhom = _withWhom.Text.Trim();
                if (string.IsNullOrEmpty(withWhom))
                {
                    _withWhom.Error = GetString(Resource.String.ErrorStructuredPlanRelationshipsDialogWith);
                    return;
                }

                var type = (ConstantsAndTypes.RELATIONSHIP_TYPE)_type.SelectedItemPosition;
                var strength = _strength.Progress;
                var feeling = _feeling.SelectedItemPosition;
                var action = (ConstantsAndTypes.ACTION_TYPE)_action.SelectedItemPosition;
                var actionOf = _actionOf.Text.Trim();
                if (string.IsNullOrEmpty(actionOf))
                {
                    _actionOf.Error = GetString(Resource.String.ErrorStructuredPlanRelationshipsDialogAction);
                    return;
                }

                Intent intent = new Intent();
                intent
                    .PutExtra("relationshipsID", _relationshipsID)
                    .PutExtra("withWhom", withWhom)
                    .PutExtra("type", (int)type)
                    .PutExtra("strength", strength)
                    .PutExtra("feeling", feeling)
                    .PutExtra("action", (int)action)
                    .PutExtra("actionOf", actionOf);

                SetResult(Result.Ok, intent);
                Finish();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanRelationshipsDialogAdd), "StructuredPlanRelationshipsDialogActivity.Add_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.StructuredPlanRelationshipsDialogActivityActionAdd);
                var itemHelp = menu.FindItem(Resource.Id.StructuredPlanRelationshipsDialogActivityActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanRelationshipsDialogActivity.SetActionIcons");
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
                _withWhom = FindViewById<EditText>(Resource.Id.edtStructuredPlanRelationshipsDialogAboutText);
                _type = FindViewById<Spinner>(Resource.Id.spnStructuredPlanRelationshipsDialogType);
                _strength = FindViewById<SeekBar>(Resource.Id.skbStructuredPlanRelationshipsDialogStrength);
                _strengthPercent = FindViewById<TextView>(Resource.Id.txtStructuredPlanRelationshipsStrengthPercent);
                _feeling = FindViewById<Spinner>(Resource.Id.spnStructuredPlanRelationshipsDialogFeeling);
                _action = FindViewById<Spinner>(Resource.Id.spnStructuredPlanRelationshipsDialogAction);
                _actionOf = FindViewById<EditText>(Resource.Id.edtStructuredPlanRelationshipsDialogActionOfText);
                _speakOf = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakRelationshipsOf);
                _speakWith = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakRelationshipsWithWhom);
                _cancel = FindViewById<Button>(Resource.Id.btnCancel);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _linStructuredPlanRelationshipsMain = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanRelationshipsMain);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsDialogGetComponents), "StructuredPlanRelationshipsDialogActivity.GetFieldComponents");
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
                    string[] types = StringHelper.RelationshipList();

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
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsDialogSetTypeSpin), "StructuredPlanRelationshipsDialogActivity.SetupTypeSpinner");
                }
            }
            else
            {
                Log.Error(TAG, "SetupTypeSpinner: _type is NULL!");
            }
        }

        private void SetupFeelingSpinner()
        {
            Globals dataHelper = new Globals();
            if (_feeling != null)
            {
                try
                {
                    dataHelper.OpenDatabase();
                    if (dataHelper != null && dataHelper.GetSQLiteDatabase().IsOpen)
                    {
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
                    dataHelper.CloseDatabase();
                }
                catch (Exception e)
                {
                    if (dataHelper != null && dataHelper.GetSQLiteDatabase().IsOpen)
                        dataHelper.CloseDatabase();
                    Log.Error(TAG, "SetupFeelingSpinner: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsDialogSetFeelSpin), "StructuredPlanRelationshipsDialogActivity.SetupFeelingSpinner");
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
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsDialogSetActSpin), "StructuredPlanRelationshipsDialogActivity.SetupActionSpinner");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "StructuredPlanRelationshipsDialogActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "StructuredPlanRelationshipsDialogActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                if(_speakWith != null)
                    _speakWith.SetImageResource(Resource.Drawable.micgreyscale);
                if(_speakOf != null)
                    _speakOf.SetImageResource(Resource.Drawable.micgreyscale);

                if(_speakWith != null)
                    _speakWith.Enabled = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "StructuredPlanRelationshipsDialogActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "StructuredPlanRelationshipsDialogActivity.AttemptPermissionRequest");
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
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.View;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Speech;
using com.spanyardie.MindYourMood.SubActivities.Help.ThoughtsHelp;
using com.spanyardie.MindYourMood.Wizards;
using Android.Content.PM;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class SituationMainActivity : AppCompatActivity, ViewPager.IOnPageChangeListener, ISituationSpeakCallback, IAlertCallback
    {
        public const string TAG = "M:SituationMainActivity";

        private ViewPager _viewPager;
        private PagerTitleStrip _titleStrip;
        private Button _continue;

        private Toolbar _toolbar;

        private const int FIRST_PAGE = 0;
        private const int SECOND_PAGE = 1;
        private const int THIRD_PAGE = 2;
        private const int LAST_PAGE = 3;

        private int _currentPage = -1;

        private ThoughtRecordSituationPagerAdapter.SituationView _situationItemIndex;
        private string _spokenText = "";
        private bool _speakPermission = false;

        private ImageLoader _imageLoader = null;

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SituationMenu, menu);

            SetActionIcons(menu);

            return true;
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
            catch (Exception ex)
            {
                Log.Error(TAG, "DispatchKeyEvent: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSituationDispatchkeyEvent), "SituationMainActivity.DispatchKeyEvent");
            }
            return base.DispatchKeyEvent(e);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ThoughtRecordMain);

            GetFieldComponents();

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.situationMainToolbar, Resource.String.situationHeading, Color.White);

            _imageLoader = ImageLoader.Instance;

            _imageLoader.LoadImage
            (
                "drawable://" + Resource.Drawable.whatsituation,
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

            CheckMicPermission();

            UpdateAdapter();
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

                switch (item.ItemId)
                {
                    case Resource.Id.situationActionCancel:
                        Cancel();
                        return true;

                    case Resource.Id.situationActionNext:
                        Intent intent = new Intent(this, typeof(ThoughtRecordWizardMoodStep));
                        StartActivity(intent);
                        return true;

                    case Resource.Id.situationActionHelp:
                        Intent helpIntent = new Intent(this, typeof(SituationHelpActivity));
                        StartActivity(helpIntent);
                        return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SetupCallbacks()
        {
            if(_continue != null)
                _continue.Click += Continue_Click;

            _viewPager.SetOnPageChangeListener(this);
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemCancel = menu.FindItem(Resource.Id.situationActionCancel);
                var itemNext = menu.FindItem(Resource.Id.situationActionNext);
                var itemHelp = menu.FindItem(Resource.Id.situationActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_24dp);
                        if (itemNext != null)
                            itemNext.SetIcon(Resource.Drawable.ic_arrow_forward_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_36dp);
                        if (itemNext != null)
                            itemNext.SetIcon(Resource.Drawable.ic_arrow_forward_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_48dp);
                        if (itemNext != null)
                            itemNext.SetIcon(Resource.Drawable.ic_arrow_forward_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SituationMainActivity.SetActionIcons");
            }
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            var page = _currentPage;
            string message = "";
            //We must have values in each of the fields
            if (GlobalData.SituationItem.What.Trim() == "")
            {
                message = GetString(Resource.String.situationWhatIncompleteError);
                page = 0;
            }
            else
            {
                if (GlobalData.SituationItem.Who.Trim() == "")
                {
                    message = GetString(Resource.String.situationWhoIncompleteError);
                    page = 1;
                }
                else
                {
                    if (GlobalData.SituationItem.Where.Trim() == "")
                    {
                        message = GetString(Resource.String.situationWhereIncompleteError);
                        page = 2;
                    }
                    else
                    {
                        if (GlobalData.SituationItem.When.Trim() == "")
                        {
                            message = GetString(Resource.String.situationWhenIncompleteError);
                            page = 3;
                        }
                    }
                }
            }
            if (message.Trim() != "")
            {
                _situationItemIndex = (ThoughtRecordSituationPagerAdapter.SituationView)page;
                UpdateAdapter(message);
                _viewPager.SetCurrentItem(page, true);
                OnPageSelected((int)_situationItemIndex);
            }
            else
            {
                //moving on
                try
                {
                    StoreSituation();

                    //on to Automatic Thoughts
                    Intent intent = new Intent(this, typeof(ThoughtRecordWizardMoodStep));
                    StartActivity(intent);
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, "Next: Exception - " + ex.Message);
                    if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorNextButtonSituation), "SituationMainActivity.Next");
                }
            }
        }

        private void GetFieldComponents()
        {
            _viewPager = FindViewById<ViewPager>(Resource.Id.pagerThoughtRecordMain);
            _titleStrip = FindViewById<PagerTitleStrip>(Resource.Id.pagerTitleThoughtRecordMain);
            _continue = FindViewById<Button>(Resource.Id.btnContinue);
        }

        private void UpdateAdapter(string errorMessage = "")
        {
            ThoughtRecordSituationPagerAdapter adapter = new ThoughtRecordSituationPagerAdapter(this, _speakPermission, errorMessage, (errorMessage.Trim()==""? ThoughtRecordSituationPagerAdapter.SituationView.SituationWhat : _situationItemIndex));

            _viewPager.Adapter = adapter;
        }

        public void OnPageScrollStateChanged(int state)
        {
            
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            
        }

        public void OnPageSelected(int position)
        {
            int picResource = -1;
            if (_continue != null)
            {
                switch (position)
                {
                    case FIRST_PAGE:
                        _continue.Visibility = ViewStates.Invisible;
                        picResource = Resource.Drawable.whatsituation;
                        break;
                    case SECOND_PAGE:
                        _continue.Visibility = ViewStates.Invisible;
                        picResource = Resource.Drawable.whosituation;
                        break;
                    case THIRD_PAGE:
                        _continue.Visibility = ViewStates.Invisible;
                        picResource = Resource.Drawable.wheresituation;
                        break;
                    case LAST_PAGE:
                        _continue.Visibility = ViewStates.Visible;
                        picResource = Resource.Drawable.whensituation;
                        break;
                }

                if(picResource != -1)
                {
                    if(_viewPager != null)
                    {
                        _imageLoader = ImageLoader.Instance;

                        _imageLoader.LoadImage
                        (
                            "drawable://" + picResource,
                            new ImageLoadingListener
                            (
                                loadingComplete: (imageUri, view, loadedImage) =>
                                {
                                    var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                                    ImageLoader_LoadingComplete(null, args);
                                }
                            )
                        );
                    }
                }
            }

            _currentPage = position;
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_viewPager != null)
                _viewPager.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        public void SpeakSituationItem(int item)
        {
            _situationItemIndex = (ThoughtRecordSituationPagerAdapter.SituationView)item;
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            //TODO: Change the prompt below
            intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.AchievementSpeakPrompt));

            Log.Info(TAG, "SpeakSituationItem: Created intent, sending request...");
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
                        _spokenText = matches[0];
                        AssignSpokenText();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCheckingVoiceRecognition), "SituationMainActivity.OnActivityResult");
            }
        }

        private void AssignSpokenText()
        {
            switch(_situationItemIndex)
            {
                case ThoughtRecordSituationPagerAdapter.SituationView.SituationWhat:
                    GlobalData.SituationItem.What = _spokenText.Trim();
                    break;
                case ThoughtRecordSituationPagerAdapter.SituationView.SituationWhen:
                    GlobalData.SituationItem.When = _spokenText.Trim();
                    break;
                case ThoughtRecordSituationPagerAdapter.SituationView.SituationWhere:
                    GlobalData.SituationItem.Where = _spokenText.Trim();
                    break;
                case ThoughtRecordSituationPagerAdapter.SituationView.SituationWho:
                    GlobalData.SituationItem.Who = _spokenText.Trim();
                    break;
            }

            var thisPage = _currentPage;
            UpdateAdapter();
            _viewPager.SetCurrentItem(thisPage, true);
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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCancelSituationAddition), "SituationMainActivity.ShowCancelDialog");
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
                    if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRemovingSituation), "SituationMainActivity.AlertPositiveButtonSelect");
                }
            }

            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                _speakPermission = false; ;
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
                else
                {
                    _speakPermission = true;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "CheckMicPermission: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "SituationMainActivity.CheckMicPermission");
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
                        _speakPermission = true; ;
                    }
                    else
                    {
                        _speakPermission = false;
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                    var item =_currentPage;
                    UpdateAdapter();
                    _viewPager.SetCurrentItem(item, true);
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "SituationMainActivity.OnRequestPermissionsResult");
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "SituationMainActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "SituationMainActivity.AttemptPermissionRequest");
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
            catch (Exception e)
            {
                Log.Error(TAG, "StoreSituation: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStoringSituation), "SituationMainActivity.StoreSituation");
            }
        }
    }
}
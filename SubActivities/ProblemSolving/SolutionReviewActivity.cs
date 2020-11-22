using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using com.spanyardie.MindYourMood.Model;
using Android.Runtime;
using Android.Speech;
using System.Collections.Generic;
using Android.Views;
using Android.Graphics;
using Android.Content.PM;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment.ProblemSolving.Review;

namespace com.spanyardie.MindYourMood.SubActivities.ProblemSolving
{
    [Activity()]
    public class SolutionReviewActivity : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:SolutionReviewActivity";

        private TextView _problemIdea;
        private EditText _solutionReview;
        private RadioButton _achieved;
        private RadioButton _notAchieved;
        private DatePicker _achievedDate;
        private Toolbar _toolbar;
        private ImageButton _speakReviewText;

        private ProblemIdea _idea;
        private SolutionReview _review;

        private Button _done;

        private bool _firstTimeShow = true;
        private int _problemIdeaID;
        private int _solutionReviewID;
        private bool _isDirty = false;

        public SolutionReviewActivity()
        {
            _problemIdeaID = -1;
            _solutionReviewID = -1;
            _isDirty = false;
            _idea = null;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("problemIdeaID", _problemIdeaID);
                outState.PutInt("solutionReviewID", _solutionReviewID);
                outState.PutBoolean("isDirty", _isDirty);
            }

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SolutionReviewMenu, menu);

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
                    _firstTimeShow = true;
                    _problemIdeaID = savedInstanceState.GetInt("problemIdeaID");
                    _solutionReviewID = savedInstanceState.GetInt("solutionReviewID");
                    _isDirty = savedInstanceState.GetBoolean("isDirty");
                }
                else
                {
                    _problemIdeaID = Intent.GetIntExtra("problemIdeaID", -1);
                }

                GetIdeaData();
                GetSolutionReviewData();

                SetContentView(Resource.Layout.SolutionReviewActivityLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.solutionReviewToolbar, Resource.String.SolutionIdeasReviewToolbarTitle, Color.White);

                GetFieldComponents();

                CheckMicPermission();

                SetupCallbacks();

                if (_firstTimeShow)
                {
                    if (_idea != null)
                    {
                        if (_problemIdea != null)
                        {
                            _problemIdea.Text = _idea.ProblemIdeaText.Trim();
                        }
                        else
                        {
                            Log.Error(TAG, "OnCreate: _problemIdea is NULL!");
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "OnCreate: _idea is NULL!");
                    }

                    if(_review != null)
                    {
                        if (_solutionReviewID != -1)
                        {
                            if (_solutionReview != null)
                            {
                                _solutionReview.Text = _review.ReviewText.Trim();
                            }
                            else
                            {
                                Log.Error(TAG, "OnCreate: _solutionReview is NULL!");
                            }
                            if (_achieved != null && _notAchieved != null)
                            {
                                if (_review.Achieved)
                                {
                                    _achieved.Checked = true;
                                    _notAchieved.Checked = false;
                                }
                                else
                                {
                                    _achieved.Checked = false;
                                    _notAchieved.Checked = true;
                                }
                            }
                            else
                            {
                                Log.Error(TAG, "OnCreate: _achieved or _notAchieved is NULL!");
                            }
                            if (_achievedDate != null)
                            {
                                _achievedDate.DateTime = _review.AchievedDate;
                            }
                            else
                            {
                                Log.Error(TAG, "OnCreate: _achievedDate is NULL!");
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "OnCreate: _review is NULL!");
                    }
                    _firstTimeShow = false;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSolutionReviewActivityCreateView), "SolutionReviewActivity.OnCreate");
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
                    case Resource.Id.solutionreviewActionAdd:
                        Save();
                        return true;
                    case Resource.Id.solutionReviewActionHelp:
                        Intent intent = new Intent(this, typeof(ProblemSolvingReviewHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void GetIdeaData()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                if(dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    _idea = dbHelp.GetIdea(_problemIdeaID);
                    dbHelp.CloseDatabase();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetIdeaData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSolutionReviewActivityGetData), "SolutionReviewActivity.GetIdeaData");
            }
        }

        private void GetSolutionReviewData()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                if (dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    Log.Info(TAG, "GetSolutionReviewData: Attempting to get review data for idea ID " + _problemIdeaID.ToString());
                    _review = dbHelp.GetSolutionReviewForIdea(_problemIdeaID);
                    dbHelp.CloseDatabase();

                    if (_review != null)
                    {
                        Log.Info(TAG, "GetSolutionReviewData: Found review!");
                        _solutionReviewID = _review.SolutionReviewID;
                    }
                    else
                    {
                        Log.Info(TAG, "GetSolutionReviewData: No review found!");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetIdeaData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSolutionReviewActivityGetSolutionData), "SolutionReviewActivity.GetSolutionPlanData");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_solutionReview != null)
                {
                    _solutionReview.TextChanged += SolutionReview_TextChanged;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _solutionReview is NULL!");
                }
                if (_achieved != null)
                {
                    _achieved.Click += Achieved_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _achieved is NULL!");
                }
                if (_notAchieved != null)
                {
                    _notAchieved.Click += Achieved_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _notAchieved is NULL!");
                }
                if(_speakReviewText != null)
                    _speakReviewText.Click += SpeakReviewText_Click;
                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSolutionReviewActivitySetCallbacks), "SolutionReviewActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void SpeakReviewText_Click(object sender, EventArgs e)
        {
            SpeakToMYM("Provide a Review of this Solution...");
        }

        private void SpeakToMYM(string message)
        {
            try
            {
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, message);

                Log.Info(TAG, "SpeakToMYM: Created intent, sending request...");
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SpeakToMYM: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Attempting Voice Recognition", "SolutionReviewActivity.SpeakToMYM");
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
                    _solutionReview.Text = matches[0];
                }
            }
        }

        private void Save()
        {
            try
            {
                if (_solutionReviewID == -1)
                {
                    SolutionReview review = new SolutionReview();
                    review.Achieved = _achieved.Checked;
                    review.AchievedDate = _achievedDate.DateTime;
                    review.IsNew = true;
                    review.IsDirty = false;
                    review.ProblemIdeaID = _problemIdeaID;
                    review.ReviewText = _solutionReview.Text.Trim();
                    review.Save();
                }
                else
                {
                    if (_isDirty)
                    {
                        _review.Achieved = _achieved.Checked;
                        _review.AchievedDate = _achievedDate.DateTime;
                        _review.IsNew = false;
                        _review.IsDirty = true;
                        _review.ProblemIdeaID = _problemIdeaID;
                        _review.ReviewText = _solutionReview.Text.Trim();
                        _review.Save();
                    }
                }

                Finish();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Save_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSolutionReviewActivitySave), "SolutionReviewActivity.Save_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.solutionreviewActionAdd);
                var itemHelp = menu.FindItem(Resource.Id.solutionReviewActionHelp);
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SolutionReviewActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Finish();
        }

        private void Achieved_Click(object sender, EventArgs e)
        {
            if (!_firstTimeShow)
                _isDirty = true;
        }

        private void SolutionReview_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if(!_firstTimeShow)
                _isDirty = true;
        }

        private void GetFieldComponents()
        {
            try
            {
                _problemIdea = FindViewById<TextView>(Resource.Id.txtSolutionReviewIdeaText);
                _solutionReview = FindViewById<EditText>(Resource.Id.edtSolutionReviewText);
                _achieved = FindViewById<RadioButton>(Resource.Id.radbtnSolutionReviewAchieved);
                _notAchieved = FindViewById<RadioButton>(Resource.Id.radbtnSolutionReviewNotAchieved);
                _achievedDate = FindViewById<DatePicker>(Resource.Id.dtpSolutionReviewAchievedStopped);
                _speakReviewText = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakReview);
                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSolutionReviewActivityGetComponents), "SolutionReviewActivity.GetFieldComponents");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "SolutionReviewActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "SolutionReviewActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                if (_speakReviewText != null)
                {
                    _speakReviewText.SetImageResource(Resource.Drawable.micgreyscale);

                    _speakReviewText.Enabled = false;
                }
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
                Log.Info(TAG, "ShowPermissionRationale: AlertTitle - " + alertHelper.AlertTitle);
                alertHelper.InstanceId = "useMic";
                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ShowPermissionRationale: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "SolutionReviewActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "SolutionReviewActivity.AttemptPermissionRequest");
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
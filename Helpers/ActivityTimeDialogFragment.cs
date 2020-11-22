using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Speech;
using Android.Runtime;
using System.Collections.Generic;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ActivityTimeDialogFragment : DialogFragment
    {
        public const string TAG = "M:ActivityTimeDialogFragment";

        private View _thisView;

        private Activity _activity;

        private Button _goBack;
        private Button _add;
        private EditText _activityName;
        private SeekBar _achievement;
        private SeekBar _intimacy;
        private SeekBar _pleasure;

        private TextView _achievementPercent;
        private TextView _intimacyPercent;
        private TextView _pleasurePercent;

        private TextView _dayName;
        private TextView _date;
        private TextView _startTime;
        private TextView _endTime;
        private ConstantsAndTypes.ACTIVITY_HOURS _activityHours;

        private int _groupPosition;
        private int _childPosition;
        private string _dateString;
        private string _startString;
        private string _endString;

        private bool _initialising = false;

        private string _dialogTitle = "";

        private ImageButton _speakActivityName;
        private bool _spokenActivityName = false;
        private string _spokenText = "";

        public ActivityTimeDialogFragment()
        {

        }

        public ActivityTimeDialogFragment(Activity activity, DateTime date, ConstantsAndTypes.ACTIVITY_HOURS activityHours, string startTime, string endTime, int groupPosition, int childPosition, string title)
        {
            _activity = activity;
            InitialiseFragment(date, startTime, endTime, activityHours, groupPosition, childPosition);
            _dialogTitle = title;
        }

        private void InitialiseFragment(DateTime date, string startTime, string endTime, ConstantsAndTypes.ACTIVITY_HOURS activityHours, int groupPosition, int childPosition)
        {
            _dateString = date.ToShortDateString();
            _startString = startTime;
            _endString = endTime;
            _activityHours = activityHours;
            _groupPosition = groupPosition;
            _childPosition = childPosition;
        }

        private void SaveValues(Bundle outState)
        {
            outState.PutString("date", _dateString);
            outState.PutString("startTime", _startString);
            outState.PutString("endTime", _endString);
            outState.PutInt("activityHours", (int)_activityHours);
            outState.PutInt("groupPosition", _groupPosition);
            outState.PutInt("childPosition", _childPosition);
            outState.PutString("dialogTitle", _dialogTitle);
        }

        private void RestoreValues(Bundle savedInstanceState)
        {
            _dateString = savedInstanceState.GetString("date");
            _startString = savedInstanceState.GetString("startTime");
            _endString = savedInstanceState.GetString("endTime");
            _activityHours = (ConstantsAndTypes.ACTIVITY_HOURS)savedInstanceState.GetInt("activityHours");
            _groupPosition = savedInstanceState.GetInt("groupPosition");
            _childPosition = savedInstanceState.GetInt("childPosition");
            _dialogTitle = savedInstanceState.GetString("dialogTitle");
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                SaveValues(outState);

            base.OnSaveInstanceState(outState);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Following style setting resolves a weird bug with the fragment
            //when used from an ItemLongClick in an ExpandableListView, this
            //gives it full width instead of weird shrinking effect
            SetStyle(DialogFragmentStyle.NoTitle, Android.Resource.Style.ThemeHoloLightDialogNoActionBarMinWidth);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            if (context != null)
                _activity = (Activity)context;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if(savedInstanceState != null)
                    RestoreValues(savedInstanceState);

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                _thisView = inflater.Inflate(Resource.Layout.ActivityTimeFragment, container, false);

                if (_thisView != null)
                {
                    _initialising = true;

                    GetFieldComponents();
                    HandleMicPermission();

                    SetupCallbacks();

                    Activities activity = GlobalData.ActivitiesForWeek[_groupPosition];
                    ActivityTime activityTime = activity.ActivityTimes[_childPosition];
                    if (_dayName != null)
                    {
                        var mymDay = ConversionHelper.ConvertFromDateTimeDaysToMYMDays(activity.ActivityDate.DayOfWeek);
                        _dayName.Text = StringHelper.DayStringForConstant(mymDay);
                    }
                    if (_date != null)
                    {
                        _date.Text = activity.ActivityDate.ToShortDateString();
                    }
                    if (_startTime != null)
                    {
                        _startTime.Text = _startString.Trim();
                    }
                    if (_endTime != null)
                    {
                        _endTime.Text = _endString.Trim();
                    }
                    if (activityTime.ActivityTimeID != -1)
                    {
                        if (_activityName != null)
                        {
                            _activityName.Text = activityTime.ActivityName.Trim();
                        }
                        if (_achievement != null)
                            _achievement.Progress = activityTime.Achievement;
                        if (_achievementPercent != null)
                            _achievementPercent.Text = activityTime.Achievement.ToString();
                        if (_intimacy != null)
                            _intimacy.Progress = activityTime.Intimacy;
                        if (_intimacyPercent != null)
                            _intimacyPercent.Text = activityTime.Intimacy.ToString();
                        if (_pleasure != null)
                            _pleasure.Progress = activityTime.Pleasure;
                        if (_pleasurePercent != null)
                            _pleasurePercent.Text = activityTime.Pleasure.ToString();

                        if (_add != null)
                            _add.Text = _activity.GetString(Resource.String.wordAcceptUpper);
                    }
                    else
                    {
                        if (_add != null)
                            _add.Text = _activity.GetString(Resource.String.wordAddUpper);
                    }
                    _initialising = false;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, GetString(Resource.String.ErrorActivityTimeFragCreateView), "ActivityTimeDialogFragment.OnCreateView");
            }
            return _thisView;
        }

        private void GetFieldComponents()
        {
            try
            {
                if(_thisView != null)
                {
                    _goBack = _thisView.FindViewById<Button>(Resource.Id.btnActivityTimeFragmentGoBack);
                    _add = _thisView.FindViewById<Button>(Resource.Id.btnActivityTimeFragmentSave);
                    _activityName = _thisView.FindViewById<EditText>(Resource.Id.edtActivityTimeFragmentName);
                    _achievement = _thisView.FindViewById<SeekBar>(Resource.Id.skbActivityTimeFragmentAchievement);
                    _intimacy = _thisView.FindViewById<SeekBar>(Resource.Id.skbActivityTimeFragmentIntimacy);
                    _pleasure = _thisView.FindViewById<SeekBar>(Resource.Id.skbActivityTimeFragmentPleasure);
                    _dayName = _thisView.FindViewById<TextView>(Resource.Id.txtActivityTimeFragmentDayText);
                    _date = _thisView.FindViewById<TextView>(Resource.Id.txtActivityTimeFragmentDateText);
                    _startTime = _thisView.FindViewById<TextView>(Resource.Id.txtActivityTimeFragmentStartTime);
                    _endTime = _thisView.FindViewById<TextView>(Resource.Id.txtActivityTimeFragmentEndTime);
                    _speakActivityName = _thisView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakActivityName);

                    _achievementPercent = _thisView.FindViewById<TextView>(Resource.Id.txtActivityAchievementPercent);
                    _intimacyPercent = _thisView.FindViewById<TextView>(Resource.Id.txtActivityIntimacyPercent);
                    _pleasurePercent = _thisView.FindViewById<TextView>(Resource.Id.txtActivityPleasurePercent);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: _thisView is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, GetString(Resource.String.ErrorActivityTimeFragGetComponents), "ActivityTimeDialogFragment.OnCreateView");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_goBack != null)
                {
                    _goBack.Click += GoBack_Click;
                }
                if(_add != null)
                {
                    _add.Click += Add_Click;
                }
                if(_achievement != null)
                {
                    _achievement.ProgressChanged += Achievement_ProgressChanged;
                }
                if(_intimacy != null)
                {
                    _intimacy.ProgressChanged += Intimacy_ProgressChanged;
                }
                if(_pleasure != null)
                {
                    _pleasure.ProgressChanged += Pleasure_ProgressChanged;
                }
                if(_speakActivityName != null)
                    _speakActivityName.Click += SpeakActivityName_Click;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, GetString(Resource.String.ErrorActivityTimeFragCallbacks), "ActivityTimeDialogFragment.SetupCallbacks");
            }
        }

        private void SpeakActivityName_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.ActivityTimeSpeakNamePrompt));

            Log.Info(TAG, "SpeakEvidenceFor_Click: Created intent, sending request...");
            StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == ConstantsAndTypes.VOICE_RECOGNITION_REQUEST && resultCode == Result.Ok)
                {
                    IList<string> matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches != null)
                    {
                        _spokenActivityName = true;
                        _spokenText = matches[0];
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCheckingVoiceRecognition), "ActivityTimeDialogFragment.OnActivityResult");
            }
        }

        public override void OnResume()
        {
            if (Dialog != null)
            {
                int width = LinearLayout.LayoutParams.MatchParent;
                int height = LinearLayout.LayoutParams.WrapContent;

                Dialog.Window.SetLayout(width, height);
            }

            base.OnResume();

            if (_spokenActivityName)
            {
                if (_activityName != null)
                    _activityName.Text = _spokenText;
                _spokenActivityName = false;
            }
        }

        private void Pleasure_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if(!_initialising)
            {
                GlobalData.ActivitiesForWeek[_groupPosition].ActivityTimes[_childPosition].IsDirty = true;
                if (_pleasurePercent != null)
                    _pleasurePercent.Text = _pleasure.Progress.ToString();
            }
        }

        private void Intimacy_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (!_initialising)
            {
                GlobalData.ActivitiesForWeek[_groupPosition].ActivityTimes[_childPosition].IsDirty = true;
                if (_intimacyPercent != null)
                    _intimacyPercent.Text = _intimacy.Progress.ToString();
            }
        }

        private void Achievement_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (!_initialising)
            {
                GlobalData.ActivitiesForWeek[_groupPosition].ActivityTimes[_childPosition].IsDirty = true;
                if (_achievementPercent != null)
                    _achievementPercent.Text = _achievement.Progress.ToString();
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_activityName.Text))
                {
                    _activityName.Error = Activity.GetString(Resource.String.ActivityTimeFragmentAddToast);
                    return;
                }

                //get the activity
                Activities activity = GlobalData.ActivitiesForWeek[_groupPosition];
                if(activity.ActivityID == -1)
                {
                    activity.IsNew = true;
                    activity.IsDirty = false;
                    Log.Info(TAG, "Add_Click: Activity ID is -1, setting IsNew, resetting IsDirty");
                }
                else
                {
                    Log.Info(TAG, "Add_Click: Activity ID is valid - " + activity.ActivityID.ToString() + ", set IsDirty, reset IsNew");
                    activity.IsDirty = true;
                    activity.IsNew = false;
                }

                ActivityTime activityTime;
                Log.Info(TAG, "Add_Click: Created ActivityTime...");
                if (activity.ActivityTimes[_childPosition].ActivityTimeID == -1)
                {
                    activityTime = new ActivityTime();
                    activityTime.IsNew = true;
                    activityTime.IsDirty = false;
                    Log.Info(TAG, "Add_Click: ActivityTime is new - set IsNew, reset IsDirty");
                }
                else
                {
                    activityTime = activity.ActivityTimes[_childPosition];
                    activityTime.IsNew = false;
                    activityTime.IsDirty = true;
                    Log.Info(TAG, "Add_Click: Existing ActivityTime ID - " + activityTime.ActivityTimeID.ToString() + ", set IsDirty, reset IsNew");
                }
                Log.Info(TAG, "Add_Click: ActivityTime has been defined as a" + (activityTime.IsNew ? " new" : "n existing") + " ActivityTime");
                activityTime.ActivityID = activity.ActivityID;
                Log.Info(TAG, "Add_Click: Set ActivityTime ActivityID to " + activityTime.ActivityID.ToString());
                activityTime.ActivityName = _activityName.Text.Trim();
                Log.Info(TAG, "Add_Click: ActivityName - " + activityTime.ActivityName);
                activityTime.ActivityTime = _activityHours;
                Log.Info(TAG, "Add_Click: ActivityTime - " + StringHelper.ActivityTimeForConstant(_activityHours));
                activityTime.Achievement = _achievement.Progress;
                activityTime.Intimacy = _intimacy.Progress;
                activityTime.Pleasure = _pleasure.Progress;
                Log.Info(TAG, "Add_Click: Achievement - " + activityTime.Achievement.ToString() + ", Intimacy - " + activityTime.Intimacy.ToString() + ", Pleasure - " + activityTime.Pleasure.ToString());

                activity.ActivityTimes[_childPosition] = activityTime;
                Log.Info(TAG, "Add_Click: Set Activity Time at position " + _childPosition.ToString());
                GlobalData.ActivitiesForWeek[_groupPosition] = activity;
                Log.Info(TAG, "Add_Click: Set Activity in GlobalData at position " + _groupPosition.ToString());
                Log.Info(TAG, "Add_Click: Calling CompletedActivityTime with groupPosition - " + _groupPosition.ToString() + ", childPosition - " + _childPosition.ToString());
                ((IActivityTimeCallback)Activity).CompletedActivityTime(_groupPosition, _childPosition);

                Dismiss();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, GetString(Resource.String.ErrorActivityTimeFragAmendingActivityTime), "ActivityTimeDialogFragment.Add_Click");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakActivityName != null)
                {
                    _speakActivityName.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakActivityName.Enabled = false;
                }
            }
        }
    }
}
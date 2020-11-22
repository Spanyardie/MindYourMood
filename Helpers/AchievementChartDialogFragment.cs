using System;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Adapters;
using Android.Content;
using Android.Speech;
using System.Collections.Generic;
using Android.Runtime;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class AchievementChartDialogFragment : DialogFragment
    {

        public static string TAG = "M:AchievementChartDialogFragment";
        
        private View _thisView;

        private Button _cancelButton;
        private Button _confirmButton;
        private EditText _achievement;
        private Spinner _achievementType;

        public string Achievement { get; set; }

        private Activity _parentActivity;

        private int _achievementID = -1;
        private int _selectedItemIndex = -1;

        private AchievementChart _achievementChart = null;

        private string _dialogTitle = "";

        private ImageButton _speakAchievement;
        private bool _spokenAchievement = false;
        private string _spokenText = "";

        public AchievementChartDialogFragment()
        {

        }

        public AchievementChartDialogFragment(Activity activity, string title, int acievementID = -1)
        {
            _parentActivity = activity;
            _achievementID = acievementID;

            if (_achievementID != -1)
                GetAchievement();
            _dialogTitle = title;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutInt("achievementID", _achievementID);
                outState.PutInt("selectedItemIndex", _selectedItemIndex);
                outState.PutString("dialogTitle", _dialogTitle);
            }

            base.OnSaveInstanceState(outState);
        }

        private void GetAchievement()
        {
            _achievementChart = GlobalData.AchievementChartItems.Find(chart => chart.AchievementId == _achievementID);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if (savedInstanceState != null)
                {
                    _achievementID = savedInstanceState.GetInt("achievementID");
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                _thisView = inflater.Inflate(Resource.Layout.AchievementChartFragment, container, false);

                GetFieldComponents();

                SetupCallbacks();

                HandleMicPermission();

                if (_achievementID != -1)
                    _achievement.Text = _achievementChart.Achievement.Trim();

                UpdateAdapter();

                if (_selectedItemIndex != -1)
                    _achievementType.SetSelection(_selectedItemIndex);

                return _thisView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCreatingChuffChartDialog), "AchievementChartDialogFragment.OnCreateView");
                return null;
            }
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            _parentActivity = (Activity)context;
        }

        private void GetFieldComponents()
        {
            if(_thisView != null)
            {
                _cancelButton = _thisView.FindViewById<Button>(Resource.Id.btnCancel);
                _confirmButton = _thisView.FindViewById<Button>(Resource.Id.btnConfirm);
                _achievement = _thisView.FindViewById<EditText>(Resource.Id.edtAchievement);
                _achievementType = _thisView.FindViewById<Spinner>(Resource.Id.spnAchievementType);
                _speakAchievement = _thisView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakAchievement);
            }
        }

        private void SetupCallbacks()
        {
            if(_cancelButton != null)
                _cancelButton.Click += CancelButton_Click;
            if(_confirmButton != null)
                _confirmButton.Click += ConfirmButton_Click;
            if(_achievementType != null)
                _achievementType.ItemSelected += AchievementType_ItemSelected;
            if(_speakAchievement != null)
                _speakAchievement.Click += SpeakAchievement_Click;
        }

        private void SpeakAchievement_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.AchievementSpeakPrompt));

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
                        _spokenAchievement = true;
                        _spokenText = matches[0];
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCheckingVoiceRecognition), "AchievementChartDialogFragment.OnActivityResult");
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (_spokenAchievement)
            {
                if (_achievement != null)
                    _achievement.Text = _spokenText;
                _spokenAchievement = false;
            }
        }

        private void AchievementType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _selectedItemIndex = e.Position;
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = -1;

                if (_achievement != null)
                    Achievement = _achievement.Text.Trim();
                if (_achievementType != null)
                {
                    selectedIndex = _achievementType.SelectedItemPosition;
                }

                if (Activity != null && !string.IsNullOrEmpty(Achievement))
                {
                    ((AchievementChartActivity)Activity).ConfirmClicked(_achievementID, Achievement, (AchievementChart.ACHIEVEMENTCHART_TYPE)selectedIndex);
                }
                Dismiss();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ConfirmButton_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorAddingChuffChartItem), "AchievementChartDialogFragment.ConfirmButton_Click");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Activity != null)
                {
                    ((AchievementChartActivity)Activity).CancelClicked();
                }
                Dismiss();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "CancelButton_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorCancellingChuffChartItem), "AchievementChartDialogFragment.CancelButton_Click");
            }
        }

        public override void Dismiss()
        {
            base.Dismiss();
        }

        private void UpdateAdapter()
        {
            try
            {
                Log.Info(TAG, "UpdateAdapter: Entered UpdateAdapter...");
                if (_achievementType != null)
                {
                    Log.Info(TAG, "UpdateAdapter: Creating AchievementChartTypesAdapter with Activity");
                    AchievementChartTypesAdapter adapter = new AchievementChartTypesAdapter(Activity, Resource.Layout.AchievementChartTypeListItem, Resource.Id.txtSpinnerSelected, new List<string>());

                    adapter.SetDropDownViewResource(Resource.Layout.AchievementChartTypeListItem);

                    _achievementType.Adapter = adapter;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, "Updating Adapter", "AchievementChartDialogFragment.UpdateAdapter");
            }
        }

        private void HandleMicPermission()
        {
            if(!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if(_speakAchievement != null)
                {
                    _speakAchievement.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakAchievement.Enabled = false;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Speech;
using com.spanyardie.MindYourMood.SubActivities.Resources;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class AppointmentQuestionDialogFragment : DialogFragment
    {
        public static string TAG = "M:AppointmentQuestionDialogFragment";

        private View _thisView;

        private EditText _question;
        private ImageButton _speakQuestion;
        private Button _goBack;
        private Button _Save;

        private int _appointmentID = -1;

        private bool _spokenQuestion = false;
        private string _spokenText;

        private Activity _activity;

        public AppointmentQuestionDialogFragment()
        {

        }

        public AppointmentQuestionDialogFragment(Activity activity, int appointmentID)
        {
            _activity = activity;
            _appointmentID = appointmentID;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("appointmentID", _appointmentID);
            }

            base.OnSaveInstanceState(outState);
        }

       
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if(savedInstanceState != null)
                {
                    _appointmentID = savedInstanceState.GetInt("appointmentID");
                }

                _thisView = inflater.Inflate(Resource.Layout.AppointmentQuestionDialogLayout, container, false);

                GetFieldComponents();
                HandleMicPermission();

                SetupCallbacks();

                if(_appointmentID == -1)
                {
                    if (_Save != null)
                        _Save.Text = _activity.GetString(Resource.String.wordAddUpper);
                }
                else
                {
                    if (_Save != null)
                        _Save.Text = _activity.GetString(Resource.String.wordAcceptUpper);
                }
                return _thisView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCreatingAppointmentQuestionDialog), "AppointmentQuestionDialogFragment.OnCreateView");
                return null;
            }
        }

        private void SetupCallbacks()
        {
            if(_speakQuestion != null)
                _speakQuestion.Click += SpeakQuestion_Click;
            if(_goBack != null)
                _goBack.Click += GoBack_Click;
            if(_Save != null)
                _Save.Click += Save_Click;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if(_activity != null)
            {
                ((ResourcesAppointmentItemActivity)_activity).QuestionAdded(_appointmentID, _question.Text.Trim());
            }

            Dismiss();
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            if (_activity != null)
                ((ResourcesAppointmentItemActivity)_activity).NoChanges();
            Dismiss();
        }

        private void SpeakQuestion_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.AppointmentQuestionSpeakTitle));

            Log.Info(TAG, "SpeakQuestion_Click: Created intent, sending request...");
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
                        _spokenQuestion = true;
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

            if (_spokenQuestion)
            {
                if (_question != null)
                    _question.Text = _spokenText;
                _spokenQuestion = false;
            }
        }

        private void GetFieldComponents()
        {
            _question = _thisView.FindViewById<EditText>(Resource.Id.edtAppointmentQuestion);
            _speakQuestion = _thisView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakQuestion);
            _goBack = _thisView.FindViewById<Button>(Resource.Id.btnAppointmentQuestionGoBack);
            _Save = _thisView.FindViewById<Button>(Resource.Id.btnAppointmentQuestionSave);
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakQuestion != null)
                {
                    _speakQuestion.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakQuestion.Enabled = false;
                }
            }
        }
    }
}
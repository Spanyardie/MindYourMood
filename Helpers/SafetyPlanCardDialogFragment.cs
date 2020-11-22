using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using Android.Util;
using Android.Content;
using Android.Speech;
using Android.Runtime;
using System.Collections.Generic;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class SafetyPlanCardDialogFragment : DialogFragment
    {
        public const string TAG = "M:SafetyPlanCardDialogFragment ";

        private View _thisView;

        private Button _cancelButton;
        private Button _confirmButton;

        private EditText _calmMyself;
        private EditText _tellMyself;
        private EditText _willCall;
        private EditText _willGoTo;

        private Activity _parentActivity;

        private ImageButton _speakCalm;
        private ImageButton _speakTell;
        private ImageButton _speakCall;
        private ImageButton _speakGo;

        private int _currentSpeakType;

        public SafetyPlanCardDialogFragment() { }

        private string _dialogTitle = "";

        public SafetyPlanCardDialogFragment(Activity activity, string title)
        {
            _parentActivity = activity;
            _dialogTitle = title;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            _parentActivity = (Activity)context;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutString("dialogTitle", _dialogTitle);
                outState.PutInt("currentSpeakType", _currentSpeakType);
            }

            base.OnSaveInstanceState(outState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if (savedInstanceState != null)
                {
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                    _currentSpeakType = savedInstanceState.GetInt("currentSpeakType", -1);
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                _thisView = inflater.Inflate(Resource.Layout.SafetyPlanCardDialogFragmentLayout, container, false);

                GetFieldComponents();
                HandleMicPermission();

                SetupCallbacks();

                return _thisView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCreatingSafetyPlanCardDialog), "SafetyPlanCardDialogFragment.OnCreateView");
                return null;
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_cancelButton != null)
                    _cancelButton.Click += CancelButton_Click;
                if (_confirmButton != null)
                    _confirmButton.Click += ConfirmButton_Click;
                if(_speakCall != null)
                    _speakCall.Click += SpeakCall_Click;
                if(_speakCalm != null)
                    _speakCalm.Click += SpeakCalm_Click;
                if(_speakGo != null)
                    _speakGo.Click += SpeakGo_Click;
                if(_speakTell != null)
                    _speakTell.Click += SpeakTell_Click;
                Log.Info(TAG, "SetupCallbacks: Completed successfully");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorSettingSafetyPlanCardCallbacks), "SafetyPlanCardDialogFragment.SetupCallbacks");
            }
        }

        private void SpeakTell_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_TELL, "Tell yourself...");
        }

        private void SpeakGo_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_GO, "Where will you go?");
        }

        private void SpeakCalm_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_CALM, "How will you remain calm?");
        }

        private void SpeakCall_Click(object sender, EventArgs e)
        {
            SpeakToMYM(ConstantsAndTypes.SPEAK_CALL, "Who will you call?");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_parentActivity, e, "Attempting Voice Recognition", "SafetyPlanCardDialogFragment.SpeakToMYM");
            }
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == ConstantsAndTypes.VOICE_RECOGNITION_REQUEST && resultCode == Result.Ok)
            {
                IList<string> matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);

                if (matches != null)
                {
                    switch (_currentSpeakType)
                    {
                        case ConstantsAndTypes.SPEAK_CALL:
                            if (_willCall != null)
                                _willCall.Text = matches[0];
                            break;
                        case ConstantsAndTypes.SPEAK_CALM:
                            if (_calmMyself != null)
                                _calmMyself.Text = matches[0];
                            break;
                        case ConstantsAndTypes.SPEAK_GO:
                            if (_willGoTo != null)
                                _willGoTo.Text = matches[0];
                            break;
                        case ConstantsAndTypes.SPEAK_TELL:
                            if (_tellMyself != null)
                                _tellMyself.Text = matches[0];
                            break;
                    }
                }
            }
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            var isValid = ValidateFields();

            Log.Info(TAG, "ConfirmButton_Click: ValidateFields returned " + (isValid ? "True" : "False"));

            try
            {
                if (isValid)
                {
                    string calmMyself = "", tellMyself = "", willCall = "", willGoTo = "";
                    if (_calmMyself != null)
                        calmMyself = _calmMyself.Text.Trim();
                    if (_tellMyself != null)
                        tellMyself = _tellMyself.Text.Trim();
                    if (_willCall != null)
                        willCall = _willCall.Text.Trim();
                    if (_willGoTo != null)
                        willGoTo = _willGoTo.Text.Trim();

                    if (Activity != null)
                    {
                        Log.Info(TAG, "ConfirmButton_Click: Calling Parent activity with - '" + calmMyself + "', '" + tellMyself + "', '" + willCall + "', '" + willGoTo + "'");
                        ((SafetyPlanCardsActivity)Activity).ConfirmCard(calmMyself, tellMyself, willCall, willGoTo);
                    }
                    Dismiss();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ConfirmButton_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorAddingSafetyPlanCard), "SafetyPlanCardDialogFragment.ConfirmButton_Click");
            }
        }

        private bool ValidateFields()
        {
            if(string.IsNullOrEmpty(_calmMyself.Text.Trim()))
            {
                _calmMyself.Error = GetString(Resource.String.SafetyPlanDialogCalmMyselfError);
                return false;
            }
            if(string.IsNullOrEmpty(_tellMyself.Text.Trim()))
            {
                _tellMyself.Error = GetString(Resource.String.SafetyPlanDialogTellMyselfError);
                return false;
            }
            if(string.IsNullOrEmpty(_willCall.Text.Trim()))
            {
                _willCall.Error = GetString(Resource.String.SafetyPlanDialogCallError);
                return false;
            }
            if(string.IsNullOrEmpty(_willGoTo.Text.Trim()))
            {
                _willGoTo.Error = GetString(Resource.String.SafetyPlanDialogGoToError);
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Activity != null)
                {
                    Log.Info(TAG, "CancelButton_Click: Calling parent activity Cancel");
                    ((SafetyPlanCardsActivity)Activity).CancelCard();
                }
                Dismiss();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "CancelButton_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorCancellingSafetyPlanCardAddition), "SafetyPlanCardDialogFragment.CancelButton_Click");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                if (_thisView != null)
                {
                    _cancelButton = _thisView.FindViewById<Button>(Resource.Id.btnBack);
                    _confirmButton = _thisView.FindViewById<Button>(Resource.Id.btnAdd);
                    _calmMyself = _thisView.FindViewById<EditText>(Resource.Id.edtCalmMyself);
                    _tellMyself = _thisView.FindViewById<EditText>(Resource.Id.edtTellMyself);
                    _willCall = _thisView.FindViewById<EditText>(Resource.Id.edtWillCall);
                    _willGoTo = _thisView.FindViewById<EditText>(Resource.Id.edtWillGoTo);
                    _speakCalm = _thisView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakCalm);
                    _speakTell = _thisView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakTell);
                    _speakCall = _thisView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakCall);
                    _speakGo = _thisView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakGo);
        Log.Info(TAG, "GetFieldComponents: Completed succesffully");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorGettingSafetyPlanCardComponents), "SafetyPlanCardDialogFragment.GetFieldComponents");
            }
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakCalm != null)
                {
                    _speakCalm.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakCalm.Enabled = false;
                }
                if (_speakTell != null)
                {
                    _speakTell.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakTell.Enabled = false;
                }
                if (_speakCall != null)
                {
                    _speakCall.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakCall.Enabled = false;
                }
                if (_speakGo != null)
                {
                    _speakGo.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakGo.Enabled = false;
                }
            }
        }
    }
}
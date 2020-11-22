using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Speech;
using System.Collections.Generic;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ProblemSolvingDialogFragment : DialogFragment
    {
        public const string TAG = "M:ProblemSolvingDialogFragment";

        Activity _activity;

        private EditText _problemText;
        private Button _goBack;
        private Button _add;

        private ImageButton _speakProblemText;

        private int _problemID = -1;

        private string _dialogTitle = "";

        public ProblemSolvingDialogFragment()
        {

        }

        public ProblemSolvingDialogFragment(Activity activity, string title, int problemID = -1)
        {
            _activity = activity;
            _problemID = problemID;
            _dialogTitle = title;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutInt("problemID", _problemID);
                outState.PutString("dialogTitle", _dialogTitle);
            }

            base.OnSaveInstanceState(outState);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            if (context != null)
                _activity = (Activity)context;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = null;

            try
            {
                if (savedInstanceState != null)
                {
                    _problemID = savedInstanceState.GetInt("problemID");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                view = inflater.Inflate(Resource.Layout.ProblemSolvingDialogFragmentLayout, container, false);

                GetFieldComponenets(view);
                HandleMicPermission();

                SetupCallbacks();

                if(_problemID != -1)
                {
                    if(_problemText != null)
                    {
                        _problemText.Text = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID).ProblemText.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "OnCreateView: _problemText is NULL!");
                    }
                    if (_add != null)
                        _add.Text = _activity.GetString(Resource.String.wordAcceptUpper);
                }
                else
                {
                    if (_add != null)
                        _add.Text = _activity.GetString(Resource.String.wordAddUpper);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingDialogCreateView), "ProblemSolvingDialogFragment.OnCreateView");
            }

            return view;
        }

        private void GetFieldComponenets(View view)
        {
            try
            {
                if(view != null)
                {
                    _problemText = view.FindViewById<EditText>(Resource.Id.edtProblemSolvingDialogFragmentProblemText);
                    _goBack = view.FindViewById<Button>(Resource.Id.btnProblemSolvingDialogFragmentGoBack);
                    _add = view.FindViewById<Button>(Resource.Id.btnProblemSolvingDialogFragmentAdd);
                    _speakProblemText = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakProblemText);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponenets: view is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponenets: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingDialogGetComponents), "ProblemSolvingDialogFragment.GetFieldComponenets");
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
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _goBack is NULL!");
                }
                if(_add != null)
                {
                    _add.Click += Add_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _add is NULL!");
                }
                if(_speakProblemText != null)
                    _speakProblemText.Click += SpeakProblemText_Click;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingDialogSetCallbacks), "ProblemSolvingDialogFragment.SetupCallbacks");
            }
        }

        private void SpeakProblemText_Click(object sender, EventArgs e)
        {
            SpeakToMYM("What problem needs solving?");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Attempting Voice Recognition", "ProblemSolvingDialogFragment.SpeakToMYM");
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
                    _problemText.Text = matches[0];
                }
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_problemText != null)
                {
                    if(string.IsNullOrEmpty(_problemText.Text))
                    {
                        _problemText.Error = Activity.GetString(Resource.String.ProblemSolvingDialogFragmentEmpty);
                        return;
                    }

                    ((IProblemSolvingCallback)Activity).ConfirmAddition(_problemID, _problemText.Text.Trim());
                    Dismiss();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add_Click: - Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorProblemSolvingDialogAdd), "ProblemSolvingDialogFragment.Add_Click");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            try
            {
                ((IProblemSolvingCallback)Activity).CancelAddition();
                Dismiss();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "GoBack_Click: - Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorProblemSolvingDialogGoBack), "ProblemSolvingDialogFragment.GoBack_Click");
            }
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakProblemText != null)
                {
                    _speakProblemText.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakProblemText.Enabled = false;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Speech;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ProblemSolvingIdeasDialogFragment : DialogFragment
    {
        public const string TAG = "M:ProblemSolvingIdeasDialogFragment";

        Activity _activity;

        private EditText _ideaText;
        private Button _goBack;
        private Button _add;

        private int _problemID = -1;
        private int _problemStepID = -1;
        private int _problemIdeaID = -1;
        private string _problemIdeaText = "";

        private ImageButton _speakIdeaText;

        private string _dialogTitle = "";

        public ProblemSolvingIdeasDialogFragment()
        {

        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("problemID", _problemID);
                outState.PutInt("problemStepID", _problemStepID);
                outState.PutInt("problemIdeaID", _problemIdeaID);
                outState.PutString("problemIdeaText", _problemIdeaText);
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

        public ProblemSolvingIdeasDialogFragment(Activity activity, string title, int problemID = -1, int problemStepID = -1, int problemIdeaID = -1, string ideaText = "")
        {
            _activity = activity;
            _problemID = problemID;
            _problemStepID = problemStepID;
            _problemIdeaID = problemIdeaID;
            _problemIdeaText = ideaText;
            _dialogTitle = title;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = null;

            try
            {
                if(savedInstanceState != null)
                {
                    _problemID = savedInstanceState.GetInt("problemID");
                    _problemStepID = savedInstanceState.GetInt("problemStepID");
                    _problemIdeaID = savedInstanceState.GetInt("problemIdeaID");
                    _problemIdeaText = savedInstanceState.GetString("problemIdeaText");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                view = inflater.Inflate(Resource.Layout.ProblemSolvingIdeasDialogFragmentLayout, container, false);

                GetFieldComponenets(view);
                HandleMicPermission();

                SetupCallbacks();

                if (_problemIdeaID != -1)
                {
                    if (_ideaText != null)
                    {
                        _ideaText.Text = _problemIdeaText.Trim();
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
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingIdeasDialogCreateView), "ProblemSolvingIdeasDialogFragment.OnCreateView");
            }

            return view;
        }

        private void GetFieldComponenets(View view)
        {
            try
            {
                if (view != null)
                {
                    _ideaText = view.FindViewById<EditText>(Resource.Id.edtProblemSolvingIdeasDialogFragmentIdeaText);
                    _goBack = view.FindViewById<Button>(Resource.Id.btnProblemSolvingIdeasDialogFragmentGoBack);
                    _add = view.FindViewById<Button>(Resource.Id.btnProblemSolvingIdeasDialogFragmentAdd);
                    _speakIdeaText = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakIdeaText);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponenets: view is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponenets: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingIdeasDialogGetComponents), "ProblemSolvingIdeasDialogFragment.GetFieldComponenets");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_goBack != null)
                {
                    _goBack.Click += GoBack_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _goBack is NULL!");
                }
                if (_add != null)
                {
                    _add.Click += Add_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _add is NULL!");
                }
                if(_speakIdeaText != null)
                    _speakIdeaText.Click += SpeakIdeaText_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingIdeasDialogSetCallbacks), "ProblemSolvingIdeasDialogFragment.SetupCallbacks");
            }
        }

        private void SpeakIdeaText_Click(object sender, EventArgs e)
        {
            SpeakToMYM("Tell me of your Idea...");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Attempting Voice Recognition", "ProblemSolvingIdeasDialogFragment.SpeakToMYM");
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
                    _ideaText.Text = matches[0];
                }
            }
        }


        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_ideaText != null)
                {
                    if(string.IsNullOrEmpty(_ideaText.Text))
                    {
                        _ideaText.Error = Activity.GetString(Resource.String.ProblemSolvingIdeasDialogFragmentEmpty);
                        return;
                    }
                    ((IProblemSolvingIdeasCallback)Activity).ConfirmAddition(_problemID, _problemStepID, _problemIdeaID, _ideaText.Text.Trim());
                    Dismiss();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: - Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorProblemSolvingIdeasDialogAdd), "ProblemSolvingIdeasDialogFragment.Add_Click");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            try
            {
                ((IProblemSolvingIdeasCallback)Activity).CancelAddition();
                Dismiss();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "GoBack_Click: - Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorProblemSolvingIdeasDialogGoBack), "ProblemSolvingIdeasDialogFragment.GoBack_Click");
            }
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakIdeaText != null)
                {
                    _speakIdeaText.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakIdeaText.Enabled = false;
                }
            }
        }
    }
}
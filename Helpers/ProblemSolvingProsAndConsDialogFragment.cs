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
    public class ProblemSolvingProsAndConsDialogFragment : DialogFragment
    {
        public const string TAG = "M:ProblemSolvingProsAndConsDialogFragment";

        Activity _activity;

        private EditText _proAndConText;
        private RadioGroup _proAndConGroup;
        private RadioButton _proButton;
        private RadioButton _conButton;
        private Button _goBack;
        private Button _add;

        private ImageButton _speakProCon;

        private int _problemID = -1;
        private int _problemStepID = -1;
        private int _problemIdeaID = -1;
        private int _problemProAndConID = -1;
        private string _problemProAndConText = "";
        private ConstantsAndTypes.PROCON_TYPES _problemProAndConType = ConstantsAndTypes.PROCON_TYPES.Pro;

        private string _dialogTitle = "";

        public ProblemSolvingProsAndConsDialogFragment()
        {

        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("problemID", _problemID);
                outState.PutInt("problemStepID", _problemStepID);
                outState.PutInt("problemIdeaID", _problemIdeaID);
                outState.PutInt("problemProAndConID", _problemProAndConID);
                outState.PutString("problemProAndConText", _problemProAndConText);
                outState.PutInt("problemProAndConType", (int)_problemProAndConType);
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

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public ProblemSolvingProsAndConsDialogFragment(Activity activity, string title, int problemID = -1, int problemStepID = -1, int problemIdeaID = -1, int problemProAndConID = -1, ConstantsAndTypes.PROCON_TYPES problemProAndConType = ConstantsAndTypes.PROCON_TYPES.Pro, string proAndConText = "")
        {
            _activity = activity;
            _problemID = problemID;
            _problemStepID = problemStepID;
            _problemIdeaID = problemIdeaID;
            _problemProAndConID = problemProAndConID;
            _problemProAndConText = proAndConText;
            _problemProAndConType = problemProAndConType;
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
                    _problemProAndConID = savedInstanceState.GetInt("problemProAndConID");
                    _problemProAndConText = savedInstanceState.GetString("problemProAndConText");
                    _problemProAndConType = (ConstantsAndTypes.PROCON_TYPES)savedInstanceState.GetInt("problemProAndConType");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                view = inflater.Inflate(Resource.Layout.ProblemSolvingProsAndConsDialogFragmentLayout, container, false);

                GetFieldComponenets(view);
                HandleMicPermission();

                SetupCallbacks();

                if (_problemProAndConID != -1)
                {
                    if (_proAndConText != null)
                    {
                        _proAndConText.Text = _problemProAndConText.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "OnCreateView: _proAndConText is NULL!");
                    }
                    if(_proButton != null && _conButton != null)
                    {
                        if(_problemProAndConType == ConstantsAndTypes.PROCON_TYPES.Pro)
                        {
                            _proButton.Checked = true;
                        }
                        else
                        {
                            _conButton.Checked = true;
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "OnCreateView: _proButton or _conButton is NULL!");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingProConDialogCreateView), "ProblemSolvingProsAndConsDialogFragment.OnCreateView");
            }

            return view;
        }

        private void GetFieldComponenets(View view)
        {
            try
            {
                if (view != null)
                {
                    _proAndConText = view.FindViewById<EditText>(Resource.Id.edtProblemSolvingProsAndConsDialogFragmentIdeaText);
                    _proButton = view.FindViewById<RadioButton>(Resource.Id.radbtnPro);
                    _conButton = view.FindViewById<RadioButton>(Resource.Id.radbtnCon);
                    _proAndConGroup = view.FindViewById<RadioGroup>(Resource.Id.radProblemSolvingProsAndCons);
                    _goBack = view.FindViewById<Button>(Resource.Id.btnProblemSolvingProsAndConsDialogFragmentGoBack);
                    _add = view.FindViewById<Button>(Resource.Id.btnProblemSolvingProsAndConsDialogFragmentAdd);
                    _speakProCon = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakProCon);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponenets: view is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponenets: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingProConDialogGetComponents), "ProblemSolvingProsAndConsDialogFragment.GetFieldComponenets");
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
                if(_speakProCon != null)
                    _speakProCon.Click += SpeakProCon_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingProConDialogSetCallbacks), "ProblemSolvingProsAndConsDialogFragment.SetupCallbacks");
            }
        }

        private void SpeakProCon_Click(object sender, EventArgs e)
        {
            SpeakToMYM("Tell me a Pro or Con for the Idea...");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Attempting Voice Recognition", "ProblemSolvingProsAndConsDialogFragment.SpeakToMYM");
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
                    _proAndConText.Text = matches[0];
                }
            }
        }


        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                ConstantsAndTypes.PROCON_TYPES proAndConType = ConstantsAndTypes.PROCON_TYPES.Pro;

                if (_proButton.Checked)
                {
                    proAndConType = ConstantsAndTypes.PROCON_TYPES.Pro;
                }
                else
                {
                    proAndConType = ConstantsAndTypes.PROCON_TYPES.Con;
                }
                if (_proAndConText != null)
                {
                    if(string.IsNullOrEmpty(_proAndConText.Text))
                    {
                        _proAndConText.Error = Activity.GetString(Resource.String.ProblemSolvingProConFragmentAddEmpty);
                        return;
                    }
                    ((IProblemSolvingProsAndConsCallback)Activity).ConfirmAddition(_problemID, _problemStepID, _problemIdeaID, _problemProAndConID, _proAndConText.Text.Trim(), proAndConType);
                    Dismiss();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: - Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorProblemSolvingProConDialogAdd), "ProblemSolvingProsAndConsDialogFragment.Add_Click");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            try
            {
                ((IProblemSolvingProsAndConsCallback)Activity).CancelAddition();
                Dismiss();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "GoBack_Click: - Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorProblemSolvingProConDialogGoBack), "ProblemSolvingProsAndConsDialogFragment.GoBack_Click");
            }
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakProCon != null)
                {
                    _speakProCon.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakProCon.Enabled = false;
                }
            }
        }
    }
}
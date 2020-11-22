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
    public class ProblemSolvingStepsFragment : DialogFragment
    {
        public const string TAG = "M:ProblemSolvingStepsFragment";

        Activity _activity;

        private Button _goBack;
        private Button _add;
        private EditText _problemStepText;
        private Spinner _priorityOrder;

        private ImageButton _speakProblemStep;

        private int _problemID;
        private int _problemStepID;
        private int _priority = -1;
        private string _stepText = "";

        private string _dialogTitle = "";

        public ProblemSolvingStepsFragment()
        {

        }

        public ProblemSolvingStepsFragment(Activity activity, string title, int problemID, int problemStepID, string stepText = "", int priorityOrder = -1)
        {
            _activity = activity;
            _problemID = problemID;
            _problemStepID = problemStepID;
            _stepText = stepText;
            _priority = priorityOrder;
            _dialogTitle = title;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("problemID", _problemID);
                outState.PutInt("problemStepID", _problemStepID);
                outState.PutInt("priority", _priority);
                outState.PutString("stepText", _stepText);
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
                if(savedInstanceState != null)
                {
                    _problemID = savedInstanceState.GetInt("problemID");
                    _problemStepID = savedInstanceState.GetInt("problemStepID");
                    _priority = savedInstanceState.GetInt("priority");
                    _stepText = savedInstanceState.GetString("stepText");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                view = inflater.Inflate(Resource.Layout.ProblemSolvingStepsDialogFragmentLayout, container, false);
                if(view != null)
                {
                    GetFieldComponents(view);
                    HandleMicPermission();

                    SetupCallbacks();

                    SetupSpinner();

                    if(_problemStepID != -1)
                    {
                        _problemStepText.Text = _stepText.Trim();
                        _priorityOrder.SetSelection(_priority - 1);
                        if (_add != null)
                            _add.Text = _activity.GetString(Resource.String.wordAcceptUpper);
                    }
                    else
                    {
                        if (_add != null)
                            _add.Text = _activity.GetString(Resource.String.wordAddUpper);
                    }
                }
                else
                {
                    Log.Error(TAG, "OnCreateView: view is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingStepsFragmentCreateView), "ProblemSolvingStepsFragment.OnCreateView");
            }
            return view;
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _problemStepText = view.FindViewById<EditText>(Resource.Id.edtProblemSolvingDialogFragmentStepText);
                _goBack = view.FindViewById<Button>(Resource.Id.btnProblemSolvingStepsDialogFragmentGoBack);
                _add = view.FindViewById<Button>(Resource.Id.btnProblemSolvingStepsDialogFragmentAdd);
                _priorityOrder = view.FindViewById<Spinner>(Resource.Id.spnProblemSolvingStepsDialogFragmentPriority);
                _speakProblemStep = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakSolveStep);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingStepsFragmentGetComponents), "ProblemSolvingStepsFragment.GetFieldComponents");
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
                if(_speakProblemStep != null)
                    _speakProblemStep.Click += SpeakProblemStep_Click;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingStepsFragmentSetCallbacks), "ProblemSolvingStepsFragment.SetupCallbacks");
            }
        }

        private void SpeakProblemStep_Click(object sender, EventArgs e)
        {
            SpeakToMYM("Tell me a step towards solving the Problem...");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Attempting Voice Recognition", "ProblemSolvingStepsFragment.SpeakToMYM");
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
                    _problemStepText.Text = matches[0];
                }
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;

            if(Activity != null)
            {
                if (_problemStepText != null)
                {
                    if(string.IsNullOrEmpty(_problemStepText.Text))
                    {
                        _problemStepText.Error = GetString(Resource.String.ProblemSolvingStepsFragmentAddEmpty);
                        return;
                    }
                    ((IProblemSolvingStepsCallback)Activity).ConfirmAddition(_problemStepID, _problemID, _problemStepText.Text.Trim(), _priorityOrder.SelectedItemPosition + 1);
                    Dismiss();
                }
            }
            else
            {
                Log.Error(TAG, "Add_Click: Activity is NULL!");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            if (Activity != null)
            {
                ((IProblemSolvingStepsCallback)Activity).CancelAddition();
                Dismiss();
            }
            else
            {
                Log.Error(TAG, "GoBack_Click: Activity is NULL!");
            }
        }

        private void SetupSpinner()
        {
            try
            {
                string[] steps = new string[50];

                for (var a = 0; a < 50; a++)
                {
                    steps[a] = (a + 1).ToString();
                }

                ArrayAdapter adapter = new ArrayAdapter((Activity)Activity, Resource.Layout.SpinnerGeneral, steps);
                if (adapter != null)
                {
                    adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                    _priorityOrder.Adapter = adapter;
                    _priorityOrder.SetSelection(0);

                    Log.Info(TAG, "SetupSpinner: Set Priority spinner Adapter");
                }
                else
                {
                    Log.Error(TAG, "SetupSpinner: Failed to create Adapter");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupSpinner: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingStepsFragmentSetSpinner), "ProblemSolvingStepsFragment.SetupSpinner");
            }
        }

        private bool Validate()
        {
            try
            {
                if(_problemStepText != null)
                {
                    if(string.IsNullOrEmpty(_problemStepText.Text.Trim()))
                    {
                        _problemStepText.Error = Activity.GetString(Resource.String.ErrorSolutionStepDialogStepText);
                        return false;
                    }
                }
                else
                {
                    Log.Error(TAG, "Validate: _problemStepText is NULL!");
                }
                var problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);
                if(problem != null)
                {
                    if (_problemStepID == -1)
                    {
                        var priorityValue = _priorityOrder.SelectedItemPosition + 1;
                        bool exists = problem.CheckPriority(priorityValue);
                        if (exists)
                        {
                            Toast.MakeText(Activity, Activity.GetString(Resource.String.ProblemSolvingStepsFragmentHasStep1Toast) + " " + priorityValue.ToString() + Activity.GetString(Resource.String.ProblemSolvingStepsFragmentHasStep2Toast), ToastLength.Long).Show();
                            return false;
                        }
                    }
                }
                else
                {
                    Log.Error(TAG, "Validate: Unable to locate problem with ID " + _problemID.ToString() + " in global cache");
                }
                return true;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Validate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorProblemSolvingStepsFragmentValidate), "ProblemSolvingStepsFragment.Validate");
                return false;
            }
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakProblemStep != null)
                {
                    _speakProblemStep.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakProblemStep.Enabled = false;
                }
            }
        }
    }
}
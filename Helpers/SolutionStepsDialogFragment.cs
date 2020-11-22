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
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Speech;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class SolutionStepsDialogFragment : DialogFragment
    {
        public const string TAG = "M:SolutionStepsDialogFragment";

        Activity _activity;

        private Button _goBack;
        private Button _add;
        private EditText _solutionStepText;
        private Spinner _priorityOrder;
        private ImageButton _speakSolutionStep;
        
        private int _problemIdeaID;
        private int _solutionPlanStepID;
        private int _priority = -1;
        private string _stepText = "";

        private string _dialogTitle = "";

        public SolutionStepsDialogFragment()
        {

        }

        public SolutionStepsDialogFragment(Activity activity, string title, int problemIdeaID, int solutionStepID, string stepText = "", int priorityOrder = -1)
        {
            _activity = activity;
            _problemIdeaID = problemIdeaID;
            _solutionPlanStepID = solutionStepID;
            _stepText = stepText;
            _priority = priorityOrder;
            _dialogTitle = title;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("problemIdeaID", _problemIdeaID);
                outState.PutInt("solutionPlanStepID", _solutionPlanStepID);
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
                    _problemIdeaID = savedInstanceState.GetInt("problemIdeaID");
                    _solutionPlanStepID = savedInstanceState.GetInt("solutionPlanStepID");
                    _priority = savedInstanceState.GetInt("priority");
                    _stepText = savedInstanceState.GetString("stepText");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                view = inflater.Inflate(Resource.Layout.SolutionStepDialogFragmentLayout, container, false);
                if (view != null)
                {
                    GetFieldComponents(view);
                    HandleMicPermission();

                    SetupCallbacks();

                    SetupSpinner();

                    if (_solutionPlanStepID != -1)
                    {
                        _solutionStepText.Text = _stepText.Trim();
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
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorSolutionStepDialogCreateView), "SolutionStepsDialogFragment.OnCreateView");
            }
            return view;
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _solutionStepText = view.FindViewById<EditText>(Resource.Id.edtSolutionPlanDialogFragmentStepText);
                _goBack = view.FindViewById<Button>(Resource.Id.btnSolutionPlanStepsDialogFragmentGoBack);
                _add = view.FindViewById<Button>(Resource.Id.btnSolutionPlanStepsDialogFragmentAdd);
                _priorityOrder = view.FindViewById<Spinner>(Resource.Id.spnSolutionPlanStepsDialogFragmentPriority);
                _speakSolutionStep = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakSolutionStep);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorSolutionStepDialogGetComponents), "SolutionStepsDialogFragment.GetFieldComponents");
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
                if(_speakSolutionStep != null)
                    _speakSolutionStep.Click += SpeakSolutionStep_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorSolutionStepDialogSetCallbacks), "SolutionStepsDialogFragment.SetupCallbacks");
            }
        }

        private void SpeakSolutionStep_Click(object sender, EventArgs e)
        {
            SpeakToMYM("Tell me a step towards a Solution...");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Attempting Voice Recognition", "SolutionStepsDialogFragment.SpeakToMYM");
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
                    _solutionStepText.Text = matches[0];
                }
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (!Validate()) return;

            if (Activity != null)
            {
                ((ISolutionPlanStepsCallback)Activity).ConfirmAddition(_solutionPlanStepID, _problemIdeaID, _solutionStepText.Text.Trim(), _priorityOrder.SelectedItemPosition + 1);
                Dismiss();
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
                ((ISolutionPlanStepsCallback)Activity).CancelAddition();
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
            catch (Exception e)
            {
                Log.Error(TAG, "SetupSpinner: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorSolutionStepDialogSetSpinner), "SolutionStepsDialogFragment.SetupSpinner");
            }
        }

        private bool Validate()
        {
            try
            {
                if (_solutionStepText != null)
                {
                    if (string.IsNullOrEmpty(_solutionStepText.Text.Trim()))
                    {
                        _solutionStepText.Error = Activity.GetString(Resource.String.ErrorSolutionStepDialogStepText);
                        return false;
                    }
                }
                else
                {
                    Log.Error(TAG, "Validate: _solutionStepText is NULL!");
                }
                var steps =
                    (from eachStep in GlobalData.SolutionPlansItems
                     where eachStep.ProblemIdeaID == _problemIdeaID
                     select eachStep).ToList();


                if (steps != null && _solutionPlanStepID == -1)
                {
                    var priorityValue = _priorityOrder.SelectedItemPosition + 1;
                    bool exists = DoesPriorityAlreadyExist(steps, _priorityOrder.SelectedItemPosition + 1);
                    if (exists)
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.SolutionStepDialogHasStep1Toast) + " " + priorityValue.ToString() + Activity.GetString(Resource.String.SolutionStepDialogHasStep2Toast), ToastLength.Long).Show();
                        return false;
                    }
                }
                else
                {
                    Log.Error(TAG, "Validate: Unable to locate step with ID " + _solutionPlanStepID.ToString() + " in global cache");
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Validate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorSolutionStepDialogValidate), "SolutionStepsDialogFragment.Validate");
                return false;
            }
        }

        private bool DoesPriorityAlreadyExist(List<SolutionPlan> list, int priority)
        {
            bool retVal = false;

            foreach(SolutionPlan step in list)
            {
                if(step.PriorityOrder == priority)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakSolutionStep != null)
                {
                    _speakSolutionStep.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakSolutionStep.Enabled = false;
                }
            }
        }
    }
}
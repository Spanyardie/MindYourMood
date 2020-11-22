using System;
using System.Collections.Generic;
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
    public class AffirmationDialogFragment : DialogFragment
    {
        public const string TAG = "M:AffirmationDialogFragment";

        Activity _activity;

        private EditText _affirmationText;
        private Button _goBack;
        private Button _add;

        private int _affirmationID;

        private string _dialogTitle = "";

        private ImageButton _speakAffirmation;
        private bool _spokenAffirmation = false;
        private string _spokenText = "";

        public AffirmationDialogFragment()
        {

        }

        public AffirmationDialogFragment(Activity activity, string title, int affirmationID = -1)
        {
            _activity = activity;
            _affirmationID = affirmationID;
            _dialogTitle = title;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutInt("affirmationID", _affirmationID);
                outState.PutString("dialogTitle", _dialogTitle);
            }
            base.OnSaveInstanceState(outState);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            if(context != null)
                _activity = (Activity)context;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = null;

            try
            {
                if (savedInstanceState != null)
                {
                    _affirmationID = savedInstanceState.GetInt("affirmationID");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                view = inflater.Inflate(Resource.Layout.AffirmationDialogFragmentLayout, container, false);

                GetFieldComponents(view);
                HandleMicPermission();

                SetupCallbacks();

                if(_affirmationID != -1)
                {
                    if(_affirmationText != null)
                    {
                        _affirmationText.Text = GlobalData.AffirmationListItems.Find(aff => aff.AffirmationID == _affirmationID).AffirmationText.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "OnCreateView: _affirmationText is NULL!");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorAffirmationDialogCreateView), "AffirmationDialogFragment.OnCreateView");
            }

            return view;
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
                if(_speakAffirmation != null)
                    _speakAffirmation.Click += SpeakAffirmation_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorAffirmationDialogSetCallbacks), "AffirmationDialogFragment.SetupCallbacks");
            }
        }

        private void SpeakAffirmation_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.AffirmationSpeakPrompt));

            Log.Info(TAG, "SpeakAffirmation_Click: Created intent, sending request...");
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
                        _spokenAffirmation = true;
                        _spokenText = matches[0];
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCheckingVoiceRecognition), "AffirmationDialogFragment.OnActivityResult");
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (_spokenAffirmation)
            {
                if (_affirmationText != null)
                    _affirmationText.Text = _spokenText;
                _spokenAffirmation = false;
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_affirmationText != null)
                {
                    if(string.IsNullOrEmpty(_affirmationText.Text))
                    {
                        _affirmationText.Error = Activity.GetString(Resource.String.AffirmationDialogFragmentEmpty);
                        return;
                    }

                    ((IAffirmationCallback)Activity).ConfirmAddition(_affirmationID, _affirmationText.Text.Trim());
                    Dismiss();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: - Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorAffirmationDialogAdd), "AffirmationDialogFragment.Add_Click");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            try
            {
                ((IAffirmationCallback)Activity).CancelAddition();
                Dismiss();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "GoBack_Click: - Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorAffirmationDialogCancel), "AffirmationDialogFragment.GoBack_Click");
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if(view != null)
                {
                    _affirmationText = view.FindViewById<EditText>(Resource.Id.edtAffirmationDialogFragmentAffirmationText);
                    _goBack = view.FindViewById<Button>(Resource.Id.btnAffirmationDialogFragmentGoBack);
                    _add = view.FindViewById<Button>(Resource.Id.btnAffirmationDialogFragmentAdd);
                    _speakAffirmation = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakAffirmation);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponenets: view is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponenets: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorAffirmationDialogGetComponents), "AffirmationDialogFragment.GetFieldComponenets");
            }
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakAffirmation != null)
                {
                    _speakAffirmation.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakAffirmation.Enabled = false;
                }
            }
        }
    }
}
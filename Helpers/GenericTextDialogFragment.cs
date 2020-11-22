using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Speech;
using Android.Runtime;
using System.Collections.Generic;

namespace com.spanyardie.MindYourMood.Helpers
{
    /// <summary>
    /// Calling classes MUST implement the callbacks:
    /// ConfirmText(string textEntered),  
    /// CancelText()
    /// </summary>
    public class GenericTextDialogFragment : DialogFragment
    {
        public const string TAG = "M:GenericTextDialogFragment";

        private Button _goBack;
        private Button _add;
        private EditText _genericText;
        private TextView _dialogTitle;

        private string _textTitle;
        private string _genericTextEntered = "";

        private Activity _parent;

        private int _genericItemID = -1;

        private string _thisDialogTitle = "";

        private ImageButton _speakGeneric;
        private bool _spokenGeneric = false;
        private string _spokenText = "";

        public GenericTextDialogFragment()
        {

        }

        public GenericTextDialogFragment(Activity parent, string dialogTitle, string textTitle, string genericText = "", int genericItemID = -1)
        {
            _parent = parent;
            _textTitle = textTitle.Trim();
            _genericTextEntered = genericText;
            _genericItemID = genericItemID;
            _thisDialogTitle = dialogTitle;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutString("textTitle", _textTitle);
                outState.PutString("genericText", _genericTextEntered.Trim());
                outState.PutString("dialogTitle", _thisDialogTitle);
            }

            base.OnSaveInstanceState(outState);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            if (context != null)
                _parent = (Activity)context;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = null;

            try
            {
                if (savedInstanceState != null)
                {
                    _textTitle = savedInstanceState.GetString("textTitle");
                    _genericTextEntered = savedInstanceState.GetString("genericText");
                    _thisDialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_thisDialogTitle);

                view = inflater.Inflate(Resource.Layout.GenericTextDialogFragmentLayout, container, false);

                if (view != null)
                {
                    GetFieldComponents(view);

                    HandleMicPermission();

                    SetupCallbacks();
                }

                if (_dialogTitle != null)
                    _dialogTitle.Text = _textTitle.Trim();

                if (_genericItemID != -1 || savedInstanceState != null)
                {
                    _genericText.Text = _genericTextEntered;
                    if (_add != null)
                        _add.Text = _parent.GetString(Resource.String.wordAcceptUpper);
                }
                else
                {
                    if (_add != null)
                        _add.Text = _parent.GetString(Resource.String.wordAddUpper);
                }
                Log.Info(TAG, "OnCreateView: Inflated view for " + _thisDialogTitle);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCreatingGenericTextDialog), "GenericTextDialogFragment.OnCreateView");
            }

            return view;
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_goBack != null)
                    _goBack.Click += GoBack_Click;
                if (_add != null)
                    _add.Click += Add_Click;
                if(_speakGeneric != null)
                    _speakGeneric.Click += SpeakGeneric_Click;
                Log.Info(TAG, "SetupCallbacks: Completed successfully");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorSettingGenericTextCallbacks), "GenericTextDialogFragment.SetupCallbacks");
            }
        }

        private void SpeakGeneric_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, _thisDialogTitle);

            Log.Info(TAG, "SpeakGeneric_Click: Created intent, sending request...");
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
                        _spokenGeneric = true;
                        _spokenText = matches[0];
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCheckingVoiceRecognition), "GenericTextDialogFragment.OnActivityResult");
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (_spokenGeneric)
            {
                if (_genericText != null)
                    _genericText.Text = _spokenText;
                _spokenGeneric = false;
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            var textEntered = "";

            try
            {
                if (_genericText != null)
                {
                    textEntered = _genericText.Text.Trim();
                    if(string.IsNullOrEmpty(textEntered))
                    {
                        _genericText.Error = Activity.GetString(Resource.String.GenericTextDialogFragmentTextEmpty);
                        return;
                    }
                    Log.Info(TAG, "Add_Click: Text entered - " + textEntered);
                }

                if (Activity != null)
                    ((IGenericTextCallback)Activity).ConfirmText(textEntered.Trim(), _genericItemID);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorAddingGenericTextItem), "GenericTextDialogFragment.Add_Click");
            }

            Dismiss();
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (Activity != null)
                {
                    Log.Info(TAG, "GoBack_Click: Returning to parent activity via CancelText");
                    ((IGenericTextCallback)Activity).CancelText();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "GoBack_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorReturningFromGenericText), "GenericTextDialogFragment.GoBack_Click");
            }

            Log.Info(TAG, "GoBack_Click: Dismissing dialog");
            Dismiss();
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _goBack = view.FindViewById<Button>(Resource.Id.btnGoBack);
                    _add = view.FindViewById<Button>(Resource.Id.btnGoSave);
                    _genericText = view.FindViewById<EditText>(Resource.Id.edtTextEntry);
                    _dialogTitle = view.FindViewById<TextView>(Resource.Id.txtTextTitle);
                    _speakGeneric = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakGeneric);
                    Log.Info(TAG, "GetFieldComponents: Completed successfully");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorGettingGenericTextFieldComponents), "GenericTextDialogFragment.GetFieldComponents");
            }
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            Log.Info(TAG, "OnDismiss: Called!");
            if (Activity != null)
                ((IGenericTextCallback)Activity).OnGenericDialogDismiss();

            base.OnDismiss(dialog);
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakGeneric != null)
                {
                    _speakGeneric.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakGeneric.Enabled = false;
                }
            }
        }
    }
}
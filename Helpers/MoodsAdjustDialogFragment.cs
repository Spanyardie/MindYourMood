using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Speech;
using com.spanyardie.MindYourMood.Model.Interfaces;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class MoodsAdjustDialogFragment : DialogFragment
    {
        public const string TAG = "M:MoodsAdjustDialogFragment";

        Activity _activity;

        private EditText _moodListText;
        private Button _goBack;
        private Button _add;

        private int _moodListID;

        private string _dialogTitle = "";
        private string _textTitle = "";
        private string _moodName = "";
        private TextView _titleText;

        private ImageButton _speakMood;
        private bool _spokenMood = false;
        private string _spokenText = "";

        public MoodsAdjustDialogFragment()
        {

        }

        public MoodsAdjustDialogFragment(Activity activity, string dialogTitle, string textTitle, string moodName = "", int moodListID = -1)
        {
            _activity = activity;
            _moodListID = moodListID;
            _dialogTitle = dialogTitle;
            _textTitle = textTitle;
            _moodName = moodName;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutInt("moodListID", _moodListID);
                outState.PutString("dialogTitle", _dialogTitle);
                outState.PutString("textTitle", _textTitle);
                outState.PutString("moodName", _moodName);
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
                    _moodListID = savedInstanceState.GetInt("moodListID");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                    _textTitle = savedInstanceState.GetString("textTitle");
                    _moodName = savedInstanceState.GetString("moodName");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                view = inflater.Inflate(Resource.Layout.MoodsAdjustDialogFragmentLayout, container, false);

                GetFieldComponents(view);
                HandleMicPermission();

                SetupCallbacks();

                if (_moodListID != -1)
                {
                    if (_moodListText != null)
                    {
                        _moodListText.Text = _moodName.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "OnCreateView: _moodListText is NULL!");
                    }
                    if (_add != null)
                        _add.Text = _activity.GetString(Resource.String.wordAcceptUpper);
                }
                else
                {
                    if (savedInstanceState != null)
                        _moodListText.Text = _moodName.Trim();
                    if (_add != null)
                        _add.Text = _activity.GetString(Resource.String.wordAddUpper);
                }

                if (_titleText != null)
                    _titleText.Text = _textTitle.Trim();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMoodsAdjustDialogCreateView), "MoodsAdjustDialogFragment.OnCreateView");
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
                if (_speakMood != null)
                    _speakMood.Click += SpeakMood_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMoodsAdjustDialogSetCallbacks), "MoodsAdjustDialogFragment.SetupCallbacks");
            }
        }

        private void SpeakMood_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.AffirmationSpeakPrompt));

            Log.Info(TAG, "SpeakMood_Click: Created intent, sending request...");
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
                        _spokenMood = true;
                        _spokenText = matches[0];
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCheckingVoiceRecognition), "MoodsAdjustDialogFragment.OnActivityResult");
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (_spokenMood)
            {
                if (_moodListText != null)
                    _moodListText.Text = _spokenText;
                _spokenMood = false;
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_moodListText != null)
                {
                    if (string.IsNullOrEmpty(_moodListText.Text))
                    {
                        _moodListText.Error = Activity.GetString(Resource.String.MoodsAdjustDialogFragmentEmpty);
                        return;
                    }

                    int moodId = -1;
                    if(_moodListID != -1)
                        moodId = GlobalData.MoodListItems[_moodListID].MoodId;
                    
                    ((IMoodsAdjustCallback)Activity).ConfirmAddition(moodId, _moodListText.Text.Trim());
                    Dismiss();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: - Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorMoodsAdjustDialogAdd), "MoodsAdjustDialogFragment.Add_Click");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            try
            {
                ((IMoodsAdjustCallback)Activity).CancelAddition();
                Dismiss();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "GoBack_Click: - Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorMoodsAdjustDialogCancel), "MoodsAdjustDialogFragment.GoBack_Click");
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _moodListText = view.FindViewById<EditText>(Resource.Id.edtMoodsAdjustDialogFragmentMoodText);
                    _goBack = view.FindViewById<Button>(Resource.Id.btnMoodsAdjustDialogFragmentGoBack);
                    _add = view.FindViewById<Button>(Resource.Id.btnMoodsAdjustDialogFragmentAdd);
                    _speakMood = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakMoodsAdjust);
                    _titleText = view.FindViewById<TextView>(Resource.Id.txtMoodsAdjustDialogFragmentTitle);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: view is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMoodsAdjustDialogGetComponents), "MoodsAdjustDialogFragment.GetFieldComponents");
            }
        }

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakMood != null)
                {
                    _speakMood.SetImageResource(Resource.Drawable.micgreyscale);
                    _speakMood.Enabled = false;
                }
            }
        }
    }
}
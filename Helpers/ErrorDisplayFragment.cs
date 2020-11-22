using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ErrorDisplayFragment : DialogFragment
    {
        public const string TAG = "M:ErrorDisplayFragment";

        private Button _goBack;
        private Button _email;

        private TextView _wasDoing;
        private TextView _activityAndFunction;
        private TextView _errorMessage;
        private Exception _exception;

        private Activity _activity;

        private string _wasDoingText;
        private string _activityAndFunctionText;

        public ErrorDisplayFragment()
        {

        }

        public ErrorDisplayFragment(Activity activity, Exception exception, string wasDoingText, string activityAndFunctionText)
        {
            _activity = activity;
            _exception = exception;
            _wasDoingText = wasDoingText;
            _activityAndFunctionText = activityAndFunctionText;
        }

        public override void OnAttach(Context context)
        {
            _activity = (Activity)context;

            base.OnAttach(context);
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutString("wasDoingText", _wasDoingText);
                outState.PutString("activityAndFunctionText", _activityAndFunctionText);
            }

            base.OnSaveInstanceState(outState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if(savedInstanceState != null)
                {
                    _wasDoingText = savedInstanceState.GetString("wasDoingText");
                    _activityAndFunctionText = savedInstanceState.GetString("activityAndFunctionText");
                }

                View view = inflater.Inflate(Resource.Layout.ErrorDisplayFragmentLayout, container, false);

                if (view != null)
                    GetFieldComponents(view);

                SetupCallbacks();

                if (_wasDoing != null)
                    _wasDoing.Text = _wasDoingText.Trim();
                if (_activityAndFunction != null)
                    _activityAndFunction.Text = _activityAndFunctionText.Trim();
                if (_errorMessage != null)
                {
                    if (_exception != null)
                        _errorMessage.Text = _exception.Message;
                }

                return view;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                return null;
            }
        }

        private void GetFieldComponents(View view)
        {
            if(view != null)
            {
                _goBack = view.FindViewById<Button>(Resource.Id.btnErrorFragmentGoBack);
                _email = view.FindViewById<Button>(Resource.Id.btnErrorFragmentEmailError);
                _wasDoing = view.FindViewById<TextView>(Resource.Id.txtErrorFragmentWhatMYMWasDoingText);
                _activityAndFunction = view.FindViewById<TextView>(Resource.Id.txtErrorFragmentActivityText);
                _errorMessage = view.FindViewById<TextView>(Resource.Id.txtErrorFragmentMessageText);
            }
        }

        private void SetupCallbacks()
        {
            if(_goBack != null)
                _goBack.Click += GoBack_Click;
            if(_email != null)
                _email.Click += Email_Click;
        }

        private void Email_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionSendto);
            intent.SetData(Android.Net.Uri.Parse("mailto:"));
            intent.PutExtra(Intent.ExtraEmail, new string[] { "mindyourmoodspanyardie@gmail.com" });
            intent.PutExtra(Intent.ExtraSubject, "Automatic error message of Mind Your Mood error");
            string bodyText = "Mind Your Mood Error Report\r\n" +
                "\r\n" +
                "Mind Your Mood was trying to:\r\n" + _wasDoingText + "\r\n\r\n" +
                "Activity and Function:\r\n" + _activityAndFunctionText + "\r\n\r\n" +
                "Error message:\r\n" + _exception.Message + "\r\n\r\n" +
                "Call stack:\r\n" + _exception.StackTrace + "\r\n\r\n" +
                "Source:\r\n" + _exception.Source;

            intent.PutExtra(Intent.ExtraText, bodyText);
            StartActivity(intent);
            Dismiss();
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}
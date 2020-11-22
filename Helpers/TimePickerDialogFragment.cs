using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model.Interfaces;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class TimePickerDialogFragment : DialogFragment
    {
        public const string TAG = "M:TimePickerActivity";

        private Button _cancel;
        private Button _okay;

        private TimePicker _timePicker;

        private DateTime _currentTime = new DateTime(1900, 1, 1, 12, 0, 0);
        private ConstantsAndTypes.TIMEPICKER_CONTEXT _timeContext;

        private Activity _activity;
        private ITimePickerCallback _callback;

        public TimePickerDialogFragment() { }

        private string _dialogTitle = "";

        public TimePickerDialogFragment(Activity activity, ITimePickerCallback callback, DateTime currentTime, ConstantsAndTypes.TIMEPICKER_CONTEXT timeContext, string title)
        {
            _activity = activity;
            _callback = callback;
            _currentTime = currentTime;
            _timeContext = timeContext;
            _dialogTitle = title;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutString("currentTime", _currentTime.ToString());
                outState.PutInt("timeContext", (int)_timeContext);
                outState.PutString("dialogTitle", _dialogTitle);
            }

            base.OnSaveInstanceState(outState);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = null;

            try
            {
                if (savedInstanceState != null)
                {
                    _currentTime = Convert.ToDateTime(savedInstanceState.GetString("currentTime"));
                    _timeContext = (ConstantsAndTypes.TIMEPICKER_CONTEXT)savedInstanceState.GetInt("timeContext");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                view = inflater.Inflate(Resource.Layout.TimePickerDialogFragmentLayout, container, false);
                if (view != null)
                {
                    GetFieldComponents(view);

                    SetupCallbacks();

                    if (_currentTime != null)
                    {
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                        {
                            _timePicker.Hour = _currentTime.Hour;
                            _timePicker.Minute = _currentTime.Minute;
                        }
                        else
                        {
                            //Depracated but required for older Android versions
                            _timePicker.CurrentHour = (Java.Lang.Integer)_currentTime.Hour;
                            _timePicker.CurrentMinute = (Java.Lang.Integer)_currentTime.Minute;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, "Error creating time picker activity", "TimePickerActivity.OnCreate");
            }

            return view;
        }

        private void SetupCallbacks()
        {
            if (_cancel != null)
                _cancel.Click += Cancel_Click;
            if (_okay != null)
                _okay.Click += Okay_Click;
        }

        private void Okay_Click(object sender, EventArgs e)
        {
            if (_callback != null)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    _callback.TimePicked(new DateTime(1900, 1, 1, _timePicker.Hour, _timePicker.Minute, 0), _timeContext);
                }
                else
                {
                    //deprecated but required for versions older than Marshmallow
                    _callback.TimePicked(new DateTime(1900, 1, 1, (int)_timePicker.CurrentHour, (int)_timePicker.CurrentMinute, 0), _timeContext);
                }
            }
            Dismiss();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _cancel = view.FindViewById<Button>(Resource.Id.btnTimePickerCancel);
                _okay = view.FindViewById<Button>(Resource.Id.btnTimePickerOkay);
                _timePicker = view.FindViewById<TimePicker>(Resource.Id.timePickerWidget);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, "Getting field components", "TimePickerActivity.GetFieldComponents");
            }
        }
    }
}
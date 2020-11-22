using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Util;
using MindYourMood.Helpers;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace MindYourMood
{
    [Activity(Label = "TimePickerActivity")]
    public class TimePickerActivity : Activity
    {
        public const string TAG = "M:TimePickerActivity";

        private Button _cancel;
        private Button _okay;

        private TimePicker _timePicker;

        private DateTime _currentTime = new DateTime(1900, 1, 1, 12, 0, 0);
        private ConstantsAndTypes.TIMEPICKER_CONTEXT _timeContext;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutString("currentTime", _currentTime.ToString());
                outState.PutInt("timeContext", (int)_timeContext);
            }

            base.OnSaveInstanceState(outState);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.TimePickerLayout);

                Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

                GetFieldComponents();

                SetupCallbacks();

                if (savedInstanceState != null)
                {
                    _currentTime = Convert.ToDateTime(savedInstanceState.GetString("currentTime"));
                    _timeContext = (ConstantsAndTypes.TIMEPICKER_CONTEXT)savedInstanceState.GetInt("timeContext");
                }

                if(Intent.HasExtra("currentTime"))
                {
                    _currentTime = Convert.ToDateTime(Intent.GetStringExtra("currentTime"));
                }

                if (Intent.HasExtra("timeContext"))
                    _timeContext = (ConstantsAndTypes.TIMEPICKER_CONTEXT)Intent.GetIntExtra("timeContext", 0);

                if (_currentTime != null)
                {
                    _timePicker.Hour = _currentTime.Hour;
                    _timePicker.Minute = _currentTime.Minute;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Error creating time picker activity", "TimePickerActivity.OnCreate");
            }
        }

        private void SetupCallbacks()
        {
            if(_cancel != null)
                _cancel.Click += Cancel_Click;
            if(_okay != null)
                _okay.Click += Okay_Click;
        }

        private void Okay_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.PutExtra("pickedTime", (new DateTime(1900, 1, 1, _timePicker.Hour, _timePicker.Minute, 0).ToString()));
            SetResult(Result.Ok, intent);

            Finish();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            SetResult(Result.Canceled);

            Finish();
        }

        private void GetFieldComponents()
        {
            try
            {
                _cancel = FindViewById<Button>(Resource.Id.btnTimePickerCancel);
                _okay = FindViewById<Button>(Resource.Id.btnTimePickerOkay);
                _timePicker = FindViewById<TimePicker>(Resource.Id.timePickerWidget);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "TimePickerActivity.GetFieldComponents");
            }
        }
    }
}
using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using Android.Support.V7.App;
using MindYourMood.Model.Interfaces;

namespace MindYourMood.Helpers
{
    //public class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
    //{
    //    public static readonly string TAG = "M:TimePickerFragment";

    //    Action<DateTime> _timeSelectedHandler = delegate { };

    //    public static TimePickerFragment NewInstance(Action<DateTime> onTimeSelected)
    //    {
    //        TimePickerFragment timePicker = new TimePickerFragment();
    //        timePicker._timeSelectedHandler = onTimeSelected;
    //        return timePicker;
    //    }

    //    public override Dialog OnCreateDialog(Bundle savedInstanceState)
    //    {
    //        DateTime defaultDate = new DateTime(1900, 1, 1, 12, 0, 0);
    //        TimePickerDialog dialog = new TimePickerDialog(Activity, this, defaultDate.Hour, defaultDate.Minute, false);
    //        return dialog;
    //    }

    //    public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
    //    {
    //        DateTime selectedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hourOfDay, minute, 0);
    //        Log.Debug(TAG, selectedTime.ToLongDateString());
    //        _timeSelectedHandler(selectedTime);
    //    }
    //}
    public class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
    {
        public static readonly string TAG = "M:TimePickerFragment";

        private Activity _activity;
        private ConstantsAndTypes.TIMEPICKER_CONTEXT _timeContext;

        public TimePickerFragment() { }

        public TimePickerFragment(Activity context, ConstantsAndTypes.TIMEPICKER_CONTEXT pickerContext)
        {
            _activity = context;
            _timeContext = pickerContext;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime defaultDate = new DateTime(1900, 1, 1, 12, 0, 0);
            TimePickerDialog dialog = new TimePickerDialog(_activity, this, defaultDate.Hour, defaultDate.Minute, false);
            return dialog;
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            DateTime selectedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hourOfDay, minute, 0);
            Log.Debug(TAG, selectedTime.ToLongDateString());
            if (_activity != null)
                ((ITimePickerCallback)_activity).TimePicked(selectedTime, _timeContext);
        }
    }
}
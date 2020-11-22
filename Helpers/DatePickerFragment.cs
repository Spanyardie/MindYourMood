using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Util;


namespace com.spanyardie.MindYourMood.Helpers
{
    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        public static readonly string TAG = "M:DatePickerFragment";

        Action<DateTime> _dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            DatePickerFragment datePicker = new DatePickerFragment();
            datePicker._dateSelectedHandler = onDateSelected;
            return datePicker;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime defaultDate = Convert.ToDateTime(savedInstanceState.GetString("defaultDate"));
            DatePickerDialog dialog = new DatePickerDialog(Activity, this, defaultDate.Year, defaultDate.Month, defaultDate.Day);
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            // Seb: Well, that's a bit shit!!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Log.Debug(TAG, selectedDate.ToLongDateString());
            _dateSelectedHandler(selectedDate);
        }
    }
}
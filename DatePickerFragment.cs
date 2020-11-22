using System;

using Android.App;
using Android.OS;
using Android.Widget;


namespace com.spanyardie.MindYourMood
{
    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener

    {
        public static readonly string TAG = "M:DatePickerFragment";

        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> _dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            DatePickerFragment frag = new DatePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            //DatePicker appears to be FUBAR, it expects the month to be month - 1, but spits out the month - 1
            //so if we are in June it expects the input to be 5 not 6, but when the date is set still to 5 hence the OnDateSet + 1
            DatePickerDialog dialog = new DatePickerDialog(Activity, this, currently.Year, currently.Month - 1, currently.Day);
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            _dateSelectedHandler(selectedDate);
        }
    }
}
using Android.Util;
using Android.App;

namespace MindYourMood.Helpers
{
    public class TimePickerHelper
    {
        public const string TAG = "M:TimePickerHelper";

        private Activity _activity; //For the Fragment manager, don't care about any specific derived instance
        private ConstantsAndTypes.TIMEPICKER_CONTEXT _timeContext;

        public TimePickerHelper(Activity activity)
        {
            _activity = activity;
        }

        public void PickTime(ConstantsAndTypes.TIMEPICKER_CONTEXT timeContext)
        {
            _timeContext = timeContext;
            if (_activity != null)
            {
                TimePickerFragment timePicker = new TimePickerFragment(_activity, _timeContext);
                timePicker.Show(_activity.FragmentManager, TimePickerFragment.TAG);
            }
            else
            {
                Log.Error(TAG, "PickTime: Activity is null");
            }
        }
    }
}
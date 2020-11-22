using System;
using Android.App;
using Android.Content;
using Android.Util;
using Java.Util;
using Java.Lang;


namespace com.spanyardie.MindYourMood.Helpers
{
    public class AlarmHelper
    {
        public const string TAG = "M:AlarmHelper";

        private const ConstantsAndTypes.ALARM_INTERVALS _defaultAlarmInterval = ConstantsAndTypes.ALARM_INTERVALS.Daily;

        private Activity _activity = null;

        public AlarmHelper(Activity activity)
        {
            _activity = activity;
        }

        public void SetAlarm(Context context, int reminderID, DateTime alarmTime, string message, ConstantsAndTypes.ALARM_INTERVALS alarmInterval = _defaultAlarmInterval, ConstantsAndTypes.DAYS_OF_THE_WEEK dayOfWeek = ConstantsAndTypes.DAYS_OF_THE_WEEK.Undefined, bool repeating = false)
        {
            try
            {
                AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);

                Log.Info(TAG, "SetAlarm: Created AlarmManager");
                
                if (alarmManager != null)
                {
                    Intent intent = new Intent(_activity, typeof(AlarmReceiver));
                    intent.PutExtra("message", message);
                    Log.Info(TAG, "SetAlarm: Added to intent, message - " + message);
                    intent.PutExtra("reminderID", reminderID);
                    Log.Info(TAG, "SetAlarm: Added to intent, reminderID - " + reminderID.ToString());
                    Log.Info(TAG, "SetAlarm: Created Intent of type AlarmHelper (this)");

                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(_activity, reminderID, intent, PendingIntentFlags.UpdateCurrent);
                    Log.Info(TAG, "SetAlarm: Created PendingIntent with reminderID - " + reminderID.ToString());

                    Calendar calendar = Calendar.GetInstance(Locale.Default);
                    calendar.TimeInMillis = JavaSystem.CurrentTimeMillis();
                    Log.Info(TAG, "SetAlarm: Set Calendar time in millis to current time - " + calendar.TimeInMillis.ToString());

                    calendar.Set(CalendarField.HourOfDay, alarmTime.Hour);
                    Log.Info(TAG, "SetAlarm: Set calendar HourOfDay to " + alarmTime.Hour.ToString());
                    calendar.Set(CalendarField.Minute, alarmTime.Minute);
                    Log.Info(TAG, "SetAlarm: Set calendar Minute to " + alarmTime.Minute.ToString());

                    if (alarmInterval == ConstantsAndTypes.ALARM_INTERVALS.Daily)
                    {
                        //most are going to be daily - however, currently, if you set a daily alarm for a time that has already passed
                        //e.g it is currently 2.00pm when you set an alarm for 08.30am, then the alarm will sound immediately
                        //this is not desirable behaviour, so we are going to take the current time and compare to the alarm time
                        //and then add 1 day to the alarm time if it has already elapsed (effectively setting the NEXT scheduled alarm time)
                        if (JavaSystem.CurrentTimeMillis() - calendar.TimeInMillis > 0)
                        {
                            Log.Info(TAG, "SetAlarm: Alarm set was in the past - adding 1 day to ensure NEXT scheduled alarm time!");
                            calendar.TimeInMillis += (long)ConstantsAndTypes.ALARM_INTERVALS.Daily;
                        }
                        Log.Info(TAG, "SetAlarm: Setting repeating alarm for (approx) time in millis - " + calendar.TimeInMillis.ToString() + ", alarm interval - " + (AlarmManager.IntervalDay).ToString());
                        alarmManager.SetRepeating(AlarmType.RtcWakeup, calendar.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);
                    }
                    if (alarmInterval == ConstantsAndTypes.ALARM_INTERVALS.Weekly)
                    {
                        calendar.Set(CalendarField.DayOfWeek, (int)dayOfWeek);
                        Log.Info(TAG, "SetAlarm: Set day of the week to " + StringHelper.DayStringForConstant(dayOfWeek));
                        Log.Info(TAG, "SetAlarm: Setting repeating alarm for (approx) time in millis - " + calendar.TimeInMillis.ToString() + ", alarm interval - " + (AlarmManager.IntervalDay * 7).ToString());
                        alarmManager.SetRepeating(AlarmType.RtcWakeup, calendar.TimeInMillis, AlarmManager.IntervalDay * 7, pendingIntent);
                    }
                }
                else
                {
                    Log.Error(TAG, "SetAlarm: AlarmManager is NULL!");
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "SetAlarm: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting Alarm", "AlarmHelper.SetAlarm");
            }
        }

        public void CancelAlarm(int reminderID)
        {
            try
            {
                AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);

                Log.Info(TAG, "CancelAlarm: Created AlarmManager");

                if (alarmManager != null)
                {
                    Intent intent = new Intent(_activity, typeof(AlarmReceiver));
                    Log.Info(TAG, "CancelAlarm: Created Intent of type AlarmHelper (this)");

                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(_activity, reminderID, intent, PendingIntentFlags.UpdateCurrent);
                    Log.Info(TAG, "CancelAlarm: Created PendingIntent");

                    alarmManager.Cancel(pendingIntent);
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "CancelAlarm: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Cancelling Alarm", "AlarmHelper.CancelAlarm");
            }
        }
    }
}
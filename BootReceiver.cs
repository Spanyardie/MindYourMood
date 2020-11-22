using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Util;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Helpers;
using Java.Util;
using Java.Lang;

namespace com.spanyardie.MindYourMood
{
    [BroadcastReceiver(Enabled = true, Name = "com.spanyardie.MindYourMood.BootReceiver", Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    [IntentFilter(new[] { Intent.ActionMyPackageReplaced })]
    public class BootReceiver : BroadcastReceiver
    {
        public const string TAG = "M:BootReceiver";

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent != null && intent.Data != null)
            {
                if (!(intent.Action.Contains("BOOT_COMPLETED") || intent.Action.Contains("MY_PACKAGE_REPLACED")))
                {
                    Log.Info(TAG, "OnReceive: Not handling action received by BootReceiver!");
                    return;
                }
            }

            if (intent.Action.Contains("BOOT_COMPLETED"))
                Log.Info(TAG, "OnReceive: Detected BOOT_COMPLETED");
            if (intent.Action.Contains("MY_PACKAGE_REPLACED"))
                Log.Info(TAG, "OnReceive: Detected MY_PACKAGE_REPLACED");

            Log.Debug(TAG, "OnReceive: Entered...");
            //upon device reboot, or update of app,  we need to re-activate any alarms that have been set
            var isoLanguageCode = Locale.Default.ISO3Language.ToLower();
            Log.Debug(TAG, "OnReceive: Found ISO Language code - " + isoLanguageCode);
            Globals dbHelp = new Globals();
            SQLiteDatabase sqlDatabase = null;
            List<Medication> medicationList = new List<Medication>();

            string ofText = "";

            switch (isoLanguageCode)
            {
                case "eng":
                    ofText = "of ";
                    break;
                case "spa":
                case "fra":
                    ofText = "de ";
                    break;
            }

            try
            {
                Log.Debug(TAG, "OnReceive: Intent data string - " + intent.DataString);
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    Log.Info(TAG, "OnReceive: Successfully opened database");
                    medicationList = dbHelp.GetAllMedicationItems();
                    if (medicationList.Count > 0)
                    {
                        Log.Info(TAG, "OnReceive: Found " + medicationList.Count.ToString() + " medication items");
                        foreach (var med in medicationList)
                        {
                            Log.Info(TAG, "OnReceive: Processing medication - " + med.MedicationName);
                            if (med.MedicationSpread.Count > 0)
                            {
                                Log.Info(TAG, "OnReceive: Medication '" + med.MedicationName + "' has " + med.MedicationSpread.Count.ToString() + " spreads.");
                                foreach (var spread in med.MedicationSpread)
                                {
                                    if (spread.MedicationTakeReminder != null)
                                    {
                                        Log.Info(TAG, "OnReceive: Found medication reminder for " + spread.MedicationTakeReminder.MedicationTime.ToString());
                                        switch (med.PrescriptionType.PrescriptionType)
                                        {
                                            case ConstantsAndTypes.PRESCRIPTION_TYPE.Daily:
                                                Log.Info(TAG, "OnReceive: Setting alarm for Daily reminder");
                                                SetAlarm(
                                                    context,
                                                    spread.MedicationTakeReminder.ID,
                                                    spread.MedicationTakeReminder.MedicationTime,
                                                    spread.Dosage.ToString() + "mg " + ofText + med.MedicationName,
                                                    ConstantsAndTypes.ALARM_INTERVALS.Daily,
                                                    ConstantsAndTypes.DAYS_OF_THE_WEEK.Undefined,
                                                    true
                                                );
                                                break;
                                            case ConstantsAndTypes.PRESCRIPTION_TYPE.Weekly:
                                                SetAlarm(
                                                    context,
                                                    spread.MedicationTakeReminder.ID,
                                                    spread.MedicationTakeReminder.MedicationTime,
                                                    spread.Dosage.ToString() + "mg " + ofText + med.MedicationName,
                                                    ConstantsAndTypes.ALARM_INTERVALS.Weekly,
                                                    spread.MedicationTakeReminder.MedicationDay,
                                                    true
                                                );
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "OnReceive: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
            }
        }
        private void SetAlarm(Context context, int reminderID, DateTime alarmTime, string message, ConstantsAndTypes.ALARM_INTERVALS alarmInterval = ConstantsAndTypes.ALARM_INTERVALS.Daily, ConstantsAndTypes.DAYS_OF_THE_WEEK dayOfWeek = ConstantsAndTypes.DAYS_OF_THE_WEEK.Undefined, bool repeating = false)
        {
            try
            {
                AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

                Log.Info(TAG, "SetAlarm: Created AlarmManager");

                if (alarmManager != null)
                {
                    Intent intent = new Intent(context, typeof(AlarmReceiver));
                    intent.PutExtra("message", message);
                    Log.Info(TAG, "SetAlarm: Added to intent, message - " + message);
                    intent.PutExtra("reminderID", reminderID);
                    Log.Info(TAG, "SetAlarm: Added to intent, reminderID - " + reminderID.ToString());
                    Log.Info(TAG, "SetAlarm: Created Intent of type AlarmHelper (this)");

                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, reminderID, intent, PendingIntentFlags.UpdateCurrent);
                    Log.Info(TAG, "SetAlarm: Created PendingIntent with reminderID - " + reminderID.ToString());

                    Calendar calendar = Calendar.GetInstance(Locale.Default);
                    calendar.TimeInMillis = JavaSystem.CurrentTimeMillis();
                    Log.Info(TAG, "SetAlarm: Set Calendar time in millis to current time - " + calendar.TimeInMillis.ToString());

                    if (dayOfWeek != ConstantsAndTypes.DAYS_OF_THE_WEEK.Undefined)
                    {
                        calendar.Set(CalendarField.DayOfWeek, (int)dayOfWeek);
                        Log.Info(TAG, "SetAlarm: Set day of the week to " + StringHelper.DayStringForConstant(dayOfWeek));
                    }
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
                    }


                    if (!repeating)
                    {
                        Log.Info(TAG, "SetAlarm: Setting one time alarm for (approx) time in millis - " + calendar.TimeInMillis.ToString());
                        alarmManager.Set(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
                    }
                    else
                    {
                        Log.Info(TAG, "SetAlarm: Setting repeating alarm for (approx) time in millis - " + calendar.TimeInMillis.ToString() + ", alarm interval - " + ((long)alarmInterval).ToString());
                        alarmManager.SetRepeating(AlarmType.RtcWakeup, calendar.TimeInMillis, (long)alarmInterval, pendingIntent);
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
            }
        }
    }
}
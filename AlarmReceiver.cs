using System;

using Android.Content;
using Android.Util;
using Android.Support.V4.App;
using com.spanyardie.MindYourMood.Model;

namespace com.spanyardie.MindYourMood
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public const string TAG = "M:AlarmReceiver";

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                var message = intent.GetStringExtra("message");
                var title = "Time to take your Medication";
                var reminderID = intent.GetIntExtra("reminderID", 9999);

                int internalType = GetAlarmNotificationSetting(); //All

                var builderCompat = new NotificationCompat.Builder(context)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetAutoCancel(true)
                    .SetDefaults(internalType)
                    .SetSmallIcon(Resource.Drawable.clockicon24x24);

                var notificationCompat = builderCompat.Build();

                var managerCompat = NotificationManagerCompat.From(context);
                managerCompat.Notify(reminderID, notificationCompat);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnReceive: Exception - " + e.Message);
            }
        }

        private int GetAlarmNotificationSetting()
        {
            Globals dbHelp = new Globals();
            int retVal = -1;

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            if (sqlDatabase != null && sqlDatabase.IsOpen)
            {
                string[] arrColumns = new string[3];

                arrColumns[0] = "ID";
                arrColumns[1] = "SettingKey";
                arrColumns[2] = "SettingValue";

                try
                {
                    var settingData = sqlDatabase.Query("Settings", arrColumns, null, null, null, null, null);
                    if (settingData != null)
                    {
                        GlobalData.Settings.Clear();
                        var count = settingData.Count;
                        if (count > 0)
                        {
                            settingData.MoveToFirst();
                            for (var loop = 0; loop < count; loop++)
                            {
                                var key = settingData.GetString(settingData.GetColumnIndex("SettingKey"));
                                if(key == "AlarmNotificationType")
                                {
                                    var value = Convert.ToInt32(settingData.GetString(settingData.GetColumnIndex("SettingValue")));
                                    retVal = (value == 0 ? -1 : value);
                                    break;
                                }
                                settingData.MoveToNext();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (sqlDatabase != null && sqlDatabase.IsOpen)
                        sqlDatabase.Close();
                    Log.Error(TAG, "GetAlarmNotificationSetting: Exception - " + e.Message);
                }
                return retVal;
            }
            else
            {
                return -1;
            }
        }
    }
}
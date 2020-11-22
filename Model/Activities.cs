using System;
using System.Collections.Generic;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class Activities : ActivitiesBase
    {
        public const string TAG = "M:Activities";

        public List<ActivityTime> ActivityTimes { get; set; }

        public Activities()
        {
            ActivityID = -1;
            ActivityDate = new DateTime(1900, 1, 1, 23, 59, 59);
            ActivityTimes = new List<ActivityTime>();
            IsNew = true;
            IsDirty = false;
        }

        public int GetTotalNumberOfActivities()
        {
            var count = 0;

            foreach(var actTime in ActivityTimes)
            {
                if (actTime.ActivityTimeID != -1)
                    count++;
            }

            return count;
        }

        public void Remove(SQLiteDatabase sqlDatabase)
        {
            if(sqlDatabase.IsOpen && !IsNew)
            {
                string commandText = "DELETE FROM Activities WHERE [ID] = " + ActivityID;

                try
                {
                    sqlDatabase.ExecSQL(commandText);

                    commandText = "DELETE FROM ActivityTimes WHERE [ActivityID] = " + ActivityID;

                    sqlDatabase.ExecSQL(commandText);
                }
                catch(Exception e)
                {
                    throw new Exception("Removing Activity from database failed - " + e.Message);
                }
            }
        }

        public void Save(SQLiteDatabase sqlDatabase)
        {
            if(sqlDatabase.IsOpen)
            {
                if(IsNew)
                {
                    Log.Info(TAG, "Save: Attempting to Save new Activity...");
                    try
                    {
                        ContentValues values = new ContentValues();
                        values.Put("ActivityDate", string.Format("{0:yyyy-MM-dd HH:mm:ss}", ActivityDate));
                        ActivityID = (int)sqlDatabase.Insert("Activities", null, values);
                        Log.Info(TAG, "Save: Saved Activity with ID - " + ActivityID.ToString());

                        foreach(ActivityTime activityTime in ActivityTimes)
                        {
                            activityTime.IsNew = true;
                            if (!string.IsNullOrEmpty(activityTime.ActivityName.Trim()))
                            {
                                Log.Info(TAG, "Save: Found ActivityTime - " + activityTime.ActivityName);
                                activityTime.ActivityID = ActivityID;
                                Log.Info(TAG, "Save: Assigned ID " + ActivityID.ToString() + " for " + activityTime.ActivityName);
                                Log.Info(TAG, "Save: Calling Save for the ActivityTime...");
                                activityTime.Save(sqlDatabase);
                            }
                        }

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch(Exception newE)
                    {
                        throw new Exception("Unable to save Activity - " + newE.Message);
                    }
                }

                if(IsDirty)
                {
                    Log.Info(TAG, "Save: No need to update existing Activity");
                    try
                    {
                        foreach (ActivityTime activityTime in ActivityTimes)
                        {
                            Log.Info(TAG, "Save: Attempting to Save ActivityTime with ActivityID " + activityTime.ActivityID.ToString() + " (reported as " + (activityTime.IsNew ? "New" : "Dirty") + " item)");
                            if (activityTime.IsNew)
                                activityTime.ActivityID = ActivityID;
                            activityTime.Save(sqlDatabase);
                        }

                        IsDirty = false;
                    }
                    catch (Exception dirtyE)
                    {
                        throw new Exception("Unable to Update Activity - " + dirtyE.Message);
                    }
                }
            }
        }
    }
}
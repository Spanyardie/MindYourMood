using System;
using Android.Content;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class ActivityTime : ActivityTimeBase
    {
        public const string TAG = "M:ActivityTime";

        public ActivityTime()
        {
            ActivityTimeID = -1;
            ActivityID = -1;
            ActivityName = "";
            ActivityTime = Helpers.ConstantsAndTypes.ACTIVITY_HOURS.SixAMToEightAM;
            Achievement = 0;
            Intimacy = 0;
            Pleasure = 0;
            IsNew = true;
            IsDirty = false;
        }

        public void Save(SQLiteDatabase sqlDatabase)
        {
            if(sqlDatabase != null && sqlDatabase.IsOpen)
            {
                //ccannot save without a corresponding ActivityID
                if(ActivityID != -1)
                {
                    Log.Info(TAG, "Save: Attempting to Save ActivityTime for Activity with ID " + ActivityID.ToString());
                    try
                    {
                        if (!string.IsNullOrEmpty(ActivityName))
                        {
                            Log.Info(TAG, "Save: Found Activity Name - " + ActivityName);
                            if(!(IsNew || IsDirty))
                            {
                                Log.Info(TAG, "Save: Unchanged ActivityTime detected - no work to do!");
                                return;
                            }
                            ContentValues values = new ContentValues();
                            values.Put("ActivityName", ActivityName);
                            values.Put("ActivityTime", (int)ActivityTime);
                            values.Put("Achievement", Achievement);
                            values.Put("Intimacy", Intimacy);
                            values.Put("Pleasure", Pleasure);
                            if (IsNew)
                            {
                                Log.Info(TAG, "Save: Is a new ActivityTime, using ActivityID - " + ActivityID.ToString());
                                values.Put("ActivityID", ActivityID);
                                ActivityTimeID = (int)sqlDatabase.Insert("ActivityTimes", null, values);
                                Log.Info(TAG, "Save: Saved ActivityTime with ID " + ActivityTimeID.ToString());
                                IsNew = false;
                                IsDirty = false;
                            }
                            if (IsDirty)
                            {
                                Log.Info(TAG, "Save: Found existing ActivityTime with ActivityID - " + ActivityID.ToString() + " and ActivityTimesID - " + ActivityTimeID.ToString());
                                var whereClause = "ActivityID = " + ActivityID + " AND ActivityTimesID = " + ActivityTimeID;
                                sqlDatabase.Update("ActivityTimes", values, whereClause, null);
                                Log.Info(TAG, "Save: Updated successfully");
                                IsDirty = false;
                            }
                        }
                        else
                        {
                            Log.Info(TAG, "Save: Skipping Save of ActivityTime because its name was null!");
                        }
                    }
                    catch(Exception e)
                    {
                        Log.Error(TAG, "Save: Exception - " + e.Message);
                    }
                }
                else
                {
                    throw new Exception("Cannot save Activity time details because Activity ID is invalid (did you forget to save the Main Activity?)");
                }
            }
        }
    }
}
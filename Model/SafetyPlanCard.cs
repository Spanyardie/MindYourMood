using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Database.Sqlite;
using Android.Database;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class SafetyPlanCard : SafetyPlanCardBase
    {
        public const string TAG = "M:SafetyPlanCard";

        public SafetyPlanCard()
        {
            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen && !IsNew)
            {
                Log.Info(TAG, "Remove: Attempting to remove Safety Plan card with ID " + ID.ToString());
                string commandText = "DELETE FROM SafetyPlanCards WHERE [ID] = " + ID;

                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                    Log.Info(TAG, "Remove: Successfully removed Safety Plan Card");
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "Remove: Exception - " + e.Message);
                    throw new SQLException("Removing Safety Plan Card from database failed - " + e.Message);
                }
            }
        }

        public void Save(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen)
            {
                if (IsNew)
                {
                    try
                    {
                        ContentValues values = new ContentValues();
                        values.Put("CalmMyself", CalmMyself.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("TellMyself", TellMyself.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("WillCall", WillCall.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("WillGoTo", WillGoTo.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        ID = (int)sqLiteDatabase.Insert("SafetyPlanCards", null, values);

                        Log.Info(TAG, "Save: Saved to database successfully with an ID of " + ID.ToString());
                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        Log.Error(TAG, "Save: Exception - " + newE.Message); 
                        throw new Exception("Unable to Save Safety Plan Card - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        string whereClause = "ID = " + ID;
                        ContentValues values = new ContentValues();

                        values.Put("CalmMyself", CalmMyself.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("TellMyself", TellMyself.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("WillCall", WillCall.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("WillGoTo", WillGoTo.Trim().Replace("'", "''").Replace("\"", "\"\""));

                        sqLiteDatabase.Update("SafetyPlanCards", values, whereClause, null);

                        IsDirty = false;
                    }
                    catch (SQLException dirtyE)
                    {
                        Log.Error(TAG, "Save: Exception - " + dirtyE.Message);
                        throw new Exception("Unable to Update Safety Plan Card - " + dirtyE.Message);
                    }
                }
            }

        }
    }
}
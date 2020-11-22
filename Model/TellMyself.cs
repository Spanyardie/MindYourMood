using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class TellMyself : TellMyselfBase
    {
        public static string TAG = "M:TellMyself";

        public TellMyself()
        {
            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen)
            {
                string commandText = "DELETE FROM TellMyself WHERE [ID] = " + ID;
                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                    Log.Info(TAG, "Remove: Removed Tell Myself item with ID " + ID.ToString());
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "Remove: Exception - " + e.Message);
                    throw new Exception("Removing Tell Myself entry from database failed - " + e.Message);
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
                        values.Put("TellType", (int)TellType);
                        values.Put("TellText", TellText.Trim());
                        values.Put("TellTitle", TellTitle.Trim());

                        ID = (int)sqLiteDatabase.Insert("TellMyself", null, values);
                        Log.Info(TAG, "Save: Added Tell Myself item - ID " + ID.ToString());

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        Log.Error(TAG, "Save: Exception - " + newE.Message);
                        throw new Exception("Unable to save Tell Myself entry to database - " + newE.Message);
                    }
                }
            }
            else
            {
                Log.Error(TAG, "Save: Sqlite database is NOT open!");
            }
        }
    }
}
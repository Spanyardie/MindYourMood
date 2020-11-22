using System;
using Android.Content;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class MoodList : MoodListBase
    {

        public MoodList()
        {
            IsNew = true;
            IsDirty = false;
            IsDefault = "false";
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
        if (sqLiteDatabase.IsOpen)
        {
                string commandText = "DELETE FROM MoodList WHERE [MoodID] = " + MoodId;
                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    throw new Exception("Removing Mood from database failed - " + e.Message);
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
                        values.Put("MoodName", MoodName);
                        values.Put("IsoCountry", MoodIsoCountryAlias);
                        values.Put("IsDefault", IsDefault);
                        MoodId = (int)sqLiteDatabase.Insert("MoodList", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save Mood to database - " + newE.Message);
                    }
                }
            }
        }
    }
}
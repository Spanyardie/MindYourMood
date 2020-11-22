using System;
using Android.Content;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class RerateMood : RerateMoodBase
    {
        private bool _fromMood = false;

        public bool FromMood
        {
            get
            {
                return _fromMood;
            }

            set
            {
                _fromMood = value;
            }
        }

        public RerateMood()
        {
            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
        if (sqLiteDatabase.IsOpen)
        {
                string commandText = "DELETE FROM RerateMood WHERE RerateMoodID = " + RerateMoodId + " AND ThoughtRecordID = " + ThoughtRecordId;

                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    throw new Exception("Removing Rerate Mood from database failed - " + e.Message);
                }
            }
        }

        public string GetMoodName(long moodsId, SQLiteDatabase sqLiteDatabase)
        {
        try
        {
                if (sqLiteDatabase.IsOpen)
                {
                    string[] columns = { "MoodName" };
                    string whereClause = "MoodID = " + MoodListId;

                    var data = sqLiteDatabase.Query("MoodList", columns, whereClause,null, null, null, null);

                    if (data != null)
                    {
                        if (data.MoveToFirst())
                        {
                            return data.GetString(0);
                        }
                    }
                }
                return "";
            }
        catch (Exception e)
        {
                throw new Exception("Getting Mood Name failed - " + e.Message);
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
                        values.Put("ThoughtRecordID", ThoughtRecordId);
                        values.Put("MoodsID", MoodsId);
                        values.Put("MoodListID", MoodListId);
                        values.Put("MoodRating", MoodRating);

                        RerateMoodId = (int)sqLiteDatabase.Insert("RerateMood", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save RerateMood - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        ContentValues values = new ContentValues();
                        values.Put("ThoughtRecordID", ThoughtRecordId);
                        values.Put("MoodListID", MoodListId);
                        values.Put("MoodRating", MoodRating);
                        string whereClause = "MoodsID = " + MoodsId;

                        sqLiteDatabase.Update("RerateMood", values, whereClause, null);

                        IsDirty = false;
                    }
                    catch (Exception dirtyE)
                    {
                        throw new Exception("Unable to Update RerateMood - " + dirtyE.Message);
                    }
                }
            }
        }


        public string toString(SQLiteDatabase sqlDatabase)
        {
            try
            {
                string ret = "";

                ret = GetMoodName(MoodListId, sqlDatabase) + ", " + MoodRating + "%";

                return ret;
            }
            catch
            {
                return "";
            }
        }
    }
}
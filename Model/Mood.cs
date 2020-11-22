using System;
using Android.Content;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class Mood : MoodBase
    {

        public Mood()
        {
            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
        if (sqLiteDatabase.IsOpen && !IsNew)
        {
                string commandText = "DELETE FROM Mood WHERE [MoodsID] = " + MoodsId + " AND [ThoughtRecordID] = " + ThoughtRecordId + " AND [MoodListID] = " + MoodListId;

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

        public string GetMoodName(long moodsId, SQLiteDatabase sqLiteDatabase)
        {
        try
        {
                if (sqLiteDatabase.IsOpen)
                {
                    string commandText = "SELECT [MoodName] FROM MoodList WHERE [MoodID] = " + MoodListId;
                    string[] columns = { "MoodName" };
                    string whereClause = "MoodID = " + MoodListId;

                    var data = sqLiteDatabase.Query("MoodList", columns, whereClause, null, null, null, null);

                    if (data != null)
                    {
                        if (data.MoveToFirst())
                        {
                            string ret = data.GetString(0);
                            return ret;
                        }
                    }
                }
                return "";
            }
        catch (Exception e)
        {
                throw new Exception("Failed to get Mood Name - " + e.Message);
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
                        values.Put("MoodListID", MoodListId);
                        values.Put("MoodRating", MoodRating);

                        MoodsId = (int)sqLiteDatabase.Insert("Mood", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save Mood - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        string commandText = "UPDATE Mood SET [ThoughtRecordID] = " + ThoughtRecordId + ", " +
                                                       " [MoodListID] = " + MoodListId + ", " +
                                                       " [MoodRating] = " + MoodRating +
                                   " WHERE [MoodsID] = " + MoodsId;

                        ContentValues values = new ContentValues();
                        values.Put("ThoughtRecordID", ThoughtRecordId);
                        values.Put("MoodListID", MoodListId);
                        values.Put("MoodRating", MoodRating);
                        string whereClause = "MoodsID = " + MoodsId;

                        sqLiteDatabase.Update("Mood", values, whereClause, null);

                        IsDirty = false;
                    }
                    catch (Exception dirtyE)
                    {
                        throw new Exception("Unable to Update Mood - " + dirtyE.Message);
                    }
                }
            }
        }


    public string toString(SQLiteDatabase sqlDatabase)
        {
            string ret = "";

            try
            {
                ret = GetMoodName(MoodListId, sqlDatabase) + ", " + MoodRating + "%";
            }
            catch
            {
                ret = "";
            }

            return ret;
        }

    }
}
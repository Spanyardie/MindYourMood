using System;
using Android.Content;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class AlternativeThoughts : AlternativeThoughtsBase 
    {
        public const string TAG = "M:AlternativeThoughts";

        public AlternativeThoughts()
        {
            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                if (sqLiteDatabase.IsOpen)
                {
                    string commandText = "DELETE FROM AlternativeThoughts WHERE [AlternativeThoughtsID] = " + AlternativeThoughtsId + " AND [ThoughtRecordID] = " + ThoughtRecordId;

                    try
                    {
                        sqLiteDatabase.RawQuery(commandText, null);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Unable to remove Alternative Thought from database - " + e.Message);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Remove: Exception - " + e.Message);
            }
        }

        public void Save(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                if (sqLiteDatabase.IsOpen)
                {
                    if (IsNew)
                    {
                        try
                        {
                            string[] columns =
                                    {
                                    "ThoughtRecordID",
                                    "Alternative",
                                    "BeliefRating"
                            };

                            ContentValues values = new ContentValues();

                            values.Put("ThoughtRecordID", ThoughtRecordId);
                            values.Put("Alternative", Alternative.Trim().Replace("'", "''").Replace("\"", "\"\""));
                            values.Put("BeliefRating", BeliefRating);

                            AlternativeThoughtsId = (int)sqLiteDatabase.Insert("AlternativeThoughts", null, values);

                            IsNew = false;
                            IsDirty = false;
                        }
                        catch (Exception newE)
                        {
                            throw new Exception("Unable to Save Alternative Thought in database - " + newE.Message);
                        }
                    }

                    if (IsDirty)
                    {
                        try
                        {
                            ContentValues values = new ContentValues();
                            values.Put("ThoughtRecordID", ThoughtRecordId);
                            values.Put("Alternative", Alternative.Trim().Replace("'", "''").Replace("\"", "\"\""));
                            values.Put("BeliefRating", BeliefRating);
                            string whereClause = "AlternativeThoughtsID = " + AlternativeThoughtsId;
                            sqLiteDatabase.Update("AlternativeThoughts", values, whereClause, null);

                            IsDirty = false;
                        }
                        catch (Exception dirtyE)
                        {
                            throw new Exception("Unable to Update Alternative Thought in database - " + dirtyE.Message);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Save: Exception - " + e.Message);
            }
        }

    public string toString()
        {
            return Alternative.Trim() + "\r\nBelief Rating: " + BeliefRating + "%";
        }

    }
}
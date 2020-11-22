using System;
using Android.Content;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class AutomaticThoughts : AutomaticThoughtsBase
    {

        public AutomaticThoughts()
        {
            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
        if (sqLiteDatabase.IsOpen)
        {
                string commandText = "DELETE FROM AutomaticThoughts WHERE [AutomaticThoughtsID] = " + AutomaticThoughtsId + " AND [ThoughtRecordID] = " + ThoughtRecordId;

                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to remove Alternative Thought from database - " + e.Message);
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
                        values.Put("ThoughtRecordID", ThoughtRecordId);
                        values.Put("Thought", Thought.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("HotThought", IsHotThought?1:0);
                        AutomaticThoughtsId = (int)sqLiteDatabase.Insert("AutomaticThoughts", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save Automatic Thought in database - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        String whereClause = "AutomaticThoughtsID = " + AutomaticThoughtsId;

                        IsDirty = false;
                        ContentValues values = new ContentValues();
                        values.Put("ThoughtRecordID", ThoughtRecordId);
                        values.Put("Thought", Thought.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("HotThought", IsHotThought ? 1 : 0);
                        sqLiteDatabase.Update("AlternativeThoughts", values, whereClause, null);
                    }
                    catch (Exception dirtyE)
                    {
                        throw new Exception("Unable to Update Automatic Thought in database - " + dirtyE.Message);
                    }
                }
            }
        }

    public string toString()
        {
            string ret = "";

            ret = Thought.Trim() + (IsHotThought ? " - HOT THOUGHT" : "");

            return ret;
        }

    }
}
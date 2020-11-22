using System;
using Android.Content;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class EvidenceForHotThought : EvidenceForHotThoughtBase
    {

        public EvidenceForHotThought()
        {
            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
        if (sqLiteDatabase.IsOpen)
        {
                string commandText = "DELETE FROM EvidenceForHotThought WHERE [EvidenceForHotThoughtID] = " + EvidenceForHotThoughtId + " AND [ThoughtRecordID] = " + ThoughtRecordId + " AND [AutomaticThoughtsID] = " + AutomaticThoughtsId;

                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to remove Evidence For Hot Thought from database - " + e.Message);
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
                        values.Put("AutomaticThoughtsID", AutomaticThoughtsId);
                        values.Put("Evidence", Evidence.Trim().Replace("'", "''").Replace("\"", "\"\""));

                        EvidenceForHotThoughtId = (int)sqLiteDatabase.Insert("EvidenceForHotThought", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save Evidence For Hot Thought in database - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        string whereClause = "EvidenceForHotThoughtID = " + EvidenceForHotThoughtId;

                        ContentValues values = new ContentValues();
                        values.Put("ThoughtRecordID", ThoughtRecordId);
                        values.Put("AutomaticThoughtsID", AutomaticThoughtsId);
                        values.Put("Evidence", Evidence.Trim().Replace("'", "''").Replace("\"", "\"\""));

                        sqLiteDatabase.Update("EvidenceAgainstHotThought", values, whereClause, null);

                        IsDirty = false;
                    }
                    catch (Exception dirtyE)
                    {
                        throw new Exception("Unable to Update Evidence For Hot Thought in database - " + dirtyE.Message);
                    }
                }
            }
        }


    public string toString()
        {
            return Evidence.Trim();
        }

    }
}
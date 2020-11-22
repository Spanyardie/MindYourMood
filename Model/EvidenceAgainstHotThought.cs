using System;
using Android.Content;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class EvidenceAgainstHotThought : EvidenceAgainstHotThoughtBase
    {


        public EvidenceAgainstHotThought()
        {
            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
        if (sqLiteDatabase.IsOpen)
        {
                string commandText = "DELETE FROM EvidenceAgainstHotThought WHERE [EvidenceAgainstHotThoughtID] = " + EvidenceAgainstHotThoughtId + " AND [ThoughtRecordID] = " + ThoughtRecordId + " AND [AutomaticThoughtsID] = " + AutomaticThoughtsId;

                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to remove Evidence against Hot Thought from database - " + e.Message);
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

                        EvidenceAgainstHotThoughtId = (int)sqLiteDatabase.Insert("EvidenceAgainstHotThought", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save Evidence Against Hot Thought in database - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        string whereClause = "EvidenceAgainstHotThoughtID = " + EvidenceAgainstHotThoughtId;

                        ContentValues values = new ContentValues();
                        values.Put("ThoughtRecordID", ThoughtRecordId);
                        values.Put("AutomaticThoughtsID", AutomaticThoughtsId);
                        values.Put("Evidence", Evidence.Trim().Replace("'", "''").Replace("\"", "\"\""));

                        sqLiteDatabase.Update("EvidenceAgainstHotThought", values, whereClause, null);

                        IsDirty = false;
                    }
                    catch (Exception dirtyE)
                    {
                        throw new Exception("Unable to Update Evidence Against Hot Thought in database - " + dirtyE.Message);
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
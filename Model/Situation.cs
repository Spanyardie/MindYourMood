using System;
using Android.Content;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class Situation : SituationBase
    {


        public Situation()
        {
            IsNew = true;
            IsDirty = false;

            What = "";
            When = "";
            Where = "";
            Who = "";
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
        if (sqLiteDatabase.IsOpen)
        {
                string commandText = "DELETE FROM Situation WHERE [SituationID] = " + SituationId + " AND [ThoughtRecordID] = " + ThoughtRecordId;

                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    throw new Exception("Removing of Situation from database failed - " + e.Message);
                }
            }
        }

        public void Save(SQLiteDatabase sqLiteDatabase)
        {
        if(sqLiteDatabase.IsOpen)
        {
                if (IsNew)
                {
                    try
                    {
                        ContentValues values = new ContentValues();
                        values.Put("ThoughtRecordID", ThoughtRecordId);
                        string what = What.Trim().Replace("'", "''").Replace("\"", "\"\"");
                        values.Put("What", what);
                        string when = When.Trim().Replace("'", "''").Replace("\"", "\"\"");
                        values.Put("[When]", when);
                        string where = Where.Trim().Replace("'", "''").Replace("\"", "\"\"");
                        values.Put("[Where]", where);
                        string who = Who.Trim().Replace("'", "''").Replace("\"", "\"\"");
                        values.Put("Who", who);

                        SituationId = (int)sqLiteDatabase.Insert("Situation", null, values);
    
                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save Situation to database - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        ContentValues values = new ContentValues();
                        values.Put("[ThoughtRecordID]", ThoughtRecordId);
                        values.Put("Who", Who.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("What", What.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("When", When.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("Where", Where.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        string whereClause = "SituationID = " + SituationId;
                        sqLiteDatabase.Update("Situation", values, whereClause, null);

                        IsDirty = false;
                    }
                    catch (Exception dirtyE)
                    {
                        throw new Exception("Unable to Update Situation in the database - " + dirtyE.Message);
                    }
                }
            }
        }


    public string toString()
        {
            string ret = "";
            ret = "What: " + What.Trim() + "\r\n";
            ret += "Who: " + Who.Trim() + "\r\n";
            ret += "Where: " + Where.Trim() + "\r\n";
            ret += "When: " + When.Trim();

            return ret;
        }

    }
}
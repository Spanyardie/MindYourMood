using System;
using Android.Content;
using Android.Database.Sqlite;
using Android.Util;
using com.spanyardie.MindYourMood.Model.LowLevel;
using System.Collections.Generic;

namespace com.spanyardie.MindYourMood.Model
{
    public class ProblemIdea : ProblemIdeaBase
    {
        public const string TAG = "M:ProblemIdea";

        public List<ProblemProAndCon> ProsAndCons { get; set; }

        public ProblemIdea()
        {
            ProblemIdeaID = -1;
            ProblemStepID = -1;
            ProblemID = -1;
            ProblemIdeaText = "";
            IsNew = true;
            IsDirty = false;

            ProsAndCons = new List<ProblemProAndCon>();
        }

        public void Remove()
        {
            SQLiteDatabase sqlDatabase = null;

            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    var sql = "DELETE FROM [ProblemIdeas] WHERE ProblemIdeaID = " + ProblemIdeaID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed ProblemIdea with ID " + ProblemIdeaID.ToString() + " successfully");

                    //Remove any solution plan
                    dbHelp.RemoveSolutionForIdea(ProblemIdeaID);

                    foreach(var proAndCon in ProsAndCons)
                    {
                        proAndCon.Remove();
                    }

                    sqlDatabase.Close();
                }
                else
                {
                    Log.Error(TAG, "Remove: SQLite database is null or was not opened - remove failed");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Remove: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                throw new System.Exception("Error during removal of Problem Idea - " + e.Message, e);
            }
        }

        public void Save()
        {
            SQLiteDatabase sqlDatabase = null;
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    if (sqlDatabase.IsOpen)
                    {
                        ContentValues values = new ContentValues();
                        values.Put("ProblemStepID", ProblemStepID);
                        values.Put("ProblemID", ProblemID);
                        values.Put("ProblemIdeaText", ProblemIdeaText.Trim());

                        if (IsNew)
                        {
                            ProblemIdeaID = (int)sqlDatabase.Insert("ProblemIdeas", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            string whereClause = "ProblemIdeaID = ?";
                            sqlDatabase.Update("ProblemIdeas", values, whereClause, new string[] { ProblemIdeaID.ToString() });
                            IsDirty = false;
                        }
                        sqlDatabase.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Save: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                throw new Exception("Error during save of Problem Idea - " + e.Message, e);
            }
        }
    }
}
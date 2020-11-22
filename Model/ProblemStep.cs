using Android.Content;
using Android.Database.Sqlite;
using Android.Util;
using com.spanyardie.MindYourMood.Model.LowLevel;
using System;
using System.Collections.Generic;

namespace com.spanyardie.MindYourMood.Model
{
    public class ProblemStep : ProblemStepBase
    {
        public const string TAG = "M:ProblemStep";

        public List<ProblemIdea> ProblemStepIdeas { get; set; }

        public ProblemStep()
        {
            ProblemStepID = -1;
            ProblemID = -1;
            ProblemStep = "";
            PriorityOrder = 0;
            IsNew = true;
            IsDirty = false;

            ProblemStepIdeas = new List<ProblemIdea>();
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
                    var sql = "DELETE FROM [ProblemSteps] WHERE ProblemStepID = " + ProblemStepID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed ProblemStep with ID " + ProblemStepID.ToString() + " successfully");

                    foreach(var idea in ProblemStepIdeas)
                    {
                        idea.Remove();
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
                        values.Put("ProblemID", ProblemID);
                        values.Put("ProblemStep", ProblemStep.Trim());
                        values.Put("PriorityOrder", PriorityOrder);

                        if (IsNew)
                        {
                            ProblemStepID = (int)sqlDatabase.Insert("ProblemSteps", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            string whereClause = "ProblemStepID = ?";
                            sqlDatabase.Update("ProblemSteps", values, whereClause, new string[] { ProblemStepID.ToString() });
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
            }
        }
    }
}
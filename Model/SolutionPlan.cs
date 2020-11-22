using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class SolutionPlan : SolutionPlanBase
    {
        public const string TAG = "M:SolutionPlan";

        public SolutionPlan()
        {
            SolutionPlanID = -1;
            ProblemIdeaID = -1;
            SolutionStep = "";
            PriorityOrder = 0;
            IsNew = true;
            IsDirty = false;
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
                    var sql = "DELETE FROM [SolutionPlans] WHERE SolutionPlanID = " + SolutionPlanID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed SolutionPlan with ID " + SolutionPlanID.ToString() + " successfully");

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
                        values.Put("ProblemIdeaID", ProblemIdeaID);
                        values.Put("SolutionStep", SolutionStep.Trim());
                        values.Put("PriorityOrder", PriorityOrder);

                        if (IsNew)
                        {
                            SolutionPlanID = (int)sqlDatabase.Insert("SolutionPlans", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            string whereClause = "SolutionPlanID = ?";
                            sqlDatabase.Update("SolutionPlans", values, whereClause, new string[] { SolutionPlanID.ToString() });
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
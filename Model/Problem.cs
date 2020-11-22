using Android.Content;
using Android.Database.Sqlite;
using Android.Util;
using com.spanyardie.MindYourMood.Model.LowLevel;
using System;
using System.Collections.Generic;

namespace com.spanyardie.MindYourMood.Model
{
    public class Problem : ProblemBase
    {
        public const string TAG = "M:Problem";

        public List<ProblemStep> ProblemSteps { get; set; }

        public Problem()
        {
            ProblemID = -1;
            ProblemText = "";
            IsNew = true;
            IsDirty = false;

            ProblemSteps = new List<ProblemStep>();
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
                    var sql = "DELETE FROM [Problems] WHERE ProblemID = " + ProblemID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Problem with ID " + ProblemID.ToString() + " successfully");

                    //also remove any problem steps associated with this problem
                    foreach (var step in ProblemSteps)
                    {
                        step.Remove();
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
                        values.Put("ProblemText", ProblemText.Trim());

                        if (IsNew)
                        {
                            ProblemID = (int)sqlDatabase.Insert("Problems", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            string whereClause = "ProblemID = ?";
                            sqlDatabase.Update("Problems", values, whereClause, new string[] { ProblemID.ToString() });
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

        public bool CheckPriority(int priority)
        {
            bool isThere = false;

            if(ProblemSteps.Count > 0)
            {
                var step = ProblemSteps.Find(check => check.PriorityOrder == priority);
                if (step != null) isThere = true;
            }

            return isThere;
        }

        public bool IsProblemSolved()
        {
            bool solved = true;

            //to determine whether a problem has been solved, we need to check:
            // Each step
            //  Solved step = true
            //   Look at the ideas
            //     does idea have a review that has been achieved?
            //       if so, move onto next step
            //       if not, Solved step = false;
            //         break from check and return
            //   step completed

            Globals dbHelp = new Globals();
            try
            {
                dbHelp.OpenDatabase();

                if (dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    if (ProblemSteps.Count > 0)
                    {
                        foreach (var step in ProblemSteps)
                        {
                            if (step.ProblemStepIdeas.Count > 0)
                            {
                                var ideas = step.ProblemStepIdeas;
                                bool ideasReviewAchieved = false;
                                foreach (var idea in ideas)
                                {
                                    var plans = dbHelp.GetAllSolutionPlans();
                                    //is there a review for this?
                                    var review = dbHelp.GetSolutionReviewForIdea(idea.ProblemIdeaID);
                                    //completed review?
                                    if (review != null)
                                    {
                                        if (review.Achieved)
                                        {
                                            ideasReviewAchieved = true;
                                            break;
                                        }
                                    }
                                }
                                if (!ideasReviewAchieved)
                                {
                                    solved = false;
                                    break;
                                }
                            }
                            else
                            {
                                solved = false;
                                break;
                            }
                        }
                        
                    }
                    else
                    {
                        solved = false;
                    }
                    dbHelp.CloseDatabase();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "IsProblemSolved: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
                throw new Exception("Error determining if Problem has been solved - " + e.Message, e);
            }

            return solved;
        }
    }
}
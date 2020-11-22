using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Util;
using Android.Database.Sqlite;

namespace com.spanyardie.MindYourMood.Model
{
    public class SolutionReview : SolutionReviewBase
    {
        public const string TAG = "M:SolutionReview";

        public SolutionReview()
        {
            SolutionReviewID = -1;
            ProblemIdeaID = -1;
            ReviewText = "";
            Achieved = false;
            AchievedDate = new DateTime(1900, 1, 1, 0, 0, 0);
        }

        public void Remove()
        {
            SQLiteDatabase sqlDatabase = null;

            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    var sql = "DELETE FROM [SolutionReviews] WHERE SolutionReviewID = " + SolutionReviewID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Solution Review with ID " + SolutionReviewID.ToString() + " successfully");

                    sqlDatabase.Close();
                }
                else
                {
                    Log.Error(TAG, "Remove: SQLite database is null or was not opened - remove failed");
                }
            }
            catch(Exception e)
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
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();
                    values.Put("ProblemIdeaID", ProblemIdeaID);
                    values.Put("ReviewText", ReviewText.Trim());
                    values.Put("Achieved", Achieved);
                    values.Put("AchievedDate", AchievedDate.ToShortDateString());

                    if(IsNew)
                    {
                        SolutionReviewID = (int)sqlDatabase.Insert("SolutionReviews", null, values);
                        IsNew = false;
                        IsDirty = false;
                    }
                    if(IsDirty)
                    {
                        string whereClause = "SolutionReviewID = ?";
                        sqlDatabase.Update("SolutionReviews", values, whereClause, new string[] { SolutionReviewID.ToString() });
                        IsDirty = false;
                    }
                    sqlDatabase.Close();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Save: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
            }
        }
    }
}
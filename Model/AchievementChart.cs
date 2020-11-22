using System;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;
using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class AchievementChart : AchievementChartBase
    {

        public enum ACHIEVEMENTCHART_TYPE
        {
            General = 0,
            Life,
            Work,
            Family,
            Relationships,
            Health,
            Financial,
            Affirmation,
            Goal,
            NothingEntered = 99
        }

        public ACHIEVEMENTCHART_TYPE AchievementChartType { get; set; }

        public AchievementChart()
        {
            AchievementId = 0;
            AchievementDate = new DateTime();
            Achievement = "";

            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen && !IsNew)
            {
                string commandText = "DELETE FROM ChuffChart WHERE [AchievementID] = " + AchievementId;

                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    throw new SQLException("Removing Achievement from database failed - " + e.Message);
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
                        values.Put("AchievementDate", string.Format("{0:yyyy-MM-dd HH:mm:ss}", AchievementDate));
                        values.Put("Achievement", Achievement.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("ChuffChartType", (int)AchievementChartType);
                        AchievementId = (int)sqLiteDatabase.Insert("ChuffChart", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save Achievement - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        string whereClause = "AchievementID = " + AchievementId;
                        ContentValues values = new ContentValues();

                        values.Put("Achievement", Achievement.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("ChuffChartType", (int)AchievementChartType);
                        sqLiteDatabase.Update("ChuffChart", values, whereClause, null);

                        IsDirty = false;
                    }
                    catch (SQLException dirtyE)
                    {
                        throw new Exception("Unable to Update Achievement - " + dirtyE.Message);
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using com.spanyardie.MindYourMood.Model;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class DumpTable
    {
        public const string TAG = "M:DumpTable";

        public static void Medication()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [ID], [MedicationName], [TotalDailyDosage] FROM [Medication];";

            var records = sqlDatabase.RawQuery(sql, null);

            if(records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**            DUMP OF MEDICATION TABLE              **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var ID = records.GetInt(0).ToString();
                    var MedicationName = records.GetString(1);
                    var TotalDailyDosage = records.GetInt(2).ToString();
                    Log.Info(TAG, "ID - " + ID + ", MedicationName - " + MedicationName + ", TotalDailyDosage - " + TotalDailyDosage);
                    records.MoveToNext(); 
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**     DUMP OF MEDICATION TABLE - EMPTY!!!          **");
                Log.Info(TAG, "******************************************************");
            }

            dbHelp.CloseDatabase();
        }

        public static void Prescription()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [ID], [MedicationID], [PrescriptionType], [WeeklyDay], [MonthlyDay] FROM [Prescription];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**            DUMP OF PRESCRIPTION TABLE            **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var ID = records.GetInt(0).ToString();
                    var MedicationID = records.GetInt(1).ToString();
                    var PrescriptionType = StringHelper.PrescriptionStringForConstant((ConstantsAndTypes.PRESCRIPTION_TYPE)records.GetInt(2));
                    var WeeklyDay = StringHelper.DayStringForConstant((ConstantsAndTypes.DAYS_OF_THE_WEEK)records.GetInt(3));
                    var MonthlyDay = records.GetInt(4).ToString();
                    Log.Info(TAG, "ID - " + ID + ", MedicationID - " + MedicationID + ", PrescriptionType - " + PrescriptionType + ", WeeklyDay - " + WeeklyDay + ", MonthlyDay - " + MonthlyDay);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**     DUMP OF PRESCRIPTION TABLE - EMPTY!!!        **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void MediCationSpread()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [ID], [MedicationID], [Dosage], [FoodRelevance] FROM [MedicationSpread];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**           DUMP OF MEDICATIONSPREAD TABLE         **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var ID = records.GetInt(0).ToString();
                    var MedicationID = records.GetInt(1).ToString();
                    var Dosage = records.GetInt(2).ToString();
                    var FoodRelevance = StringHelper.MedicationFoodForConstant((ConstantsAndTypes.MEDICATION_FOOD)records.GetInt(3));
                    Log.Info(TAG, "ID - " + ID + ", MedicationID - " + MedicationID + ", Dosage - " + Dosage + ", FoodRelevance - " + FoodRelevance);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**     DUMP OF MEDICATIONSPREAD TABLE - EMPTY!!!    **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void MedicationTime()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [ID], [MedicationSpreadID], [MedicationTime], [TakenTime] FROM [MedicationTime];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**           DUMP OF MEDICATIONTIME TABLE           **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var ID = records.GetInt(0).ToString();
                    var MedicationSpreadID = records.GetInt(1).ToString();
                    var MedicationTime = StringHelper.MedicationTimeForConstant((ConstantsAndTypes.MEDICATION_TIME)records.GetInt(2));
                    var TakenTime = records.GetString(3);
                    Log.Info(TAG, "ID - " + ID + ", MedicationSpreadID - " + MedicationSpreadID + ", MedicationTime - " + MedicationTime + ", TakenTime - " + TakenTime);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**     DUMP OF MEDICATIONTIME TABLE - EMPTY!!!      **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void MedicationReminder()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [ID], [MedicationSpreadID], [MedicationDay], [MedicationTime] FROM [MedicationReminder];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**          DUMP OF MEDICATIONREMINDER TABLE        **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var ID = records.GetInt(0).ToString();
                    var MedicationSpreadID = records.GetInt(1).ToString();
                    var MedicationDay = StringHelper.DayStringForConstant((ConstantsAndTypes.DAYS_OF_THE_WEEK)records.GetInt(2));
                    var MedicationTime = records.GetString(3);
                    Log.Info(TAG, "ID - " + ID + ", MedicationSpreadID - " + MedicationSpreadID + ", MedicationDay - " + MedicationDay + ", MedicationTime - " + MedicationTime);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**    DUMP OF MEDICATIONREMINDER TABLE - EMPTY!!!   **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void AlternativeThoughts()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [Alternative], [AlternativeThoughtsID], [BeliefRating], [ThoughtRecordID] FROM [AlternativeThoughts];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**          DUMP OF ALTERNATIVETHOUGHTS TABLE       **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var Alternative = records.GetString(0);
                    var AlternativeThoughtsID = records.GetInt(1).ToString();
                    var BeliefRating = records.GetInt(2).ToString();
                    var ThoughtRecordID = records.GetInt(3);
                    Log.Info(TAG, "Alternative - " + Alternative + ", AlternativeThoughtsID - " + AlternativeThoughtsID + ", BeliefRating - " + BeliefRating + ", ThoughtRecordID - " + ThoughtRecordID);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**   DUMP OF ALTERNATIVETHOUGHTS TABLE - EMPTY!!!   **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void AutomaticThoughts()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [AutomaticThoughtsID], [HotThought], [Thought], [ThoughtRecordID] FROM [AutomaticThoughts];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**          DUMP OF AUTOMATICTHOUGHTS TABLE         **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var AutomaticThoughtsID = records.GetInt(0).ToString();
                    var HotThought = records.GetInt(1).ToString();
                    var Thought = records.GetString(2);
                    var ThoughtRecordID = records.GetInt(3);
                    Log.Info(TAG, "AutomaticThoughtsID - " + AutomaticThoughtsID + ", HotThought - " + HotThought + ", Thought - " + Thought + ", ThoughtRecordID - " + ThoughtRecordID);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**     DUMP OF AUTOMATICTHOUGHTS TABLE - EMPTY!!!   **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void AchievementChart()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [AchievementID], [Achievement], [AchievementDate], [ChuffChartType] FROM [ChuffChart];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**    DUMP OF ACHIEVEMENTCHART TABLE    **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var AchievementID = records.GetInt(0).ToString();
                    var Achievement = records.GetString(1);
                    var AchievementDate = records.GetString(2);
                    var AchievementChartType = records.GetInt(3).ToString();
                    Log.Info(TAG, "AchievementID - " + AchievementID + ", Achievement - " + Achievement + ", AchievementDate - " + AchievementDate + ", AchievementChartType - " + AchievementChartType);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "*DUMP OF ACHIEVEMENTCHART TABLE - EMPTY!**");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void EvidenceAgainstHotThought()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [EvidenceAgainstHotThoughtID], [AutomaticThoughtsID], [Evidence], [ThoughtRecordID] FROM [EvidenceAgainstHotThought];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**      DUMP OF EVIDENCEAGAINSTHOTTHOUGHT TABLE     **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var EvidenceAgainstHotThoughtID = records.GetInt(0).ToString();
                    var AutomaticThoughtsID = records.GetInt(1).ToString();
                    var Evidence = records.GetString(2);
                    var ThoughtRecordID = records.GetInt(3).ToString();
                    Log.Info(TAG, "EvidenceAgainstHotThoughtID - " + EvidenceAgainstHotThoughtID + ", AutomaticThoughtsID - " + AutomaticThoughtsID + ", Evidence - " + Evidence + ", ThoughtRecordID - " + ThoughtRecordID);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "** DUMP OF EVIDENCEAGAINSTHOTTHOUGHT TABLE - EMPTY! **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void EvidenceForHotThought()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [EvidenceForHotThoughtID], [AutomaticThoughtsID], [Evidence], [ThoughtRecordID] FROM [EvidenceForHotThought];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**      DUMP OF EVIDENCEFORHOTTHOUGHT TABLE         **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var EvidenceForHotThoughtID = records.GetInt(0).ToString();
                    var AutomaticThoughtsID = records.GetInt(1).ToString();
                    var Evidence = records.GetString(2);
                    var ThoughtRecordID = records.GetInt(3).ToString();
                    Log.Info(TAG, "EvidenceForHotThoughtID - " + EvidenceForHotThoughtID + ", AutomaticThoughtsID - " + AutomaticThoughtsID + ", Evidence - " + Evidence + ", ThoughtRecordID - " + ThoughtRecordID);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**   DUMP OF EVIDENCEFORHOTTHOUGHT TABLE - EMPTY!   **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void Mood()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [MoodsID], [MoodListID], [MoodRating], [ThoughtRecordID] FROM [Mood];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**                DUMP OF MOOD TABLE                **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var MoodsID = records.GetInt(0).ToString();
                    var MoodListID = records.GetInt(1).ToString();
                    var MoodRating = records.GetInt(2).ToString();
                    var ThoughtRecordID = records.GetInt(3).ToString();
                    Log.Info(TAG, "MoodsID - " + MoodsID + ", MoodListID - " + MoodListID + ", MoodRating - " + MoodRating + ", ThoughtRecordID - " + ThoughtRecordID);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**           DUMP OF MOOD TABLE - EMPTY!            **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void MoodList()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [MoodID], [MoodName], [IsoCountry] FROM [MoodList];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**              DUMP OF MOODLIST TABLE              **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var MoodID = records.GetInt(0).ToString();
                    var MoodName = records.GetString(1);
                    var country = records.GetString(2);
                    Log.Info(TAG, "MoodID - " + MoodID + ", MoodName - " + MoodName + ", ISOCountry - " + country);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**         DUMP OF MOODLIST TABLE - EMPTY!          **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void ReRateMood()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [RerateMoodID], [MoodListID], [MoodRating], [MoodsID], [ThoughtRecordID] FROM [ReRateMood];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**             DUMP OF RERATEMOOD TABLE             **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var RerateMoodID = records.GetInt(0).ToString();
                    var MoodListID = records.GetInt(1).ToString();
                    var MoodRating = records.GetInt(2).ToString();
                    var MoodsID = records.GetInt(3).ToString();
                    var ThoughtRecordID = records.GetInt(4).ToString();
                    Log.Info(TAG, "RerateMoodID - " + RerateMoodID + ", MoodListID - " + MoodListID + ", MoodRating - " + MoodRating + ", MoodsID - " + MoodsID + ", ThoughtRecordID - " + ThoughtRecordID);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**        DUMP OF RERATEMOOD TABLE - EMPTY!         **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void Situation()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [SituationID], [ThoughtRecordID], [Who], [What], [When], [Where] FROM [Situation];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**             DUMP OF Situation TABLE              **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var SituationID = records.GetInt(0).ToString();
                    var ThoughtRecordID = records.GetInt(1).ToString();
                    var Who = records.GetString(2);
                    var What = records.GetString(3);
                    var When = records.GetString(4);
                    var Where = records.GetString(4);
                    Log.Info(TAG, "SituationID - " + SituationID + ", ThoughtRecordID - " + ThoughtRecordID + ", Who - " + Who + ", What - " + What + ", When - " + When + ", Where - " + Where);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**        DUMP OF Situation TABLE - EMPTY!          **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void Activities()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            //string clear = "DELETE FROM [ActivityTimes];";
            //sqlDatabase.ExecSQL(clear);
            //clear = "DELETE FROM [Activities]";
            //sqlDatabase.ExecSQL(clear);

            string sql = "SELECT [ID], [ActivityDate] FROM [Activities];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**             DUMP OF Activities TABLE             **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var ActivityID = records.GetInt(0).ToString();
                    var ActivityDate = records.GetString(1);
                    Log.Info(TAG, "ActivityID - " + ActivityID + ", ActivityDate - " + ActivityDate);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**        DUMP OF Activities TABLE - EMPTY!         **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void ActivityTimes()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [ActivityTimesID], [ActivityID], [ActivityName], [ActivityTime], [Achievement], [Intimacy], [Pleasure] FROM [ActivityTimes];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**             DUMP OF ActivityTimes TABLE          **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var ActivityTimesID = records.GetInt(0).ToString();
                    var ActivityID = records.GetInt(1).ToString();
                    var ActivityName = records.GetString(2);
                    var ActivityTime = records.GetInt(3).ToString();
                    var Achievement = records.GetInt(4).ToString();
                    var Intimacy = records.GetInt(5).ToString();
                    var Pleasure = records.GetInt(6).ToString();
                    Log.Info(TAG, "ActivityTimesID - " + ActivityTimesID + ", ActivityID - " + ActivityID + ", ActivityName - " + ActivityName + ", ActivityTime - " + ActivityTime + ", Achievement - " + Achievement + ", Intimacy - " + Intimacy + ", Pleasure - " + Pleasure);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**        DUMP OF ActivityTimes TABLE - EMPTY!      **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void PlayLists()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [PlayListID], [PlayListName], [PlayListTrackCount] FROM [PlayLists];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**             DUMP OF PlayLists TABLE              **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var PlayListID = records.GetInt(0).ToString();
                    var PlayListName = records.GetString(1);
                    var PlayListTrackCount = records.GetInt(2).ToString();
                    Log.Info(TAG, "PlayListID - " + PlayListID + ", PlayListName - " + PlayListName + ", PlayListTrackCount - " + PlayListTrackCount);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**         DUMP OF PlayLists TABLE - EMPTY!         **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void Tracks()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [TrackID], [PlayListID], [TrackName], [TrackArtist], [TrackDuration], [TrackOrderNumber], [TrackUri] FROM [Tracks];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**               DUMP OF Tracks TABLE               **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var TrackID = records.GetInt(0).ToString();
                    var PlayListID = records.GetInt(1).ToString();
                    var TrackName = records.GetString(2);
                    var TrackArtist = records.GetString(3);
                    var TrackDuration = records.GetFloat(4).ToString();
                    var TrackOrderNumber = records.GetInt(5).ToString();
                    var TrackUri = records.GetString(6);
                    Log.Info(TAG, "TrackID - " + TrackID + ", PlayListID - " + PlayListID + ", TrackName - " + TrackName + ", TrackArtist - " + TrackArtist + ", TrackDuration - " + TrackDuration + ", TrackOrderNumber - " + TrackOrderNumber + ", TrackUri - " + TrackUri);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**          DUMP OF Tracks TABLE - EMPTY!           **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }

        public static void DumpAppointments()
        {
            Globals dbHelp = new Globals();

            dbHelp.OpenDatabase();

            var sqlDatabase = dbHelp.GetSQLiteDatabase();

            string sql = "SELECT [AppointmentID], [AppointmentDate], [AppointmentType], [Location], [WithWhom], [AppointmentTime], [Notes] FROM [Appointments];";

            var records = sqlDatabase.RawQuery(sql, null);

            if (records != null && records.Count > 0)
            {
                records.MoveToFirst();
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**               DUMP OF Appointments TABLE         **");
                Log.Info(TAG, "******************************************************");

                for (var a = 0; a < records.Count; a++)
                {
                    var appointmentID = records.GetInt(0).ToString();
                    var appointmentDate = records.GetString(1);
                    var appointmentType = records.GetInt(2);
                    var location = records.GetString(3);
                    var withWhom = records.GetString(4);
                    var appointmentTime = records.GetString(5);
                    var notes = records.GetString(6);
                    Log.Info(TAG, "AppointmentID - " + appointmentID + ", AppointmentDate - " + appointmentDate + ", AppointmentType - " + appointmentType + ", Location - " + location + ", WithWhom - " + withWhom + ", AppointmentTime - " + appointmentTime + ", Notes - " + notes);
                    records.MoveToNext();
                }
                Log.Info(TAG, "******************************************************");
            }
            else
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**          DUMP OF Appointments TABLE - EMPTY!     **");
                Log.Info(TAG, "******************************************************");
            }
            dbHelp.CloseDatabase();
        }
    }
}
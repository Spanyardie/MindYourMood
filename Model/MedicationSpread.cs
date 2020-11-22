using System;
using Android.Content;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Database.Sqlite;
using Android.Database;

namespace com.spanyardie.MindYourMood.Model
{
    public class MedicationSpread
    {
        public const string TAG = "M:MedicationSpread";

        public int ID { get; set; }
        public int MedicationID { get; set; }
        public int Dosage { get; set; }
        public MedicationTime MedicationTakeTime { get; set; }
        public ConstantsAndTypes.MEDICATION_FOOD FoodRelevance { get; set; }
        public MedicationReminder MedicationTakeReminder { get; set; }

        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }

        public string Tag { get; set; }

        public MedicationSpread()
        {
            ID = -1;
            MedicationID = -1;
            Dosage = 0;
            FoodRelevance = ConstantsAndTypes.MEDICATION_FOOD.DoesntMatter;
            IsNew = true;
            IsDirty = false;
        }

        public bool LoadMedicationSpread()
        {
            SQLiteDatabase sqlDatabase = null;

            try
            {
                var sql = @"SELECT [Dosage], [FoodRelevance] FROM [MedicationSpread] WHERE [ID] = ? AND [MedicationID] = ?";
                string[] values = new string[]
                {
                    ID.ToString(),
                    MedicationID.ToString()
                };
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if(sqlDatabase != null)
                {
                    if(sqlDatabase.IsOpen)
                    {
                        ICursor medicationItem = sqlDatabase.RawQuery(sql, values);
                        if(medicationItem.Count > 0)
                        {
                            medicationItem.MoveToFirst();
                            MedicationTakeTime = new MedicationTime();
                            MedicationTakeTime.LoadMedicationTime(ID);
                            MedicationTakeReminder = new MedicationReminder();
                            MedicationTakeReminder.LoadMedicationTime(ID);
                            if (!MedicationTakeReminder.IsSet)
                                MedicationTakeReminder = null;
                            Dosage = medicationItem.GetInt(medicationItem.GetColumnIndex("Dosage"));
                            FoodRelevance = (ConstantsAndTypes.MEDICATION_FOOD)medicationItem.GetInt(medicationItem.GetColumnIndex("FoodRelevance"));
                        }
                        else
                        {
                            dbHelp.CloseDatabase();
                            return false;
                        }
                    }
                    dbHelp.CloseDatabase();
                }
                return true;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "LoadMedicationSpread: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                return false;
            }
        }

        public bool Save(int medicationSpreadID, int medicationID)
        {
            SQLiteDatabase sqlDatabase = null;

            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if(sqlDatabase != null)
                {
                    if(sqlDatabase.IsOpen)
                    {
                        if(medicationSpreadID == -1)
                        {
                            Log.Info(TAG, "Save (insert): Saving New Medication Spread");
                            ContentValues values = new ContentValues();
                            values.Put("MedicationID", medicationID);
                            values.Put("Dosage", Dosage.ToString());
                            values.Put("FoodRelevance", (int)FoodRelevance);
                            ID = (int)sqlDatabase.Insert("MedicationSpread", null, values);
                            Log.Info(TAG, "Save (insert): Saved Spread with ID - " + ID.ToString() + ", medicationID - " + medicationID.ToString() + ", Dosage - " + Dosage.ToString() + ", Food Relevance - " + StringHelper.MedicationFoodForConstant(FoodRelevance));
                            IsNew = false;
                            IsDirty = false;
                            if(MedicationTakeTime != null)
                                MedicationTakeTime.Save(ID);
                            if (MedicationTakeReminder != null)
                                MedicationTakeReminder.Save(ID);
                        }
                        else
                        {
                            ContentValues values = new ContentValues();
                            values.Put("MedicationID", medicationID);
                            values.Put("Dosage", Dosage.ToString());
                            values.Put("FoodRelevance", (int)FoodRelevance);
                            string whereClause = "ID = ? AND MedicationID = ?";
                            string[] wheres = new string[]
                            {
                                ID.ToString(),
                                MedicationID.ToString()
                            };
                            sqlDatabase.Update("MedicationSpread", values, whereClause, wheres);
                            if (MedicationTakeTime != null)
                                MedicationTakeTime.Save(ID);
                            if (MedicationTakeReminder != null)
                                MedicationTakeReminder.Save(ID);
                            IsDirty = false;
                        }
                        sqlDatabase.Close();
                        return true;
                    }
                }
                return false;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Save: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                return false;
            }
        }

        public bool Remove()
        {
            SQLiteDatabase sqlDatabase = null;

            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    var sql = @"DELETE FROM [MedicationSpread] WHERE ID = " + ID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Medication Spread with ID of " + ID.ToString());
                    if (MedicationTakeTime != null)
                        MedicationTakeTime.Remove();
                    if (MedicationTakeReminder != null)
                        MedicationTakeReminder.Remove();
                    sqlDatabase.Close();
                    return true;
                }
                Log.Error(TAG, "Remove: SQLite database is null or was not opened - remove failed");
                return false;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Remove: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                return false;
            }
        }
    }
}
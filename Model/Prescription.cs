using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using com.spanyardie.MindYourMood.Helpers;
using Android.Database.Sqlite;
using Android.Util;
using Android.Database;

namespace com.spanyardie.MindYourMood.Model
{
    public class Prescription : PrescriptionBase
    {
        public const string TAG = "M:Prescription";

        public Prescription()
        {
            ID = -1;
            MedicationID = -1;
            PrescriptionType = ConstantsAndTypes.PRESCRIPTION_TYPE.Daily;
            WeeklyDay = ConstantsAndTypes.DAYS_OF_THE_WEEK.Monday;
            MonthlyDay = 0;
            IsNew = true;
            IsDirty = false;
        }

        public bool LoadPrescription(int medicationID)
        {
            SQLiteDatabase sqlDatabase = null;
            try
            {
                var sql = @"SELECT [ID], [PrescriptionType], [WeeklyDay], [MonthlyDay] FROM [Prescription] WHERE [MedicationID] = ?";
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    ICursor prescription = sqlDatabase.RawQuery(sql, new string[] { medicationID.ToString() });
                    if (prescription.Count > 0)
                    {
                        prescription.MoveToFirst();
                        ID = prescription.GetInt(prescription.GetColumnIndex("ID"));
                        PrescriptionType = (ConstantsAndTypes.PRESCRIPTION_TYPE)prescription.GetInt(prescription.GetColumnIndex("PrescriptionType"));
                        WeeklyDay = (ConstantsAndTypes.DAYS_OF_THE_WEEK)prescription.GetInt(prescription.GetColumnIndex("WeeklyDay"));
                        MonthlyDay = prescription.GetInt(prescription.GetColumnIndex("MonthlyDay"));
                        IsNew = false;
                        IsDirty = false;
                    }
                    else
                    {
                        dbHelp.CloseDatabase();
                        return false;
                    }
                    dbHelp.CloseDatabase();
                    return true;
                }
                dbHelp.CloseDatabase();
                return false;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "LoadPrescription: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                return false;
            }
        }

        public bool Save(int medicationID)
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
                        if(IsNew)
                        {
                            ContentValues values = new ContentValues();
                            values.Put("MedicationID", medicationID);
                            values.Put("PrescriptionType", (int)PrescriptionType);
                            values.Put("WeeklyDay", (int)WeeklyDay);
                            values.Put("MonthlyDay", MonthlyDay);

                            ID = (int)sqlDatabase.Insert("Prescription", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if(IsDirty)
                        {
                            ContentValues values = new ContentValues();
                            values.Put("MedicationID", medicationID);
                            values.Put("PrescriptionType", (int)PrescriptionType);
                            values.Put("WeeklyDay", (int)WeeklyDay);
                            values.Put("MonthlyDay", MonthlyDay);
                            string whereClause = "ID = ?";
                            sqlDatabase.Update("Prescription", values, whereClause, new string[] { ID.ToString() });
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
                    var sql = "DELETE FROM [Prescription] WHERE ID = " + ID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Prescription with ID " + ID.ToString() + " successfully");
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
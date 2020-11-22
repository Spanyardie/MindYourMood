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
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Database.Sqlite;
using Android.Util;
using Android.Database;
using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model
{
    public class MedicationTime : MedicationTimeBase
    {
        public const string TAG = "M:MedicationTime";

        public MedicationTime()
        {
            ID = -1;
            MedicationSpreadID = -1;
            MedicationTime = ConstantsAndTypes.MEDICATION_TIME.Morning;
            TakenTime = new DateTime(1900, 1, 1);
        }

        public bool LoadMedicationTime(int medicationSpreadID)
        {
            SQLiteDatabase sqlDatabase = null;

            try
            {
                Globals dbHelp = new Globals();
                //Log.Info(TAG, "LoadMedicationTime: Opening database..");
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    //Log.Info(TAG, "LoadMedicationTime: Attempting to load Medication Time for Spread ID - " + medicationSpreadID.ToString());
                    var sql = @"SELECT [ID], [MedicationTime], [TakenTime] FROM [MedicationTime] WHERE MedicationSpreadID = ?";
                    ICursor times = sqlDatabase.RawQuery(sql, new string[] { medicationSpreadID.ToString() });
                    if(times != null && times.Count > 0)
                    {
                        times.MoveToFirst();
                        ID = times.GetInt(times.GetColumnIndex("ID"));
                        //Log.Info(TAG, "LoadMedicationTime: Found Medication Time with ID - " + ID.ToString());
                        MedicationSpreadID = medicationSpreadID;
                        MedicationTime = (ConstantsAndTypes.MEDICATION_TIME)times.GetInt(times.GetColumnIndex("MedicationTime"));
                        TakenTime = Convert.ToDateTime("1900-01-01 " + times.GetString(times.GetColumnIndex("TakenTime")));
                        //Log.Info(TAG, "LoadMedicationTime: Retrieved Medication Time, ID - " + ID.ToString() + ", Medication Time - " + StringHelper.MedicationTimeForConstant(MedicationTime) + ", Taken Time - " + TakenTime.ToShortTimeString());
                        sqlDatabase.Close();
                        return true;
                    }
                }
                return false;
            }
            catch(Exception e)
            {
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                Log.Error(TAG, "LoadMedicationTime: Exception - " + e.Message);
                return false;
            }
        }

        public bool Save(int medicationSpreadID)
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
                        if (IsNew)
                        {
                            Log.Info(TAG, "Save: Saving new Medication Time to Spread ID - " + medicationSpreadID.ToString());
                            ContentValues values = new ContentValues();
                            values.Put("MedicationSpreadID", medicationSpreadID);
                            values.Put("MedicationTime", (int)MedicationTime);
                            values.Put("TakenTime", string.Format("{0:HH:mm:ss}", TakenTime));
                            Log.Info(TAG, "Save: Stored Medication Time - " + StringHelper.MedicationTimeForConstant(MedicationTime) + ", Time Taken - " + TakenTime.ToShortTimeString());
                            ID = (int)sqlDatabase.Insert("MedicationTime", null, values);
                            Log.Info(TAG, "Save: Saved Medication Time with new ID - " + ID.ToString());
                            IsNew = false;
                            IsDirty = false;
                            Log.Info(TAG, "Save: IsNew - FALSE, IsDirty - FALSE");
                        }
                        if(IsDirty)
                        {
                            Log.Info(TAG, "Save (Update): Updating existing Medication Time for Spread ID - " + medicationSpreadID.ToString());
                            ContentValues values = new ContentValues();
                            values.Put("MedicationSpreadID", medicationSpreadID);
                            values.Put("MedicationTime", (int)MedicationTime);
                            values.Put("TakenTime", string.Format("{0:HH:mm:ss}", TakenTime));
                            Log.Info(TAG, "Save (Update): Stored Medication Time - " + StringHelper.MedicationTimeForConstant(MedicationTime) + ", Taken Time - " + TakenTime.ToShortTimeString());
                            string whereClause = "ID = ?";
                            sqlDatabase.Update("MedicationTime", values, whereClause, new string[] { ID.ToString() });
                            IsDirty = false;
                            Log.Info(TAG, "Save (Update): IsDirty - FALSE");
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
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    Log.Info(TAG, "Remove: Attempting to Remove Medication Time with ID - " + ID.ToString());
                    var sql = "DELETE FROM [MedicationTime] WHERE ID = " + ID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Medication Time with ID of " + ID.ToString() + " successfully");
                    sqlDatabase.Close();
                    return true;
                }
                Log.Error(TAG, "Remove: SQLite database is null or was not opened - remove failed");
                return false;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Remove: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                return false;
            }
        }
    }
}
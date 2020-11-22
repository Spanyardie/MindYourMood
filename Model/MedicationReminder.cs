using System;
using Android.Content;
using Android.Database.Sqlite;
using Android.Util;

using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Database;
using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model
{
    public class MedicationReminder : MedicationReminderBase
    {
        public const string TAG = "M:MedicationReminder";

        public MedicationReminder()
        {
            ID = -1;
            MedicationSpreadID = -1;
            MedicationDay = ConstantsAndTypes.DAYS_OF_THE_WEEK.Monday;
            MedicationTime = new DateTime(1900, 1, 1, 0, 0, 0);
            IsSet = false;
            IsNew = true;
            IsDirty = false;
        }

        public bool LoadMedicationTime(int medicationSpreadID)
        {
            SQLiteDatabase sqlDatabase = null;
            try
            {
                var sql = @"SELECT [ID], [MedicationDay], [MedicationTime] FROM [MedicationReminder] WHERE [MedicationSpreadID] = ?";
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if(sqlDatabase != null)
                {
                    if(sqlDatabase.IsOpen)
                    {
                        ICursor medicationTime = sqlDatabase.RawQuery(sql, new string[] { medicationSpreadID.ToString() });
                        if(medicationTime.Count > 0)
                        {
                            medicationTime.MoveToFirst();
                            ID = medicationTime.GetInt(medicationTime.GetColumnIndex("ID"));
                            MedicationSpreadID = medicationSpreadID;
                            MedicationDay = (ConstantsAndTypes.DAYS_OF_THE_WEEK)medicationTime.GetInt(medicationTime.GetColumnIndex("MedicationDay"));
                            MedicationTime = Convert.ToDateTime("1900-01-01 " + medicationTime.GetString(medicationTime.GetColumnIndex("MedicationTime")));
                            IsSet = true;
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
            catch (Exception e)
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
                            Log.Info(TAG, "Save: New Reminder to Save for Spread with ID - " + medicationSpreadID.ToString());
                            ContentValues values = new ContentValues();
                            values.Put("MedicationSpreadID", medicationSpreadID);
                            values.Put("MedicationDay", (int)MedicationDay);
                            values.Put("MedicationTime", string.Format("{0:HH:mm:ss}", MedicationTime));
                            Log.Info(TAG, "Save: Saved Medication Day - " + StringHelper.DayStringForConstant(MedicationDay) + ", Medication Time - " + MedicationTime.ToShortTimeString());

                            ID = (int)sqlDatabase.Insert("MedicationReminder", null, values);
                            Log.Info(TAG, "Save: Reminder saved with new ID - " + ID.ToString());
                            IsNew = false;
                            IsDirty = false;
                            IsSet = true;
                            Log.Info(TAG, "Save: IsNew - FALSE, IsDirty - FALSE");
                        }
                        if(IsDirty)
                        {
                            Log.Info(TAG, "Save: Updating existing Reminder, ID - " + ID.ToString());
                            ContentValues values = new ContentValues();
                            values.Put("MedicationSpreadID", medicationSpreadID);
                            values.Put("MedicationDay", (int)MedicationDay);
                            values.Put("MedicationTime", string.Format("{0:HH:mm:ss}", MedicationTime));
                            Log.Info(TAG, "Save: Stored Medication Day - " + StringHelper.DayStringForConstant(MedicationDay) + ", Medication Time - " + MedicationTime.ToShortTimeString());
                            string whereClause = "ID = ?";
                            sqlDatabase.Update("MedicationReminder", values, whereClause, new string[] { ID.ToString() });
                            IsDirty = false;
                            Log.Info(TAG, "Save: Updated, IsDirty - FALSE");
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
                    var sql = "DELETE FROM [MedicationReminder] WHERE ID = " + ID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed MedicationReminder with ID " + ID.ToString() + " successfully");
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
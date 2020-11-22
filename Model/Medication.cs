using System;
using Android.Database.Sqlite;
using Android.Util;
using System.Collections.Generic;
using Android.Database;
using Android.Content;

namespace com.spanyardie.MindYourMood.Model
{
    public class Medication
    {
        public const string TAG = "M:Medication";

        public int ID { get; set; }
        public string MedicationName { get; set; }
        public int TotalDailyDosage { get; set; }
        public List<MedicationSpread> MedicationSpread { get; set; }
        public Prescription PrescriptionType { get; set; }

        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }

        public Medication()
        {
            MedicationSpread = new List<MedicationSpread>();
            PrescriptionType = new Prescription();
            ID = -1;
            MedicationName = "";
            TotalDailyDosage = 0;
            IsNew = true;
            IsDirty = false;
        }

        public bool LoadMedication()
        {
            SQLiteDatabase sqlDatabase = null;

            try
            {
                var sql = @"SELECT [MedicationName], [TotalDailyDosage] FROM [Medication] WHERE [ID] = ?";
                Globals dbHelp = new Globals();
                Log.Info(TAG, "LoadMedication: Opening database");
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if(sqlDatabase != null)
                {
                    if(sqlDatabase.IsOpen)
                    {
                        //Log.Info(TAG, "LoadMedication: Attempting to Load Medication with ID - " + ID.ToString());
                        ICursor med = sqlDatabase.RawQuery(sql, new string[] { ID.ToString() });
                        if(med.Count > 0)
                        {
                            med.MoveToFirst();
                            MedicationName = med.GetString(med.GetColumnIndex("MedicationName")).Trim();
                            TotalDailyDosage = med.GetInt(med.GetColumnIndex("TotalDailyDosage"));
                            //Log.Info(TAG, "LoadMedication: Retrieved Medication Name - " + MedicationName + ", Total Daily Dosage - " + TotalDailyDosage.ToString());
                            if (MedicationSpread == null)
                                MedicationSpread = new List<MedicationSpread>();
                            //Log.Info(TAG, "LoadMedication: Loading Medication Spreads for Medication with ID - " + ID.ToString());
                            LoadMedicationSpreads(sqlDatabase);
                            var preps = new Prescription();
                            preps.MedicationID = ID;
                            var succeeded = preps.LoadPrescription(ID);
                            if (succeeded)
                                PrescriptionType = preps;
                            IsNew = false;
                            IsDirty = false;
                        }
                        else
                        {
                            Log.Info(TAG, "LoadMedication: No result!");
                        }
                        dbHelp.CloseDatabase();
                        return true;
                    }
                }
                return false;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "LoadMedication: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                return false;
            }
        }

        private bool LoadMedicationSpreads(SQLiteDatabase sqlDatabase)
        {
            try
            {
                if (MedicationSpread == null)
                    MedicationSpread = new List<MedicationSpread>();

                MedicationSpread.Clear();

                var sql = @"SELECT [ID] FROM [MedicationSpread] WHERE MedicationID = ?";
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    ICursor spreads = sqlDatabase.RawQuery(sql, new string[] { ID.ToString() });
                    if(spreads != null && spreads.Count > 0)
                    {
                        Log.Info(TAG, "LoadMedicationSpreads: Found " + spreads.Count.ToString() + " items");
                        spreads.MoveToFirst();
                        MedicationSpread spread = null;
                        for(var a = 0; a < spreads.Count; a++)
                        {
                            spread = new MedicationSpread();
                            spread.IsNew = false;
                            spread.IsDirty = false;
                            spread.ID = spreads.GetInt(spreads.GetColumnIndex("ID"));
                            spread.MedicationID = ID;
                            spread.LoadMedicationSpread();
                            //Log.Info(TAG, "LoadMedicationSpreads: Loaded Medication spread with ID - " + spread.ID.ToString());
                            MedicationSpread.Add(spread);
                            //Log.Info(TAG, "LoadMedicationSpreads: Added Spread to Medication with ID - " + ID.ToString() + ", count - " + MedicationSpread.Count.ToString());
                            spreads.MoveToNext();
                        }
                        sqlDatabase.Close();
                        return true;
                    }
                }
                return false;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "LoadMedicationSpreads: Exception - " + e.Message);
                return false;
            }
        }

        public bool Save()
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
                            Log.Info(TAG, "Save: New Medication item to Save...");
                            ContentValues values = new ContentValues();
                            values.Put("MedicationName", MedicationName.Trim());
                            values.Put("TotalDailyDosage", TotalDailyDosage);
                            ID = (int)sqlDatabase.Insert("Medication", null, values);
                            Log.Info(TAG, "Save: Successfully saved - ID - " + ID.ToString());
                            IsNew = false;
                            IsDirty = false;
                            if(MedicationSpread != null && MedicationSpread.Count > 0)
                            {
                                foreach(var spread in MedicationSpread)
                                {
                                    spread.Save(spread.ID, ID);
                                    Log.Info(TAG, "Save (insert): Saved Medication Spread with ID - " + spread.ID.ToString());
                                }
                            }
                            if (PrescriptionType != null)
                            {
                                PrescriptionType.Save(ID);
                                Log.Info(TAG, "Save (insert): Saved Prescription Type ID - " + ID.ToString());
                            }
                        }
                        if(IsDirty)
                        {
                            Log.Info(TAG, "LoadMedicationSpreads: Exisitng Medication to Update with ID - " + ID.ToString());
                            ContentValues values = new ContentValues();
                            values.Put("MedicationName", MedicationName.Trim());
                            values.Put("TotalDailyDosage", TotalDailyDosage);
                            string whereClause = "ID = ?";
                            sqlDatabase.Update("Medication", values, whereClause, new string[] { ID.ToString() });
                            Log.Info(TAG, "Save (update): Updated Medication with ID - " + ID.ToString());
                            IsDirty = false;
                            if (MedicationSpread != null && MedicationSpread.Count > 0)
                            {
                                foreach (var spread in MedicationSpread)
                                {
                                    spread.Save(spread.ID, ID);
                                    Log.Info(TAG, "Save (update): Saved Medication Spread with ID - " + spread.ID.ToString());
                                }
                            }
                            if (PrescriptionType != null)
                            {
                                PrescriptionType.Save(ID);
                                Log.Info(TAG, "Save (Update): Saved Prescription type with ID - " + PrescriptionType.ID.ToString());
                            }
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
                    Log.Info(TAG, "Remove: Attempting to Remove Medication with ID - " + ID.ToString());
                    var sql = @"DELETE FROM [Medication] WHERE ID = " + ID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Medication with ID " + ID.ToString());
                    if(MedicationSpread != null && MedicationSpread.Count > 0)
                    {
                        foreach(var spread in MedicationSpread)
                        {
                            Log.Info(TAG, "Remove: Removing Spread with ID - " + spread.ID.ToString());
                            spread.Remove();
                        }
                        if (PrescriptionType != null)
                        {
                            Log.Info(TAG, "Remove: Removing Prescription Type with ID - " + PrescriptionType.ID.ToString());
                            PrescriptionType.Remove();
                            PrescriptionType = new Prescription();
                        }
                        sqlDatabase.Close();
                        return true;
                    }
                }
                Log.Error(TAG, "Remove: SQLite database is null or was not open - remove failed");
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

        public int GetTotalSpreadDosage()
        {
            int totalSpreadDosage = 0;

            if(MedicationSpread != null)
            {
                if(MedicationSpread.Count > 0)
                {
                    foreach(var spread in MedicationSpread)
                    {
                        totalSpreadDosage += spread.Dosage;
                    }
                }
            }
            return totalSpreadDosage;
        }
    }
}
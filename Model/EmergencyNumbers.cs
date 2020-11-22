using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Database.Sqlite;

namespace com.spanyardie.MindYourMood.Model
{
    public class EmergencyNumber : EmergencyNumberBase
    {
        public EmergencyNumber()
        {

        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen)
            {
                string commandText = "DELETE FROM [EmergencyNumbers] WHERE [EmergencyNumberID] = " + EmergencyNumberID;

                try
                {
                    sqLiteDatabase.RawQuery(commandText, null);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to remove Emergency Number from database - " + e.Message);
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
                        string[] columns =
                                {
                                    "CountryName",
                                    "PoliceNumber",
                                    "AmbulanceNumber",
                                    "FireNumber",
                                    "Notes"
                            };

                        ContentValues values = new ContentValues();

                        values.Put("CountryName", CountryName.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("PoliceNumber", PoliceNumber.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("AmbulanceNumber", AmbulanceNumber.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("FireNumber", FireNumber.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("Notes", Notes.Trim().Replace("'", "''").Replace("\"", "\"\""));

                        EmergencyNumberID = (int)sqLiteDatabase.Insert("EmergencyNumbers", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save EmergencyNumber in database - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        ContentValues values = new ContentValues();
                        values.Put("CountryName", CountryName.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("PoliceNumber", PoliceNumber.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("AmbulanceNumber", AmbulanceNumber.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("FireNumber", FireNumber.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        values.Put("Notes", Notes.Trim().Replace("'", "''").Replace("\"", "\"\""));
                        string whereClause = "EmergencyNumberID = " + EmergencyNumberID;
                        sqLiteDatabase.Update("EmergencyNumbers", values, whereClause, null);

                        IsDirty = false;
                    }
                    catch (Exception dirtyE)
                    {
                        throw new Exception("Unable to Update Emergency Number in database - " + dirtyE.Message);
                    }
                }
            }
        }

        public string toString()
        {
            return CountryName.Trim() + ", Police: " + PoliceNumber.Trim() + ", Ambulance: " + AmbulanceNumber.Trim() + ", Fire: " + FireNumber.Trim();
        }

    }
}
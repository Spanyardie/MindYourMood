using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class Contact : ContactBase
    {
        public const string TAG = "M:Contact";

        public Contact()
        {
            ContactEmergencyCall = false;
            ContactEmergencyEmail = false;
            ContactEmergencySms = false;

            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen)
            {
                string commandText = "DELETE FROM Contacts WHERE [ID] = " + ID;
                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                    Log.Info(TAG, "Remove: Removed Contact with ID " + ID.ToString() + " from user Contact list");
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "Remove: Exception - " + e.Message);
                    throw new Exception("Removing Contact from database failed - " + e.Message);
                }
            }
        }

        public void Save(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen)
            {
                ContentValues values = new ContentValues();
                try
                {
                    values.Put("URI", ContactUri);
                    values.Put("ContactName", ContactName.Trim());
                    values.Put("ContactTelephoneNumber", ContactTelephoneNumber.Trim());
                    values.Put("ContactPhotoId", 0);
                    values.Put("ContactEmail", ContactEmail.Trim());
                    values.Put("ContactUseEmergencyCall", Convert.ToInt16(ContactEmergencyCall));
                    values.Put("ContactUseEmergencySms", Convert.ToInt16(ContactEmergencySms));
                    values.Put("ContactUseEmergencyEmail", Convert.ToInt16(ContactEmergencyEmail));
                }
                catch (Exception valE)
                {
                    Log.Error(TAG, "Save: Exception - " + valE.Message);
                    return;
                }

                if (IsNew)
                {
                    try
                    {
                        ID = (int)sqLiteDatabase.Insert("Contacts", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        Log.Error(TAG, "Save: Save Exception - " + newE.Message);
                        throw new Exception("Unable to save Contact to database - " + newE.Message);
                    }
                }
                if(IsDirty)
                {
                    try
                    {
                        //values.Put("ID", ID);
                        sqLiteDatabase.Update("Contacts", values, "ID = ?", new string[] { ID.ToString() });
                        IsDirty = false;
                    }
                    catch(Exception updE)
                    {
                        Log.Error(TAG, "Save: Update Exception - " + updE.Message);
                    }
                }
            }
        }
    }
}
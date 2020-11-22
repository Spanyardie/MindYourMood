using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class GenericText : GenericTextBase
    {
        public const string TAG = "M:GenericText";

        public GenericText()
        {
            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen)
            {
                string commandText = "DELETE FROM GenericText WHERE [ID] = " + ID;
                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                    Log.Info(TAG, "Remove: Removed Generic Text item with ID " + ID.ToString() + " from user GenericText list");
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "Remove: Exception - " + e.Message);
                    throw new Exception("Removing Generic Text from database failed - " + e.Message);
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
                        values.Put("TextType", (int)TextType);
                        values.Put("TextValue", TextValue.Trim());

                        ID = (int)sqLiteDatabase.Insert("GenericText", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to save Generic Text to database - " + newE.Message);
                    }
                }

                if(IsDirty)
                {
                    try
                    {
                        string whereClause = "ID = " + ID;

                        ContentValues values = new ContentValues();
                        values.Put("TextType", (int)TextType);
                        values.Put("TextValue", TextValue.Trim());

                        sqLiteDatabase.Update("GenericText", values, whereClause, null);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception updE)
                    {
                        throw new Exception("Unable to update Generic Text in database - " + updE.Message);
                    }
                }
            }
        }
    }
}
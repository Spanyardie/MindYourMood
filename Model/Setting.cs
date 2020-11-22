using System;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Content;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class Setting : SettingBase
    {
        public const string TAG = "M:Setting";

        public int SettingId { get; set; }

        public Setting()
        {
            SettingId = -1;
            SettingKey = "";
            SettingValue = "";
            IsNew = true;
            IsDirty = false;
        }

        public void SaveSetting()
        {
            Globals dbHelp = new Globals();
            SQLiteDatabase sqlDatabase = null;
            try
            {
                if (dbHelp != null)
                {
                    dbHelp.OpenDatabase();
                    sqlDatabase = dbHelp.GetSQLiteDatabase();
                    if (sqlDatabase != null && sqlDatabase.IsOpen)
                    {
                        ContentValues values = new ContentValues();
                        values.Put("SettingKey", SettingKey.Trim());
                        values.Put("SettingValue", SettingValue.Trim());

                        if (IsNew)
                        {
                            SettingId = (int)sqlDatabase.Insert("Settings", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if(IsDirty)
                        {
                            string whereClause = "ID = ?";
                            sqlDatabase.Update("Settings", values, whereClause, new string[] { SettingId.ToString() });
                            IsDirty = false;
                        }
                        sqlDatabase.Close();
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SaveSetting: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
            }
        }
    }
}
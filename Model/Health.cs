using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using com.spanyardie.MindYourMood.Helpers;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class Health : HealthBase
    {
        public const string TAG = "M:Health";

        public Health()
        {
            HealthID = -1;
            Aspect = "";
            Importance = 0;
            Type = ConstantsAndTypes.REACTION_TYPE.Positive;
            Action = ConstantsAndTypes.ACTION_TYPE.DoMore;
            ActionOf = "";
            IsNew = true;
            IsDirty = false;
        }

        public void Remove()
        {
            SQLiteDatabase sqlDatabase = null;

            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    var sql = "DELETE FROM [Health] WHERE HealthID = " + HealthID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Health with ID " + HealthID.ToString() + " successfully");
                    sqlDatabase.Close();
                }
                Log.Error(TAG, "Remove: SQLite database is null or was not opened - remove failed");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Remove: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
            }
        }

        public void Save()
        {
            SQLiteDatabase sqlDatabase = null;
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    if (sqlDatabase.IsOpen)
                    {
                        if (IsNew)
                        {
                            ContentValues values = new ContentValues();
                            values.Put("Aspect", Aspect);
                            values.Put("Importance", Importance);
                            values.Put("Type", (int)Type);
                            values.Put("Action", (int)Action);
                            values.Put("ActionOf", ActionOf);

                            HealthID = (int)sqlDatabase.Insert("Health", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            ContentValues values = new ContentValues();
                            values.Put("Aspect", Aspect);
                            values.Put("Importance", Importance);
                            values.Put("Type", (int)Type);
                            values.Put("Action", (int)Action);
                            values.Put("ActionOf", ActionOf);

                            string whereClause = "HealthID = ?";
                            sqlDatabase.Update("Health", values, whereClause, new string[] { HealthID.ToString() });
                            IsDirty = false;
                        }
                        sqlDatabase.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Save: Exception - " + e.Message);
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
            }
        }
    }
}
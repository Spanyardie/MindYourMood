using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using com.spanyardie.MindYourMood.Helpers;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class Attitudes : AttitudesBase
    {
        public const string TAG = "M:Attitudes";

        public Attitudes()
        {
            AttitudesID = -1;
            ToWhat = "";
            TypeOf = ConstantsAndTypes.ATTITUDE_TYPES.Optimism;
            Belief = 0;
            Feeling = 0;
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
                    var sql = "DELETE FROM [Attitudes] WHERE AttitudesID = " + AttitudesID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Attitude with ID " + AttitudesID.ToString() + " successfully");
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
                            values.Put("ToWhat", ToWhat);
                            values.Put("TypeOf", (int)TypeOf);
                            values.Put("Belief", Belief);
                            values.Put("Feeling", Feeling);
                            values.Put("Action", (int)Action);
                            values.Put("ActionOf", ActionOf);

                            AttitudesID = (int)sqlDatabase.Insert("Attitudes", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            ContentValues values = new ContentValues();
                            values.Put("ToWhat", ToWhat);
                            values.Put("TypeOf", (int)TypeOf);
                            values.Put("Belief", Belief);
                            values.Put("Feeling", Feeling);
                            values.Put("Action", (int)Action);
                            values.Put("ActionOf", ActionOf);

                            string whereClause = "AttitudesID = ?";
                            sqlDatabase.Update("Attitudes", values, whereClause, new string[] { AttitudesID.ToString() });
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
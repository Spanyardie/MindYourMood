using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using com.spanyardie.MindYourMood.Helpers;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class Feelings : FeelingsBase
    {
        public const string TAG = "M:Feelings";
        
        public Feelings()
        {
            FeelingsID = -1;
            AboutWhat = "";
            Strength = 0;
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
                    var sql = "DELETE FROM [Feelings] WHERE FeelingsID = " + FeelingsID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Feeling with ID " + FeelingsID.ToString() + " successfully");
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
                            values.Put("AboutWhat", AboutWhat);
                            values.Put("Strength", Strength);
                            values.Put("Type", (int)Type);
                            values.Put("Action", (int)Action);
                            values.Put("ActionOf", ActionOf);

                            FeelingsID = (int)sqlDatabase.Insert("Feelings", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            ContentValues values = new ContentValues();
                            values.Put("AboutWhat", AboutWhat);
                            values.Put("Strength", Strength);
                            values.Put("Type", (int)Type);
                            values.Put("Action", (int)Action);
                            values.Put("ActionOf", ActionOf);

                            string whereClause = "FeelingsID = ?";
                            sqlDatabase.Update("Feelings", values, whereClause, new string[] { FeelingsID.ToString() });
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
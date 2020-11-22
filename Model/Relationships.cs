using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using com.spanyardie.MindYourMood.Helpers;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class Relationships : RelationshipsBase
    {
        public const string TAG = "M:Relationships";

        public Relationships()
        {
            RelationshipsID = -1;
            WithWhom = "";
            Type = ConstantsAndTypes.RELATIONSHIP_TYPE.Aquaintance;
            Strength = 0;
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
                    var sql = "DELETE FROM [Relationships] WHERE RelationshipsID = " + RelationshipsID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Relationship with ID " + RelationshipsID.ToString() + " successfully");
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
                            values.Put("WithWhom", WithWhom);
                            values.Put("Type", (int)Type);
                            values.Put("Strength", Strength);
                            values.Put("Feeling", Feeling);
                            values.Put("Action", (int)Action);
                            values.Put("ActionOf", ActionOf);

                            RelationshipsID = (int)sqlDatabase.Insert("Relationships", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            ContentValues values = new ContentValues();
                            values.Put("WithWhom", WithWhom);
                            values.Put("Type", (int)Type);
                            values.Put("Strength", Strength);
                            values.Put("Feeling", Feeling);
                            values.Put("Action", (int)Action);
                            values.Put("ActionOf", ActionOf);

                            string whereClause = "RelationshipsID = ?";
                            sqlDatabase.Update("Relationships", values, whereClause, new string[] { RelationshipsID.ToString() });
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
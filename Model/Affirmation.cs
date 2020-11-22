using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Util;
using Android.Database.Sqlite;

namespace com.spanyardie.MindYourMood.Model
{
    public class Affirmation : AffirmationBase
    {
        public const string TAG = "M:Affirmation";

        public Affirmation()
        {
            AffirmationID = -1;
            AffirmationText = "";
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
                    var sql = "DELETE FROM [Affirmations] WHERE AffirmationID = " + AffirmationID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Affirmation with ID " + AffirmationID.ToString() + " successfully");
                    sqlDatabase.Close();
                }
                else
                {
                    Log.Error(TAG, "Remove: SQLite database is null or was not opened - remove failed");
                }
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
                        ContentValues values = new ContentValues();
                        values.Put("AffirmationText", AffirmationText.Trim());
                        if (IsNew)
                        {
                            AffirmationID = (int)sqlDatabase.Insert("Affirmations", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            string whereClause = "AffirmationID = ?";
                            sqlDatabase.Update("Affirmations", values, whereClause, new string[] { AffirmationID.ToString() });
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
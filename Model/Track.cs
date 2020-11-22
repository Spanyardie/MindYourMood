using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Util;
using Android.Database.Sqlite;

namespace com.spanyardie.MindYourMood.Model
{
    public class Track : TrackBase
    {
        public const string TAG = "M:Track";

        public Track()
        {
            TrackID = -1;
            PlayListID = -1;
            TrackName = "";
            TrackArtist = "";
            TrackDuration = 0.0f;
            TrackOrderNumber = 0;
            TrackUri = "";
            IsNew = true;
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
                    var sql = "DELETE FROM [Tracks] WHERE TrackID = " + TrackID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Track with ID " + TrackID.ToString() + " successfully");
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
                        values.Put("PlayListID", PlayListID);
                        values.Put("TrackName", TrackName.Trim());
                        values.Put("TrackArtist", TrackArtist.Trim());
                        values.Put("TrackDuration", TrackDuration);
                        values.Put("TrackOrderNumber", TrackOrderNumber);
                        values.Put("TrackUri", TrackUri.Trim());
                        if (IsNew)
                        {
                            TrackID = (int)sqlDatabase.Insert("Tracks", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            string whereClause = "TrackID = ?";
                            sqlDatabase.Update("Tracks", values, whereClause, new string[] { TrackID.ToString() });
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
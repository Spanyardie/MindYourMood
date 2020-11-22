using System;
using System.Collections.Generic;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Util;
using Android.Database.Sqlite;

namespace com.spanyardie.MindYourMood.Model
{
    public class PlayList : PlayListBase
    {
        public const string TAG = "M:PlayList";

        public List<Track> PlayListTracks { get; set; }

        public PlayList()
        {
            PlayListTracks = new List<Track>();
            PlayListID = -1;
            PlayListName = "";
            PlayListTrackCount = 0;
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
                    var sql = "DELETE FROM [PlayLists] WHERE PlayListID = " + PlayListID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed PlayList with ID " + PlayListID.ToString() + " successfully");
                    sql = "DELETE FROM [Tracks] WHERE PlayListID = " + PlayListID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed Tracks with PLayList ID " + PlayListID.ToString() + " successfully");
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
                        values.Put("PlayListName", PlayListName.Trim());
                        values.Put("PlayListTrackCount", PlayListTrackCount);
                        if (IsNew)
                        {
                            PlayListID = (int)sqlDatabase.Insert("PlayLists", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            string whereClause = "PlayListID = ?";
                            sqlDatabase.Update("PlayLists", values, whereClause, new string[] { PlayListID.ToString() });
                            IsDirty = false;
                        }
                        sqlDatabase.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Save: Exception - " + e.Message);
            }
        }

        public int GetPlayListTrackCount()
        {
            return PlayListTracks.Count;
        }
    }
}
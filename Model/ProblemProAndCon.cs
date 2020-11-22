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
using com.spanyardie.MindYourMood.Helpers;
using Android.Database.Sqlite;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class ProblemProAndCon : ProblemProAndConBase
    {
        public const string TAG = "M:ProblemProAndCon";

        public ProblemProAndCon()
        {
            ProblemProAndConID = -1;
            ProblemIdeaID = -1;
            ProblemStepID = -1;
            ProblemID = -1;
            ProblemProAndConText = "";
            ProblemProAndConType = ConstantsAndTypes.PROCON_TYPES.Pro;
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
                    var sql = "DELETE FROM [ProblemProsAndCons] WHERE ProblemProAndConID = " + ProblemProAndConID.ToString();
                    sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "Remove: Removed ProblemProAndCon with ID " + ProblemProAndConID.ToString() + " successfully");


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
                        values.Put("ProblemIdeaID", ProblemIdeaID);
                        values.Put("ProblemStepID", ProblemStepID);
                        values.Put("ProblemID", ProblemID);
                        values.Put("ProblemProAndConText", ProblemProAndConText.Trim());
                        values.Put("ProblemProAndConType", (int)ProblemProAndConType);

                        if (IsNew)
                        {
                            ProblemProAndConID = (int)sqlDatabase.Insert("ProblemProsAndCons", null, values);
                            IsNew = false;
                            IsDirty = false;
                        }
                        if (IsDirty)
                        {
                            string whereClause = "ProblemProAndConID = ?";
                            sqlDatabase.Update("ProblemProsAndCons", values, whereClause, new string[] { ProblemProAndConID.ToString() });
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
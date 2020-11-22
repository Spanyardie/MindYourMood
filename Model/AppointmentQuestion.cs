using System;
using Android.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Database.Sqlite;
using Android.Database;

namespace com.spanyardie.MindYourMood.Model
{
    public class AppointmentQuestion : AppointmentQuestionsBase
    {
        public AppointmentQuestion()
        {
            QuestionsID = -1;
            AppointmentID = -1;
            Question = "";
            Answer = "";

            IsNew = true;
            IsDirty = false;
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen && !IsNew)
            {
                string commandText = "DELETE FROM AppointmentQuestions WHERE [QuestionsID] = " + QuestionsID;

                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    throw new SQLException("Removing Appointment Question from database failed - " + e.Message);
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
                        values.Put("AppointmentID", AppointmentID.ToString());
                        values.Put("Question", Question.Trim());
                        values.Put("Answer", Answer.Trim());
                        QuestionsID = (int)sqLiteDatabase.Insert("AppointmentQuestions", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save Appointment Question - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        string whereClause = "QuestionsID = " + QuestionsID;
                        ContentValues values = new ContentValues();

                        values.Put("AppointmentID", AppointmentID);
                        values.Put("Question", Question.Trim());
                        values.Put("Answer", Answer.Trim());
                        sqLiteDatabase.Update("AppointmentQuestions", values, whereClause, null);

                        IsDirty = false;
                    }
                    catch (SQLException dirtyE)
                    {
                        throw new Exception("Unable to Update Appointment Question - " + dirtyE.Message);
                    }
                }
            }
        }
    }
}
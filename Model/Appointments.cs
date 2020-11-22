using System;
using System.Collections.Generic;

using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class Appointments : AppointmentBase
    {
        public const string TAG = "M:Appointment";

        private List<AppointmentQuestion> _questions;

        public List<AppointmentQuestion> Questions
        {
            get
            {
                return _questions;
            }

            set
            {
                _questions = value;
            }
        }

        public Appointments()
        {
            _questions = new List<AppointmentQuestion>();

            AppointmentID = -1;
            AppointmentDate = new DateTime();
            AppointmentType = -1;
            Location = "";
            WithWhom = "";
            AppointmentTime = new DateTime();
            Notes = "";
            IsNew = true;
            IsDirty = false;
        }

        public void RemoveAllQuestionsForAppointment(SQLiteDatabase sqLiteDatabase, int appointmentID)
        {
            if (sqLiteDatabase.IsOpen && !IsNew)
            {
                string commandText = "DELETE FROM AppointmentQuestions WHERE [AppointmentID] = " + appointmentID;

                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    throw new SQLException("Removing Appointment Questions from database failed - " + e.Message);
                }
            }
        }

        public void Remove(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen && !IsNew)
            {
                string commandText = "DELETE FROM Appointments WHERE [AppointmentID] = " + AppointmentID + ";";

                sqLiteDatabase.ExecSQL("BEGIN TRANSACTION;");

                try
                {

                    RemoveAllQuestionsForAppointment(sqLiteDatabase, AppointmentID);
                }
                catch (Exception eQ)
                {
                    sqLiteDatabase.ExecSQL("ROLLBACK;");
                    throw new SQLException("Removing Appointment Questions from database failed - " + eQ.Message);
                }

                try
                {
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    sqLiteDatabase.ExecSQL("ROLLBACK;");
                    throw new SQLException("Removing Appointment from database failed - " + e.Message);
                }

                sqLiteDatabase.ExecSQL("COMMIT;");
            }
        }

        public void LoadAppointmentQuestions(SQLiteDatabase sqLiteDatabase)
        {
            if(sqLiteDatabase.IsOpen && !IsNew)
            {
                if (_questions == null)
                    _questions = new List<AppointmentQuestion>();

                _questions.Clear();

                string[] arrColumns = new string[3];
                arrColumns[0] = "QuestionsID";
                arrColumns[1] = "Question";
                arrColumns[2] = "Answer";

                try
                {
                    var questionData = sqLiteDatabase.Query("AppointmentQuestions", arrColumns, "AppointmentID = " + AppointmentID.ToString(), null, null, null, null);
                    if(questionData != null)
                    {
                        var count = questionData.Count;
                        if(count > 0)
                        {
                            questionData.MoveToFirst();
                            for(var loop = 0; loop < count; loop++)
                            {
                                var question = new AppointmentQuestion();
                                question.AppointmentID = AppointmentID;
                                question.QuestionsID = questionData.GetInt(questionData.GetColumnIndex("QuestionsID"));
                                question.Question = questionData.GetString(questionData.GetColumnIndex("Question"));
                                question.Answer = questionData.GetString(questionData.GetColumnIndex("Answer"));
                                question.IsNew = false;
                                _questions.Add(question);
                                questionData.MoveToNext();
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Error(TAG, "LoadAppointmentQuestions: Exception - " + e.Message);
                    _questions = new List<AppointmentQuestion>();
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
                        values.Put("AppointmentDate", string.Format("{0:yyyy-MM-dd HH:mm:ss}", AppointmentDate));
                        values.Put("AppointmentType", AppointmentType);
                        values.Put("Location", Location.Trim());
                        values.Put("WithWhom", WithWhom.Trim());
                        values.Put("AppointmentTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", AppointmentTime));
                        values.Put("Notes", Notes.Trim());
                        AppointmentID = (int)sqLiteDatabase.Insert("Appointments", null, values);

                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception newE)
                    {
                        throw new Exception("Unable to Save Appointment - " + newE.Message);
                    }
                }

                if (IsDirty)
                {
                    try
                    {
                        string whereClause = "AppointmentID = " + AppointmentID;
                        ContentValues values = new ContentValues();

                        values.Put("AppointmentDate", string.Format("{0:yyyy-MM-dd HH:mm:ss}", AppointmentDate));
                        values.Put("AppointmentType", AppointmentType);
                        values.Put("Location", Location.Trim());
                        values.Put("WithWhom", WithWhom.Trim());
                        values.Put("AppointmentTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", AppointmentTime));
                        values.Put("Notes", Notes.Trim());
                        sqLiteDatabase.Update("Appointments", values, whereClause, null);

                        IsDirty = false;
                    }
                    catch (SQLException dirtyE)
                    {
                        throw new Exception("Unable to Update Appointment Question - " + dirtyE.Message);
                    }
                }

                if(_questions.Count > 0)
                {
                    foreach(var question in _questions)
                    {
                        if(question.IsNew || question.IsDirty)
                        {
                            if (sqLiteDatabase.IsOpen)
                            {
                                if (question.IsNew)
                                    question.AppointmentID = AppointmentID;
                                question.Save(sqLiteDatabase);
                            }
                        }
                    }
                }
            }
        }
    }
}
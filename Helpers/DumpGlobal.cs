using System.Collections.Generic;
using Android.Util;
using com.spanyardie.MindYourMood.Model;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class DumpGlobal
    {
        public const string TAG = "M:DumpGlobal";

        public static void Medication()
        {
            if(GlobalData.MedicationItems != null && GlobalData.MedicationItems.Count > 0)
            {
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "**           DUMP OF MEDICATION GLOBAL              **");
                Log.Info(TAG, "******************************************************");
                Log.Info(TAG, "");
                foreach(var med in GlobalData.MedicationItems)
                {
                    var MedicationName = med.MedicationName;
                    Log.Info(TAG, "MedicationName - " + MedicationName);
                    Log.Info(TAG, "-----------------------------------------------------");
                    var ID = med.ID.ToString();
                    Log.Info(TAG, "ID - " + ID);
                    var IsDirty = med.IsDirty ? "TRUE" : "FALSE";
                    Log.Info(TAG, "IsDirty - " + IsDirty);
                    var IsNew = med.IsNew ? "TRUE" : "FALSE";
                    Log.Info(TAG, "IsNew - " + IsNew);

                    var MedicationSpread = med.MedicationSpread;
                    if(MedicationSpread != null && MedicationSpread.Count > 0)
                    {
                        foreach(var spread in MedicationSpread)
                        {
                            Log.Info(TAG, "");
                            Log.Info(TAG, "    Medication Spread");
                            Log.Info(TAG, "    -----------------");
                            var Dosage = spread.Dosage.ToString();
                            Log.Info(TAG, "        Dosage - " + Dosage);
                            var FoodRelevance = StringHelper.MedicationFoodForConstant(spread.FoodRelevance);
                            Log.Info(TAG, "        FoodRelevance - " + FoodRelevance);
                            var SpreadID = spread.ID.ToString();
                            Log.Info(TAG, "        SpreadID - " + SpreadID);
                            var SpreadIsDirty = spread.IsDirty ? "TRUE" : "FALSE";
                            Log.Info(TAG, "        SpreadIsDirty - " + SpreadIsDirty);
                            var SpreadIsNew = spread.IsNew ? "TRUE" : "FALSE";
                            Log.Info(TAG, "        SpreadIsNew - " + SpreadIsNew);
                            var MedicationID = spread.MedicationID.ToString();
                            Log.Info(TAG, "        MedicationID - " + MedicationID);

                            var MedicationTakeReminder = spread.MedicationTakeReminder;
                            if(MedicationTakeReminder != null && MedicationTakeReminder.ID != -1)
                            {
                                Log.Info(TAG, "");
                                Log.Info(TAG, "        Medication Reminder");
                                Log.Info(TAG, "        -------------------");
                                var ReminderID = MedicationTakeReminder.ID.ToString();
                                Log.Info(TAG, "            ReminderID - " + ReminderID);
                                var ReminderIsDirty = MedicationTakeReminder.IsDirty ? "TRUE" : "FALSE";
                                Log.Info(TAG, "            ReminderIsDirty - " + ReminderIsDirty);
                                var ReminderIsNew = MedicationTakeReminder.IsNew ? "TRUE" : "False";
                                Log.Info(TAG, "            ReminderIsNew - " + ReminderIsNew);
                                var ReminderIsSet = MedicationTakeReminder.IsSet ? "TRUE" : "False";
                                Log.Info(TAG, "            ReminderIsSet - " + ReminderIsSet);
                                var MedicationDay = StringHelper.DayStringForConstant(MedicationTakeReminder.MedicationDay);
                                Log.Info(TAG, "            MedicationDay - " + MedicationDay);
                                var MedicationSpreadID = MedicationTakeReminder.MedicationSpreadID.ToString();
                                Log.Info(TAG, "            MedicationSpreadID - " + MedicationSpreadID);
                                var MedicationTime = MedicationTakeReminder.MedicationTime.ToShortTimeString();
                                Log.Info(TAG, "            MedicationTime - " + MedicationTime);
                                Log.Info(TAG, "");
                            }

                            var MedicationTakeTime = spread.MedicationTakeTime;
                            if(MedicationTakeTime != null && MedicationTakeTime.ID != -1)
                            {
                                Log.Info(TAG, "");
                                Log.Info(TAG, "        Medication Time");
                                Log.Info(TAG, "        ---------------");
                                var TimeID = MedicationTakeTime.ID.ToString();
                                Log.Info(TAG, "            TimeID - " + TimeID);
                                var TimeIsDirty = MedicationTakeTime.IsDirty ? "TRUE" : "FALSE";
                                Log.Info(TAG, "            TimeIsDirty - " + TimeIsDirty);
                                var TimeIsNew = MedicationTakeTime.IsNew ? "TRUE" : "FALSE";
                                Log.Info(TAG, "            TimeIsNew - " + TimeIsNew);
                                var TimeMedicationSpreadID = MedicationTakeTime.MedicationSpreadID.ToString();
                                Log.Info(TAG, "            TimeMedicationSpreadID - " + TimeMedicationSpreadID);
                                var TimeMedicationTime = StringHelper.MedicationTimeForConstant(MedicationTakeTime.MedicationTime);
                                Log.Info(TAG, "            TimeMedicationTime - " + TimeMedicationTime);
                                var TimeTakenTime = MedicationTakeTime.TakenTime.ToShortTimeString();
                                Log.Info(TAG, "            TimeTakenTime - " + TimeTakenTime);
                                Log.Info(TAG, "");
                            }
                        }
                    }

                    var PrescriptionType = med.PrescriptionType;
                    if(PrescriptionType != null)
                    {
                        Log.Info(TAG, "");
                        Log.Info(TAG, "    Prescription");
                        Log.Info(TAG, "    ------------");
                        var PrescriptionID = PrescriptionType.ID.ToString();
                        Log.Info(TAG, "        PrescriptionID - " + PrescriptionID);
                        var PrescriptionIsDirty = PrescriptionType.IsDirty ? "TRUE" : "FALSE";
                        Log.Info(TAG, "        PrescriptionIsDirty - " + PrescriptionIsDirty);
                        var PrescriptionIsNew = PrescriptionType.IsNew ? "TRUE" : "FALSE";
                        Log.Info(TAG, "        PrescriptionIsNew - " + PrescriptionIsNew);
                        var PrescriptionMedicationID = PrescriptionType.MedicationID.ToString();
                        Log.Info(TAG, "        PrescriptionMedicationID - " + PrescriptionMedicationID);
                        //var PrescriptionMonthlyDay = PrescriptionType.MonthlyDay.ToString();
                        //Log.Info(TAG, "        PrescriptionMonthlyDay - " + PrescriptionMonthlyDay);
                        var PrescriptionTypePrescriptionType = StringHelper.PrescriptionStringForConstant(PrescriptionType.PrescriptionType);
                        Log.Info(TAG, "        PrescriptionPrescriptionType - " + PrescriptionTypePrescriptionType);
                        var PrescriptionWeeklyDay = StringHelper.DayStringForConstant(PrescriptionType.WeeklyDay);
                        Log.Info(TAG, "        PrescriptionWeeklyDay - " + PrescriptionWeeklyDay);
                        Log.Info(TAG, "");
                    }

                    var TotalDailyDosage = med.TotalDailyDosage.ToString();
                    Log.Info(TAG, "TotalDailyDosage - " + TotalDailyDosage);
                    Log.Info(TAG, "******************************************************");
                    Log.Info(TAG, "");
                }
            }
        }

        public static void Appointments(List<Appointments> appointments)
        {

            if(appointments != null && appointments.Count > 0)
            {
                foreach(Appointments appt in appointments)
                {
                    Log.Info(TAG, "");
                    Log.Info(TAG, "Appointment");
                    Log.Info(TAG, "-----------");
                    Log.Info(TAG, "");
                    Log.Info(TAG, "Appointment ID - " + appt.AppointmentID.ToString());
                    Log.Info(TAG, "Appointment Date - " + appt.AppointmentDate.ToShortDateString());
                    Log.Info(TAG, "Appointment Type - " + appt.AppointmentType.ToString());
                    Log.Info(TAG, "Location - " + appt.Location);
                    Log.Info(TAG, "With - " + appt.WithWhom);
                    Log.Info(TAG, "Appointment Time - " + appt.AppointmentTime.ToShortTimeString());
                    Log.Info(TAG, "Notes - " + appt.Notes);
                    Log.Info(TAG, "");
                    Log.Info(TAG, "Questions");
                    Log.Info(TAG, "");
                    if(appt.Questions != null && appt.Questions.Count > 0)
                    {
                        foreach(AppointmentQuestion question in appt.Questions)
                        {
                            Log.Info(TAG, "");
                            Log.Info(TAG, "Appointment ID - " + question.AppointmentID.ToString());
                            Log.Info(TAG, "Questions ID - " + question.QuestionsID.ToString());
                            Log.Info(TAG, "Question - " + question.Question);
                            Log.Info(TAG, "Answer - " + question.Answer);
                        }
                    }
                    else
                    {
                        Log.Info(TAG, "No question defined");
                    }
                    Log.Info(TAG, "    ");
                }
            }
            else
            {
                Log.Info(TAG, "No appointments in passed list");
            }
        }
    }
}
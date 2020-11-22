using System;
using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IMedicationReminder
    {
        void MedicationReminderAdded(ConstantsAndTypes.DAYS_OF_THE_WEEK day, DateTime reminderTime, int medicationTimeID);
    }
}
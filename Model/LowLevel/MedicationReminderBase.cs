using com.spanyardie.MindYourMood.Helpers;
using System;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class MedicationReminderBase
    {
        public int ID { get; set; }
        public int MedicationSpreadID { get; set; }
        public ConstantsAndTypes.DAYS_OF_THE_WEEK MedicationDay { get; set; }
        public DateTime MedicationTime { get; set; }

        public bool IsSet { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
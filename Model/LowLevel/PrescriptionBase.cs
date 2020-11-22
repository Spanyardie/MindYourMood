using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class PrescriptionBase
    {
        public int ID { get; set; }
        public int MedicationID { get; set; }
        public ConstantsAndTypes.PRESCRIPTION_TYPE PrescriptionType { get; set; }
        public ConstantsAndTypes.DAYS_OF_THE_WEEK WeeklyDay { get; set; }
        public int MonthlyDay { get; set; }

        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
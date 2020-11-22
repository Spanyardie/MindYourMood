using System;
using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class MedicationTimeBase
    {
        public int ID { get; set; }
        public int MedicationSpreadID { get; set; }
        public ConstantsAndTypes.MEDICATION_TIME MedicationTime { get; set; }
        public DateTime TakenTime { get; set; }

        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
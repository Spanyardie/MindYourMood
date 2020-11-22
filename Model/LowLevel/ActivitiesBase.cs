using System;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ActivitiesBase
    {
        public int ActivityID { get; set; }
        public DateTime ActivityDate { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
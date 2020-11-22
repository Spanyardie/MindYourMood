using MindYourMood.Helpers;
using System;

namespace MindYourMood.Model.LowLevel
{
    public class ActivitiesBase
    {
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public DateTime ActivityDate { get; set; }
        public ConstantsAndTypes.ACTIVITY_HOURS ActivityTime { get; set; }
        public int Achievement { get; set; }
        public int Intimacy { get; set; }
        public int Pleasure { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
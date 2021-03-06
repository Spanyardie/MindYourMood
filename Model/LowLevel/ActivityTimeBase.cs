using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ActivityTimeBase
    {
        public int ActivityTimeID { get; set; }
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public ConstantsAndTypes.ACTIVITY_HOURS ActivityTime { get; set; }
        public int Achievement { get; set; }
        public int Intimacy { get; set; }
        public int Pleasure { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
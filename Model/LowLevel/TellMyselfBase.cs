using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class TellMyselfBase
    {
        public int ID { get; set; }
        public ConstantsAndTypes.TELL_TYPE TellType { get; set; }
        public string TellText { get; set; }
        public string TellTitle { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
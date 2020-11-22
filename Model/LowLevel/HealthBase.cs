using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class HealthBase
    {
        public int HealthID { get; set; }
        public string Aspect { get; set; }
        public int Importance { get; set; }
        public ConstantsAndTypes.REACTION_TYPE Type { get; set; }
        public ConstantsAndTypes.ACTION_TYPE Action { get; set; }
        public string ActionOf { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
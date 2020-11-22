using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class FeelingsBase
    {
        public int FeelingsID { get; set; }
        public string AboutWhat { get; set; }
        public int Strength { get; set; }
        public ConstantsAndTypes.REACTION_TYPE Type { get; set; }
        public ConstantsAndTypes.ACTION_TYPE Action { get; set; }
        public string ActionOf { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
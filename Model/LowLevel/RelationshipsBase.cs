using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class RelationshipsBase
    {
        public int RelationshipsID { get; set; }
        public string WithWhom { get; set; }
        public ConstantsAndTypes.RELATIONSHIP_TYPE Type { get; set; }
        public int Strength { get; set; }
        public int Feeling { get; set; }
        public ConstantsAndTypes.ACTION_TYPE Action { get; set; }
        public string ActionOf { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
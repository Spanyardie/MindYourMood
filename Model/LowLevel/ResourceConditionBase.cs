namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ResourceConditionBase
    {
        public int ConditionId { get; set; }
        public string ConditionTitle { get; set; }
        public string ConditionDescription { get; set; }
        public string ConditionCitation { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class AffirmationBase
    {
        public int AffirmationID { get; set; }
        public string AffirmationText { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
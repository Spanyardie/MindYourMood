namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class PlayListBase
    {
        public int PlayListID { get; set; }
        public string PlayListName { get; set; }
        public int PlayListTrackCount { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
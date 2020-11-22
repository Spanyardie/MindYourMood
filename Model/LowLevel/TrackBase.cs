namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class TrackBase
    {
        public int TrackID { get; set; }
        public int PlayListID { get; set; }
        public string TrackName { get; set; }
        public string TrackArtist { get; set; }
        public float TrackDuration { get; set; }
        public int TrackOrderNumber { get; set; }
        public string TrackUri { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ImageryBase
    {
        public int ImageryID { get; set; }
        public string ImageryURI { get; set; }
        public string ImageryComment { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
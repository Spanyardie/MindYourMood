using Android.Graphics;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ContactBase
    {
        public int ID { get; set; }
        public string ContactUri { get; set; }
        public string ContactName { get; set; }
        public string ContactTelephoneNumber { get; set; }
        public Bitmap ContactPhoto { get; set; }
        public string ContactEmail { get; set; }
        public bool ContactEmergencyCall { get; set; }
        public bool ContactEmergencySms { get; set; }
        public bool ContactEmergencyEmail { get; set; }

        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
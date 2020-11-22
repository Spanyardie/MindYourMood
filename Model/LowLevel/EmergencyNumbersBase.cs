namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class EmergencyNumberBase
    {
        private bool _isNew;
        private bool _isDirty;

        public int EmergencyNumberID { get; set; }
        public string CountryName { get; set; }
        public string PoliceNumber { get; set; }
        public string AmbulanceNumber { get; set; }
        public string FireNumber { get; set; }
        public string Notes { get; set; }

        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }

            set
            {
                _isDirty = value;
            }
        }

        public bool IsNew
        {
            get
            {
                return _isNew;
            }

            set
            {
                _isNew = value;
            }
        }
    }
}
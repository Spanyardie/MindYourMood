namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class MoodListBase
    {
        private int _moodId;
        private string _moodName;
        private string _moodIsoCountryAlias;

        private string _isDefault = "false";

        private bool _isNew;

        private bool _isDirty;

        public string MoodIsoCountryAlias
        {
            get
            {
                return _moodIsoCountryAlias;
            }

            set
            {
                _moodIsoCountryAlias = value;
            }
        }

        public int MoodId
        {
            get
            {
                return _moodId;
            }

            set
            {
                _moodId = value;
            }
        }

        public string MoodName
        {
            get
            {
                return _moodName;
            }

            set
            {
                _moodName = value;
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

        public string IsDefault
        {
            get
            {
                return _isDefault;
            }

            set
            {
                _isDefault = value;
            }
        }
    }
}
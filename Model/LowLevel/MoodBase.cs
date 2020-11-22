namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class MoodBase
    {
        private long _moodsId;
        private long _thoughtRecordId;
        private long _moodListId;
        private int _moodRating;

        private bool _isNew;
        private bool _isDirty;

        public long MoodsId
        {
            get
            {
                return _moodsId;
            }

            set
            {
                _moodsId = value;
            }
        }

        public long ThoughtRecordId
        {
            get
            {
                return _thoughtRecordId;
            }

            set
            {
                _thoughtRecordId = value;
            }
        }

        public long MoodListId
        {
            get
            {
                return _moodListId;
            }

            set
            {
                _moodListId = value;
            }
        }

        public int MoodRating
        {
            get
            {
                return _moodRating;
            }

            set
            {
                _moodRating = value;
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
    }
}
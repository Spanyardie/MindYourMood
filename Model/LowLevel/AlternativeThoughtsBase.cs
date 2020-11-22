namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class AlternativeThoughtsBase
    {
        private int _alternativeThoughtsId;
        private long _thoughtRecordId;
        private string _alternative;
        private int _beliefRating;

        private bool _isNew;
        private bool _isDirty;

        public int AlternativeThoughtsId
        {
            get
            {
                return _alternativeThoughtsId;
            }

            set
            {
                _alternativeThoughtsId = value;
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

        public string Alternative
        {
            get
            {
                return _alternative;
            }

            set
            {
                _alternative = value;
            }
        }

        public int BeliefRating
        {
            get
            {
                return _beliefRating;
            }

            set
            {
                _beliefRating = value;
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
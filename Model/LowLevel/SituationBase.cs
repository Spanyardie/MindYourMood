namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class SituationBase
    {
        private long _situationId;
        private long _thoughtRecordId;
        private string _who;
        private string _what;
        private string _when;
        private string _where;

        private bool _isNew;
        private bool _isDirty;

        public long SituationId
        {
            get
            {
                return _situationId;
            }

            set
            {
                _situationId = value;
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

        public string Who
        {
            get
            {
                return _who;
            }

            set
            {
                _who = value;
            }
        }

        public string What
        {
            get
            {
                return _what;
            }

            set
            {
                _what = value;
            }
        }

        public string When
        {
            get
            {
                return _when;
            }

            set
            {
                _when = value;
            }
        }

        public string Where
        {
            get
            {
                return _where;
            }

            set
            {
                _where = value;
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
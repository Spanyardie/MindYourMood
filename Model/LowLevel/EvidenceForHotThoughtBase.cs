namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class EvidenceForHotThoughtBase
    {
        private long _evidenceForHotThoughtId;
        private long _thoughtRecordId;
        private long _automaticThoughtsId;
        private string _evidence;

        private bool _isNew;
        private bool _isDirty;

        public long EvidenceForHotThoughtId
        {
            get
            {
                return _evidenceForHotThoughtId;
            }

            set
            {
                _evidenceForHotThoughtId = value;
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

        public long AutomaticThoughtsId
        {
            get
            {
                return _automaticThoughtsId;
            }

            set
            {
                _automaticThoughtsId = value;
            }
        }

        public string Evidence
        {
            get
            {
                return _evidence;
            }

            set
            {
                _evidence = value;
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
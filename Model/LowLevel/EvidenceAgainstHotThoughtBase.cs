namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class EvidenceAgainstHotThoughtBase
    {
        private int _evidenceAgainstHotThoughtId;
        private long _thoughtRecordId;
        private int _automaticThoughtsId;
        private string _evidence;

        private bool _isNew;
        private bool _isDirty;

        public int EvidenceAgainstHotThoughtId
        {
            get
            {
                return _evidenceAgainstHotThoughtId;
            }

            set
            {
                _evidenceAgainstHotThoughtId = value;
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

        public int AutomaticThoughtsId
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
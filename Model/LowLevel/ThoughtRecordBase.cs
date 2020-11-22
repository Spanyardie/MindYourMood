using System;
using System.Collections.Generic;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ThoughtRecordBase
    {
        private long _thoughtRecordId;
        private DateTime _recordDateTime;

        private Situation _situation;
        private List<Mood> _moods;
        private List<AutomaticThoughts> _automaticThoughtsList;
        private List<EvidenceForHotThought> _evidenceForHotThoughtList;
        private List<EvidenceAgainstHotThought> _evidenceAgainstHotThoughtList;
        private List<AlternativeThoughts> _alternativeThoughtsList;
        private List<RerateMood> _rerateMoodList;

        private bool _isNew;
        private bool _isDirty;

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

        public DateTime RecordDateTime
        {
            get
            {
                return _recordDateTime;
            }

            set
            {
                _recordDateTime = value;
            }
        }

        public Situation Situation
        {
            get
            {
                return _situation;
            }

            set
            {
                _situation = value;
            }
        }

        public List<Mood> Moods
        {
            get
            {
                return _moods;
            }

            set
            {
                _moods = value;
            }
        }

        public List<AutomaticThoughts> AutomaticThoughtsList
        {
            get
            {
                return _automaticThoughtsList;
            }

            set
            {
                _automaticThoughtsList = value;
            }
        }

        public List<EvidenceForHotThought> EvidenceForHotThoughtList
        {
            get
            {
                return _evidenceForHotThoughtList;
            }

            set
            {
                _evidenceForHotThoughtList = value;
            }
        }

        public List<EvidenceAgainstHotThought> EvidenceAgainstHotThoughtList
        {
            get
            {
                return _evidenceAgainstHotThoughtList;
            }

            set
            {
                _evidenceAgainstHotThoughtList = value;
            }
        }

        public List<AlternativeThoughts> AlternativeThoughtsList
        {
            get
            {
                return _alternativeThoughtsList;
            }

            set
            {
                _alternativeThoughtsList = value;
            }
        }

        public List<RerateMood> RerateMoodList
        {
            get
            {
                return _rerateMoodList;
            }

            set
            {
                _rerateMoodList = value;
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
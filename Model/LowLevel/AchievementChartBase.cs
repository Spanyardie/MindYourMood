using System;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class AchievementChartBase
    {
        private int _achievementId;
        private DateTime _achievementDate;
        private string _achievement;

        private bool _isNew;
        private bool _isDirty;

        public int AchievementId
        {
            get
            {
                return _achievementId;
            }

            set
            {
                _achievementId = value;
            }
        }

        public DateTime AchievementDate
        {
            get
            {
                return _achievementDate;
            }

            set
            {
                _achievementDate = value;
            }
        }

        public string Achievement
        {
            get
            {
                return _achievement;
            }

            set
            {
                _achievement = value;
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
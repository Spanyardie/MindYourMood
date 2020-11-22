namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class RerateMoodBase : MoodBase
    {
        private int _rerateMoodId;

        public int RerateMoodId
        {
            get
            {
                return _rerateMoodId;
            }

            set
            {
                _rerateMoodId = value;
            }
        }
    }
}
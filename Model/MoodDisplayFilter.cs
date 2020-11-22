namespace com.spanyardie.MindYourMood.Model
{
    public class MoodDisplayFilter
    {
        private bool _selected;

        public bool Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;
            }
        }

        public MoodDisplayFilter()
        {
            //required for deserialisation to work
        }

    }
}
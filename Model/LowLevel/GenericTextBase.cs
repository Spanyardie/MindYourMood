using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class GenericTextBase
    {
        public int ID { get; set; }
        public ConstantsAndTypes.GENERIC_TEXT_TYPE TextType { get; set; }
        public string TextValue { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
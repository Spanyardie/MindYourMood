using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ProblemProAndConBase
    {
        public int ProblemProAndConID { get; set; }
        public int ProblemIdeaID { get; set; }
        public int ProblemStepID { get; set; }
        public int ProblemID { get; set; }
        public string ProblemProAndConText { get; set; }
        public ConstantsAndTypes.PROCON_TYPES ProblemProAndConType { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ProblemIdeaBase
    {
        public int ProblemIdeaID { get; set; }
        public int ProblemStepID { get; set; }
        public int ProblemID { get; set; }
        public string ProblemIdeaText { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
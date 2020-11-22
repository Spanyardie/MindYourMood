namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ProblemStepBase
    {
        public int ProblemStepID { get; set; }
        public int ProblemID { get; set; }
        public string ProblemStep { get; set; }
        public int PriorityOrder { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
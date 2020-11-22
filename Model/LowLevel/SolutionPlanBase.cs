namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class SolutionPlanBase
    {
        public int SolutionPlanID { get; set; }
        public int ProblemIdeaID { get; set; }
        public string SolutionStep { get; set; }
        public int PriorityOrder { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ProblemBase
    {
        public int ProblemID { get; set; }
        public string ProblemText { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
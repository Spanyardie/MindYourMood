using System;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class SolutionReviewBase
    {
        public int SolutionReviewID { get; set; }
        public int ProblemIdeaID { get; set; }
        public string ReviewText { get; set; }
        public bool Achieved { get; set; }
        public DateTime AchievedDate { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
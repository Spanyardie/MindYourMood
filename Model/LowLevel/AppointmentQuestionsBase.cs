namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class AppointmentQuestionsBase
    {
        public int QuestionsID { get; set; }
        public int AppointmentID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
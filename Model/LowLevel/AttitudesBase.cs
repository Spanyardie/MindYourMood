using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class AttitudesBase
    {
        public int AttitudesID { get; set; }
        public string ToWhat { get; set; }
        public ConstantsAndTypes.ATTITUDE_TYPES TypeOf { get; set; }
        public int Belief { get; set; }
        public int Feeling { get; set; }
        public ConstantsAndTypes.ACTION_TYPE Action { get; set; }
        public string ActionOf { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
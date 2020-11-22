using System;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class AppointmentBase
    {
        public int AppointmentID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int AppointmentType { get; set; }
        public string Location { get; set; }
        public string WithWhom { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string Notes { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
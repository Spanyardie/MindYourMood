using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class SafetyPlanCardBase
    {
        public int ID { get; set; }
        public string CalmMyself { get; set; }
        public string TellMyself { get; set; }
        public string WillCall { get; set; }
        public string WillGoTo { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}
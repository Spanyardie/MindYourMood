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

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    public interface IMedicationSpeakCallback
    {
        void SpeakMedicationName();
        void SpeakMedicationDosage();
        void SpeakMonthDay();
    }
}
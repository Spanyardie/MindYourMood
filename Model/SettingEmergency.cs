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
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class SettingEmergency : Setting
    {
        public string PoliceNumber { get; set; }
        public string AmbulanceNumber { get; set; }
        public string FireNumber { get; set; }
        public string Notes { get; set; }

        public SettingEmergency()
        {
            PoliceNumber = "";
            AmbulanceNumber = "";
            FireNumber = "";
            Notes = "";
        }

        public void LoadSettingValuesForCountry()
        {
            string policeNumber = "";
            string ambulanceNumber = "";
            string fireNumber = "";
            string notes = "";

            try
            {
                if (SettingKey != null && SettingKey == "EmergencyLocale")
                {
                    if (SettingValue != null && SettingValue.Trim() != "")
                    {
                        GlobalData.GetEmergencyNumbersForCountry(SettingValue.Trim(), out policeNumber, out ambulanceNumber, out fireNumber, out notes);
                    }
                    PoliceNumber = policeNumber;
                    AmbulanceNumber = ambulanceNumber;
                    FireNumber = fireNumber;
                    Notes = notes;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "LoadSettingValuesForCountry: Exception - " + e.Message);
                throw;
            }
        }
    }
}
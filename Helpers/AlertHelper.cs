using System;
using Android.Util;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.App;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class AlertHelper
    {
        public const string TAG = "M:AlertHelper";

        private Activity _activity;

        public string AlertTitle { get; set; }
        public string AlertMessage { get; set; }
        public string AlertPositiveCaption { get; set; }
        public string AlertNegativeCaption { get; set; }
        public int AlertIconResourceID { get; set; }
        public string InstanceId { get; set; }

        public AlertHelper(Activity activity)
        {
            _activity = activity;
            AlertIconResourceID = -1;
            AlertMessage = "Default message";
            AlertNegativeCaption = "Default negative caption";
            AlertPositiveCaption = "Default positive caption";
            AlertTitle = "Default title";
            InstanceId = "";
        }

        public void ShowAlert()
        {
            try
            {
                AlertDialog.Builder dialog = new AlertDialog.Builder(_activity);
                if (dialog != null)
                {
                    Log.Info(TAG, "ShowAlert: Alert Title - " + AlertTitle);
                    dialog.SetTitle(AlertTitle);
                    dialog.SetMessage(AlertMessage);
                    dialog.SetIcon(AlertIconResourceID);
                    dialog.SetPositiveButton(AlertPositiveCaption, (senderObject, eventArgs) =>
                    {
                        if (_activity != null)
                        {
                            ((IAlertCallback)_activity).AlertPositiveButtonSelect(senderObject, eventArgs, InstanceId);
                        }
                    });
                    if (AlertNegativeCaption != "Default negative caption")
                    {
                        dialog.SetNegativeButton(AlertNegativeCaption, (senderObject, eventArgs) =>
                        {
                            if (_activity != null)
                            {
                                ((IAlertCallback)_activity).AlertNegativeButtonSelect(senderObject, eventArgs, InstanceId);
                            }
                        });
                    }
                    dialog.Show();
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ShowAlert: Exception - " + e.Message);
            }
        }
    }
}
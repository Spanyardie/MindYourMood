using Android.Content;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IAlertCallback
    {
        void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId);
        void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId);
    }
}
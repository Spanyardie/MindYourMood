using Android;
using Android.Content.PM;
using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class PermissionBase
    {
        public int PermissionType { get; set; }
        public ConstantsAndTypes.AppPermission ApplicationPermission { get; set; }
        public Permission PermissionGranted { get; set; }
    }
}
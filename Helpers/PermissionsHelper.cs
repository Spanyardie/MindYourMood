using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.Content;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Support.V4.App;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class PermissionsHelper
    {

        public static Permission CheckPermission(Context context, string permission)
        {
            return ContextCompat.CheckSelfPermission(context, permission);
        }

        public static List<PermissionBase> SetupDefaultPermissionList(Context context)
        {
            List<PermissionBase> permissionList = new List<PermissionBase>();

            PermissionBase permission = null;

            //set up each required permission

            //REQUEST_CODE_PERMISSION_READ_CONTACTS
            permission = new PermissionBase();
            permission.PermissionType = ConstantsAndTypes.REQUEST_CODE_PERMISSION_READ_CONTACTS;
            permission.ApplicationPermission = ConstantsAndTypes.AppPermission.ReadContacts;
            permission.PermissionGranted = CheckPermission(context, ConstantsAndTypes.READ_CONTACTS);
            permissionList.Add(permission);

            //REQUEST_CODE_PERMISSION_USE_MICROPHONE
            permission = new PermissionBase();
            permission.PermissionType = ConstantsAndTypes.REQUEST_CODE_PERMISSION_USE_MICROPHONE;
            permission.ApplicationPermission = ConstantsAndTypes.AppPermission.UseMicrophone;
            permission.PermissionGranted = CheckPermission(context, ConstantsAndTypes.USE_MICROPHONE);
            permissionList.Add(permission);

            //REQUEST_CODE_PERMISSION_WRITE_SMS
            permission = new PermissionBase();
            permission.PermissionType = ConstantsAndTypes.REQUEST_CODE_PERMISSION_WRITE_SMS;
            permission.ApplicationPermission = ConstantsAndTypes.AppPermission.WriteSms;
            permission.PermissionGranted = CheckPermission(context, ConstantsAndTypes.WRITE_SMS);
            permissionList.Add(permission);

            //REQUEST_CODE_PERMISSION_SEND_SMS
            permission = new PermissionBase();
            permission.PermissionType = ConstantsAndTypes.REQUEST_CODE_PERMISSION_SEND_SMS;
            permission.ApplicationPermission = ConstantsAndTypes.AppPermission.SendSms;
            permission.PermissionGranted = CheckPermission(context, ConstantsAndTypes.SEND_SMS);
            permissionList.Add(permission);

            //REQUEST_CODE_PERMISSION_MAKE_CALLS
            permission = new PermissionBase();
            permission.PermissionType = ConstantsAndTypes.REQUEST_CODE_PERMISSION_MAKE_CALLS;
            permission.ApplicationPermission = ConstantsAndTypes.AppPermission.MakeCalls;
            permission.PermissionGranted = CheckPermission(context, ConstantsAndTypes.MAKE_CALLS);
            permissionList.Add(permission);

            //REQUEST_CODE_PERMISSION_MODIFY_AUDIO_SETTINGS - Normal permission that is automatically granted

            //REQUEST_CODE_PERMISSION_READ_EXTERNAL_STORAGE
            permission = new PermissionBase();
            permission.PermissionType = ConstantsAndTypes.REQUEST_CODE_PERMISSION_READ_EXTERNAL_STORAGE;
            permission.ApplicationPermission = ConstantsAndTypes.AppPermission.ReadExternalStorage;
            permission.PermissionGranted = CheckPermission(context, ConstantsAndTypes.READ_EXTERNAL_STORAGE);
            permissionList.Add(permission);

            //REQUEST_CODE_PERMISSION_READ_PHONE_STATE
            permission = new PermissionBase();
            permission.PermissionType = ConstantsAndTypes.REQUEST_CODE_PERMISSION_READ_PHONE_STATE;
            permission.ApplicationPermission = ConstantsAndTypes.AppPermission.ReadPhoneState;
            permission.PermissionGranted = CheckPermission(context, ConstantsAndTypes.READ_PHONE_STATE);
            permissionList.Add(permission);

            //REQUEST_CODE_PERMISSION_READ_PROFILE
            permission = new PermissionBase();
            permission.PermissionType = ConstantsAndTypes.REQUEST_CODE_PERMISSION_READ_PROFILE;
            permission.ApplicationPermission = ConstantsAndTypes.AppPermission.ReadProfile;
            permission.PermissionGranted = CheckPermission(context, ConstantsAndTypes.READ_PROFILE);
            permissionList.Add(permission);

            //REQUEST_CODE_PERMISSION_RECEIVE_BOOT_COMPLETED - Normal permission that is automatically granted

            //REQUEST_CODE_PERMISSION_SET_ALARM - Normal permission that is automatically granted

            //REQUEST_CODE_PERMISSION_WAKE_LOCK - Normal permission that is automatically granted

            return permissionList;
        }

        public static bool HasPermission(Context context, ConstantsAndTypes.AppPermission permission)
        {
            if (GlobalData.ApplicationPermissions == null)
                GlobalData.ApplicationPermissions = SetupDefaultPermissionList(context);

            var thePermission = GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == permission);

            return (thePermission != null);
        }

        public static bool PermissionGranted(Context context, ConstantsAndTypes.AppPermission permission)
        {
            if(GlobalData.ApplicationPermissions == null)
                GlobalData.ApplicationPermissions = SetupDefaultPermissionList(context);

            var thePermission = GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == permission);

            if (thePermission != null)
            {
                return (thePermission.PermissionGranted == Permission.Granted);
            }

            return false;
        }

        public static int GetRequestCodeForPermission(Context context, ConstantsAndTypes.AppPermission permission)
        {
            if (GlobalData.ApplicationPermissions == null)
                GlobalData.ApplicationPermissions = SetupDefaultPermissionList(context);

            var thePermission = GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == permission);

            if(thePermission != null)
            {
                return thePermission.PermissionType;
            }

            return -1;
        }

        //Calling activities MUST override OnRequestPermissionsResult
        public static void RequestApplicationPermission(Activity activity, ConstantsAndTypes.AppPermission permission)
        {
            if (GlobalData.ApplicationPermissions == null)
                GlobalData.ApplicationPermissions = SetupDefaultPermissionList(activity);

            var thePermission = GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == permission);

            string[] permissionString = new string[1];
            permissionString = StringHelper.GetPermissionStringForEnum(permission);

            if (thePermission != null && permissionString != null)
            {
                ActivityCompat.RequestPermissions(activity, permissionString, thePermission.PermissionType);
            }
        }

        public static bool ShouldShowPermissionRationale(Activity activity, ConstantsAndTypes.AppPermission permission)
        {
            if (GlobalData.ApplicationPermissions == null)
                GlobalData.ApplicationPermissions = SetupDefaultPermissionList(activity);

            var thePermission = GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == permission);

            string[] permissionString = new string[1];
            permissionString = StringHelper.GetPermissionStringForEnum(permission);

            if (thePermission != null && permissionString != null)
            {
               return ActivityCompat.ShouldShowRequestPermissionRationale(activity, permissionString[0]);
            }

            return false;
        }
    }
}
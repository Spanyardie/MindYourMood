using Android.App;
using Android.Content;
using Android.Database;
using Android.Provider;

using Android.Util;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ContactsHelper
    {
        public const string TAG = "M:ContactsHelper";
        private const string CONTACTS = "contacts";

        private Activity _activity;

        public struct ProfileName
        {
            public string FirstName;
            public string Surname;
            public string ThumbnailUri;
        }

        public ProfileName DeviceOwner { get; set; }

        public ContactsHelper(Activity activity)
        {
            _activity = activity;
            DeviceOwner = new ProfileName();
        }

        public bool ReadProfile()
        {
            Android.Net.Uri uri = ContactsContract.Profile.ContentUri;
            string[] projection = 
            {
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
                ContactsContract.Contacts.InterfaceConsts.PhotoUri
            };

            CursorLoader loader = new CursorLoader(_activity, uri, projection, null, null, null);
            ICursor cursor = (ICursor)loader.LoadInBackground();

            if (cursor != null)
            {
                if (cursor.MoveToFirst())
                {
                    var displayName = cursor.GetString(cursor.GetColumnIndex(projection[0]));
                    var thumbUri = cursor.GetString(cursor.GetColumnIndex(projection[1]));
                    Log.Info(TAG, "ReadProfile: displayName - " + displayName);
                    Log.Info(TAG, "ReadProfile: thumbUri - " + thumbUri);

                    string[] nameSplit = displayName.Split(' ');

                    ProfileName owner = new ProfileName()
                    {
                        FirstName = nameSplit[0],
                        Surname = nameSplit[1],
                        ThumbnailUri = thumbUri
                    };

                    DeviceOwner = owner;
                    return true;
                }
            }
            return false;
        }
    }
}
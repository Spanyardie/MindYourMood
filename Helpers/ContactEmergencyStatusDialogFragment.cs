using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using com.spanyardie.MindYourMood.Model;
using Android.Database.Sqlite;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ContactEmergencyStatusDialogFragment : DialogFragment
    {
        public const string TAG = "M:ContactEmergencyStatusDialogFragment";

        Activity _activity;

        private ImageView _contactThumb;
        private TextView _contact;

        private CheckBox _call;
        private CheckBox _email;
        private CheckBox _sms;

        private ImageButton _done;

        private int _contactId = -1;
        private string _contactName;
        private Bitmap _thumb;
        private bool _willCall = false;
        private bool _willEMail = false;
        private bool _willSMS = false;

        private bool _isDirty = false;

        private bool _updating = false;

        public ContactEmergencyStatusDialogFragment() { }

        public override void OnResume()
        {
            int width = LinearLayout.LayoutParams.MatchParent;
            int height = LinearLayout.LayoutParams.WrapContent;

            Dialog.Window.SetLayout(width, height);

            Dialog.SetTitle(_activity.GetString(Resource.String.ContactEmergencyStatusDialogTitle));
            base.OnResume();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutInt("contactId", _contactId);
                outState.PutString("contactName", _contactName);
                outState.PutBoolean("call", _willCall);
                outState.PutBoolean("email", _willEMail);
                outState.PutBoolean("sms", _willSMS);

                outState.PutParcelable("thumb", _thumb);
            }

            base.OnSaveInstanceState(outState);
        }

        public ContactEmergencyStatusDialogFragment(Activity activity, Int32 contactId, Bitmap thumb, string name, bool willCall, bool willEMail, bool willSMS)
        {
            _activity = activity;

            _contactId = contactId;
            _contactName = name;
            _thumb = thumb;
            _willCall = willCall;
            _willEMail = willEMail;
            _willSMS = willSMS;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            if (context != null)
                _activity = (Activity)context;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = null;

            try
            {
                if (savedInstanceState != null)
                {
                    _contactId = savedInstanceState.GetInt("contactId");
                    _contactName = savedInstanceState.GetString("contactName", "");
                    _willCall = savedInstanceState.GetBoolean("call");
                    _willEMail = savedInstanceState.GetBoolean("email");
                    _willSMS = savedInstanceState.GetBoolean("sms");
                    _thumb = (Bitmap)savedInstanceState.GetParcelable("thumb");
                }

                view = inflater.Inflate(Resource.Layout.ContactEmergencyItemStatusFragmentLayout, container, false);

                if(view != null)
                {
                    GetFieldComponents(view);
                    SetupCallbacks();

                    if(_contactThumb != null)
                    {
                        if(_thumb != null)
                        {
                            _contactThumb.SetImageBitmap(_thumb);
                        }
                        else
                        {
                            _contactThumb.SetImageResource(Resource.Drawable.Moods2);
                        }
                    }
                    if (_contact != null)
                        _contact.Text = _contactName.Trim();

                    _updating = true;
                    if (_call != null)
                        _call.Checked = _willCall;
                    if (_email != null)
                        _email.Checked = _willEMail;
                    if (_sms != null)
                        _sms.Checked = _willSMS;
                    _updating = false;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Creating View", "ContactEmergencyStatusDialogFragment.OnCreateView");
            }

            return view;
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if(view != null)
                {
                    _contactThumb = view.FindViewById<ImageView>(Resource.Id.imgStatusContactImage);
                    _contact = view.FindViewById<TextView>(Resource.Id.txtStatusContactName);

                    _call = view.FindViewById<CheckBox>(Resource.Id.chkContactCall);
                    _call.Tag = "call";

                    _email = view.FindViewById<CheckBox>(Resource.Id.chkContactEMail);
                    _email.Tag = "email";

                    _sms = view.FindViewById<CheckBox>(Resource.Id.chkContactSMS);
                    _sms.Tag = "sms";

                    _done = view.FindViewById<ImageButton>(Resource.Id.imgbtnDone);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting field components", "ContactEmergencyStatusDialogFragment.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_call != null)
                    _call.CheckedChange += CheckedChange;
                if(_email != null)
                    _email.CheckedChange += CheckedChange;
                if(_sms != null)
                    _sms.CheckedChange += CheckedChange;

                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting up callbacks", "ContactEmergencyStatusDialogFragment.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            SQLiteDatabase sqlDatabase = null;
            try
            {
                Contact contact = GlobalData.ContactsUserItems.Find(item => item.ID == _contactId);
                if (_isDirty && contact != null)
                {
                    Globals dbHelp = new Globals();
                    dbHelp.OpenDatabase();
                    sqlDatabase = dbHelp.GetSQLiteDatabase();
                    if (sqlDatabase != null && sqlDatabase.IsOpen)
                        contact.Save(sqlDatabase);
                    sqlDatabase.Close();
                }
            }
            catch(Exception ex)
            {
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                Log.Error(TAG, "Done_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ex, "Saving Contact Status", "ContactEmergencyStatusDialogFragment.Done_Click");
            }

            Dismiss();
        }

        private void SaveContactStatus(string senderType, bool status)
        {
            Contact contact = GlobalData.ContactsUserItems.Find(item => item.ID == _contactId);

            if (contact != null)
            {
                switch (senderType)
                {
                    case "call":
                        contact.ContactEmergencyCall = status;
                        break;
                    case "email":
                        contact.ContactEmergencyEmail = status;
                        break;
                    case "sms":
                        contact.ContactEmergencySms = status;
                        break;
                }
                contact.IsDirty = true;
            }
        }

        private void CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (_updating) return;

            _isDirty = true;
            CheckBox type = (CheckBox)sender;

            if (type != null)
            {
                string tag = type.Tag.ToString();
                bool status = type.Checked;
                SaveContactStatus(tag, status);
            }
        }
    }
}
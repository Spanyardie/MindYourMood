using System;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model;
using Android.Content;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ContactDialogFragment : DialogFragment
    {
        public static string TAG = "M:ContactDialogFragment";

        private View _thisView;

        private Button _cancelButton;
        private Button _confirmButton;
        private ListView _contactList;

        public long ContactUri { get; set; }

        private Activity _parentActivity;

        private int _selectedItemIndex = -1;

        private string _dialogTitle = "";

        public ContactDialogFragment()
        {

        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutInt("selectedItemIndex", _selectedItemIndex);
                outState.PutString("dialogTitle", _dialogTitle);
            }

            base.OnSaveInstanceState(outState);
        }

        public override void OnResume()
        {
            if (Dialog != null)
            {
                int width = LinearLayout.LayoutParams.MatchParent;
                int height = LinearLayout.LayoutParams.WrapContent;

                Dialog.Window.SetLayout(width, height);
            }
            base.OnResume();
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            if (context != null)
                _parentActivity = (Activity)context;
        }

        public ContactDialogFragment(Activity activity, string title)
        {
            _parentActivity = activity;
            _dialogTitle = title;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if (savedInstanceState != null)
                {
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                _thisView = inflater.Inflate(Resource.Layout.ContactFragment, container, false);

                GetFieldComponents();
                SetupCallbacks();

                UpdateAdapter();

                return _thisView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCreatingContactDialog), "ContactDialogFragment.OnCreateView");
                return null;
            }
        }

        private void GetFieldComponents()
        {
            if (_thisView != null)
            {
                _cancelButton = _thisView.FindViewById<Button>(Resource.Id.btnCancel);
                _confirmButton = _thisView.FindViewById<Button>(Resource.Id.btnConfirm);
                _contactList = _thisView.FindViewById<ListView>(Resource.Id.lstContacts);
            }
        }

        private void SetupCallbacks()
        {
            if (_cancelButton != null)
                _cancelButton.Click += CancelButton_Click;
            if (_confirmButton != null)
                _confirmButton.Click += ConfirmButton_Click;
            if(_contactList != null)
                _contactList.ItemClick += ContactList_ItemClick;
        }

        private void ContactList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;
                UpdateAdapter();
                if (_contactList != null)
                    _contactList.SetSelection(_selectedItemIndex);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ContactList_ItemClick: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorSelectingContact), "ContactDialogFragment.ContactList_ItemClick");
            }
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Activity != null)
                {
                    if (_selectedItemIndex != -1)
                    {
                        Contact contact = ((ContactsListAdapter)_contactList.Adapter)._contacts[_selectedItemIndex];
                        //there is not much point in saving this contact if there is a duff telephone number
                        if (string.IsNullOrEmpty(contact.ContactTelephoneNumber))
                        {
                            Toast.MakeText(Activity, Resource.String.ErrorNoTelephoneContact, ToastLength.Short).Show();
                            return;
                        }
                        else
                        {
                            ((ContactActivity)Activity).ContactSelected(contact.ContactUri, contact.ContactName, contact.ContactTelephoneNumber, contact.ContactPhoto, contact.ContactEmail);
                        }
                    }
                }
                Dismiss();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ConfirmButton_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorAddingContactItem), "ContactDialogFragment.ConfirmButton_Click");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Activity != null)
                {
                    ((ContactActivity)Activity).ContactCancel();
                }
                Dismiss();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "CancelButton_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorCancellingContactAddition), "ContactDialogFragment.CancelButton_Click");
            }
        }

        public override void Dismiss()
        {
            base.Dismiss();
        }

        private void UpdateAdapter()
        {
            if (_contactList != null)
            {
                ContactsListAdapter adapter = new ContactsListAdapter(Activity, this);
                _contactList.Adapter = adapter;
            }
        }
    }
}
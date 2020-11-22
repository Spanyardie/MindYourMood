using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using System;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Android.Content;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class ContactsListAdapter : BaseAdapter
    {
        public const string TAG = "M:ContactsListAdapter";

        public List<Model.Contact> _contacts;
        Activity _activity;
        DialogFragment _parentFragment;

        private ImageView _contactPhotoImage;
        private TextView _contactName;
        private TextView _contactTelephoneNumber;
        private TextView _contactEmail;

        public ContactsListAdapter(Activity activity)
        {
            _activity = activity;

            _contacts = new List<Model.Contact>();

            GetAllContactsData();
        }

        public ContactsListAdapter(Activity activity, DialogFragment parentFragment)
        {
            _activity = activity;
            _parentFragment = parentFragment;

            _contacts = new List<Model.Contact>();

            GetAllContactsData();
        }

        private void GetAllContactsData()
        {
            try
            {
                List<Model.Contact> contactsList = new List<Model.Contact>();

                if (GlobalData.ContactItems != null && GlobalData.ContactItems.Count > 0)
                {
                    _contacts = GlobalData.ContactItems;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetAllContactsData: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Contacts Data", "ContactsListAdapter.GetAllContactsData");
            }
        }

        public override int Count
        {
            get
            {
                if (_contacts != null)
                {
                    return _contacts.Count;
                }
                return -1;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _contacts.ElementAt(position).ID;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.ContactListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.ContactListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                _contactPhotoImage = convertView.FindViewById<ImageView>(Resource.Id.imgContactPhoto);
                _contactName = convertView.FindViewById<TextView>(Resource.Id.txtContactName);
                _contactTelephoneNumber = convertView.FindViewById<TextView>(Resource.Id.txtContactTelephoneNumber);
                _contactEmail = convertView.FindViewById<TextView>(Resource.Id.txtContactEmailAddress);

                if (_contactPhotoImage != null)
                {
                    if (_contacts[position].ContactPhoto == null)
                    {
                        _contactPhotoImage.SetImageResource(Resource.Drawable.Moods2);
                    }
                    else
                    {
                        _contactPhotoImage.SetImageBitmap(_contacts[position].ContactPhoto);
                    }
                }
                if (!string.IsNullOrEmpty(_contacts[position].ContactTelephoneNumber))
                {
                    _contactTelephoneNumber.Text = _contacts[position].ContactTelephoneNumber.Trim();
                }
                else
                {
                    _contactTelephoneNumber.Text = "";
                }
                if (!string.IsNullOrEmpty(_contacts[position].ContactName))
                {
                    _contactName.Text = _contacts[position].ContactName;
                }
                else
                {
                    _contactName.Text = "";
                }
                if (!string.IsNullOrEmpty(_contacts[position].ContactEmail))
                {
                    _contactEmail.Text = _contacts[position].ContactEmail.Trim();
                }
                else
                {
                    _contactEmail.Text = "";
                }

                var parentHeldSelectedItemIndex = ((ContactDialogFragment)_parentFragment).GetSelectedItemIndex();
                if (position == parentHeldSelectedItemIndex)
                {
                    convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    _contactName.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    _contactTelephoneNumber.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    _contactPhotoImage.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    _contactEmail.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                }
                else
                {
                    convertView.SetBackgroundDrawable(null);
                    _contactName.SetBackgroundDrawable(null);
                    _contactTelephoneNumber.SetBackgroundDrawable(null);
                    _contactPhotoImage.SetBackgroundDrawable(null);
                    _contactEmail.SetBackgroundDrawable(null);
                }
                return convertView;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Contacts View", "ContactsListAdapter.GetView");
                return null;
            }
        }
    }
}
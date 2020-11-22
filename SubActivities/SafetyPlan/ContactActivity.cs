using System;

using Android.App;
using Android.OS;
using Android.Widget;

using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Graphics;
using Android.Views;
using Android.Util;
using Android.Content;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.SubActivities.Help.SafetyPlan.SafetyPlanItems;
using Android.Content.PM;
using Android.Runtime;
using Java.Lang;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class ContactActivity : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:ContactActivity";

        private ListView _contactList;

        private Toolbar _toolbar;

        public ContactDialogFragment _contactSelector;

        public ProgressDialog _updatingContacts;

        private Button _btnDone;

        private int _selectedItemIndex;
        private bool _settingMenuItems = false;
        private int _listItemSelected = -1;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("selectedItemIndex", _selectedItemIndex);
                outState.PutInt("listItemSelected", _listItemSelected);
                outState.PutBoolean("settingMenuItems", _settingMenuItems);
            }

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ContactMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if(savedInstanceState != null)
                {
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");
                    _listItemSelected = savedInstanceState.GetInt("listItemSelected");
                    _settingMenuItems = savedInstanceState.GetBoolean("settingMenuItems");
                }

                SetContentView(Resource.Layout.ContactLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.contactToolbar, Resource.String.safetyPlanWhoCanIRingActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.contacts,
                    new ImageLoadingListener
                    (
                        loadingComplete: (imageUri, view, loadedImage) =>
                        {
                            var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                            ImageLoader_LoadingComplete(null, args);
                        }
                    )
                );

                SetupCallbacks();

                UpdateAdapter();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingContactActivity), "ContactActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_contactList != null)
                _contactList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void UpdateAdapter()
        {
            try
            {
                ContactsUserListAdapter adapter = new ContactsUserListAdapter(this);
                if (_contactList != null)
                    _contactList.Adapter = adapter;
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorUpdatingContactDisplay), "ContactActivity.UpdateAdapter");
            }
        }

        private void SetupCallbacks()
        {
            if (_contactList != null)
            {
                _contactList.ItemClick += ContactList_ItemClick;
                _contactList.ItemLongClick += ContactList_ItemLongClick;
            }
            if(_btnDone != null)
                _btnDone.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void ContactList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var itemIndex = e.Position;
            Int32 contactId = -1;
            Bitmap thumb = null;

            bool call = false;
            bool email = false;
            bool sms = false;
            string name = "";

            if(GlobalData.ContactsUserItems != null && GlobalData.ContactsUserItems.Count > 0)
            {
                contactId = GlobalData.ContactsUserItems[itemIndex].ID;
                call = GlobalData.ContactsUserItems[itemIndex].ContactEmergencyCall;
                email = GlobalData.ContactsUserItems[itemIndex].ContactEmergencyEmail;
                sms = GlobalData.ContactsUserItems[itemIndex].ContactEmergencySms;
                name = GlobalData.ContactsUserItems[itemIndex].ContactName;
                thumb = GlobalData.ContactsUserItems[itemIndex].ContactPhoto;
            }

            ContactEmergencyStatusDialogFragment contactFragment = new ContactEmergencyStatusDialogFragment(this, contactId, thumb, name, call, email, sms);

            var fragmentTransaction = FragmentManager.BeginTransaction();
            if (fragmentTransaction != null)
            {
                Log.Info(TAG, "ContactList_ItemLongClick: Showing dialog Fragment");
                contactFragment.Show(fragmentTransaction, contactFragment.Tag);
            }

        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.contactActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.contactActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.contactActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ContactActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(SafetyPlanActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void ContactList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;
                UpdateAdapter();
                _contactList.SetSelection(_selectedItemIndex);

                Log.Info(TAG, "ContactList_ItemClick: User selected contact with index " + _selectedItemIndex.ToString());
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "ContactList_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSelectingContact), "ContactActivity.ContactList_ItemClick");
            }
        }

        private void Add()
        {
            if (!CanUseContacts()) return;

            try
            {
                //request a pick from the users system contact list
                _contactSelector = new ContactDialogFragment(this, "Add Contact");

                var fragmentTransaction = FragmentManager.BeginTransaction();
                _contactSelector.Show(fragmentTransaction, _contactSelector.Tag);
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAddingContactItem), "ContactActivity.Add_Click");
            }
        }

        private bool CanUseContacts()
        {
            if (!PermissionsHelper.PermissionGranted(BaseContext, ConstantsAndTypes.AppPermission.ReadContacts))
            {
                if (PermissionsHelper.ShouldShowPermissionRationale(this, ConstantsAndTypes.AppPermission.ReadContacts))
                {
                    if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagContacts").SettingValue == "True")
                    {
                        Toast.MakeText(this, Resource.String.ContactsPermissionDenialToast, ToastLength.Short).Show();
                        return false;
                    }
                    AlertUserToPermissionRationale();
                    return false;
                }
                else
                {
                    //just request the permission
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.ReadContacts);
                    return false;
                }
            }

            return true;
        }

        private void AlertUserToPermissionRationale()
        {
            AlertHelper alertHelper = new AlertHelper(this);
            alertHelper.AlertIconResourceID = Resource.Drawable.SymbolInformation;
            alertHelper.AlertMessage = GetString(Resource.String.RequestPermissionReadContactsAlertMessage);
            alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
            alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
            alertHelper.AlertTitle = GetString(Resource.String.RequestPermissionReadContactsAlertTitle);
            alertHelper.InstanceId = "showRationale";
            alertHelper.ShowAlert();
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        private void Remove()
        {
            if(GlobalData.ContactsUserItems.Count == 0)
            {
                Toast.MakeText(this, Resource.String.ContactItemListEmptyToast, ToastLength.Short).Show();
                return;
            }

            if(_selectedItemIndex == -1)
            {
                Toast.MakeText(this, Resource.String.ContactItemListNoSelectionToast, ToastLength.Short).Show();
                return;
            }

            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertTitle = GetString(Resource.String.removeEmergencyContactTitle);
                alertHelper.AlertMessage = GetString(Resource.String.removeEmergencyContactQuestion);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertPositiveCaption = GetString(Resource.String.removeEmergencyContactConfirm);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.removeEmergencyContactCancel);
                alertHelper.InstanceId = "remove";
                alertHelper.ShowAlert();
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "Remove: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Removing Contact", "ContactActivity.Remove");
            }
        }

        public void ContactSelected(string contactUri, string contactName, string contactTelephoneNumber, Bitmap contactPhoto, string contactEmail)
        {
            try
            {
                Contact contact = new Contact();
                if (!string.IsNullOrEmpty(contactUri))
                {
                    contact.ContactUri = contactUri;
                    contact.ContactName = !string.IsNullOrEmpty(contactName) ? contactName : "";
                    contact.ContactTelephoneNumber = !string.IsNullOrEmpty(contactTelephoneNumber) ? contactTelephoneNumber : "";
                    contact.ContactPhoto = contactPhoto;
                    contact.ContactEmail = !string.IsNullOrEmpty(contactEmail) ? contactEmail : "";
                    contact.ContactEmergencyCall = false;
                    contact.ContactEmergencySms = false;
                    contact.ContactEmergencyEmail = false;

                    Globals dbHelp = new Globals();
                    dbHelp.OpenDatabase();
                    var sqlDatabase = dbHelp.GetSQLiteDatabase();
                    if (sqlDatabase != null)
                    {
                        contact.Save(sqlDatabase);
                        GlobalData.ContactsUserItems.Add(contact);
                        UpdateAdapter();
                    }
                    dbHelp.CloseDatabase();
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "ContactSelected: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSelectingContact), "ContactActivity.ContactSelected");
            }
        }

        public void ContactCancel()
        {
            Toast.MakeText(this, Resource.String.CancelledContactSelectionToast, ToastLength.Long).Show();
        }

        private void GetFieldComponents()
        {
            _contactList = FindViewById<ListView>(Resource.Id.lstContacts);
            _btnDone = FindViewById<Button>(Resource.Id.btnDone);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    GoBack();
                    return true;
                }

                switch (item.ItemId)
                {
                    case Resource.Id.contactActionAdd:
                        Add();
                        return true;

                    case Resource.Id.contactActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.contactActionHelp:
                        Intent intent = new Intent(this, typeof(SafetyPlanContactHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            try
            {
                if (instanceId == "remove")
                {
                    Globals dbHelp = new Globals();
                    dbHelp.OpenDatabase();
                    var sqlDatabase = dbHelp.GetSQLiteDatabase();
                    if (sqlDatabase.IsOpen)
                    {
                        GlobalData.ContactsUserItems[_selectedItemIndex].Remove(sqlDatabase);
                    }
                    dbHelp.CloseDatabase();
                    GlobalData.ContactsUserItems.RemoveAt(_selectedItemIndex);
                    UpdateAdapter();
                    _selectedItemIndex = -1;
                }
                if(instanceId == "showRationale")
                {
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.ReadContacts);
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Removing Emergency Contact", "ContactActivity.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            //If we have only just been given permission, then we need to build our list of contacts
            if(requestCode == ConstantsAndTypes.REQUEST_CODE_PERMISSION_READ_CONTACTS)
            {
                if(grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    //now update the global permission
                    if (GlobalData.ApplicationPermissions == null)
                    {
                        //if null then we can go get permissions
                        PermissionsHelper.SetupDefaultPermissionList(this);
                    }
                    else
                    {
                        //we need to update the existing permission
                        if (PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.ReadContacts))
                        {
                            GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == ConstantsAndTypes.AppPermission.ReadContacts).PermissionGranted = Permission.Granted;
                        }
                    }
                    ShowProgressDialogAsync();
                }
            }
        }

        private void ShowProgressDialogAsync()
        {
            Thread progressAsync = new Thread(new Runnable(ActionHandler));

            progressAsync.Start();

            Initialisation init = new Initialisation();
            GlobalData.ContactItems = init.RetrieveAllContacts(BaseContext);

            //progressAsync.Stop();
            progressAsync.Dispose();

            _updatingContacts.Dismiss();
            _updatingContacts = null;

            Add();
        }

        private void ActionHandler()
        {
            Looper.Prepare();

            _updatingContacts = new ProgressDialog(this);
            _updatingContacts.SetTitle(Resource.String.ReadingContactsProgressDialogTitle);
            _updatingContacts.SetMessage(GetString(Resource.String.ReadingContactsProgressDialogMessage));
            _updatingContacts.SetCancelable(false);
            _updatingContacts.Indeterminate = true;
            _updatingContacts.Show();

            Looper.Loop();
        }
    }
}
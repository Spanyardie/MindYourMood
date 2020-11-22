using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Helpers;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using Android.Graphics;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class MoodsAdjustActivity : AppCompatActivity, IMoodsAdjustCallback, IAlertCallback
    {
        public const string TAG = "M:MoodsAdjustActivity";

        private Toolbar _toolbar;
        private ListView _moodList;
        private Button _btnDone;

        private LinearLayout _linMoodsListMain;

        private int _selectedItemIndex = -1;

        private ImageLoader _imageLoader = null;

        public int SelectedItemIndex
        {
            get
            {
                return _selectedItemIndex;
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                return;
            }

            var adapter = _moodList.Adapter;
            if (adapter != null)
            {
                if (_selectedItemIndex != -1)
                {
                    try
                    {
                        var item = ((MoodsAdjustListAdapter)adapter).GetItemAtPosition(_selectedItemIndex);
                        if (item != null)
                        {
                            Globals dbHelp = new Globals();
                            dbHelp.OpenDatabase();
                            var sqlDatabase = dbHelp.GetSQLiteDatabase();
                            if (sqlDatabase.IsOpen)
                            {
                                item.Remove(sqlDatabase);
                            }
                            dbHelp.CloseDatabase();
                            GlobalData.MoodListItems.Remove(item);
                            item = null;
                            _selectedItemIndex = -1;
                            UpdateAdapter();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                        if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRemovingMoodsAdjustItem), "MoodsAdjustActivity.AlertPositiveButtonSelect");
                    }
                }
            }
        }

        public void OnGenericDialogDismiss()
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");

                SetContentView(Resource.Layout.MoodAdjustLayout);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.moodsAdjustToolbar, Resource.String.MoodsAdjustActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.moodsDream,
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

                if (_selectedItemIndex != -1)
                    _moodList.SetSelection(_selectedItemIndex);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingMoodsAdjustActivity), "MoodsAdjustActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linMoodsListMain != null)
                _linMoodsListMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemIndex", _selectedItemIndex);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MoodsAdjustMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.moodsAdjustActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.moodsAdjustActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.moodsAdjustActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if (itemRemove != null)
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
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "MoodsAdjustActivity.SetActionIcons");
            }
        }

        private void GetFieldComponents()
        {
            _moodList = FindViewById<ListView>(Resource.Id.lstMoodsAdjustList);
            _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            _linMoodsListMain = FindViewById<LinearLayout>(Resource.Id.linMoodsListMain);
        }

        private void SetupCallbacks()
        {
            if (_moodList != null)
            {
                _moodList.ItemClick += GenericList_ItemClick;
                _moodList.ItemLongClick += GenericList_ItemLongClick;
            }
            if(_btnDone != null)
                _btnDone.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        private void GenericList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            try
            {
                var adapter = _moodList.Adapter;
                _selectedItemIndex = e.Position;
                var moodItem = GlobalData.MoodListItems[_selectedItemIndex];

                if (moodItem != null)
                {
                    if(moodItem.IsDefault == "true")
                    {
                        Toast.MakeText(this, Resource.String.MoodsAdjustDefaultEntryToast, ToastLength.Short).Show();
                        return;
                    }

                    MoodsAdjustDialogFragment genFragment = new MoodsAdjustDialogFragment(this, "Edit item", GetString(Resource.String.MoodsAdjustGenericTextDialogTitle), moodItem.MoodName, _selectedItemIndex);

                    FragmentTransaction transaction = FragmentManager.BeginTransaction();

                    genFragment.Show(transaction, genFragment.Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "GenericList_ItemLongClick: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMoodsAdjustEditEntry), "MoodsAdjustActivity.GenericList_ItemLongClick");
            }
        }

        private void GenericList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateAdapter();
            if (_moodList != null)
                _moodList.SetSelection(_selectedItemIndex);
            Log.Info(TAG, "GenericList_ItemClick: Selected index " + _selectedItemIndex.ToString());
        }
        private void UpdateAdapter()
        {
            try
            {
                MoodsAdjustListAdapter adapter = new MoodsAdjustListAdapter(this);
                if (_moodList != null)
                    _moodList.Adapter = adapter;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorUpdatingOthersDoDisplay), "MoodsAdjustActivity.UpdateAdapter");
            }
        }

        private void CheckMicPermission()
        {
            try
            {
                if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.UseMicrophone)))
                {
                    AttemptPermissionRequest();
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "CheckMicPermission: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "MoodsAdjustActivity.CheckMicPermission");
            }
        }

        public void AttemptPermissionRequest()
        {
            try
            {
                if (PermissionsHelper.ShouldShowPermissionRationale(this, ConstantsAndTypes.AppPermission.UseMicrophone))
                {
                    ShowPermissionRationale();
                    return;
                }
                else
                {
                    //just request the permission
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "AttemptPermissionRequest: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "MoodsAdjustActivity.AttemptPermissionRequest");
            }
        }
        private void ShowPermissionRationale()
        {
            try
            {
                if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagMic").SettingValue == "True") return;

                AlertHelper alertHelper = new AlertHelper(this);

                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolInformation;
                alertHelper.AlertMessage = GetString(Resource.String.RequestPermissionUseMicrophoneAlertMessage);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertTitle = GetString(Resource.String.RequestPermissionUseMicrophoneAlertTitle);
                alertHelper.InstanceId = "useMic";
                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ShowPermissionRationale: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "MoodsAdjustActivity.ShowPermissionRationale");
            }
        }

        public void ConfirmAddition(int moodID, string moodText)
        {
            try
            {
                if (moodText.Trim() == "") return;

                MoodList moodListText = new MoodList();
                if (moodID != -1)
                {
                    moodListText.IsNew = false;
                    moodListText.IsDirty = true;
                    moodListText.MoodId = moodID;
                }
                else
                {
                    moodListText.IsDirty = false;
                    moodListText.IsNew = true;
                }

                moodListText.MoodName = moodText.Trim();
                moodListText.MoodIsoCountryAlias = SystemHelper.GetIsoCountryAlias();
                moodListText.IsDefault = "false";

                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                moodListText.Save(sqlDatabase);
                dbHelp.CloseDatabase();

                if (moodID == -1)
                {
                    GlobalData.MoodListItems.Add(moodListText);
                    Log.Info(TAG, "ConfirmAddition: Added text " + moodText.Trim() + ", ID '" + moodListText.MoodId.ToString() + "'");
                    Toast.MakeText(this, Resource.String.MoodsAdjustConfirmTextToast, ToastLength.Short).Show();
                }
                else
                {
                    var index = GlobalData.MoodListItems.IndexOf(GlobalData.MoodListItems.Find(mood => mood.MoodId == moodID));
                    GlobalData.MoodListItems[index] = moodListText;
                    Log.Info(TAG, "ConfirmAddition: Updated text " + moodText.Trim() + ", ID '" + moodListText.MoodId.ToString() + "'");
                    Toast.MakeText(this, Resource.String.MoodsAdjustConfirmAmendTextToast, ToastLength.Short).Show();
                }

                UpdateAdapter();

            }
            catch (Exception e)
            {
                Log.Error(TAG, "ConfirmText: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAddingOthersDoItem), "MoodsAdjustActivity.ConfirmAddition");
            }
        }

        public void CancelAddition()
        {
            Toast.MakeText(this, Resource.String.MoodsAdjustCancelTextToast, ToastLength.Short).Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                if (item != null)
                {
                    if (item.ItemId == Android.Resource.Id.Home)
                    {
                        Finish();
                        return true;
                    }

                    switch (item.ItemId)
                    {
                        case Resource.Id.moodsAdjustActionAdd:
                            Add();
                            return true;
                        case Resource.Id.moodsAdjustActionRemove:
                            Remove();
                            return true;
                        case Resource.Id.moodsAdjustActionHelp:
                            Intent intent = new Intent(this, typeof(MoodsAdjustHelpActivity));
                            StartActivity(intent);
                            return true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnOptionsItemSelected: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSelectingMenuOption), "MoodsAdjustActivity.OnOptionsItemSelected");
            }
            return base.OnOptionsItemSelected(item);
        }

        private void Add()
        {
            try
            {
                MoodsAdjustDialogFragment moodsAdjustFragment = new MoodsAdjustDialogFragment(this, GetString(Resource.String.MoodsAdjustDialogTitleAdd), GetString(Resource.String.MoodsAdjustGenericTextDialogTitle), "", - 1);
                Log.Info(TAG, "Add_Click: New Mood, passing ID -1 to dialog fragment");

                var fragmentTransaction = FragmentManager.BeginTransaction();
                if (fragmentTransaction != null)
                {
                    Log.Info(TAG, "Add_Click: Showing dialog Fragment");
                    moodsAdjustFragment.Show(fragmentTransaction, moodsAdjustFragment.Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMoodsAdjustActivityAdd), "MoodsAdjustActivity.Add");
            }
        }

        private void Remove()
        {
            try
            {
                var adapter = _moodList.Adapter;
                var selectedIndexID = (int)adapter.GetItemId(_selectedItemIndex);
                var moodItem = GlobalData.MoodListItems.Find(mood => mood.MoodId == selectedIndexID);
                if (moodItem != null)
                {
                    if (moodItem.IsDefault == "true")
                    {
                        Toast.MakeText(this, Resource.String.MoodsAdjustRemoveDefaultEntryToast, ToastLength.Short).Show();
                        return;
                    }

                    if (_selectedItemIndex != -1)
                    {
                        AlertHelper alertHelper = new AlertHelper(this);
                        alertHelper.AlertTitle = GetString(Resource.String.MoodsAdjustActivityRemoveAlertTitle);
                        alertHelper.AlertMessage = GetString(Resource.String.MoodsAdjustActivityRemoveAlertMessage);
                        alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                        alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                        alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                        alertHelper.InstanceId = "remove";
                        alertHelper.ShowAlert();
                    }
                    else
                    {
                        if (GlobalData.MoodListItems != null && GlobalData.MoodListItems.Count > 0)
                        {
                            Toast.MakeText(this, Resource.String.MoodsAdjustActivityRemoveToast, ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(this, Resource.String.MoodsAdjustActivityRemoveNoEntriesToast, ToastLength.Short).Show();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Remove: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMoodsAdjustActivityRemove), "MoodsAdjustActivity.Remove");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            try
            {
                if (requestCode == ConstantsAndTypes.REQUEST_CODE_PERMISSION_USE_MICROPHONE)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
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
                            if (PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone))
                            {
                                GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == ConstantsAndTypes.AppPermission.UseMicrophone).PermissionGranted = Permission.Granted;
                            }
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "MoodsAdjustActivity.OnRequestPermissionsResult");
            }
        }
    }
}
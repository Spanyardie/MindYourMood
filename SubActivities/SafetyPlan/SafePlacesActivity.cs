using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model;
using Android.Content;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.SafetyPlan.SafetyPlanItems;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.SubActivities.SafetyPlan
{
    [Activity]
    public class SafePlacesActivity : AppCompatActivity, IGenericTextCallback, IAlertCallback
    {
        public static string TAG = "M:SafePlacesActivity";

        private Toolbar _toolbar;
        private ListView _genericList;

        private GenericTextDialogFragment _genericText;
        private Button _btnDone;

        private int _selectedItemIndex = -1;

        private ImageLoader _imageLoader = null;

        public int SelectedItemIndex
        {
            get
            {
                return _selectedItemIndex;
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemIndex", _selectedItemIndex);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SafePlacesMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");

                SetContentView(Resource.Layout.SafePlacesLayout);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.safePlacesToolbar, Resource.String.safetyPlanSafePlacesActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.safeplaces,
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
                    _genericList.SetSelection(_selectedItemIndex);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingSafePlacesActivity), "SafePlacesActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_genericList != null)
                _genericList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_genericList != null)
                {
                    _genericList.ItemClick += GenericList_ItemClick;
                    _genericList.ItemLongClick += GenericList_ItemLongClick;
                }
                if(_btnDone != null)
                    _btnDone.Click += Done_Click;
                Log.Info(TAG, "SetupCallbacks: Successfully setup callbacks");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting up callbacks", "SafePlacesActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void GenericList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            try
            {
                var adapter = _genericList.Adapter;
                var selectedIndexID = (int)adapter.GetItemId(e.Position);
                var genericTextItem = GlobalData.GenericTextItemsList.Find(gen => gen.ID == selectedIndexID);

                GenericTextDialogFragment genFragment = new GenericTextDialogFragment(this, "Edit item", GetString(Resource.String.safePlacesGenericTextDialogTitle), genericTextItem.TextValue, selectedIndexID);

                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();

                genFragment.Show(transaction, genFragment.Tag);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "GenericList_ItemLongClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSafePlacesEditEntry), "SafePlacesActivity.GenericList_ItemLongClick");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.safeplacesActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.safeplacesActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.safeplacesActionHelp);

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
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SafePlacesActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(SafetyPlanActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void GenericList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateAdapter();
            if (_genericList != null)
                _genericList.SetSelection(_selectedItemIndex);
            Log.Info(TAG, "GenericList_ItemClick: User selected item with index - " + _selectedItemIndex.ToString());
        }

        private void GetFieldComponents()
        {
            try
            {
                _genericList = FindViewById<ListView>(Resource.Id.lstSafePlacesList);
                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
                Log.Info(TAG, "GetFieldComponents: Successfully retrieved field components");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorGettingSafePlacesFieldComponents), "SafePlacesActivity.GetFieldComponents");
            }
        }

        private void Add()
        {
            try
            {
                _genericText = new GenericTextDialogFragment(this, "Safe Places", GetString(Resource.String.safePlacesGenericTextDialogTitle));

                var fragmentTransaction = FragmentManager.BeginTransaction();
                _genericText.Show(fragmentTransaction, _genericText.Tag);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAddingSafePlacesItem), "SafePlacesActivity.Add_Click");
            }
        }

        private void Remove()
        {
            if (_selectedItemIndex != -1)
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertTitle = GetString(Resource.String.safePlacesRemoveTitle);
                alertHelper.AlertMessage = GetString(Resource.String.safePlacesRemoveQuestion);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertPositiveCaption = GetString(Resource.String.safePlacesRemoveConfirm);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.safePlacesRemoveCancel);
                alertHelper.InstanceId = "remove";
                alertHelper.ShowAlert();
            }
            else
            {
                if (GlobalData.GenericTextItemsList != null && GlobalData.GenericTextItemsList.Count > 0)
                {
                    var itemList = GlobalData.GenericTextItemsList.FindAll(item => item.TextType == ConstantsAndTypes.GENERIC_TEXT_TYPE.SafePlaces);
                    if (itemList != null && itemList.Count > 0)
                    {
                        Toast.MakeText(this, Resource.String.SafePlaceToastSelectItem, ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.SafePlaceToastNoItems, ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, Resource.String.SafePlaceToastNoItems, ToastLength.Short).Show();
                }
            }
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
                    case Resource.Id.safeplacesActionAdd:
                        Add();
                        return true;

                    case Resource.Id.safeplacesActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.safeplacesActionHelp:
                        Intent intent = new Intent(this, typeof(SafetyPlanSafePlacesHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void UpdateAdapter()
        {
            try
            {
                GenericTextListAdapter adapter = new GenericTextListAdapter(this, ConstantsAndTypes.GENERIC_TEXT_TYPE.SafePlaces);
                if (_genericList != null)
                    _genericList.Adapter = adapter;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorUpdatingSafePlacesDisplay), "SafePlacesActivity.UpdateAdapter");
            }
        }

        public void ConfirmText(string textEntered, int genericTextID)
        {
            try
            {
                if (textEntered.Trim() == "") return;

                GenericText genericText = new GenericText();
                if (genericTextID != -1)
                {
                    genericText.IsNew = false;
                    genericText.IsDirty = true;
                    genericText.ID = genericTextID;
                }
                else
                {
                    genericText.IsDirty = false;
                    genericText.IsNew = true;
                }

                genericText.TextType = ConstantsAndTypes.GENERIC_TEXT_TYPE.SafePlaces;
                genericText.TextValue = textEntered.Trim();
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                genericText.Save(sqlDatabase);
                dbHelp.CloseDatabase();

                if (genericTextID == -1)
                {
                    GlobalData.GenericTextItemsList.Add(genericText);
                    Log.Info(TAG, "ConfirmText: Added text " + textEntered.Trim() + ", ID '" + genericText.ID.ToString() + "'");
                }
                else
                {
                    var index = GlobalData.GenericTextItemsList.IndexOf(GlobalData.GenericTextItemsList.Find(gen => gen.ID == genericTextID));
                    GlobalData.GenericTextItemsList[index] = genericText;
                    Log.Info(TAG, "ConfirmText: Updated text " + textEntered.Trim() + ", ID '" + genericText.ID.ToString() + "'");
                }

                UpdateAdapter();

                Toast.MakeText(this, Resource.String.safePlacesConfirmTextToast, ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ConfirmText: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAddingSafePlacesItem), "SafePlacesActivity.Confirmtext");
            }
        }

        public void CancelText()
        {
            Toast.MakeText(this, Resource.String.safePlacesCancelTextToast, ToastLength.Short).Show();
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                return;
            }

            try
            {
                Log.Info(TAG, "Remove_Click: Removing item '" + GlobalData.GenericTextItemsList[_selectedItemIndex].TextValue + "'");
                var adapter = _genericList.Adapter;
                if (adapter != null)
                {
                    if (_selectedItemIndex != -1)
                    {
                        var item = ((GenericTextListAdapter)adapter).GetItemAtPosition(_selectedItemIndex);
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
                            GlobalData.GenericTextItemsList.Remove(item);
                            item = null;
                            _selectedItemIndex = -1;
                            UpdateAdapter();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRemovingSafePlacesItem), "SafePlacesActivity.AlertPositiveButtonSelect");
            }
            Log.Info(TAG, "AlertPositiveButtonSelect: Remove succeeded");
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                return;
            }

            Toast.MakeText(this, Resource.String.safePlacesCancelDeleteToast, ToastLength.Short).Show();
        }

        public void OnGenericDialogDismiss()
        {
            
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "SafePlacesActivity.CheckMicPermission");
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
                        //PermissionResultUpdate(Permission.Granted);
                    }
                    else
                    {
                        //PermissionResultUpdate(Permission.Denied);
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "SafePlacesActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {

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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "OthersDoActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "SafePlacesActivity.AttemptPermissionRequest");
            }
        }
    }
}
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Util;
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
    public class WarningSignsActivity : AppCompatActivity, IGenericTextCallback, IAlertCallback
    {
        public static string TAG = "M:WarningSignsActivity";

        private Toolbar _toolbar;
        private ListView _genericList;

        private LinearLayout _linListMain;

        private GenericTextDialogFragment _genericText;
        private Button _done;

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
            MenuInflater.Inflate(Resource.Menu.WarningSignsMenu, menu);

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

                SetContentView(Resource.Layout.WarningSignsLayout);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.warningSignsToolbar, Resource.String.safetyPlanWarningSignsActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.warningsignsmain,
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
                Log.Error(TAG, "OnCreate: Error occurred during creation - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingWarningSignsActivity), "WarningSignsActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linListMain != null)
                _linListMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            if (_genericList != null)
            {
                _genericList.ItemClick += GenericList_ItemClick;
                _genericList.ItemLongClick += GenericList_ItemLongClick;
            }
            if(_done != null)
                _done.Click += Done_Click;
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

                GenericTextDialogFragment genFragment = new GenericTextDialogFragment(this, "Edit item", GetString(Resource.String.warningSignsGenericTextDialogTitle), genericTextItem.TextValue, selectedIndexID);

                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();

                genFragment.Show(transaction, genFragment.Tag);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "GenericList_ItemLongClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorWarningSignsEditEntry), "WarningSignsActivity.GenericList_ItemLongClick");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.warningsignsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.warningsignsActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.warningsignsActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "WarningSignsActivity.SetActionIcons");
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
            Log.Info(TAG, "GenericList_ItemClick: Selected index " + _selectedItemIndex.ToString());
        }

        private void GetFieldComponents()
        {
            _genericList = FindViewById<ListView>(Resource.Id.lstWarningSignsList);
            _done = FindViewById<Button>(Resource.Id.btnDone);
            _linListMain = FindViewById<LinearLayout>(Resource.Id.linListMain);
        }

        private void Add()
        {
            try
            {
                _genericText = new GenericTextDialogFragment(this, "Warning Signs", GetString(Resource.String.warningSignsGenericTextDialogTitle));

                var fragmentTransaction = FragmentManager.BeginTransaction();
                _genericText.Show(fragmentTransaction, _genericText.Tag);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAddingWarningSignItem), "WarningSignsActivity.Add_Click");
            }
        }

        private void Remove()
        {
            try
            {
                if (_selectedItemIndex != -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.warningSignsRemoveTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.warningSignsRemoveQuestion);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.warningSignsRemoveConfirm);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.warningSignsRemoveCancel);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                else
                {
                    if (GlobalData.GenericTextItemsList != null && GlobalData.GenericTextItemsList.Count > 0)
                    {
                        var itemList = GlobalData.GenericTextItemsList.FindAll(item => item.TextType == ConstantsAndTypes.GENERIC_TEXT_TYPE.WarningSigns);
                        if (itemList != null && itemList.Count > 0)
                        {
                            Toast.MakeText(this, Resource.String.WarningSignsToastSelectItem, ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(this, Resource.String.WarningSignsToastNoItems, ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.WarningSignsToastNoItems, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Remove_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRemovingWarningSignItem), "WarningSignsActivity.Remove_Click");
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
                    case Resource.Id.warningsignsActionAdd:
                        Add();
                        return true;

                    case Resource.Id.warningsignsActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.warningsignsActionHelp:
                        Intent intent = new Intent(this, typeof(SafetyPlanWarningSignsHelpActivity));
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
                GenericTextListAdapter adapter = new GenericTextListAdapter(this, ConstantsAndTypes.GENERIC_TEXT_TYPE.WarningSigns);
                if (_genericList != null)
                    _genericList.Adapter = adapter;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorUpdatingWarningSignDisplay), "WarningSignsActivity.UpdateAdapter");
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

                genericText.TextType = ConstantsAndTypes.GENERIC_TEXT_TYPE.WarningSigns;
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

                Toast.MakeText(this, Resource.String.warningSignsConfirmTextToast, ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ConfirmText: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingWarningSignItem), "WarningSignsActivity.ConfirmText");
            }
        }

        public void CancelText()
        {
            Toast.MakeText(this, Resource.String.warningSignsCancelTextToast, ToastLength.Short).Show();
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                return;
            }

            if (_selectedItemIndex != -1)
            {
                try
                {
                    var adapter = _genericList.Adapter;
                    if (adapter != null)
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
                catch (Exception ex)
                {
                    Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRemovingWarningSignItem), "WarningSignsActivity.AlertPositiveButtonSelect");
                }
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                return;
            }
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "WarningSignsActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "WarningSignsActivity.OnRequestPermissionsResult");
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "WarningSignsActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "WarningSignsActivity.AttemptPermissionRequest");
            }
        }
    }
}
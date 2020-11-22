using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Adapters;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class AffirmationsActivity : AppCompatActivity, IAffirmationCallback, IAlertCallback
    {
        public const string TAG = "M:AffirmationsActivity";

        private Toolbar _toolbar;

        private ListView _affirmationList;

        private Button _btnDone;

        private int _selectedItemIndex = -1;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemIndex", _selectedItemIndex);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.AffirmationsMenu, menu);

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

                SetContentView(Resource.Layout.AffirmationsLayout);

                CheckMicPermission();

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.affirmationsToolbar, Resource.String.AchievementTypesAffirmation, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.affirmations,
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
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateAffirmationsActivity), "AffirmationsActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_affirmationList != null)
                _affirmationList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
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
                        case Resource.Id.AffirmationsActionAdd:
                            Add();
                            return true;
                        case Resource.Id.AffirmationsActionRemove:
                            Remove();
                            return true;
                        case Resource.Id.AffirmationsActionHelp:
                            Intent intent = new Intent(this, typeof(TreatmentAffirmationsHelpActivity));
                            StartActivity(intent);
                            return true;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnOptionsItemSelected: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSelectingMenuOption), "AffirmationsActivity.OnOptionsItemSelected");
            }
            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            try
            {
                _affirmationList = FindViewById<ListView>(Resource.Id.lstAffirmations);

                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
                Log.Info(TAG, "GetFieldComponents: Retrieved components successfully");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAffirmationsActivityGetComponents), "AffirmationsActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_affirmationList != null)
                {
                    _affirmationList.ItemClick += AffirmationList_ItemClick;
                    _affirmationList.ItemLongClick += AffirmationList_ItemLongClick;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _problemStepList is NULL!");
                }

                if(_btnDone != null)
                    _btnDone.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAffirmationsActivitySetCallbacks), "AffirmationsActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void Add()
        {
            try
            {
                AffirmationDialogFragment affirmationFragment = new AffirmationDialogFragment(this, "Add Affirmation", -1);
                Log.Info(TAG, "Add: New affirmation, passing ID -1 to dialog fragment");

                var fragmentTransaction = FragmentManager.BeginTransaction();
                if (fragmentTransaction != null)
                {
                    Log.Info(TAG, "Add: Showing dialog Fragment");
                    affirmationFragment.Show(fragmentTransaction, affirmationFragment.Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAffirmationsActivityAdd), "AffirmationsActivity.Add");
            }
        }

        private void Remove()
        {
            try
            {
                if (_selectedItemIndex != -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.AffirmationActivityRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.AffirmationActivityRemoveAlertMessage);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                else
                {
                    if (GlobalData.AffirmationListItems != null && GlobalData.AffirmationListItems.Count > 0)
                    {
                        Toast.MakeText(this, Resource.String.AffirmationsActivityRemoveToast, ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.AffirmationsActivityRemoveNoEntriesToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Remove: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAffirmationsActivityRemove), "AffirmationsActivity.Remove");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TreatmentPlanActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void AffirmationList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            try
            {
                Affirmation affirmationItem = null;
                AffirmationDialogFragment affirmationFragment = null;

                Log.Info(TAG, "AffirmationList_ItemLongClick: selected item - " + e.Position.ToString());
                affirmationItem = GlobalData.AffirmationListItems[e.Position];
                if (affirmationItem != null)
                {
                    Log.Info(TAG, "AffirmationList_ItemLongClick: Found affirmation with ID - " + affirmationItem.AffirmationID + ", affirmationText - " + affirmationItem.AffirmationText);
                    affirmationFragment = new AffirmationDialogFragment(this, "Edit Affirmation", affirmationItem.AffirmationID);
                }
                var fragmentTransaction = FragmentManager.BeginTransaction();
                if (fragmentTransaction != null)
                {
                    affirmationFragment.Show(fragmentTransaction, affirmationFragment.Tag);
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AffirmationList_ItemLongClick: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAttemptingEditAffirmation), "AffirmationsActivity.AffirmationList_ItemLongClick");
            }
        }

        private void AffirmationList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateAdapter();
            _affirmationList.SetSelection(_selectedItemIndex);
        }

        public void ConfirmAddition(int affirmationID, string affirmationText)
        {
            try
            {
                Affirmation affirmation = null;
                if (affirmationID == -1)
                {
                    affirmation = new Affirmation();
                    affirmation.IsNew = true;
                    affirmation.IsDirty = false;
                    Log.Info(TAG, "ConfirmAddition: Adding a new affirmation...");
                }
                else
                {
                    affirmation = GlobalData.AffirmationListItems.Find(afirm => afirm.AffirmationID == affirmationID);
                    affirmation.IsNew = false;
                    affirmation.IsDirty = true;
                }

                affirmation.AffirmationText = affirmationText.Trim();
                affirmation.Save();
                Log.Info(TAG, "ConfirmAddition: Saved affirmation with ID " + affirmation.AffirmationID.ToString());

                if (affirmationID == -1)
                {
                    GlobalData.AffirmationListItems.Add(affirmation);
                    Log.Info(TAG, "ConfirmAddition: Added item to global cache as it is new!");
                }

                UpdateAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "ConfirmAddition: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorConfirmingAffirmationAddition), "AffirmationsActivity.ConfirmAddition");
            }
        }

        private void UpdateAdapter()
        {
            AffirmationListAdapter adapter = new AffirmationListAdapter(this);
            if(_affirmationList != null)
            {
                _affirmationList.Adapter = adapter;
            }
        }

        public void CancelAddition()
        {
            Toast.MakeText(this, Resource.String.AffirmationsActivityCancelToast, ToastLength.Short).Show();
            Log.Info(TAG, "CancelAddition: Nothing to do but make toast!");
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "remove")
            {
                try
                {
                    var affirmation = GlobalData.AffirmationListItems[_selectedItemIndex];
                    Log.Info(TAG, "AlertPositiveButtonSelect: Found affirmation with ID " + affirmation.AffirmationID.ToString());

                    affirmation.Remove();
                    Log.Info(TAG, "AlertPositiveButtonSelect: Removed successfully");
                    GlobalData.AffirmationListItems.Remove(affirmation);

                    _selectedItemIndex = -1;

                    UpdateAdapter();
                    Log.Info(TAG, "AlertPositiveButtonSelect: Updated adapter");
                }
                catch(Exception remE)
                {
                    Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + remE.Message);
                    if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, remE, GetString(Resource.String.ErrorRemovingAffirmationConfirm), "AffirmationsActivity.AlertPositiveButtonSelect");
                }
            }

            if (instanceId == "useMic")
            {
                try
                {
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                }
                catch(Exception permE)
                {
                    Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + permE.Message);
                    if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, permE, GetString(Resource.String.ErrorRequestingApplicationPermission), "AffirmationsActivity.AlertPositiveButtonSelect");
                }
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "remove")
            {
                Toast.MakeText(this, Resource.String.AffirmationActivityRemoveCancelToast, ToastLength.Short).Show();
            }

            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.AffirmationsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.AffirmationsActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.AffirmationsActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "AffirmationsActivity.SetActionIcons");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "AffirmationsActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "AffirmationsActivity.OnRequestPermissionsResult");
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "AffirmationsActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "AffirmationsActivity.AttemptPermissionRequest");
            }
        }
    }
}
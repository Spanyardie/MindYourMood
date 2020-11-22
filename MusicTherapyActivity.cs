using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Media;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class MusicTherapyActivity : AppCompatActivity, IGenericTextCallback, IAlertCallback
    {
        public const string TAG = "M:MusicTherapyActivity";

        private int _selectedItemIndex = -1;

        private Toolbar _toolbar;

        private GridView _playListGrid;

        private Button _btnDone;

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
            MenuInflater.Inflate(Resource.Menu.MusicTherapyMenu, menu);

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

                SetContentView(Resource.Layout.MusicTherapyLayout);

                GetFieldComponents();
                CheckMicPermission();

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.musictherapy,
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

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.musicTherapyToolbar, Resource.String.MusicTherapyToolbarTitle, Color.White);

                UpdateGridAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateMusicTherapyActivity), "MusicTherapyActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_playListGrid != null)
                _playListGrid.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            if(_playListGrid != null)
            {
                _playListGrid.ItemClick += PlayListGrid_ItemClick;
                _playListGrid.ItemLongClick += PlayListGrid_ItemLongClick;
            }
            if(_btnDone != null)
                _btnDone.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void Add()
        {
            try
            {
                GenericTextDialogFragment genericText = new GenericTextDialogFragment(this, "Add Playlist", GetString(Resource.String.MusicTherapyActivityPlayListAddText));

                var transaction = FragmentManager.BeginTransaction();
                genericText.Show(transaction, genericText.Tag);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicTherapyActivityAdd), "MusicTherapyActivity.Add_Click");
            }
        }

        private void Remove()
        {
            try
            {
                if (_selectedItemIndex != -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.MusicTherapyActivityRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.MusicTherapyActivityRemoveAlertMessage);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                else
                {
                    if(GlobalData.PlayListItems != null && GlobalData.PlayListItems.Count > 0)
                    {
                        Toast.MakeText(this, Resource.String.MusicPlayListActivityRemoveWarning2, ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.MusicPlayListActivityRemoveWarning1, ToastLength.Short).Show();
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Remove: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicTherapyActivityRemove), "MusicTherapyActivity.Remove");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.musictherapyActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.musictherapyActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.musictherapyActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSettingActionIcons), "MusicTherapyActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(PersonalMediaActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void PlayListGrid_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(MusicPlayListTracksActivity));
                intent.PutExtra("playListID", GlobalData.PlayListItems[e.Position].PlayListID);
                StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "PlayListGrid_ItemLongClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicTherapyActivityMoveToTracks), "MusicTherapyActivity.PlayListGrid_ItemLongClick");
            }
        }

        private void PlayListGrid_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;
                UpdateGridAdapter();
                _playListGrid.SetSelection(_selectedItemIndex);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "PlayListGrid_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicTherapyActivitySelect), "MusicTherapyActivity.PlayListGrid_ItemClick");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _playListGrid = FindViewById<GridView>(Resource.Id.grdMusicTherapy);

                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMusicTherapyActivityGetComponents), "MusicTherapyActivity.GetFieldComponents");
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
                    case Resource.Id.musictherapyActionAdd:
                        Add();
                        return true;
                    case Resource.Id.musictherapyActionRemove:
                        Remove();
                        return true;
                    case Resource.Id.musictherapyActionHelp:
                        Intent intent = new Intent(this, typeof(MediaMusicTherapyHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        private void UpdateGridAdapter()
        {
            try
            {
                PlayListsGridAdapter playListAdapter = new PlayListsGridAdapter(this);

                if (_playListGrid != null)
                    _playListGrid.Adapter = playListAdapter;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateGridAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMusicTherapyActivityUpdateAdapater), "MusicTherapyActivity.UpdateGridAdapter");
            }
        }

        public void ConfirmText(string textEntered, int GenericTextID)
        {
            try
            {
                PlayList playList = new PlayList();

                playList.PlayListName = textEntered;
                playList.Save();

                GlobalData.PlayListItems.Add(playList);

                UpdateGridAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "ConfirmText: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMusicTherapyActivityConfirm), "MusicTherapyActivity.ConfirmText");
            }
        }

        public void CancelText()
        {
            Toast.MakeText(this, Resource.String.MusicTherapyActivityCancelToast, ToastLength.Short).Show();
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
                var playList = GlobalData.PlayListItems[_selectedItemIndex];

                if (playList != null)
                    playList.Remove();

                GlobalData.PlayListItems.Remove(playList);
                _selectedItemIndex = -1;
                UpdateGridAdapter();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicTherapyRemovePlaylist), "MusicTherapyActivity.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                return;
            }

            Toast.MakeText(this, Resource.String.MusicTherapyActivityRemoveCancelToast, ToastLength.Short).Show();
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCheckingApplicationPermission), "ImageryActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedicationListProcessResponse), "ImageryActivity.OnRequestPermissionsResult");
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ImageryActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRequestingApplicationPermission), "ImageryActivity.AttemptPermissionRequest");
            }
        }
    }
}
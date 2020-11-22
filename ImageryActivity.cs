using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using Android.Widget;
using com.spanyardie.MindYourMood.Adapters;
using Android.Content;
using Android.Runtime;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Media;
using Android.Content.PM;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class ImageryActivity : AppCompatActivity, IGenericTextCallback, IAlertCallback
    {
        public const string TAG = "M:ImageryActivity";

        private Toolbar _toolbar;

        private GridView _imageGrid;
        private LinearLayout _imageryLin;

        private Button _done;

        private int _pickImageID = 0;
        private int _selectedItemIndex = -1;
        private string _imageUri = "";

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
            if(outState != null)
            {
                outState.PutInt("pickImageID", _pickImageID);
                outState.PutInt("selectedItemIndex", _selectedItemIndex);
                outState.PutString("imageUri", _imageUri);
            }

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ImageryMenu, menu);

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
                    _pickImageID = savedInstanceState.GetInt("pickImageID");
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");
                    _imageUri = savedInstanceState.GetString("imageUri");
                }

                SetContentView(Resource.Layout.ImageryLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.imageryToolbar, Resource.String.ImageryToolbarTitle, Color.White);

                GetFieldComponents();
                CheckMicPermission();

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.imagerymain,
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
                UpdateGridAdapter();

            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateImageryActivity), "ImageryActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_imageryLin != null)
                _imageryLin.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        protected override void OnResume()
        {
            base.OnResume();
            _selectedItemIndex = GlobalData.MyLastSelectedImageIndex;
            UpdateGridAdapter();
            _imageGrid.SetSelection(_selectedItemIndex);
        }
        private void GetFieldComponents()
        {
            try
            {
                _imageGrid = FindViewById<GridView>(Resource.Id.grdImagery);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _imageryLin = FindViewById<LinearLayout>(Resource.Id.linImageryGrid);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorImageryActivityGetComponents), "ImageryActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_imageGrid != null)
                {
                    _imageGrid.ItemLongClick += ImageGrid_ItemLongClick;
                    _imageGrid.ItemClick += ImageGrid_ItemClick;
                }
                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorImageryActivitySetCallbacks), "ImageryActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void ImageGrid_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateGridAdapter();
            _imageGrid.SetSelection(_selectedItemIndex);
        }

        private void ImageGrid_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;
                GlobalData.MyLastSelectedImageIndex = _selectedItemIndex;

                Intent intent = new Intent(this, typeof(ImageDetailActivity));
                intent.PutExtra("imageUri", GlobalData.ImageListItems[_selectedItemIndex].ImageryURI);
                intent.PutExtra("imageComment", GlobalData.ImageListItems[_selectedItemIndex].ImageryComment);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this, e.View, "imageTransition");
                    Log.Info(TAG, "ImageGrid_ItemClick: Created activity options for image transition as Version code >= Lollipop");
                    StartActivity(intent, options.ToBundle());
                }
                else
                {
                    Log.Info(TAG, "ImageGrid_ItemClick: Starting regular activity");
                    StartActivity(intent);
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ImageGrid_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorImageryActivitySelect), "ImageryActivity.ImageGrid_ItemClick");
            }
        }

        private void Add()
        {
            try
            {
                _pickImageID = DateTime.Now.Millisecond;

                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, GetString(Resource.String.ImageryActivityChooserText)), _pickImageID);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Add: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingActivityAddImage), "ImageryActivity.Add");
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == _pickImageID && resultCode == Result.Ok && data != null)
                {
                    _imageUri = data.Data.ToString();
                    AskForComment();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorImageryActivityCreateUri), "ImageryActivity.OnActivityResult");
            }
        }

        private void AskForComment()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertTitle = GetString(Resource.String.ImageryActivityCommentAlertTitle);
                alertHelper.AlertMessage = GetString(Resource.String.ImageryActivityCommentAlertQuestion);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.InstanceId = "comment";
                alertHelper.ShowAlert();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "AskForComment: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingCommentDialog), "ImageryActivity.AskForComment");
            }
        }

        private void Remove()
        {
            try
            {
                if (_selectedItemIndex != -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.ImageryActivityRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.ImageryActivityRemoveAlertMessage);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                else
                {
                    string toastText = "";
                    if(GlobalData.ImageListItems != null && GlobalData.ImageListItems.Count == 0)
                    {
                        toastText = GetString(Resource.String.ImageryActivityRemoveWarning1);
                    }
                    else
                    {
                        toastText = GetString(Resource.String.ImageryActivityRemoveWarning2);
                    }
                    Toast.MakeText(this, toastText, ToastLength.Short).Show();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Remove: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorImageryActivityRemove), "ImageryActivity.Remove");
            }
        }


        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.imageryActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.imageryActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.imageryActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSettingActionIcons), "ImageryActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(PersonalMediaActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void UpdateGridAdapter()
        {
            ImageryGridAdapter gridAdapter = new ImageryGridAdapter(this);
            if (_imageGrid != null)
                _imageGrid.Adapter = gridAdapter;
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
                    case Resource.Id.imageryActionAdd:
                        Add();
                        return true;

                    case Resource.Id.imageryActionRemove:
                        Remove();
                        return true;
                    case Resource.Id.imageryActionHelp:
                        Intent intent = new Intent(this, typeof(MediaImageryHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        public void ConfirmText(string textEntered, int GenericTextID)
        {
            try
            {
                Imagery imagery = new Imagery();
                imagery.ImageryComment = textEntered.Trim();
                imagery.ImageryURI = _imageUri.Trim();
                imagery.IsNew = true;
                imagery.IsDirty = false;

                imagery.Save();
                GlobalData.ImageListItems.Add(imagery);
                UpdateGridAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "ConfirmText: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorConfirmImageComment), "ImageryActivity.ConfirmText");
            }
        }

        public void CancelText()
        {
            try
            {
                Imagery imagery = new Imagery();
                imagery.ImageryComment = "";
                imagery.ImageryURI = _imageUri.Trim();
                imagery.IsNew = true;
                imagery.IsDirty = false;

                imagery.Save();
                GlobalData.ImageListItems.Add(imagery);
                UpdateGridAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "CancelText: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCancelImageComment), "ImageryActivity.CancelText");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "remove")
            {
                try
                {
                    Imagery image = GlobalData.ImageListItems[_selectedItemIndex];
                    image.Remove();

                    GlobalData.ImageListItems.Remove(image);
                    UpdateGridAdapter();
                    _selectedItemIndex = -1;
                }
                catch(Exception er)
                {
                    Log.Error(TAG, "AlertPositiveButtonSelect (Remove instance): Exception - " + er.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, er, "Removing image", "ImageryActivity.AlertPositiveButtonSelect(Remove instance)");
                }
            }
            if(instanceId == "comment")
            {
                try
                {
                    GenericTextDialogFragment textFragment = new GenericTextDialogFragment(this, "Image Comment", GetString(Resource.String.ImageryActivityCommentGenericTitle));
                    FragmentTransaction transaction = FragmentManager.BeginTransaction();
                    textFragment.Show(transaction, textFragment.Tag);
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, "AlertPositiveButtonSelect (Comment instance): Exception - " + ex.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Adding comment to image selection", "ImageryActivity.AlertPositiveButtonSelect(Comment instance)");
                }
            }
            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "remove")
            {
                Toast.MakeText(this, Resource.String.ImageryActivityRemoveCancelToast, ToastLength.Short).Show();
            }

            if(instanceId == "comment")
            {
                ConfirmText("", -1);
            }

            if (instanceId == "useMic")
            {
                //PermissionResultUpdate(Permission.Denied);
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ImageryActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ImageryActivity.OnRequestPermissionsResult");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "ImageryActivity.AttemptPermissionRequest");
            }
        }
    }
}
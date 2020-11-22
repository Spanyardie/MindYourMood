using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;

using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using Android.Views;
using Android.Support.V4.View;
using com.spanyardie.MindYourMood.Adapters;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity()]
    public class MainActivity : AppCompatActivity
    {
        public const string TAG = "M:MainActivity";

        private Toolbar _toolbar;

        private ImageView _ownerThumb;
        private TextView _ownerWelcome;

        private ViewPager _viewPager;

        private ImageLoader _imageLoader = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            try
            {
                SetContentView(Resource.Layout.Main2);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.my_toolbar, Resource.String.MainScreenActionBarTitle, Color.White);

                _toolbar.SetLogo(Resource.Drawable.ic_launcher);

                GetFieldComponents();

                if (_viewPager != null)
                {
                    _viewPager.Adapter = new MainPagerAdapter(SupportFragmentManager);
                    _viewPager.OffscreenPageLimit = 2;
                }

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.mainbkgrnd4,
                    new ImageLoadingListener
                    (
                        loadingComplete: (imageUri, view, loadedImage) =>
                        {
                            var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                            ImageLoader_LoadingComplete(null, args);
                        }
                    )
                );

                GetOwnerInfo();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateMainActivity), "MainActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_viewPager != null)
                _viewPager.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemSettings = menu.FindItem(Resource.Id.MainActionSettings);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemSettings != null)
                            itemSettings.SetIcon(Resource.Drawable.ic_settings_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemSettings != null)
                            itemSettings.SetIcon(Resource.Drawable.ic_settings_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemSettings != null)
                            itemSettings.SetIcon(Resource.Drawable.ic_settings_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "MainActivity.SetActionIcons");
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Resource.Id.MainActionSettings)
                {
                    OpenSettings();
                    return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void OpenSettings()
        {
            Intent intent = new Intent(this, typeof(SettingsActivity));
            StartActivity(intent);
        }

        private void GetOwnerInfo()
        {
            try
            {
                if (PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.ReadContacts) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.ReadContacts))
                {
                    ContactsHelper contactHelper = new ContactsHelper(this);
                    if (contactHelper.ReadProfile())
                    {
                        string ownerUri = contactHelper.DeviceOwner.ThumbnailUri;
                        string ownerFirstName = contactHelper.DeviceOwner.FirstName;
                        string ownerSurname = contactHelper.DeviceOwner.Surname;
                        if (_ownerThumb != null)
                        {
                            if (!string.IsNullOrEmpty(ownerUri))
                            {
                                ImageryHelper imageHelper = new ImageryHelper(this, _ownerThumb);
                                imageHelper.GetImageFromStringUri(contactHelper.DeviceOwner.ThumbnailUri);
                            }
                            else
                            {
                                _ownerThumb.SetImageResource(Resource.Drawable.ic_launcher);
                            }
                        }
                        if (_ownerWelcome != null)
                        {
                            if (!(string.IsNullOrEmpty(ownerFirstName) || string.IsNullOrEmpty(ownerSurname)))
                            {
                                _ownerWelcome.Text = GetString(Resource.String.WelcomeMindYourMood) + " " + contactHelper.DeviceOwner.FirstName + " " + contactHelper.DeviceOwner.Surname;
                            }
                            else
                            {
                                _ownerWelcome.Text = GetString(Resource.String.WelcomeMindYourMoodNoContacts);
                            }
                        }
                    }
                }
                else
                {
                    if(_ownerWelcome != null)
                        _ownerWelcome.Text = GetString(Resource.String.WelcomeMindYourMoodNoContacts);
                    if(_ownerThumb != null)
                        _ownerThumb.SetImageResource(Resource.Drawable.ic_launcher);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetOwnerInfo: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMainGetOwnerInfo), "MainActivity.GetOwnerInfo");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _ownerWelcome = FindViewById<TextView>(Resource.Id.txtMainActivityWelcomeDeviceOwner);
                _ownerThumb = FindViewById<ImageView>(Resource.Id.imgMainActivityDeviceOwner);
                _viewPager = FindViewById<ViewPager>(Resource.Id.vp_main);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMainActivityGetComponents), "MainActivity.GetFieldComponents");
            }
        }
    }
}


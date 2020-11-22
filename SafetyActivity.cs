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
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help;
using com.spanyardie.MindYourMood.Adapters;
using Android.Support.V4.View;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class SafetyActivity : AppCompatActivity
    {
        public const string TAG = "M:SafetyActivity";

        private Toolbar _toolbar;
        private ViewPager _viewPager;

        private Button _done;

        private ImageLoader _imageLoader = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.SafetyLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.safetyToolbar, Resource.String.SafetyActionBarTitle, Color.White);

                GetFieldComponents();

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

                SetupCallbacks();

                if (_viewPager != null)
                {
                    _viewPager.Adapter = new SafetyMainPagerAdapter(SupportFragmentManager);
                    _viewPager.OffscreenPageLimit = 2;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateSafetyActivity), "SafetyActivity.OnCreate");
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
            MenuInflater.Inflate(Resource.Menu.SafetyMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    Finish();
                    return true;
                }
                if(item.ItemId == Resource.Id.safetyActionHelp)
                {
                    Intent intent = new Intent(this, typeof(SafetyHelpActivity));
                    StartActivity(intent);
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            try
            {
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _viewPager = FindViewById<ViewPager>(Resource.Id.vp_main);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSafetyGetComponents), "SafetyActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSafetySetupCallbacks), "SafetyActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHelp = menu.FindItem(Resource.Id.safetyActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SafetyActivity.SetActionIcons");
            }
        }
    }
}
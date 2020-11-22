using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help;
using Android.Support.V4.View;
using com.spanyardie.MindYourMood.Adapters;
using Android.Widget;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class ResourcesActivity : AppCompatActivity
    {
        public const string TAG = "M:ResourcesActivity";

        private Toolbar _toolbar;

        private ViewPager _viewPager;
        private Button _btnDone;

        private ImageLoader _imageLoader = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.ResourcesLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.resourcesMainToolbar, Resource.String.ResourcesActionBarTitle, Color.White);

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
                    _viewPager.Adapter = new ResourcesPagerAdapter(SupportFragmentManager);
                    _viewPager.OffscreenPageLimit = 2;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateResourcesActivity), "ResourcesActivity.OnCreate");
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
            MenuInflater.Inflate(Resource.Menu.ResourcesMenu, menu);

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

                if(item.ItemId == Resource.Id.resourcesActionHelp)
                {
                    Intent intent = new Intent(this, typeof(ResourcesHelpActivity));
                    StartActivity(intent);
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            try
            {
                _viewPager = FindViewById<ViewPager>(Resource.Id.vp_main);
                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "ResourcesActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            if(_btnDone != null)
                _btnDone.Click += Done_Click;
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
                var itemHelp = menu.FindItem(Resource.Id.resourcesActionHelp);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ResourcesActivity.SetActionIcons");
            }
        }
    }
}
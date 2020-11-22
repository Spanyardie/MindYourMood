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
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help;
using Android.Support.V4.View;
using com.spanyardie.MindYourMood.Adapters;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class SafetyPlanActivity : AppCompatActivity
    {
        public static string TAG = "M:SafetyPlanActivity";

        private Toolbar _toolbar;

        private LinearLayout _linActionButtons;

        private ViewPager _viewPager;

        private Button _btnDone;

        private ImageLoader _imageLoader = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.SafetyPlanLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.safetyPlanToolbar, Resource.String.SafetyPlanActionBarTitle, Color.White);

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
                    _viewPager.Adapter = new SafetyPlanPagerAdapter(SupportFragmentManager);
                    _viewPager.OffscreenPageLimit = 2;
                }

            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Error occurred during creation - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Creating Safety Plan Activity", "SafetyPlanActivity.OnCreate");
            }
        }


        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_viewPager != null)
                _viewPager.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void GetFieldComponents()
        {
            try
            {
                _linActionButtons = FindViewById<LinearLayout>(Resource.Id.linActionButtons);
                _viewPager = FindViewById<ViewPager>(Resource.Id.vp_main);
                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
                Log.Info(TAG, "GetFieldComponents: Retrieved field components successfully");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Error occurred retrieving view components - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting Field Components", "SafetyPlanActivity.GetFieldComponents");
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SafetyPlanMenu, menu);

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
                if(item.ItemId == Resource.Id.safetyPlanActionHelp)
                {
                    Intent intent = new Intent(this, typeof(SafetyPlanHelpActivity));
                    StartActivity(intent);
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHelp = menu.FindItem(Resource.Id.safetyPlanActionHelp);

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
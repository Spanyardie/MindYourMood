using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using com.spanyardie.MindYourMood.SubActivities.Help.Media;

namespace com.spanyardie.MindYourMood.SubActivities.Help
{
    [Activity()]
    public class MediaHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:MediaHelpActivity";

        private Toolbar _toolbar;

        private LinearLayout _imageryContainer;
        private ImageView _imageryImage;
        private TextView _imageryText;
        private LinearLayout _musicTherapyContainer;
        private ImageView _musicTherapyImage;
        private TextView _musicTherapyText;

        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MediaHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.mediaHelpToolbar, Resource.String.MediaHelpTitle, Color.White);

            GetFieldComponents();
            SetupCallbacks();
        }

        private void SetupCallbacks()
        {
            if(_imageryContainer != null)
                _imageryContainer.Click += ImageryContainer_Click;
            if(_imageryImage != null)
                _imageryImage.Click += ImageryContainer_Click;
            if(_imageryText != null)
                _imageryText.Click += ImageryContainer_Click;
            if(_musicTherapyContainer != null)
                _musicTherapyContainer.Click += MusicTherapyContainer_Click;
            if(_musicTherapyImage != null)
                _musicTherapyImage.Click += MusicTherapyContainer_Click;
            if(_musicTherapyText != null)
                _musicTherapyText.Click += MusicTherapyContainer_Click;
            if (_done != null)
                _done.Click += Done_Click;
        }

        private void MusicTherapyContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MediaMusicTherapyHelpActivity));
            StartActivity(intent);
        }

        private void ImageryContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MediaImageryHelpActivity));
            StartActivity(intent);
        }

        private void GetFieldComponents()
        {
            try
            {
                _imageryContainer = FindViewById<LinearLayout>(Resource.Id.linMediaHelpImagery);
                _imageryImage = FindViewById<ImageView>(Resource.Id.imgMediaHelpImagery);
                _imageryText = FindViewById<TextView>(Resource.Id.txtMediaHelpImagery);
                _musicTherapyContainer = FindViewById<LinearLayout>(Resource.Id.linMusicTherapyHelp);
                _musicTherapyImage = FindViewById<ImageView>(Resource.Id.imgMusicTherapyHelp);
                _musicTherapyText = FindViewById<TextView>(Resource.Id.txtMusicTherapyHelp);
                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFIeldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "MediaHelpActivity.GetFieldComponents");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MediaHelpMenu, menu);

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

                switch (item.ItemId)
                {
                    case Resource.Id.mediaHelpActionHome:
                        Intent intent = new Intent(this, typeof(MainHelpActivity));
                       Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                        return true;
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
                var itemHome = menu.FindItem(Resource.Id.mediaHelpActionHome);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemHome != null)
                            itemHome.SetIcon(Resource.Drawable.ic_home_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemHome != null)
                            itemHome.SetIcon(Resource.Drawable.ic_home_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemHome != null)
                            itemHome.SetIcon(Resource.Drawable.ic_home_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "MediaHelpActivity.SetActionIcons");
            }
        }
    }
}
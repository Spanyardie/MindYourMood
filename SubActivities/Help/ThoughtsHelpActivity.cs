using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Android.Util;
using com.spanyardie.MindYourMood.SubActivities.Help.ThoughtsHelp;
using UniversalImageLoader.Core;

namespace com.spanyardie.MindYourMood.SubActivities.Help
{
    [Activity()]
    public class ThoughtsHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:ThoughtsHelpActivity";

        private Toolbar _toolbar;

        private LinearLayout _enterThoughtContainer;
        private ImageView _enterThoughtImage;
        private TextView _enterThoughtText;

        private LinearLayout _viewThoughtContainer;
        private ImageView _viewThoughtImage;
        private TextView _viewThoughtText;

        private LinearLayout _showProgressContainer;
        private ImageView _showProgressImage;
        private TextView _showProgressText;

        private ImageView _progressFrom;
        private ImageView _progressTo;
        private Button _done;

        private ImageLoader _imageLoader = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ThoughtsHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.thoughtsHelpToolbar, Resource.String.ThoughtRecordsHelpTitle, Color.White);

            GetFIeldComponents();
            SetupCallbacks();
            _imageLoader = ImageLoader.Instance;

            SetupImages();
        }

        private void SetupImages()
        {
            if (_enterThoughtImage != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.enter, _enterThoughtImage, GlobalData.ImageOptions);
            if (_viewThoughtImage != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.view, _viewThoughtImage, GlobalData.ImageOptions);
            if (_showProgressImage != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.calendarb, _showProgressImage, GlobalData.ImageOptions);
            if (_progressFrom != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.datefrom, _progressFrom, GlobalData.ImageOptions);
            if (_progressTo != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.dateto, _progressTo, GlobalData.ImageOptions);
        }

        private void SetupCallbacks()
        {
            if(_enterThoughtContainer != null)
                _enterThoughtContainer.Click += EnterThoughtContainer_Click;
            if (_enterThoughtImage != null)
                _enterThoughtImage.Click += EnterThoughtContainer_Click;
            if (_enterThoughtText != null)
                _enterThoughtText.Click += EnterThoughtContainer_Click;

            if (_viewThoughtContainer != null)
                _viewThoughtContainer.Click += ViewThoughtContainer_Click;
            if (_viewThoughtImage != null)
                _viewThoughtImage.Click += ViewThoughtContainer_Click;
            if (_viewThoughtText != null)
                _viewThoughtText.Click += ViewThoughtContainer_Click;

            if (_showProgressContainer != null)
                _showProgressContainer.Click += ShowProgressContainer_Click;
            if (_showProgressImage != null)
                _showProgressImage.Click += ShowProgressContainer_Click;
            if (_showProgressText != null)
                _showProgressText.Click += ShowProgressContainer_Click;
            if (_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void ShowProgressContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ViewProgressHelpActivity));
            StartActivity(intent);
        }

        private void ViewThoughtContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ViewThoughtsHelpActivity));
            StartActivity(intent);
        }

        private void EnterThoughtContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(EnterThoughtsHelpActivity));
            StartActivity(intent);
        }

        private void GetFIeldComponents()
        {
            try
            {
                _enterThoughtContainer = FindViewById<LinearLayout>(Resource.Id.linEnterThoughtHelpContainer);
                _viewThoughtContainer = FindViewById<LinearLayout>(Resource.Id.linViewThoughtHelpContainer);
                _showProgressContainer = FindViewById<LinearLayout>(Resource.Id.linShowProgressHelpContainer);
                _enterThoughtImage = FindViewById<ImageView>(Resource.Id.imgEnterThoughtHelpImage);
                _viewThoughtImage = FindViewById<ImageView>(Resource.Id.imgViewThoughtHelpImage);
                _showProgressImage = FindViewById<ImageView>(Resource.Id.imgShowProgressHelpImage);
                _enterThoughtText = FindViewById<TextView>(Resource.Id.txtEnterThoughtHelpText);
                _viewThoughtText = FindViewById<TextView>(Resource.Id.txtViewThoughtHelpText);
                _showProgressText = FindViewById<TextView>(Resource.Id.txtShowProgressHelpText);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _progressFrom = FindViewById<ImageView>(Resource.Id.imgProgressFrom);
                _progressTo = FindViewById<ImageView>(Resource.Id.imgProgressTo);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "ThoughtsHelpActivity.GetFieldComponents");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ThoughtRecordHelpMenu, menu);

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
                    case Resource.Id.thoughtsHelpActionHome:
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
                var itemHome = menu.FindItem(Resource.Id.thoughtsHelpActionHome);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ThoughtsHelpActivity.SetActionIcons");
            }
        }
    }
}
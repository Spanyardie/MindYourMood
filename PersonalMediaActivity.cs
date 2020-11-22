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
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class PersonalMediaActivity : AppCompatActivity
    {
        public const string TAG = "M:PersonalMediaActivity";

        private Toolbar _toolbar;
        private LinearLayout _linImagery;
        private LinearLayout _linMusic;
        private TextView _imagery;
        private TextView _music;
        private Button _btnDone;

        private ImageLoader _imageLoader = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.PersonalMediaLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.personalMediaToolbar, Resource.String.PersonalMediaActionBarTitle, Color.White);

                GetFieldComponents();

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.imagery,
                    new ImageLoadingListener
                    (
                        loadingComplete: (imageUri, view, loadedImage) =>
                        {
                            var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                            ImageLoader_ImageryLoadingComplete(null, args);
                        }
                    )
                );

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.cds,
                    new ImageLoadingListener
                    (
                        loadingComplete: (imageUri, view, loadedImage) =>
                        {
                            var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                            ImageLoader_CdsLoadingComplete(null, args);
                        }
                    )
                );

                SetupCallbacks();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatePersonalMediaActivity), "PersonalMediaActivity.OnCreate");
            }
        }

        private void ImageLoader_ImageryLoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linImagery != null)
                _linImagery.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void ImageLoader_CdsLoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linMusic != null)
                _linMusic.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void GetFieldComponents()
        {
            try
            {
                _linImagery = FindViewById<LinearLayout>(Resource.Id.linImagery);
                _linMusic = FindViewById<LinearLayout>(Resource.Id.linMusic);

                _imagery = FindViewById<TextView>(Resource.Id.txtImagery);
                _music = FindViewById<TextView>(Resource.Id.txtMusic);

                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorPersonalMediaGetComponents), "PersonalMediaActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_linImagery != null)
                    _linImagery.Click += Imagery_Click;
                if(_linMusic != null)
                    _linMusic.Click += Music_Click;
                if(_imagery != null)
                    _imagery.Click += Imagery_Click;
                if(_music != null)
                    _music.Click += Music_Click;
                if(_btnDone != null)
                    _btnDone.Click += Done_Click;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorPersonalMediaSetupCallbacks), "PersonalMediaActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
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
                if(item.ItemId == Resource.Id.mediaActionHelp)
                {
                    Intent intent = new Intent(this, typeof(MediaHelpActivity));
                    StartActivity(intent);
                    return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.PersonalMediaMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void Music_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MusicTherapyActivity));
            StartActivity(intent);
        }

        private void Imagery_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ImageryActivity));
            StartActivity(intent);
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHelp = menu.FindItem(Resource.Id.mediaActionHelp);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "PersonalMediaActivity.SetActionIcons");
            }
        }
    }
}
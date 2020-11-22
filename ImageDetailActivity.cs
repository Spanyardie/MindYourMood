using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using UniversalImageLoader.Core;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class ImageDetailActivity : Activity
    {
        public const string TAG = "M:ImageDetailActivity";

        //private Toolbar _toolbar;
        private ImageView _image;
        private TextView _comment;

        private Button _goBack;

        private ImageLoader _imageLoader;

        public ImageDetailActivity()
        {
            _imageLoader = ImageLoader.Instance;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.ImagerySelectDetailLayout);

                if (Intent != null && Intent.Extras != null)
                {
                    string imageComment = Intent.GetStringExtra("imageComment");
                    Log.Info(TAG, "OnCreate: Image comment - " + imageComment);

                    string imageUri = Intent.GetStringExtra("imageUri");
                    Log.Info(TAG, "OnCreate: Image Uri - " + imageUri);

                    GetFieldComponents();

                    SetupCallbacks();

                    if (_image != null)
                    {
                        Log.Info(TAG, "OnCreate: Using Image Loader to load image");
                        _imageLoader.DisplayImage(imageUri, _image, GlobalData.ImageOptions);
                    }
                    else
                    {
                        Log.Error(TAG, "OnCreate: _image is NULL!");
                    }

                    if (_comment != null)
                    {
                        _comment.Text = imageComment.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "_comment is NULL!");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorImageDetailActivityCreate), "ImageDetailActivity.OnCreate");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _image = FindViewById<ImageView>(Resource.Id.imgImageDetailImage);
                _comment = FindViewById<TextView>(Resource.Id.txtImageDetailComment);
                _goBack = FindViewById<Button>(Resource.Id.btnImageryDetailGoBack);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorImageDetailActivityGetComponents), "ImageDetailActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            if(_goBack != null)
                _goBack.Click += GoBack_Click;
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ImageryActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
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
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}
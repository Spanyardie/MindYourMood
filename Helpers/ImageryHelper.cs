using System;
using Android.Widget;
using UniversalImageLoader.Core;
using Android.Util;
using Android.App;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ImageryHelper
    {
        public const string TAG = "M:ImageryHelper";

        public string PictureOfTheDayUri { get; set; }

        private ImageLoader _imageLoader;
        private Activity _activity;
        private ImageView _imageView;

        public ImageryHelper(Activity activity, ImageView imageView)
        {
            _activity = activity;
            _imageView = imageView;

            _imageLoader = ImageLoader.Instance;
        }

        public ImageView GetPositivePictureOfTheDay()
        {
            try
            {
                if (GlobalData.ImageListItems != null && GlobalData.ImageListItems.Count > 0)
                {
                    var count = GlobalData.ImageListItems.Count;
                    //pick one at random
                    Random random = new Random();
                    var index = random.Next(GlobalData.ImageListItems.Count);

                    var imagery = GlobalData.ImageListItems[index];

                    var uri = imagery.ImageryURI;

                    PictureOfTheDayUri = uri;

                    _imageLoader.DisplayImage(uri, _imageView, GlobalData.ImageOptions);
                }
                else
                {
                    //grab a default picture
                    _imageView.SetImageResource(Resource.Drawable.ic_launcher);
                    PictureOfTheDayUri = "";
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetPositivePictureOfTheDay: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorImageryHelperGetPositiveImageDay), "ImageryHelper.GetPositivePictureOfTheDay");
            }

            return _imageView;
        }

        public ImageView GetImageFromStringUri(string uri)
        {
            try
            {
                if (_imageView != null)
                {
                    if (_imageLoader != null)
                    {
                        Log.Info(TAG, "GetImageFromStringUri: Attempting to load " + uri);
                        _imageView.SetImageURI(Android.Net.Uri.Parse(uri));
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetImageFromStringUri: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting image from Uri", "ImageryHelper.GetImageFromStringUri");
            }
            return _imageView;
        }
    }
}
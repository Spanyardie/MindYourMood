using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using System;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using UniversalImageLoader.Core;
using Android.Content;
using Android.App;
using Android.OS;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class ImageryGridAdapter : BaseAdapter
    {
        public const string TAG = "M:ImageryGridAdapter";

        Activity _activity;

        private List<Imagery> _images = null;

        private ImageLoader _imageLoader = null;

        public ImageryGridAdapter(Activity activity)
        {
            _activity = activity;

            _images = new List<Imagery>();

            GetImageData();

            _imageLoader = ImageLoader.Instance;
        }

        private void GetImageData()
        {
            if(GlobalData.ImageListItems != null)
            {
                _images = GlobalData.ImageListItems;
            }
        }

        public override int Count
        {
            get
            {
                return _images.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if(_images != null)
            {
                if(position <= _images.Count)
                {
                    return _images[position].ImageryID;
                }
            }
            return -1;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView = null;
            TextView textComment = null;
            try
            {
                bool isSelected = ((ImageryActivity)_activity).SelectedItemIndex == position;

                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.ImageGridListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.ImageGridListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                imageView = convertView.FindViewById<ImageView>(Resource.Id.imgGridListItem);
                textComment = convertView.FindViewById<TextView>(Resource.Id.txtGridListItemComment);

                imageView.LayoutParameters = new LinearLayout.LayoutParams(400, 400);
                imageView.SetScaleType(ImageView.ScaleType.FitCenter);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    imageView.TransitionName = "imageTransition";
                }
                imageView.SetPadding(18, 18, 18, 18);

                if(isSelected)
                    imageView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    
                _imageLoader.DisplayImage(_images[position].ImageryURI, imageView, GlobalData.ImageOptions);
                textComment.Text = _images[position].ImageryComment.Trim();
                Log.Info(TAG, "GetView: Created new image");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorImageryListGetView), "ImageryGridAdapter.GetView");
            }
            return convertView;
        }
    }
}
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Android.Util;
using System;
using com.spanyardie.MindYourMood.Helpers;
using Android.App;
using com.spanyardie.MindYourMood.SubActivities.Help;
using UniversalImageLoader.Core;
using com.spanyardie.MindYourMood.SubActivities.StructuredPlan;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class StructuredPlanHorizontalPagerAdapter : PagerAdapter
    {

        private StructuredPlanHorizontalPagerFragment _pagerFragment;
        private Context _context;
        private LayoutInflater _inflater;

        private ImageView _itemImage;
        private TextView _itemText;

        private int[] _images;
        private string[] _texts;

        private ImageLoader _imageLoader = null;

        public StructuredPlanHorizontalPagerAdapter(StructuredPlanHorizontalPagerFragment pagerFragment, Context context)
        {
            _pagerFragment = pagerFragment;
            _context = context;
            _inflater = LayoutInflater.From(context);

            _imageLoader = ImageLoader.Instance;

            SetupResources();
        }

        private void SetupResources()
        {
            _images = new int[6];
            _texts = new string[6];

            SetupImages();
            SetupTexts();
        }

        private void SetupTexts()
        {
            if (_texts != null)
            {
                _texts[0] = ((Activity)_context).GetString(Resource.String.StructuredPlanFeelingsToolbarTitle);
                _texts[1] = ((Activity)_context).GetString(Resource.String.StructuredPlanReactionsToolbarTitle);
                _texts[2] = ((Activity)_context).GetString(Resource.String.StructuredPlanAttitudeToolbarTitle);
                _texts[3] = ((Activity)_context).GetString(Resource.String.StructuredPlanHealthToolbarTitle);
                _texts[4] = ((Activity)_context).GetString(Resource.String.StructuredPlanFantasiesToolbarTitle);
                _texts[5] = ((Activity)_context).GetString(Resource.String.StructuredPlanRelationshipsToolbarTitle);
            }
        }

        private void SetupImages()
        {
            if (_images != null)
            {
                _images[0] = Resource.Drawable.feelingspager;
                _images[1] = Resource.Drawable.reactionspager;
                _images[2] = Resource.Drawable.attitudespager;
                _images[3] = Resource.Drawable.healthpager;
                _images[4] = Resource.Drawable.fantasypager;
                _images[5] = Resource.Drawable.relationshipspager;
            }
        }

        public override int Count
        {
            get
            {
                return _images.Length;
            }
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
        {
            return view.Equals(objectValue);
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            View view;
            if (_context.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
            {
                view = _inflater.Inflate(Resource.Layout.ItemPortraitLayout, container, false);
            }
            else
            {
                view = _inflater.Inflate(Resource.Layout.ItemHorizontalLayout, container, false);
            }
            if (view != null)
            {
                GetFieldComponents(view);

                SetupCallbacks();

                if (_itemImage != null)
                {
                    _imageLoader.DisplayImage("drawable://" + _images[position], _itemImage, GlobalData.ImageOptions);
                    _itemImage.Tag = _texts[position];
                }
                if (_itemText != null)
                {
                    _itemText.Text = _texts[position];
                    _itemText.Tag = _texts[position];
                }
                view.Tag = _texts[position];
            }

            container.AddView(view);
            return view;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            container.RemoveView((View)objectValue);
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _itemImage = view.FindViewById<ImageView>(Resource.Id.imgItemImage);
                _itemText = view.FindViewById<TextView>(Resource.Id.txtItemText);
            }
            catch (Exception e)
            {
                Log.Error("StructuredPlanHorizontalPagerAdapter", "Exception - " + e.Message);
            }
        }

        private void SetupCallbacks()
        {
            if (_itemImage != null)
                _itemImage.Click += ItemImage_Click;

            if (_itemText != null)
                _itemText.Click += ItemText_Click;
        }

        private void ItemText_Click(object sender, EventArgs e)
        {
            string theTag = ((TextView)sender).Tag.ToString();
            DoNavigation(theTag);
        }

        private void ItemImage_Click(object sender, EventArgs e)
        {
            string theTag = ((ImageView)sender).Tag.ToString();
            DoNavigation(theTag);
        }

        private void DoNavigation(string theTag)
        {
            Intent intent = null;

            switch (theTag)
            {
                case "Feelings":
                case "Sentimientos":
                    intent = new Intent(_context, typeof(StructuredPlanFeelings));
                    break;
                case "Reactions":
                case "Reacciones":
                    intent = new Intent(_context, typeof(StructuredPlanReactions));
                    break;
                case "Attitudes":
                case "Actitudes":
                    intent = new Intent(_context, typeof(StructuredPlanAttitudes));
                    break;
                case "Health":
                case "Salud":
                    intent = new Intent(_context, typeof(StructuredPlanHealth));
                    break;
                case "Fantasies":
                case "Fantasías":
                    intent = new Intent(_context, typeof(StructuredPlanFantasies));
                    break;
                case "Relationships":
                case "Relaciones":
                    intent = new Intent(_context, typeof(StructuredPlanRelationships));
                    break;
            }

            if (intent != null)
                ((Activity)_context).StartActivity(intent);
        }
    }
}
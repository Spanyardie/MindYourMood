using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using com.spanyardie.MindYourMood.SubActivities.SafetyPlan;
using UniversalImageLoader.Core;

namespace com.spanyardie.MindYourMood.Adapters
{
    class SafetyPlanHorizontalPagerAdapter : PagerAdapter
    {

        private SafetyPlanHorizontalPagerFragment _pagerFragment;
        private Context _context;
        private LayoutInflater _inflater;

        private ImageView _itemImage;
        private TextView _itemText;

        private int[] _images;
        private string[] _texts;

        private ImageLoader _imageLoader = null;

        public SafetyPlanHorizontalPagerAdapter(SafetyPlanHorizontalPagerFragment pagerFragment, Context context)
        {
            _pagerFragment = pagerFragment;
            _context = context;
            _inflater = LayoutInflater.From(context);

            _imageLoader = ImageLoader.Instance;

            SetupResources();
        }

        private void SetupResources()
        {
            _images = new int[8];
            _texts = new string[8];

            SetupImages();
            SetupTexts();
        }

        private void SetupTexts()
        {
            if (_texts != null)
            {
                _texts[0] = ((Activity)_context).GetString(Resource.String.titleStopMyself);
                _texts[1] = ((Activity)_context).GetString(Resource.String.safetyPlanWarningSignsActivityTitle);
                _texts[2] = ((Activity)_context).GetString(Resource.String.safetyPlanWorkedInThePastActivityTitle);
                _texts[3] = ((Activity)_context).GetString(Resource.String.KeepCalmHelpScreenTitle);
                _texts[4] = ((Activity)_context).GetString(Resource.String.TellMyselfHelpScreenTitle);
                _texts[5] = ((Activity)_context).GetString(Resource.String.wordOthers);
                _texts[6] = ((Activity)_context).GetString(Resource.String.ReadingContactsProgressDialogTitle);
                _texts[7] = ((Activity)_context).GetString(Resource.String.SafePlacesLayoutTitleMain);
            }
        }

        private void SetupImages()
        {
            if (_images != null)
            {
                _images[0] = Resource.Drawable.stopactsuicide;
                _images[1] = Resource.Drawable.warningsignsgetbad;
                _images[2] = Resource.Drawable.methodsofcopingpast;
                _images[3] = Resource.Drawable.keepingcalm;
                _images[4] = Resource.Drawable.thingstellmyself;
                _images[5] = Resource.Drawable.thingsotherscando;
                _images[6] = Resource.Drawable.whocanicontact;
                _images[7] = Resource.Drawable.safeplacestogo;
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
            View view = null;

            try
            {
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
            }
            catch(Exception e)
            {
                Log.Error("SafetyPlanHorizontalPagerAdapter.InstantiateItem", "Exception - " + e.Message);
            }
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
                Log.Error("SafetyPlanHorizontalPagerAdapter.DestroyItem", "Exception - " + e.Message);
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
                case "Stop Myself":
                case "Deténgame":
                    intent = new Intent(_context, typeof(StopSuicideActivity));
                    break;
                case "Warning Signs":
                case "Señales de advertencia":
                    intent = new Intent(_context, typeof(WarningSignsActivity));
                    break;
                case "Coping Methods":
                case "Hacer frente a los métodos":
                    intent = new Intent(_context, typeof(WorkedPastActivity));
                    break;
                case "Keep Calm":
                case "Mantener la calma":
                    intent = new Intent(_context, typeof(HowToCalmActivity));
                    break;
                case "Tell Myself":
                case "Dime a mí mismo":
                    intent = new Intent(_context, typeof(TellMyselfActivity));
                    break;
                case "Others":
                case "Otros":
                    intent = new Intent(_context, typeof(OthersDoActivity));
                    break;
                case "Contacts":
                case "Contactos":
                    intent = new Intent(_context, typeof(ContactActivity));
                    break;
                case "Safe Places":
                case "Los lugares seguros":
                    intent = new Intent(_context, typeof(SafePlacesActivity));
                    break;
            }

            if (intent != null)
                ((Activity)_context).StartActivity(intent);
        }
    }
}
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

namespace com.spanyardie.MindYourMood.Adapters
{
    public class HorizontalPagerAdapter : PagerAdapter
    {

        private HorizontalPagerFragment _pagerFragment;
        private Context _context;
        private LayoutInflater _inflater;

        private ImageView _itemImage;
        private TextView _itemText;

        private int[] _images;
        private string[] _texts;

        private ImageLoader _imageLoader = null;

        public HorizontalPagerAdapter(HorizontalPagerFragment pagerFragment, Context context)
        {
            _pagerFragment = pagerFragment;
            _context = context;
            _inflater = LayoutInflater.From(context);

            _imageLoader = ImageLoader.Instance;

            SetupResources();
        }

        private void SetupResources()
        {
            _images = new int[11];
            _texts = new string[11];

            SetupImages();
            SetupTexts();
        }

        private void SetupTexts()
        {
            if (_texts != null)
            {
                _texts[0] = ((Activity)_context).GetString(Resource.String.HelpNowHelpScreenTitle);
                _texts[1] = ((Activity)_context).GetString(Resource.String.ThoughtsHelpScreenTitle);
                _texts[2] = ((Activity)_context).GetString(Resource.String.AchievementsHelpScreenTitle);
                _texts[3] = ((Activity)_context).GetString(Resource.String.SafetyHelpScreenTitle);
                _texts[4] = ((Activity)_context).GetString(Resource.String.ActivitiesHelpScreenTitle);
                _texts[5] = ((Activity)_context).GetString(Resource.String.TreatmentPlanTitle);
                _texts[6] = ((Activity)_context).GetString(Resource.String.PersonalMediaTitle);
                _texts[7] = ((Activity)_context).GetString(Resource.String.ResourcesTitle);
                _texts[8] = ((Activity)_context).GetString(Resource.String.SummaryTitle);
                _texts[9] = ((Activity)_context).GetString(Resource.String.wordHelp);
                _texts[10] = ((Activity)_context).GetString(Resource.String.AboutTitle);
            }
        }

        private void SetupImages()
        {
            if(_images != null)
            {
                _images[0] = Resource.Drawable.helpmenow;
                _images[1] = Resource.Drawable.thoughtbubbleswoman;
                _images[2] = Resource.Drawable.achievement;
                _images[3] = Resource.Drawable.safetypager;
                _images[4] = Resource.Drawable.activities;
                _images[5] = Resource.Drawable.treatmentpager;
                _images[6] = Resource.Drawable.Media;
                _images[7] = Resource.Drawable.resourcespager;
                _images[8] = Resource.Drawable.summarypager;
                _images[9] = Resource.Drawable.help;
                _images[10] = Resource.Drawable.about;
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
                Log.Error("InstantiateItem", "Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog)
                    ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Instantiating item", "HorizontalPagerAdapter.InstantiateItem");
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
            catch(Exception e)
            {
                Log.Error("GetFieldComponents", "Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog)
                    ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Getting field components", "HorizontalPagerAdapter.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            if(_itemImage != null)
                _itemImage.Click += ItemImage_Click;

            if(_itemText != null)
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

            switch(theTag)
            {
                case "HELP NOW":
                case "AYUDA AHORA":
                    intent = new Intent(_context, typeof(HelpNowActivity));
                    break;
                case "Thoughts":
                case "Pensamientos":
                    intent = new Intent(_context, typeof(ThoughtRecordsActivity));
                    break;
                case "Achievements":
                case "Logros":
                    intent = new Intent(_context, typeof(AchievementChartActivity));
                    break;
                case "Safety":
                case "Seguridad":
                    intent = new Intent(_context, typeof(SafetyActivity));
                    break;
                case "Activities":
                case "Actividades":
                    intent = new Intent(_context, typeof(ActivitiesActivity));
                    break;
                case "Treatment":
                case "Plan de tratamiento":
                    intent = new Intent(_context, typeof(TreatmentPlanActivity));
                    break;
                case "Media":
                case "Medios personales":
                    intent = new Intent(_context, typeof(PersonalMediaActivity));
                    break;
                case "Resources":
                case "Recursos":
                    intent = new Intent(_context, typeof(ResourcesActivity));
                    break;
                case "Summary":
                case "Resumen":
                    intent = new Intent(_context, typeof(SummaryActivity));
                    break;
                case "Help":
                case "Ayuda":
                    intent = new Intent(_context, typeof(MainHelpActivity));
                    break;
                case "About":
                case "Acerca de":
                    intent = new Intent(_context, typeof(AboutActivity));
                    break;
            }

            if (intent != null)
                ((Activity)_context).StartActivity(intent); 
        }
    }
}
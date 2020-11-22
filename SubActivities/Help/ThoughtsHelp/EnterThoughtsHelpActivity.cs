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
using UniversalImageLoader.Core;

namespace com.spanyardie.MindYourMood.SubActivities.Help.ThoughtsHelp
{
    [Activity()]
    public class EnterThoughtsHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:EnterThoughtsHelpActivity";

        private Toolbar _toolbar;

        private LinearLayout _situationContainer;
        private ImageView _situationImage;
        private TextView _situationText;

        private LinearLayout _moodsContainer;
        private ImageView _moodsImage;
        private TextView _moodsText;

        private LinearLayout _automaticThoughtsContainer;
        private ImageView _automaticThoughtsImage;
        private TextView _automaticThoughtsText;

        private LinearLayout _evidenceForContainer;
        private ImageView _evidenceForImage;
        private TextView _evidenceForText;

        private LinearLayout _evidenceAgainstContainer;
        private ImageView _evidenceAgainstImage;
        private TextView _evidenceAgainstText;

        private LinearLayout _alternativeThoughtsContainer;
        private ImageView _alternativeThoughtsImage;
        private TextView _alternativeThoughtsText;

        private LinearLayout _rerateMoodsContainer;
        private ImageView _rerateMoodsImage;
        private TextView _rerateMoodsText;

        private Button _done;

        private ImageLoader _imageLoader = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EnterThoughtsHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.enterThoughtsHelpToolbar, Resource.String.EnterThoughtRecordHelpTitle, Color.White);

            GetFieldComponents();
            SetupCallbacks();

            _imageLoader = ImageLoader.Instance;

            SetupImages();
        }

        private void SetupImages()
        {
            if(_situationImage != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.enter, _situationImage, GlobalData.ImageOptions);
            if(_moodsImage != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.moodFace, _moodsImage, GlobalData.ImageOptions);
            if(_automaticThoughtsImage != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.thoughtbubbleswoman, _automaticThoughtsImage, GlobalData.ImageOptions);
            if(_evidenceForImage != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.evidence, _evidenceForImage, GlobalData.ImageOptions);
            if (_evidenceAgainstImage != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.evidence, _evidenceAgainstImage, GlobalData.ImageOptions);
            if (_alternativeThoughtsImage != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.alternative, _alternativeThoughtsImage, GlobalData.ImageOptions);
            if (_rerateMoodsImage != null)
                _imageLoader.DisplayImage("drawable://" + Resource.Drawable.moodFace, _rerateMoodsImage, GlobalData.ImageOptions);
        }

        private void SetupCallbacks()
        {
            if(_situationContainer != null)
                _situationContainer.Click += SituationContainer_Click;
            if (_situationImage != null)
                _situationImage.Click += SituationContainer_Click;
            if (_situationText != null)
                _situationText.Click += SituationContainer_Click;

            if (_moodsContainer != null)
                _moodsContainer.Click += MoodsContainer_Click;
            if (_moodsImage != null)
                _moodsImage.Click += MoodsContainer_Click;
            if (_moodsText != null)
                _moodsText.Click += MoodsContainer_Click;

            if (_automaticThoughtsContainer != null)
                _automaticThoughtsContainer.Click += AutomaticThoughtsContainer_Click;
            if (_automaticThoughtsImage != null)
                _automaticThoughtsImage.Click += AutomaticThoughtsContainer_Click;
            if (_automaticThoughtsText != null)
                _automaticThoughtsText.Click += AutomaticThoughtsContainer_Click;

            if (_evidenceForContainer != null)
                _evidenceForContainer.Click += EvidenceForContainer_Click;
            if (_evidenceForImage != null)
                _evidenceForImage.Click += EvidenceForContainer_Click;
            if (_evidenceForText != null)
                _evidenceForText.Click += EvidenceForContainer_Click;

            if (_evidenceAgainstContainer != null)
                _evidenceAgainstContainer.Click += EvidenceAgainstContainer_Click;
            if (_evidenceAgainstImage != null)
                _evidenceAgainstImage.Click += EvidenceAgainstContainer_Click;
            if (_evidenceAgainstText != null)
                _evidenceAgainstText.Click += EvidenceAgainstContainer_Click;

            if (_alternativeThoughtsContainer != null)
                _alternativeThoughtsContainer.Click += AlternativeThoughtsContainer_Click;
            if (_alternativeThoughtsImage != null)
                _alternativeThoughtsImage.Click += AlternativeThoughtsContainer_Click;
            if (_alternativeThoughtsText != null)
                _alternativeThoughtsText.Click += AlternativeThoughtsContainer_Click;

            if (_rerateMoodsContainer != null)
                _rerateMoodsContainer.Click += RerateMoodsContainer_Click;
            if (_rerateMoodsImage != null)
                _rerateMoodsImage.Click += RerateMoodsContainer_Click;
            if (_rerateMoodsText != null)
                _rerateMoodsText.Click += RerateMoodsContainer_Click;
            if (_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void RerateMoodsContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RerateMoodsHelpActivity));
            StartActivity(intent);
        }

        private void AlternativeThoughtsContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(AlternativeThoughtsHelpActivity));
            StartActivity(intent);
        }

        private void EvidenceAgainstContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(EvidenceAgainstHelpActivity));
            StartActivity(intent);
        }

        private void EvidenceForContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(EvidenceForHelpActivity));
            StartActivity(intent);
        }

        private void AutomaticThoughtsContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(AutomaticThoughtsHelpActivity));
            StartActivity(intent);
        }

        private void MoodsContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MoodsHelpActivity));
            StartActivity(intent);
        }

        private void SituationContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SituationHelpActivity));
            StartActivity(intent);
        }

        private void GetFieldComponents()
        {
            try
            {
                _situationContainer = FindViewById<LinearLayout>(Resource.Id.linEnterThoughtsSituationHelpContainer);
                _situationImage = FindViewById<ImageView>(Resource.Id.imgEnterThoughtsSituationHelpImage);
                _situationText = FindViewById<TextView>(Resource.Id.txtEnterThoughtsSituationHelpText);

                _moodsContainer = FindViewById<LinearLayout>(Resource.Id.linEnterThoughtsMoodsHelpContainer);
                _moodsImage = FindViewById<ImageView>(Resource.Id.imgEnterThoughtsMoodsHelpImage);
                _moodsText = FindViewById<TextView>(Resource.Id.txtEnterThoughtsMoodsHelpText);

                _automaticThoughtsContainer = FindViewById<LinearLayout>(Resource.Id.linEnterThoughtsAutomaticThoughtsHelpContainer);
                _automaticThoughtsImage = FindViewById<ImageView>(Resource.Id.imgEnterThoughtsAutomaticThoughtsHelpImage);
                _automaticThoughtsText = FindViewById<TextView>(Resource.Id.txtEnterThoughtsAutomaticThoughtsHelpText);

                _evidenceForContainer = FindViewById<LinearLayout>(Resource.Id.linEnterThoughtsEvidenceForHelpContainer);
                _evidenceForImage = FindViewById<ImageView>(Resource.Id.imgEnterThoughtsEvidenceForHelpImage);
                _evidenceForText = FindViewById<TextView>(Resource.Id.txtEnterThoughtsEvidenceForHelpText);

                _evidenceAgainstContainer = FindViewById<LinearLayout>(Resource.Id.linEnterThoughtsEvidenceAgainstHelpContainer);
                _evidenceAgainstImage = FindViewById<ImageView>(Resource.Id.imgEnterThoughtsEvidenceAgainstHelpImage);
                _evidenceAgainstText = FindViewById<TextView>(Resource.Id.txtEnterThoughtsEvidenceAgainstHelpText);

                _alternativeThoughtsContainer = FindViewById<LinearLayout>(Resource.Id.linEnterThoughtsAlternativeThoughtsHelpContainer);
                _alternativeThoughtsImage = FindViewById<ImageView>(Resource.Id.imgEnterThoughtsAlternativeThoughtsHelpImage);
                _alternativeThoughtsText = FindViewById<TextView>(Resource.Id.txtEnterThoughtsAlternativeThoughtsHelpText);

                _rerateMoodsContainer = FindViewById<LinearLayout>(Resource.Id.linEnterThoughtsRerateMoodsHelpContainer);
                _rerateMoodsImage = FindViewById<ImageView>(Resource.Id.imgEnterThoughtsRerateMoodsHelpImage);
                _rerateMoodsText = FindViewById<TextView>(Resource.Id.txtEnterThoughtsRerateMoodsHelpText);

                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "EnterThoughtsHelpActivity.GetFieldComponents");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.EnterThoughtRecordHelpMenu, menu);

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
                    case Resource.Id.enterThoughtsHelpActionHome:
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
                var itemHome = menu.FindItem(Resource.Id.enterThoughtsHelpActionHome);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "EnterThoughtsHelpActivity.SetActionIcons");
            }
        }
    }
}
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
using com.spanyardie.MindYourMood.SubActivities.Help.SafetyPlan;

namespace com.spanyardie.MindYourMood.SubActivities.Help
{
    [Activity()]
    public class SafetyHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:SafetyHelpActivity";

        private Toolbar _toolbar;

        private ImageView _safetyPlanImage;
        private ImageView _calmCardsImage;
        private TextView _safetyPlanText;
        private TextView _calmCardsText;
        private LinearLayout _safetyPlanContainer;
        private LinearLayout _calmCardsContainer;

        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SafetyHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.safetyHelpToolbar, Resource.String.SafetyHelpTitle, Color.White);

            GetFieldComponents();
            SetupCallbacks();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SafetyHelpMenu, menu);

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
                    case Resource.Id.safetyHelpActionHome:
                        Intent intent = new Intent(this, typeof(MainHelpActivity));
                       Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            try
            {
                _calmCardsContainer = FindViewById<LinearLayout>(Resource.Id.linSafetyHelpCalmCards);
                _calmCardsImage = FindViewById<ImageView>(Resource.Id.imgSafetyHelpCalmCards);
                _calmCardsText = FindViewById<TextView>(Resource.Id.txtSafetyHelpCalmCards);
                _safetyPlanContainer = FindViewById<LinearLayout>(Resource.Id.linSafetyHelpSafetyPlan);
                _safetyPlanImage = FindViewById<ImageView>(Resource.Id.imgSafetyHelpSafetyPlan);
                _safetyPlanText = FindViewById<TextView>(Resource.Id.txtSafetyHelpSafetyPlan);
                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "SafetyHelpActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_calmCardsContainer != null)
                    _calmCardsContainer.Click += CalmCardsContainer_Click;
                if (_calmCardsImage != null)
                    _calmCardsImage.Click += CalmCardsContainer_Click;
                if (_calmCardsText != null)
                    _calmCardsText.Click += CalmCardsContainer_Click;
                if (_safetyPlanContainer != null)
                    _safetyPlanContainer.Click += SafetyPlanContainer_Click;
                if (_safetyPlanImage != null)
                    _safetyPlanImage.Click += SafetyPlanContainer_Click;
                if (_safetyPlanText != null)
                    _safetyPlanText.Click += SafetyPlanContainer_Click;
                if (_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Setting up Callbacks", "SafetyHelpActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void SafetyPlanContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(SafetyPlanHelpActivity));
                StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "SafetyPlanContainer_Click: Exception - " + ex.Message);
                ErrorDisplay.ShowErrorAlert(this, ex, "Showing Safety Plan help", "SafetyHelpActivity.SafetyPlanContainer_Click");
            }
        }

        private void CalmCardsContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(SafetyPlanCardsHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SafetyPlanContainer_Click: Exception - " + ex.Message);
                ErrorDisplay.ShowErrorAlert(this, ex, "Showing Calm Card help", "SafetyHelpActivity.CalmCardsContainer_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHome = menu.FindItem(Resource.Id.safetyHelpActionHome);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SafetyHelpActivity.SetActionIcons");
            }
        }
    }
}
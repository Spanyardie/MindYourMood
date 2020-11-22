using Android.App;
using Android.Content;
using Android.Support.V7.App;
using Android.Util;
using System;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class ToolbarHelper
    {
        public const string TAG = "M:ToolbarHelper";

        public static void SetNavigationIcon(Toolbar toolbar, AppCompatActivity activity)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Small:
                        toolbar.SetNavigationIcon(Resource.Drawable.ic_arrow_back_white_18dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Normal:
                        toolbar.SetNavigationIcon(Resource.Drawable.ic_arrow_back_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        toolbar.SetNavigationIcon(Resource.Drawable.ic_arrow_back_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        toolbar.SetNavigationIcon(Resource.Drawable.ic_arrow_back_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetNavigationIcon: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(activity, e, "Setting Navigation Icon", "ToolbarHelper.SetNavigationIcon");
            }
        }

        public static Toolbar SetupToolbar(AppCompatActivity activity, int toolbarId, int toolbarTitle, int textColour)
        {
            Toolbar toolbar = null;
            ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

            try
            {
                toolbar = activity.FindViewById<Toolbar>(toolbarId);

                if (toolbar != null)
                {
                    activity.SetSupportActionBar(toolbar);
                    if (toolbar.Id != Resource.Id.my_toolbar)
                    {
                        activity.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                        activity.SupportActionBar.SetDisplayShowHomeEnabled(true);
                    }

                    if (toolbarTitle != -1)
                    {
                        
                        activity.SupportActionBar.SetTitle(toolbarTitle);
                        toolbar.SetTitleTextColor(textColour);

                        switch (screenSize)
                        {
                            case ConstantsAndTypes.ScreenSize.Normal:
                                toolbar.SetTitleTextAppearance(activity, Resource.Style.MindYourMood_ActivityTheme_TitleTextAppearanceNormal);
                                break;
                            case ConstantsAndTypes.ScreenSize.Large:
                                toolbar.SetTitleTextAppearance(activity, Resource.Style.MindYourMood_ActivityTheme_TitleTextAppearanceLarge);
                                break;
                            case ConstantsAndTypes.ScreenSize.ExtraLarge:
                                toolbar.SetTitleTextAppearance(activity, Resource.Style.MindYourMood_ActivityTheme_TitleTextAppearanceExtraLarge);
                                break;
                        }
                    }
                    else
                    {
                        activity.SupportActionBar.SetDisplayShowTitleEnabled(false);
                    }

                    activity.SupportActionBar.Elevation = 1.0f;

                    if(toolbar.Id != Resource.Id.my_toolbar)
                        SetNavigationIcon(toolbar, activity);

                    toolbar.SetContentInsetsAbsolute(0, 0);
                    toolbar.SetContentInsetsRelative(0, 0);
                }
                return toolbar;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupToolbar: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(activity, e, "Setting up toolbar", "ToolbarHelper.SetupToolbar");

                return null;
            }
        }
    }
}
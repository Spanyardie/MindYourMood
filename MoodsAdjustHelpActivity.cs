using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help;
using Android.Util;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class MoodsAdjustHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:MoodsAdjustHelpActivity";

        private Toolbar _toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MoodsAdjustHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.moodsAdjustHelpToolbar, Resource.String.MoodsAdjustHelpScreenTitle, Color.White);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MoodsAdjustHelpMenu, menu);

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
                    case Resource.Id.MoodsAdjustHelpActionHome:
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
                var itemHome = menu.FindItem(Resource.Id.MoodsAdjustHelpActionHome);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "MoodsAdjustHelpActivity.SetActionIcons");
            }
        }
    }
}
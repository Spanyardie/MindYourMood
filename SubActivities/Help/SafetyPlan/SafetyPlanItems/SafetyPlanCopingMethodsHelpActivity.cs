using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using Android.Widget;

namespace com.spanyardie.MindYourMood.SubActivities.Help.SafetyPlan.SafetyPlanItems
{
    [Activity()]
    public class SafetyPlanCopingMethodsHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:SafetyPlanCopingMethodsHelpActivity";

        private Toolbar _toolbar;

        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SafetyPlanCopingMethodsHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.safetyPLanCopingMethodsHelpToolbar, Resource.String.CopingMethodsHelpTitle, Color.White);

            _done = FindViewById<Button>(Resource.Id.btnDone);
            if (_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.CopingMethodsHelpMenu, menu);

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
                    case Resource.Id.safetyPlanCopingMethodsHelpActionHome:
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
                var itemHome = menu.FindItem(Resource.Id.safetyPlanCopingMethodsHelpActionHome);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SafetyPlanCopingMethodsHelpActivity.SetActionIcons");
            }
        }
    }
}
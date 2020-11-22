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

namespace com.spanyardie.MindYourMood.SubActivities.Help.Resources
{
    [Activity()]
    public class ResourcesAppointmentPlannerHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:ResourcesAppointmentPlannerHelpActivity";

        private Toolbar _toolbar;

        private LinearLayout _appointmentContainer;
        private ImageView _appointmentImage;
        private TextView _appointmentText;

        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ResourcesAppointmentPlannerHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.resourcesAppointmentPlannerHelpToolbar, Resource.String.AppointmentPlannerHelpTitle, Color.White);

            GetFieldComponents();
            SetupCallbacks();
        }

        private void SetupCallbacks()
        {
            if(_appointmentContainer != null)
                _appointmentContainer.Click += AppointmentContainer_Click;
            if(_appointmentImage != null)
                _appointmentImage.Click += AppointmentContainer_Click;
            if(_appointmentText != null)
                _appointmentText.Click += AppointmentContainer_Click;
            if (_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void AppointmentContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ResourcesAppointmentHelpActivity));
            StartActivity(intent);
        }

        private void GetFieldComponents()
        {
            try
            {
                _appointmentContainer = FindViewById<LinearLayout>(Resource.Id.linAppointmentContainer);
                _appointmentImage = FindViewById<ImageView>(Resource.Id.imgAppointmentImage);
                _appointmentText = FindViewById<TextView>(Resource.Id.txtAppointmentText);
                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "ResourcesAppointmentPlannerHelpActivity.GetFieldComponents");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.AppointmentPlannerHelpMenu, menu);

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
                    case Resource.Id.appointmentPlannerHelpActionHome:
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
                var itemHome = menu.FindItem(Resource.Id.appointmentPlannerHelpActionHome);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ResourcesAppointmentPlannerHelpActivity.SetActionIcons");
            }
        }
    }
}
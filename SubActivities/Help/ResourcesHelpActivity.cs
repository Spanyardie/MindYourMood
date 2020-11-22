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
using com.spanyardie.MindYourMood.SubActivities.Help.Resources;

namespace com.spanyardie.MindYourMood.SubActivities.Help
{
    [Activity()]
    public class ResourcesHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:ResourcesHelpActivity";

        private Toolbar _toolbar;

        private LinearLayout _resourcesMedicationContainer;
        private ImageView _resourcesMedicationImage;
        private TextView _resourcesMedicationText;

        private LinearLayout _resourcesConditionsContainer;
        private ImageView _resourcesConditionsImage;
        private TextView _resourcesConditionsText;

        private LinearLayout _resourcesStrategiesContainer;
        private ImageView _resourcesStrategiesImage;
        private TextView _resourcesStrategiesText;

        private LinearLayout _resourcesAppointmentPlannerContainer;
        private ImageView _resourcesAppointmentPlannerImage;
        private TextView _resourcesAppointmentPlannerText;

        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ResourcesHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.resourcesHelpToolbar, Resource.String.ResourcesHelpTitle, Color.White);

            GetFieldComponents();
            SetupCallbacks();
        }

        private void SetupCallbacks()
        {
            if(_resourcesMedicationContainer != null)
                _resourcesMedicationContainer.Click += ResourcesMedicationContainer_Click;
            if(_resourcesMedicationImage != null)
                _resourcesMedicationImage.Click += ResourcesMedicationContainer_Click;
            if(_resourcesMedicationText != null)
                _resourcesMedicationText.Click += ResourcesMedicationContainer_Click;

            if (_resourcesConditionsContainer != null)
                _resourcesConditionsContainer.Click += ResourcesConditionsContainer_Click;
            if (_resourcesConditionsImage != null)
                _resourcesConditionsImage.Click += ResourcesConditionsContainer_Click;
            if (_resourcesConditionsText != null)
                _resourcesConditionsText.Click += ResourcesConditionsContainer_Click;

            if (_resourcesStrategiesContainer != null)
                _resourcesStrategiesContainer.Click += ResourcesStrategiesContainer_Click;
            if (_resourcesStrategiesImage != null)
                _resourcesStrategiesImage.Click += ResourcesStrategiesContainer_Click;
            if (_resourcesStrategiesText != null)
                _resourcesStrategiesText.Click += ResourcesStrategiesContainer_Click;

            if (_resourcesAppointmentPlannerContainer != null)
                _resourcesAppointmentPlannerContainer.Click += ResourcesAppointmentPlannerContainer_Click;
            if (_resourcesAppointmentPlannerImage != null)
                _resourcesAppointmentPlannerImage.Click += ResourcesAppointmentPlannerContainer_Click;
            if (_resourcesAppointmentPlannerText != null)
                _resourcesAppointmentPlannerText.Click += ResourcesAppointmentPlannerContainer_Click;
            if (_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void ResourcesAppointmentPlannerContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ResourcesAppointmentPlannerHelpActivity));
            StartActivity(intent);
        }

        private void ResourcesStrategiesContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ResourcesStrategiesHelpActivity));
            StartActivity(intent);
        }

        private void ResourcesConditionsContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ResourcesConditionsHelpActivity));
            StartActivity(intent);
        }

        private void ResourcesMedicationContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ResourcesMedicationHelpActivity));
            StartActivity(intent);
        }

        private void GetFieldComponents()
        {
            try
            {
                _resourcesMedicationContainer = FindViewById<LinearLayout>(Resource.Id.linResourcesMedicationHelp);
                _resourcesConditionsContainer = FindViewById<LinearLayout>(Resource.Id.linResourcesConditionsHelp);
                _resourcesStrategiesContainer = FindViewById<LinearLayout>(Resource.Id.linResourcesStrategiesHelp);
                _resourcesAppointmentPlannerContainer = FindViewById<LinearLayout>(Resource.Id.linResourcesAppointmentPlannerHelp);

                _resourcesMedicationImage = FindViewById<ImageView>(Resource.Id.imgResourcesMedicationHelp);
                _resourcesConditionsImage = FindViewById<ImageView>(Resource.Id.imgResourcesConditionsHelp);
                _resourcesStrategiesImage = FindViewById<ImageView>(Resource.Id.imgResourcesStrategiesHelp);
                _resourcesAppointmentPlannerImage = FindViewById<ImageView>(Resource.Id.imgResourcesAppointmentPlannerHelp);

                _resourcesMedicationText = FindViewById<TextView>(Resource.Id.txtResourcesMedicationHelp);
                _resourcesConditionsText = FindViewById<TextView>(Resource.Id.txtResourcesConditionsHelp);
                _resourcesStrategiesText = FindViewById<TextView>(Resource.Id.txtResourcesStrategiesHelp);
                _resourcesAppointmentPlannerText = FindViewById<TextView>(Resource.Id.txtResourcesAppointmentPlannerHelp);
                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "ResourcesHelpActivity.GetFieldComponents");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ResourcesHelpMenu, menu);

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
                    case Resource.Id.resourcesHelpActionHome:
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
                var itemHome = menu.FindItem(Resource.Id.resourcesHelpActionHome);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ResourcesHelpActivity.SetActionIcons");
            }
        }
    }
}
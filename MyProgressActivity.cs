using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using MikePhil.Charting.Charts;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.ThoughtsHelp;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class MyProgressActivity : AppCompatActivity
    {
        public static string TAG = "M:MyProgressActivity";

        private LineChart _progressChart;
        private LinearLayout _progressLineChartContainer;
        private ProgressChartHelper _chartHelper;

        private Toolbar _toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.MyProgressLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.myProgressToolbar, Resource.String.MyProgressToolbarTitle, Color.White);

                _chartHelper = new ProgressChartHelper(this, _progressLineChartContainer, _progressChart);

                _progressLineChartContainer = _chartHelper.SetupLineChart();

                var startDate = Intent.Extras.GetString("StartDate");
                Log.Info(TAG, "OnCreate: Start date for chart data - " + startDate);
                var endDate = Intent.Extras.GetString("EndDate");
                Log.Info(TAG, "OnCreate: End date for chart data - " + endDate);
                var actualStartDate = Convert.ToDateTime(startDate);
                var actualEndDate = Convert.ToDateTime(endDate);

                _progressChart = _chartHelper.SetupChartData(actualStartDate, actualEndDate);

                Log.Info(TAG, "OnCreate: Successful completion");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Creating Activity", "MyProgressActivity.OnCreate");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _progressLineChartContainer = FindViewById<LinearLayout>(Resource.Id.linProgressLineChart);
                Log.Info(TAG, "GetFieldComponents: Successfully retrieved specified components");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "MyProgressActivity.GetFieldComponents");
            }
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ThoughtRecordsActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ShowProgressMenu, menu);

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
                if (item.ItemId == Resource.Id.myProgressActionHelp)
                {
                    Intent intent = new Intent(this, typeof(ViewProgressHelpActivity));
                    StartActivity(intent);
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
                var itemHelp = menu.FindItem(Resource.Id.myProgressActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "MedicationListActivity.SetActionIcons");
            }
        }
    }
}
using System;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Widget;
using com.spanyardie.MindYourMood.Helpers;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Content;
using Android.Support.V7.App;
using Android.Views;
using UniversalImageLoader.Core;

namespace com.spanyardie.MindYourMood.SubActivities.Help
{
    [Activity()]
    public class MainHelpActivity : AppCompatActivity
    {
        public const string TAG = "M:MainHelpActivity";

        private Toolbar _toolbar;

        private ImageButton _thoughtButton;
        private ImageButton _achievementChartButton;
        private ImageButton _safetyButton;
        private ImageButton _activitiesButton;
        private ImageButton _treatmentPlanButton;
        private ImageButton _personalMediaButton;
        private ImageButton _myResourcesButton;
        private ImageButton _summaryButton;

        private TextView _thoughtRecords;
        private TextView _chuffChart;
        private TextView _safety;
        private TextView _activities;
        private TextView _treatmentPlan;
        private TextView _personalMedia;
        private TextView _resources;
        private TextView _summary;

        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.MainHelpLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.mainHelpToolbar, Resource.String.MainHelpTitle, Color.White);

                GetFieldComponents();
                SetupCallbacks();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateMainActivity), "MainHelpActivity.OnCreate");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_thoughtButton != null)
                    _thoughtButton.Click += ThoughtButton_Click;
                if (_achievementChartButton != null)
                    _achievementChartButton.Click += AchievementChart_Click;
                if (_safetyButton != null)
                    _safetyButton.Click += Safety_Click;
                if (_activitiesButton != null)
                    _activitiesButton.Click += Activities_Click;
                if (_treatmentPlanButton != null)
                    _treatmentPlanButton.Click += TreatmentPlan_Click;
                if (_personalMediaButton != null)
                    _personalMediaButton.Click += PersonalMedia_Click;
                if (_myResourcesButton != null)
                    _myResourcesButton.Click += MyResources_Click;
                if (_summaryButton != null)
                    _summaryButton.Click += SummaryButton_Click;

                if (_thoughtRecords != null)
                    _thoughtRecords.Click += ThoughtButton_Click;
                if (_chuffChart != null)
                    _chuffChart.Click += AchievementChart_Click;
                if (_safety != null)
                    _safety.Click += Safety_Click;
                if (_activities != null)
                    _activities.Click += Activities_Click;
                if (_treatmentPlan != null)
                    _treatmentPlan.Click += TreatmentPlan_Click;
                if (_personalMedia != null)
                    _personalMedia.Click += PersonalMedia_Click;
                if (_resources != null)
                    _resources.Click += MyResources_Click;
                if (_summary != null)
                    _summary.Click += SummaryButton_Click;
                if (_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMainSetupCallbacks), "MainHelpActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
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
            }
            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            _thoughtButton = FindViewById<ImageButton>(Resource.Id.imgHelpThoughtRecords);
            _achievementChartButton = FindViewById<ImageButton>(Resource.Id.imgHelpChuffChart);
            _safetyButton = FindViewById<ImageButton>(Resource.Id.imgHelpSafety);
            _activitiesButton = FindViewById<ImageButton>(Resource.Id.imgHelpActivities);
            _treatmentPlanButton = FindViewById<ImageButton>(Resource.Id.imgHelpTreatmentPlan);
            _personalMediaButton = FindViewById<ImageButton>(Resource.Id.imgHelpPersonalMedia);
            _myResourcesButton = FindViewById<ImageButton>(Resource.Id.imgHelpResources);
            _summaryButton = FindViewById<ImageButton>(Resource.Id.imgbtnHelpSummary);

            _thoughtRecords = FindViewById<TextView>(Resource.Id.txtHelpThoughtRecords);
            _chuffChart = FindViewById<TextView>(Resource.Id.txtHelpChuffChart);
            _safety = FindViewById<TextView>(Resource.Id.txtHelpSafety);
            _activities = FindViewById<TextView>(Resource.Id.txtHelpActivities);
            _treatmentPlan = FindViewById<TextView>(Resource.Id.txtHelpTreatmentPlan);
            _personalMedia = FindViewById<TextView>(Resource.Id.txtHelpPersonalMedia);
            _resources = FindViewById<TextView>(Resource.Id.txtHelpResources);
            _summary = FindViewById<TextView>(Resource.Id.txtHelpSummary);
            _done = FindViewById<Button>(Resource.Id.btnDone);
        }

        private void SummaryButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SummaryHelpActivity));
            StartActivity(intent);
        }

        private void MyResources_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ResourcesHelpActivity));
            StartActivity(intent);
        }

        private void PersonalMedia_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MediaHelpActivity));
            StartActivity(intent);
        }

        private void TreatmentPlan_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TreatmentHelpActivity));
            StartActivity(intent);
        }

        private void Activities_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ActivitiesHelpActivity));
            StartActivity(intent);
        }

        private void Safety_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SafetyHelpActivity));
            StartActivity(intent);
        }

        private void AchievementChart_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(AchievementsHelpActivity));
            StartActivity(intent);
        }

        private void ThoughtButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ThoughtsHelpActivity));
            StartActivity(intent);
        }
    }
}
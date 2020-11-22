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
using AppCompatActivity = Android.Support.V7.App.AppCompatActivity;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using MindYourMood.Helpers;
using Android.Util;

namespace MindYourMood
{
    [Activity(Label = "Weekly Planner")]
    public class WeeklyPlannerActivity : AppCompatActivity
    {
        public const string TAG = "M:WeeklyPlannerActivity";

        private Toolbar _toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.WeeklyPlannerLayout);

                GetFieldComponents();

                SetSupportActionBar(_toolbar);
                SupportActionBar.SetTitle(Resource.String.WeeklyPlannerActionBarTitle);

                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateWeeklyPlannerActivity), "WeeklyPlannerActivity.OnCreate");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _toolbar = (Toolbar)FindViewById(Resource.Id.weeklyPlannerToolbar);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorWeeklyPlannerGetComponents), "WeeklyPlannerActivity.GetFieldComponents");
            }
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
    }
}
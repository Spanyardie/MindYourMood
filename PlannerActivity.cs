using System;

using Android.App;
using Android.OS;
using MindYourMood.Helpers;
using Android.Util;

namespace MindYourMood
{
    [Activity(Label = "Planner")]
    public class PlannerActivity : Activity
    {
        public const string TAG = "M:PlannerActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.PlannerLayout);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatePlannerActivity), "PlannerActivity.OnCreate");
            }
        }
    }
}
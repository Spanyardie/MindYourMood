using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Util;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace MindYourMood.SubActivities.SafetyPlan
{
    [Activity(Label = "PeopleCallActivity")]
    public class PeopleCallActivity : AppCompatActivity
    {
        public static string TAG = "M:PeopleCallActivity";

        private Toolbar _toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.PeopleCallLayout);

                GetFieldComponents();

                SetSupportActionBar(_toolbar);
                SupportActionBar.SetTitle(Resource.String.safetyPlanWhoCanIRingActivityTitle);

                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);

                SetupCallbacks();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
            }
        }

        private void SetupCallbacks()
        {

        }

        private void GetFieldComponents()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.PeopleCallToolbar);
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
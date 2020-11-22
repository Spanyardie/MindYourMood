using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Util;

namespace com.spanyardie.MindYourMood.SubActivities.Resources
{
    [Activity]
    public class ResourcesStrategiesActivity : AppCompatActivity
    {
        public const string TAG = "M:ResourcesStrategiesActivity";

        private Toolbar _toolbar;
        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ResourceStrategies);

            GetFieldComponents();
            SetupCallbacks();

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.resourcesStrategiesToolbar, Resource.String.ResourcesStrategiesActionBarTitle, Color.White);
        }

        private void GetFieldComponents()
        {
            _done = FindViewById<Button>(Resource.Id.btnDone);
        }

        private void SetupCallbacks()
        {
            if(_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                if (item != null)
                {
                    if (item.ItemId == Android.Resource.Id.Home)
                    {
                        Finish();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnOptionsItemSelected: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Processing selected option", "ResourcesStrategiesActivity.OnOptionsItemSelected");
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}
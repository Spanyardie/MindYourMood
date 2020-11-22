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
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Util;

namespace com.spanyardie.MindYourMood.SubActivities.Resources
{
    [Activity]
    public class ResourcesUsefulServicesActivity : AppCompatActivity
    {
        public const string TAG = "M:ResourcesUsefulServicesActivity";

        private Toolbar _toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ResourceUsefulServices);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.resourcesUsefulServicesToolbar, Resource.String.ResourcesUsefulServicesActionBarTitle, Color.White);
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Processing selected option", "ResourcesUsefulServicesActivity.OnOptionsItemSelected");
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}
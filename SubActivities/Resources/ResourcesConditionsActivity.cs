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
using com.spanyardie.MindYourMood.Adapters;

namespace com.spanyardie.MindYourMood.SubActivities.Resources
{
    [Activity]
    public class ResourcesConditionsActivity : AppCompatActivity
    {
        public const string TAG = "M:ResourcesConditionsActivity";

        private Toolbar _toolbar;
        private ListView _conditionList;
        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ResourceConditions);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.resourcesConditionsToolbar, Resource.String.ResourcesConditionsActionBarTitle, Color.White);

            GetFieldComponents();
            SetupCallbacks();
            UpdateAdapter();
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Processing selected option", "ResourcesConditionsActivity.OnOptionsItemSelected");
            }
            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            try
            {
                _conditionList = FindViewById<ListView>(Resource.Id.lstConditions);
                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "ResourceConditionsActivity.GetFieldComponents");
            }
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

        private void UpdateAdapter()
        {
            ResourceConditionListAdapter adapter = new ResourceConditionListAdapter(this);

            if (_conditionList != null)
                _conditionList.Adapter = adapter;
        }
    }
}
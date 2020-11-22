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
using com.spanyardie.MindYourMood.Adapters;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Util;

namespace com.spanyardie.MindYourMood.SubActivities.Resources
{
    [Activity]
    public class ResourcesMedicationActivity : AppCompatActivity
    {
        public const string TAG = "M:ResourcesMedicationActivity";

        private Toolbar _toolbar;
        private ListView _typeList;
        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ResourceMedication);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.resourcesMedicationToolbar, Resource.String.MedicationToolbarTitle, Color.White);

            GetFieldComponents();
            SetupCallbacks();
            UpdateAdapter();
        }

        private void UpdateAdapter()
        {
            ResourceMedicationTypeAdapter adapter = new ResourceMedicationTypeAdapter(this);
            if (_typeList != null)
                _typeList.Adapter = adapter;
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Processing selected option", "ResourcesMedicationActivity.OnOptionsItemSelected");
            }
            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            _typeList = FindViewById<ListView>(Resource.Id.lstMedicationTypeList);
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
    }
}
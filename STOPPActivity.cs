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
    [Activity(Label = "STOPP")]
    public class STOPPActivity : AppCompatActivity
    {
        public const string TAG = "M:STOPPActivity";

        private Toolbar _toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.STOPPLayout);

                _toolbar = (Toolbar)FindViewById(Resource.Id.stoppToolbar);
                SetSupportActionBar(_toolbar);
                SupportActionBar.SetTitle(Resource.String.STOPPActionBarTitle);

                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateSTOPPActivity), "STOPPActivity.OnCreate");
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
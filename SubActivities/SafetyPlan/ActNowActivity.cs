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
using Android.Util;
using AppCompatActivity = Android.Support.V7.App.AppCompatActivity;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace MindYourMood.SubActivities.SafetyPlan
{
    [Activity(Label = "ActNowActivity")]
    public class ActNowActivity : AppCompatActivity
    {
        public static string TAG = "M:ActNowActivity";

        private Toolbar _toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            GetFieldComponents();

            SetSupportActionBar(_toolbar);
            //SupportActionBar.SetTitle(Resource.String.saf);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            SetupCallbacks();
        }

        private void SetupCallbacks()
        {
            
        }

        private void GetFieldComponents()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.actNowToolbar);
        }
    }
}
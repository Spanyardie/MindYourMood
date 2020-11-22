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

namespace MindYourMood
{
    [Activity(Label = "Enter Activities")]
    public class EnterActivitiesActivity : AppCompatActivity
    {
        private Toolbar _toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EnterActivitiesLayout);

            _toolbar = (Toolbar)FindViewById(Resource.Id.enterActivitiesToolbar);
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetTitle(Resource.String.EnterActivitiesActionBarTitle);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            //Spinner spinner = FindViewById<Spinner>(Resource.Id.spnMood);

            //spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            ////var adapter = ArrayAdapter.CreateFromResource(
            ////        this, Resource.Array.mood_list, Android.Resource.Layout.SimpleSpinnerItem);

            //adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            //spinner.Adapter = adapter;
        }
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;

            string toast = string.Format("The Mood is {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
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
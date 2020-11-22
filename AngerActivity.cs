
using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using Android.Graphics;
using System;
using Android.Widget;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class AngerActivity : AppCompatActivity
    {
        public const string TAG = "M:AngerActivity";

        private Toolbar _toolbar;
        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.AngerLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.angerToolbar, Resource.String.AngerActionBarTitle, Color.White);

                GetFieldComponents();
                SetupCallbacks();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateAngerActivity), "AngerActivity.OnCreate");
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
    }
}
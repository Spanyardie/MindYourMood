
using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Java.Lang;
using Android.Util;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using System;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class AnxietyActivity : AppCompatActivity
    {
        public const string TAG = "M:AnxietyActivity";

        private Toolbar _toolbar;
        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.AnxietyLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.anxietyToolbar, Resource.String.AnxietyActionBarTitle, Color.White);
                GetFieldComponents();
                SetupCallbacks();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateAnxietyActivity), "AnxietyActivity.OnCreate");
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

        private void GetFieldComponents()
        {
            _done = FindViewById<Button>(Resource.Id.btnDone);
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
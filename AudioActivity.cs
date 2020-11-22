
using Android.App;
using Android.OS;
using Android.Views;
using AppCompatActivity = Android.Support.V7.App.AppCompatActivity;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using MindYourMood.Helpers;
using Android.Util;
using Java.Lang;

namespace MindYourMood
{
    [Activity(Label = "Audio")]
    public class AudioActivity : AppCompatActivity
    {
        public const string TAG = "M:AudioActivity";

        private Toolbar _toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.AudioLayout);

                _toolbar = (Toolbar)FindViewById(Resource.Id.audioToolbar);
                SetSupportActionBar(_toolbar);
                SupportActionBar.SetTitle(Resource.String.AudioActionBarTitle);

                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateAudioActivity), "AudioActivity.OnCreate");
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
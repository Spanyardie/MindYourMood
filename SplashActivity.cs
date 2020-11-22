
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using System.Threading.Tasks;

namespace com.spanyardie.MindYourMood
{
    [Activity(Label = "Mind Your Mood", MainLauncher = true, Theme = "@style/MindYourMood.Splash", Icon = "@drawable/ic_launcher", NoHistory = true)]
    public class SplashActivity : Activity
    {
        public const string TAG = "M:SplashActivity";

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Log.Info(TAG, "OnCreate: Splash Activity started");

            await Task.Factory.StartNew(async () =>
           {
               Log.Info(TAG, "OnCreate: Awaiting task 2000");
               await Task.Delay(2000);
               Log.Info(TAG, "OnCreate: Starting MainActivity");
               StartActivity(new Intent(Application.Context, typeof(MainActivity)));
           });
        }
    }
}
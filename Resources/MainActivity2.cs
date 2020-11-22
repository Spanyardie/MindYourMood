using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Util;

namespace JazzyViewPager
{
    [Activity(Label = "JazzyViewPager", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity2 : AppCompatActivity
    {
        private int _pageSelected = -1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main2);

            var viewPager = FindViewById<ViewPager>(Resource.Id.vp_main);
            if (viewPager != null)
            {
                viewPager.Adapter = new MainPagerAdapter(SupportFragmentManager);
                viewPager.OffscreenPageLimit = 2;
            }
        }
    }
}


using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Com.Gigamole.Infinitecycleviewpager;
using com.spanyardie.MindYourMood.Adapters;


namespace com.spanyardie.MindYourMood.Helpers
{
    public class SafetyMainHorizontalPagerFragment : Fragment
    {
        private int _pageSelected = -1;

        private HorizontalInfiniteCycleViewPager _horizontalInfiniteCycleViewPager;
        public SafetyMainHorizontalPagerFragment() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.HorizontalFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _horizontalInfiniteCycleViewPager = view.FindViewById<HorizontalInfiniteCycleViewPager>(Resource.Id.hicvp);
            _horizontalInfiniteCycleViewPager.Adapter = new SafetyMainHorizontalPagerAdapter(this, Context);

            _horizontalInfiniteCycleViewPager.PageSelected += HorizontalInfiniteCycleViewPager_PageSelected;
        }

        private void HorizontalInfiniteCycleViewPager_PageSelected(object sender, Android.Support.V4.View.ViewPager.PageSelectedEventArgs e)
        {
            _pageSelected = _horizontalInfiniteCycleViewPager.RealItem;
        }

        public int GetPageSelected()
        {
            return _pageSelected;
        }
    }
}
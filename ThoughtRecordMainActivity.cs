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
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.View;
using Android.Util;
using JazzyViewPager.helpers;

namespace JazzyViewPager
{
    [Activity]
    public class ThoughtRecordMainActivity : AppCompatActivity, ViewPager.IOnPageChangeListener
    {
        public const string TAG = "M:ThoughtRecordMainActivity";

        private ViewPager _viewPager;
        private PagerTitleStrip _titleStrip;
        private Button _continue;

        private ThoughtRecordSituationPagerAdapter.SituationView _currentView = ThoughtRecordSituationPagerAdapter.SituationView.SituationWhat;

        private const int FIRST_PAGE = 0;
        private const int SECOND_PAGE = 1;
        private const int THIRD_PAGE = 2;
        private const int LAST_PAGE = 3;

        private int _currentPage = -1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ThoughtRecordMain);

            GetFieldComponents();

            SetupCallbacks();

            UpdateAdapter();
        }

        private void SetupCallbacks()
        {
            if(_continue != null)
                _continue.Click += Continue_Click;

            _viewPager.Click += ViewPager_Click;

            _viewPager.SetOnPageChangeListener(this);
        }

        private void ViewPager_Click(object sender, EventArgs e)
        {
            Log.Info("asasas", "In Click " + _viewPager.CurrentItem);
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            //Take me to the next step
        }

        private void GetFieldComponents()
        {
            _viewPager = FindViewById<ViewPager>(Resource.Id.pagerThoughtRecordMain);
            _titleStrip = FindViewById<PagerTitleStrip>(Resource.Id.pagerTitleThoughtRecordMain);
            _continue = FindViewById<Button>(Resource.Id.btnContinue);
        }

        private void UpdateAdapter()
        {
            ThoughtRecordSituationPagerAdapter adapter = new ThoughtRecordSituationPagerAdapter(this);

            _viewPager.Adapter = adapter;
        }

        public void OnPageScrollStateChanged(int state)
        {
            
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            
        }

        public void OnPageSelected(int position)
        {
            int picResource = -1;
            if (_continue != null)
            {
                switch (position)
                {
                    case FIRST_PAGE:
                        _continue.Visibility = ViewStates.Invisible;
                        picResource = Resource.Drawable.whatsituation;
                        break;
                    case SECOND_PAGE:
                        _continue.Visibility = ViewStates.Invisible;
                        picResource = Resource.Drawable.whosituation;
                        break;
                    case THIRD_PAGE:
                        _continue.Visibility = ViewStates.Invisible;
                        picResource = Resource.Drawable.wheresituation;
                        break;
                    case LAST_PAGE:
                        _continue.Visibility = ViewStates.Visible;
                        picResource = Resource.Drawable.whensituation;
                        break;
                }

                if(picResource != -1)
                {
                    if(_viewPager != null)
                    {
                        _viewPager.SetBackgroundResource(picResource);
                    }
                }
            }

            _currentPage = position;
        }
    }
}
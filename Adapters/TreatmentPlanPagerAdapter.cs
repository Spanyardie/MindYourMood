﻿using Android.Support.V4.App;
using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class TreatmentPlanPagerAdapter : FragmentStatePagerAdapter
    {

        private static int COUNT = 1;

        private const int HORIZONTAL = 0;

        public TreatmentPlanPagerAdapter(FragmentManager fm) : base(fm) { }

        public override int Count
        {
            get
            {
                return COUNT;
            }
        }

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case HORIZONTAL:
                default:
                    return new TreatmentPlanHorizontalPagerFragment();
            }
        }
    }
}
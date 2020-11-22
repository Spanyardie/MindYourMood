using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Support.V4.View;
using Android.Util;

namespace com.spanyardie.MindYourMood
{
    public class NonScrollableViewPager : ViewPager
    {
        public NonScrollableViewPager(Context context) : base(context)
        {

        }

        public NonScrollableViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public NonScrollableViewPager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return false;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return false;
        }
    }
}
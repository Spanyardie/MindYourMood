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
using Android.Views.Animations;
using Android.Util;

namespace MindYourMood.Helpers
{
    public class SlidingInterpolator : LinearLayout
    {
        public static string TAG = "M:SlidingInterpolator";

        public enum InterpolatorType
        {
            AccelerateDecelerate = 0,
            Accelerate,
            Anticipate,
            AnticipateOvershoot,
            Bounce,
            Cycle,
            Decelerate,
            Linear,
            Overshoot
        }

        public InterpolatorType Type { get; set; }

        public enum AnimationTime
        {
            OmgThatsTooLong = 0,
            ExtraLong,
            DoubleLong,
            Long,
            Medium,
            Short
        }

        public AnimationTime Duration { get; set; }

        public struct Deltas
        {
            public float XDelta;
            public float YDelta;
        }

        public Deltas DeltaXY { get; set; }

        private View _animatedView;
        private ViewPropertyAnimator _animator;

        public SlidingInterpolator(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
        {

            if (view == null)
                throw new Exception("SlidingInterpolator requires a View in its constructor");

            _animatedView = view;
            _animator = view.Animate();
        }

        public void Interpolate()
        {
            if(_animatedView != null)
            {

                switch(Type)
                {
                    case InterpolatorType.Accelerate:
                        Accelerate();
                        break;
                    case InterpolatorType.AccelerateDecelerate:
                        AccelerateDecelerate();
                        break;
                    case InterpolatorType.Anticipate:
                        Anticipate();
                        break;
                    case InterpolatorType.AnticipateOvershoot:
                        AnticipateOvershoot();
                        break;
                    case InterpolatorType.Bounce:
                        Bounce();
                        break;
                    case InterpolatorType.Cycle:
                        Cycle();
                        break;
                    case InterpolatorType.Decelerate:
                        Decelerate();
                        break;
                    case InterpolatorType.Linear:
                        Linear();
                        break;
                    case InterpolatorType.Overshoot:
                        Overshoot();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Overshoot()
        {
            try
            {
                if (_animatedView != null)
                {
                    var interpolator = new OvershootInterpolator();

                    _animator.TranslationY(DeltaXY.YDelta);
                    _animator.SetDuration(GetResourceAnimationTimeValue()); //duration
                    _animator.SetInterpolator(interpolator);

                    _animator.Start();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Overshoot: Exception - " + e.Message);
            }
        }

        private void Linear()
        {
            throw new NotImplementedException();
        }

        private void Decelerate()
        {
            throw new NotImplementedException();
        }

        private void Cycle()
        {
            throw new NotImplementedException();
        }

        private void Bounce()
        {
            throw new NotImplementedException();
        }

        private void AnticipateOvershoot()
        {
            throw new NotImplementedException();
        }

        private void Anticipate()
        {
            throw new NotImplementedException();
        }

        private void AccelerateDecelerate()
        {
            throw new NotImplementedException();
        }

        private void Accelerate()
        {
            throw new NotImplementedException();
        }

        private long GetResourceAnimationTimeValue()
        {
            string animTimeText = "";
            bool useSystemTimeValue = true;
            var timeValue = 0;

            try
            {
                if (_animatedView != null)
                {
                    var resourceId = -1;
                    switch (Duration)
                    {
                        case AnimationTime.DoubleLong:
                            resourceId = 1000;
                            animTimeText = "DoubleLong";
                            useSystemTimeValue = false;
                            break;
                        case AnimationTime.ExtraLong:
                            resourceId = 2000;
                            animTimeText = "ExtraLong";
                            useSystemTimeValue = false;
                            break;
                        case AnimationTime.OmgThatsTooLong:
                            resourceId = 3000;
                            animTimeText = "OmgThatsTooLong";
                            useSystemTimeValue = false;
                            break;
                        case AnimationTime.Long:
                            resourceId = Android.Resource.Integer.ConfigLongAnimTime;
                            animTimeText = "Long";
                            break;

                        case AnimationTime.Medium:
                            resourceId = Android.Resource.Integer.ConfigMediumAnimTime;
                            animTimeText = "Medium";
                            break;

                        case AnimationTime.Short:   //defaults to short
                        default:
                            animTimeText = "Short";
                            resourceId = Android.Resource.Integer.ConfigShortAnimTime;
                            break;
                    }

                    if (useSystemTimeValue)
                    {
                        timeValue = _animatedView.Context.Resources.GetInteger(resourceId);
                        Log.Info(TAG, "GetResourceAnimationTimeValue: Config Integer value - " + animTimeText + " (" + resourceId.ToString() + ")");
                    }
                    else
                    {
                        timeValue = resourceId;
                        Log.Info(TAG, "GetResourceAnimationTimeValue: Application defined Integer value - " + animTimeText + " (" + resourceId.ToString() + ")");
                    }

                    Log.Info(TAG, "GetResourceAnimationTimeValue: Duration - " + timeValue.ToString());
                    return timeValue;
                }
                return -1;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetResourceAnimationTimeValue: Error occurred getting Resource for duration - " + e.Message);
                return -1;
            }
        }
    }
}
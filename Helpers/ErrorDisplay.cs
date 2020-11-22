using System;
using Android.Util;
using Android.App;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class ErrorDisplay
    {
        public static string TAG = "M:ErrorDisplay";

        public static void ShowErrorAlert(Activity activity, Exception exception, string wasDoingText, string activityAndFunction)
        {
            if(exception == null)
            {
                Log.Error(TAG, "ShowErrorAlert: Error - Exception not supplied");
                return;
            }

            if(string.IsNullOrEmpty(wasDoingText))
            {
                Log.Error(TAG, "ShowErrorAlert: Error - Was Doing text not supplied");
                return;
            }

            if(string.IsNullOrEmpty(activityAndFunction))
            {
                Log.Error(TAG, "ShowErrorAlert: Error - Activity and Function not supplied");
                return;
            }

            if (activity != null)
            {
                var contactSelector = new ErrorDisplayFragment(activity, exception, wasDoingText, activityAndFunction);

                var fragmentTransaction = activity.FragmentManager.BeginTransaction();
                contactSelector.Show(fragmentTransaction, contactSelector.Tag);
            }
            else
            {
                Log.Error(TAG, "ShowErrorAlert: Error - Activity not supplied");
            }
        }
    }
}
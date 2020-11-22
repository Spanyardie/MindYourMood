using System.Collections.Generic;
using System.Linq;
using System;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Graphics;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;


namespace com.spanyardie.MindYourMood.Wizards
{
    public class MoodItemsAdapter : BaseAdapter
    {
        public const string TAG = "M:MoodItemsAdapter";

        List<Mood> _moodEntries;
        Activity _activity;

        //private int _selectedPosition;

        public MoodItemsAdapter(Activity activity)
        {
            _activity = activity;
            GetMoods();
        }

        private void GetMoods()
        {
            _moodEntries = GlobalData.MoodItems;
        }

        public override int Count
        {
            get
            {
                if (_moodEntries != null)
                {
                    return _moodEntries.Count;
                }
                else
                {
                    _moodEntries = new List<Mood>();
                    return 0;
                }
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _moodEntries.ElementAt(position).MoodsId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.MoodListItem, parent, false);

                TextView moodName = view.FindViewById<TextView>(Resource.Id.txtMoodNameListItem);

                TextView moodRating = view.FindViewById<TextView>(Resource.Id.txtRatingListItem);

                moodName.Text = GetMoodName(_moodEntries.ElementAt(position).MoodListId);
                moodRating.Text = _moodEntries.ElementAt(position).MoodRating.ToString() + "%";

                var parentHeldSelectedItemIndex = ((ThoughtRecordWizardMoodStep)_activity).GetSelectedItem();
                if (position == parentHeldSelectedItemIndex)
                {
                    view.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    moodName.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    moodRating.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                }
                else
                {
                    view.SetBackgroundDrawable(null);
                    moodName.SetBackgroundDrawable(null);
                    moodRating.SetBackgroundDrawable(null);
                }

                return view;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetMoodItemsAdapterView), "MoodItemsAdapter.GetView");
                return null;
            }
        }

        private string GetMoodName(long moodListId)
        {
            try
            {
                string retVal = "";

                foreach (var moodListItem in GlobalData.MoodListItems)
                {
                    if (moodListItem.MoodId == moodListId)
                    {
                        retVal = moodListItem.MoodName.Trim();
                        break;
                    }
                }

                return retVal;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetMoodName: Exception - " + e.Message);
                if(_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGettingMoodName), "MoodItemsAdapter.GetMoodName");
                return "";
            }
        }
    }
}
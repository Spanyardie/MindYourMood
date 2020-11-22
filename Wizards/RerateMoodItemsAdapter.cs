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
    public class RerateMoodItemsAdapter : BaseAdapter
    {
        public const string TAG = "M:RerateMoodItemsAdapter";

        private List<RerateMood> _moodEntries;
        private Activity _activity;

        //private int _selectedPosition;
        private bool _firstTimeInit = false;

        public RerateMoodItemsAdapter(Activity activity)
        {
            _activity = activity;
            GetAllMoods();
        }

        public RerateMoodItemsAdapter(Activity activity, bool firstTimeInit = false)
        {
            _activity = activity;
            _firstTimeInit = firstTimeInit;
            GetAllMoods();
        }

        public void GetAllMoods()
        {
            try
            {
                if (_moodEntries == null)
                    _moodEntries = new List<RerateMood>();

                if (_moodEntries != null)
                {
                    _moodEntries.Clear();
                    //this is a compound list
                    if (_firstTimeInit)
                    {
                        GlobalData.RerateMoodsItems.Clear();
                        foreach (var initialMood in GlobalData.MoodItems)
                        {
                            RerateMood mood = new RerateMood();
                            mood.FromMood = true;
                            mood.MoodListId = initialMood.MoodListId;
                            mood.MoodsId = initialMood.MoodsId;
                            mood.MoodRating = initialMood.MoodRating;
                            mood.ThoughtRecordId = initialMood.ThoughtRecordId;
                            GlobalData.RerateMoodsItems.Add(mood);
                        }
                    }
                    foreach (var mood in GlobalData.RerateMoodsItems)
                    {
                        _moodEntries.Add(mood);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetAllMoods: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGettingAllMoods), "RerateMoodItemsAdapter.GetAllMoods");
            }
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
                    _moodEntries = new List<RerateMood>();
                    return 0;
                }
            }
        }

        public bool FirstTimeInit
        {
            get
            {
                return _firstTimeInit;
            }

            set
            {
                _firstTimeInit = value;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return ((RerateMood)_moodEntries.ElementAt(position)).RerateMoodId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.RerateMoodListItem, parent, false);

                TextView moodName = view.FindViewById<TextView>(Resource.Id.txtRerateMoodNameListItem);

                TextView moodRating = view.FindViewById<TextView>(Resource.Id.txtRerateRatingListItem);

                moodName.Text = GetMoodName(_moodEntries.ElementAt(position).MoodListId);
                moodRating.Text = _moodEntries.ElementAt(position).MoodRating.ToString() + "%";

                var parentHeldSelectedItemIndex = ((ThoughtRecordWizardRerateMoodStep)_activity).GetSelectedItem();
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
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGettingRerateMoodItemsView), "RerateMoodItemsAdapter.GetView");
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
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGettingMoodName), "RerateMoodItemsAdapter.GetMoodName");
                return "";
            }
        }
    }
}
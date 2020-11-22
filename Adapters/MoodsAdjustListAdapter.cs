using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class MoodsAdjustListAdapter : BaseAdapter
    {
        public const string TAG = "M:MoodsAdjustListAdapter";

        Activity _activity;

        private List<MoodList> _moods = null;

        private TextView _moodText;
        private TextView _moodDefault;

        public MoodsAdjustListAdapter(Activity activity)
        {
            _activity = activity;
            _moods = new List<MoodList>();
            GetMoodListData();
        }

        private void GetMoodListData()
        {
            if (GlobalData.MoodListItems != null)
                _moods = GlobalData.MoodListItems;
        }

        public override int Count
        {
            get
            {
                return _moods.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if(_moods != null)
            {
                if(position <= _moods.Count)
                {
                    return _moods[position].MoodId;
                }
            }
            return -1;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if(convertView == null)
                {
                    if(_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.MoodsAdjustListItem, parent, false);
                    }
                    else if(parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.MoodsAdjustListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                if(convertView != null)
                {
                    GetFieldComponents(convertView);

                    if(_moodText != null)
                    {
                        _moodText.Text = _moods[position].MoodName.Trim();
                    }
                    if(_moodDefault != null)
                    {
                        _moodDefault.Text = (_moods[position].IsDefault == "true") ? _activity.GetString(Resource.String.wordDefault) : "";
                    }
                    if (position == ((MoodsAdjustActivity)_activity).GetSelectedItemIndex())
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_moodText != null)
                            _moodText.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_moodDefault != null)
                            _moodDefault.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                    else
                    {
                        convertView.SetBackgroundDrawable(null);
                        if (_moodText != null)
                            _moodText.SetBackgroundDrawable(null);
                        if (_moodDefault != null)
                            _moodDefault.SetBackgroundDrawable(null);
                    }
                }
                else
                {
                    Log.Error(TAG, "GetView: view is NULL!");
                }
                return convertView;
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMoodsAdjustListGetView), "MoodsAdjustListAdapter.GetView");
                return null;
            }
        }

        private void GetFieldComponents(View convertView)
        {
            try
            {
                if(convertView != null)
                {
                    _moodText = convertView.FindViewById<TextView>(Resource.Id.txtMoodListItem);
                    _moodDefault = convertView.FindViewById<TextView>(Resource.Id.txtMoodListDefault);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: view is NULL!");
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMoodsAdjustListGetComponents), "MoodsAdjustListAdapter.GetFieldComponents");
            }
        }

        public MoodList GetItemAtPosition(int position)
        {
            if (_moods != null)
            {
                return _moods[position];
            }
            return null;
        }
    }
}
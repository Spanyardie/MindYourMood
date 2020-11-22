using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using Android.Graphics;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class ActivitiesExpandableListAdapter : BaseExpandableListAdapter
    {
        public const string TAG = "M:ActivitiesExpandableListAdapter";

        private Activity _parent;

        private List<Activities> _activities;

        private TextView _titleTextView;
        private TextView _groupDate;
        private TextView _numberOfActivities;

        private ImageView _hasActivitySet;
        private TextView _childStartTime;
        private TextView _childEndTime;
        private TextView _childSep;
        private TextView _childTextView;

        private LinearLayout _activitySummary;
        private TextView _achievementSummary;
        private TextView _achievementSummaryLabel;
        private TextView _intimacySummary;
        private TextView _intimacySummaryLabel;
        private TextView _pleasureSummary;
        private TextView _pleasureSummaryLabel;

        public ActivitiesExpandableListAdapter(Activity activity)
        {
            _parent = activity;
            GetActivityData();
        }

        private void GetActivityData()
        {
            _activities = GlobalData.ActivitiesForWeek;
        }

        public override int GroupCount
        {
            get
            {
                return _activities.Count;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return null;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return _activities[groupPosition].ActivityTimes.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_parent != null)
                    {
                        convertView = _parent.LayoutInflater.Inflate(Resource.Layout.ActivitiesChildItem, null);
                        Log.Info(TAG, "GetChildView: Inflated Activities Child Item successfully, groupPosition - " + groupPosition.ToString() + ", childPosition - " + childPosition.ToString());
                    }
                    else if(parent != null)
                    {
                        LayoutInflater inflator = (LayoutInflater)_parent.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflator.Inflate(Resource.Layout.ActivitiesChildItem, null);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                if (convertView != null)
                {
                    GetChildFieldComponents(convertView);

                    if (_childTextView != null)
                    {
                        var childItem = _activities[groupPosition].ActivityTimes[childPosition];
                        var activityName = (string.IsNullOrEmpty(childItem.ActivityName) ? _parent.GetString(Resource.String.ActivitiesNoActivity) : childItem.ActivityName);
                        _childTextView.Text = activityName;
                        if(childItem.ActivityTimeID == -1)
                        {
                            _hasActivitySet.Visibility = ViewStates.Invisible;
                            _activitySummary.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            _hasActivitySet.Visibility = ViewStates.Visible;
                            _activitySummary.Visibility = ViewStates.Visible;
                            _achievementSummary.Text = childItem.Achievement.ToString() + "/10";
                            _intimacySummary.Text = childItem.Intimacy.ToString() + "/10";
                            _pleasureSummary.Text = childItem.Pleasure.ToString() + "/10";
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetChildView: _childTextView is NULL!");
                    }
                    string startTime, endTime;
                    StringHelper.ActivityTimeBeginEndForConstant(_activities[groupPosition].ActivityTimes[childPosition].ActivityTime, out startTime, out endTime);
                    if(_childStartTime != null)
                    {
                        _childStartTime.Text = startTime;
                    }
                    else
                    {
                        Log.Error(TAG, "GetChildView: _childStartTime is NULL!");
                    }
                    if(_childEndTime != null)
                    {
                        _childEndTime.Text = endTime;
                    }
                    else
                    {
                        Log.Error(TAG, "GetChildView: _childEndTime is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetChildView: convertView is NULL!");
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "GetChildView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_parent, e, _parent.GetString(Resource.String.ErrorActivityTimeAdapterChildView), "ActivitiesExpandableListAdapter.GetChildView");
            }
            return convertView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return null;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    LayoutInflater inflater = (LayoutInflater)_parent.GetSystemService(Context.LayoutInflaterService);
                    convertView = inflater.Inflate(Resource.Layout.ActivitiesGroupItem, null);
                    Log.Info(TAG, "GetGroupView: Inflated Activities Group Item successfully, groupPosition - " + groupPosition.ToString());
                }

                if (convertView != null)
                {
                    GetGroupFieldComponents(convertView);

                    if (_titleTextView != null)
                    {
                        var mymDay = ConversionHelper.ConvertFromDateTimeDaysToMYMDays(_activities[groupPosition].ActivityDate.DayOfWeek);
                        var stringDay = StringHelper.DayStringForConstant(mymDay);

                        _titleTextView.Text = stringDay;
                    }
                    else
                    {
                        Log.Error(TAG, "GetGroupView: _titleTextView is NULL!");
                    }
                    if(_groupDate != null)
                    {
                        _groupDate.Text = _activities[groupPosition].ActivityDate.ToShortDateString();
                    }
                    else
                    {
                        Log.Error(TAG, "GetGroupView: _groupDate is NULL!");
                    }
                    if(_numberOfActivities != null)
                    {
                        var numActivities = _activities[groupPosition].GetTotalNumberOfActivities().ToString();
                        _numberOfActivities.Text = (_activities[groupPosition].GetTotalNumberOfActivities() == 0 ? _parent.GetString(Resource.String.WordNo) : numActivities) + (_activities[groupPosition].GetTotalNumberOfActivities() == 1 ? " " + _parent.GetString(Resource.String.WordActivity) : " " + _parent.GetString(Resource.String.WordActivities));
                    }
                    else
                    {
                        Log.Error(TAG, "GetGroupView: _numberOfActivities is NULL!");
                    }

                    if(((ActivitiesActivity)_parent).SelectedGroupPosition == groupPosition)
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        _titleTextView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        _titleTextView.SetTextColor(Color.White);
                        _groupDate.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        _groupDate.SetTextColor(Color.White);
                        _numberOfActivities.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        _numberOfActivities.SetTextColor(Color.White);
                    }
                    else
                    {
                        Color color = new Color(21, 116, 211);
                        convertView.SetBackgroundColor(color);
                        _titleTextView.SetBackgroundColor(color);
                        _titleTextView.SetTextColor(Color.White);
                        _groupDate.SetBackgroundColor(color);
                        _groupDate.SetTextColor(Color.White);
                        _numberOfActivities.SetBackgroundColor(color);
                        _numberOfActivities.SetTextColor(Color.White);
                    }
                }
                else
                {
                    Log.Error(TAG, "GetGroupView: convertView is NULL!");
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "GetGroupView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_parent, e, _parent.GetString(Resource.String.ErrorActivityTimeAdapterGroupView), "ActivitiesExpandableListAdapter.GetGroupView");
            }
            return convertView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        private void GetGroupFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _titleTextView = view.FindViewById<TextView>(Resource.Id.txtActivitiesGroupItemGroupHeading);
                    _groupDate = view.FindViewById<TextView>(Resource.Id.txtActivitiesGroupItemDate);
                    _numberOfActivities = view.FindViewById<TextView>(Resource.Id.txtActivitiesGroupItemNumActivities);
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "GetGroupFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_parent, e, "Error getting group field components", "ActivitiesExpandableListAdapter.GetGroupFieldComponents");
            }
        }

        private void GetChildFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _hasActivitySet = view.FindViewById<ImageView>(Resource.Id.imgHasActivitySet);
                    _childTextView = view.FindViewById<TextView>(Resource.Id.txtActivityChildItemText);
                    _childStartTime = view.FindViewById<TextView>(Resource.Id.txtActivitiesChildStartTime);
                    _childEndTime = view.FindViewById<TextView>(Resource.Id.txtActivitiesChildEndTime);
                    _childSep = view.FindViewById<TextView>(Resource.Id.txtActivitiesChildSeparator);

                    _activitySummary = view.FindViewById<LinearLayout>(Resource.Id.linActivitySummary);
                    _achievementSummary = view.FindViewById<TextView>(Resource.Id.txtAchievementSummary);
                    _achievementSummaryLabel = view.FindViewById<TextView>(Resource.Id.txtAchievementSummaryLabel);
                    _intimacySummary = view.FindViewById<TextView>(Resource.Id.txtIntimacySummary);
                    _intimacySummaryLabel = view.FindViewById<TextView>(Resource.Id.txtIntimacySummaryLabel);
                    _pleasureSummary = view.FindViewById<TextView>(Resource.Id.txtPleasureSummary);
                    _pleasureSummaryLabel = view.FindViewById<TextView>(Resource.Id.txtPleasureSummaryLabel);
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "GetChildFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_parent, e, "Error getting child field components", "ActivitiesExpandableListAdapter.GetChildFieldComponents");
            }
        }
    }
}
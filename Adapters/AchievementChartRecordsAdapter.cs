using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Content;
using Android.Graphics;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class AchievementChartRecordsAdapter : BaseAdapter
    {
        public const string TAG = "M:AchievementChartRecordsAdapter";

        List<AchievementChart> _achievementChartRecords;
        Activity _activity;

        private ImageView _typeImage;
        private TextView _typeName;

        public AchievementChartRecordsAdapter(Activity activity)
        {
            _activity = activity;
            GetAllAchievementChartRecordData();
        }

        private void GetAllAchievementChartRecordData()
        {
            try
            {
                _achievementChartRecords = GlobalData.AchievementChartItems;
                if (_achievementChartRecords.Count == 0)
                {
                    AchievementChart noEntries = new AchievementChart();
                    if (_activity != null)
                    {
                        noEntries.Achievement = _activity.GetString(Resource.String.chuffChartNoEntriesText);
                        noEntries.AchievementChartType = AchievementChart.ACHIEVEMENTCHART_TYPE.NothingEntered;
                        _achievementChartRecords.Add(noEntries);
                    }
                }
            }
            catch(Exception e)
            {
                _achievementChartRecords = new List<AchievementChart>();
                Log.Error(TAG, "GetAllAchievementChartRecordData: Exception - " + e.Message);
                if(_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorAchievementAdapterGetData), "AchievementChartRecordsAdapter.GetAllAchievementChartRecordData");
            }
        }

        public override int Count
        {
            get
            {
                if (_achievementChartRecords != null)
                {
                    return _achievementChartRecords.Count;
                }
                return -1;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_achievementChartRecords != null)
            {
                return _achievementChartRecords.ElementAt(position).AchievementId;
            }
            else
            {
                return -1;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            int selectedItemIndex = -1;
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.AchievementChartListItem, parent, false);
                    }
                    else if(parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.AchievementChartListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                _typeName = convertView.FindViewById<TextView>(Resource.Id.txtAchievement);
                _typeImage = convertView.FindViewById<ImageView>(Resource.Id.imgChuffChartType);

                if (_achievementChartRecords != null)
                {
                    if (_typeImage != null)
                    {
                        SetImageForType(_achievementChartRecords[position].AchievementChartType);
                    }

                    if (_typeName != null)
                    {
                        _typeName.Text = _achievementChartRecords[position].Achievement.Trim();
                    }
                }

                if (_activity != null)
                    selectedItemIndex = ((AchievementChartActivity)_activity).GetSelectedItemIndex();

                if (position == selectedItemIndex && convertView != null)
                {
                    convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    _typeName.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    _typeImage.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                }
                else
                {
                    convertView.SetBackgroundDrawable(null);
                    _typeName.SetBackgroundDrawable(null);
                    _typeImage.SetBackgroundDrawable(null);
                }

                return convertView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorAchievementAdapterGetView), "AchievementChartRecordsAdapter.GetView");
                return convertView;
            }
        }

        private void SetImageForType(AchievementChart.ACHIEVEMENTCHART_TYPE position)
        {
            try
            {
                if (_typeImage != null)
                {
                    switch (position)
                    {
                        case AchievementChart.ACHIEVEMENTCHART_TYPE.General:
                            _typeImage.SetImageResource(Resource.Drawable.general);
                            break;

                        case AchievementChart.ACHIEVEMENTCHART_TYPE.Life:
                            _typeImage.SetImageResource(Resource.Drawable.life);
                            break;

                        case AchievementChart.ACHIEVEMENTCHART_TYPE.Work:
                            _typeImage.SetImageResource(Resource.Drawable.work);
                            break;

                        case AchievementChart.ACHIEVEMENTCHART_TYPE.Family:
                            _typeImage.SetImageResource(Resource.Drawable.family);
                            break;

                        case AchievementChart.ACHIEVEMENTCHART_TYPE.Relationships:
                            _typeImage.SetImageResource(Resource.Drawable.relationships);
                            break;

                        case AchievementChart.ACHIEVEMENTCHART_TYPE.Health:
                            _typeImage.SetImageResource(Resource.Drawable.health);
                            break;

                        case AchievementChart.ACHIEVEMENTCHART_TYPE.Financial:
                            _typeImage.SetImageResource(Resource.Drawable.financial);
                            break;

                        case AchievementChart.ACHIEVEMENTCHART_TYPE.Affirmation:
                            _typeImage.SetImageResource(Resource.Drawable.affirmation);
                            break;

                        case AchievementChart.ACHIEVEMENTCHART_TYPE.Goal:
                            _typeImage.SetImageResource(Resource.Drawable.goal);
                            break;

                        case AchievementChart.ACHIEVEMENTCHART_TYPE.NothingEntered:
                        default:
                            _typeImage.SetImageResource(Resource.Drawable.information);
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetImageForType: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorAchievementAdapterSetViewImage), "AchievementChartRecordsAdapter.SetImageForType");
            }
        }
    }
}
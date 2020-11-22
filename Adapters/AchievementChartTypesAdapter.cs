using System;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Content;
using System.Collections;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class AchievementChartTypesAdapter : ArrayAdapter
    {
        public const string TAG = "M:AchievementChartTypesAdapter";

        string[] _achievementChartTypes;
        Activity _activity;

        private ImageView _typeImage;
        private TextView _typeName;

        private enum ViewType
        {
            vtGeneral = 0,
            vtDropDown
        }

        public AchievementChartTypesAdapter(Context activity, int layoutResource, int viewItemResource, IList objects) : base(activity, layoutResource, viewItemResource, objects)
        {
            _activity = (Activity)activity;

            GetAllAchievementChartTypeData();
            base.AddAll((ICollection)_achievementChartTypes);
        }

        private void GetAllAchievementChartTypeData()
        {
            if (GlobalData.AchievementChartTypes != null)
            {
                _achievementChartTypes = GlobalData.AchievementChartTypes;
            }
        }

        public override int Count
        {
            get
            {
                if (_achievementChartTypes != null)
                {
                    return _achievementChartTypes.Length;
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
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return Initialise(position, convertView, parent, ViewType.vtGeneral);
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            return Initialise(position, convertView, parent, ViewType.vtDropDown);
        }

        private View Initialise(int position, View convertView, ViewGroup parent, ViewType viewType)
        {
            int layoutType;
            if(viewType == ViewType.vtGeneral)
            {
                layoutType = Resource.Layout.AchievementChartTypeListItemSelected;
            }
            else
            {
                layoutType = Resource.Layout.AchievementChartTypeListItem;
            }

            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(layoutType, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(layoutType, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                _typeImage = convertView.FindViewById<ImageView>(Resource.Id.imgChuffChartType);
                _typeName = convertView.FindViewById<TextView>(Resource.Id.txtChuffChartType);

                if (_typeImage != null)
                {
                    SetImageForType((AchievementChart.ACHIEVEMENTCHART_TYPE)position);
                }
                if (_typeName != null)
                    _typeName.Text = _achievementChartTypes[position].Trim();

                return convertView;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Initialise: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorAchievementTypeAdapterGetView), "AchievementChartTypesAdapter.Initialise");
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
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetImageForType: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorAchievementTypeAdapterSetViewImage), "AchievementChartTypesAdapter.SetImageForType");
            }
        }
    }
}

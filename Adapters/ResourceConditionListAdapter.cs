using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class ResourceConditionListAdapter : BaseAdapter
    {
        public const string TAG = "M:ResourceConditionListAdapter";

        private List<ResourceCondition> _conditions;
        private Activity _activity;

        private TextView _conditionTitle;
        private TextView _conditionDescription;
        private TextView _conditionCitation;

        public ResourceConditionListAdapter(Activity activity)
        {
            _conditions = GlobalData.ResourceConditions;
            _activity = activity;
        }

        public override int Count
        {
            get
            {
                return _conditions.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _conditions[position].ConditionId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.ResourceConditionListItem, null);
                    }

                }
                if (convertView != null)
                {
                    GetFieldComponents(convertView);
                    if (_conditionTitle != null)
                        _conditionTitle.Text = _conditions[position].ConditionTitle;
                    if (_conditionDescription != null)
                        _conditionDescription.Text = _conditions[position].ConditionDescription;
                    if (_conditionCitation != null)
                        _conditionCitation.Text = _conditions[position].ConditionCitation;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Conditions view", "ResourceMedicationTypeAdapter.GetView");
            }
            return convertView;
        }

        private void GetFieldComponents(View convertView)
        {
            try
            {
                _conditionTitle = convertView.FindViewById<TextView>(Resource.Id.txtConditionTitle);
                _conditionDescription = convertView.FindViewById<TextView>(Resource.Id.txtConditionDescription);
                _conditionCitation = convertView.FindViewById<TextView>(Resource.Id.txtConditionCitation);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting adapter field components", "ResourceMedicationTypeAdapter.GetFieldComponents");
            }
        }
    }
}
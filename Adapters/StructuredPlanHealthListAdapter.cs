using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using Android.Graphics;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.SubActivities.StructuredPlan;
using Android.Content;
using Android.App;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class StructuredPlanHealthListAdapter : BaseAdapter
    {
        public const string TAG = "M:StructuredPlanHealthListAdapter";

        List<Health> _health;

        Activity _activity;

        private TextView _aspect = null;
        private TextView _importance = null;
        private TextView _reaction = null;

        private TextView _strengthLabel;
        private TextView _reactionLabel;

        public StructuredPlanHealthListAdapter(Activity activity)
        {
            _activity = activity;
            _health = new List<Health>();
            GetAllHealthData();
        }

        private void GetAllHealthData()
        {
            if (GlobalData.StructuredPlanHealth != null)
            {
                Log.Info(TAG, "GetAllHealthData: Getting all Health Items...");
                _health = GlobalData.StructuredPlanHealth;
                Log.Info(TAG, "GetAllHealthData: Retrieved " + _health.Count.ToString() + " items");
            }
        }

        public override int Count
        {
            get
            {
                return _health.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_health != null)
            {
                if (_health.Count > 0)
                    return _health[position].HealthID;
            }
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.StructuredPlanHealthListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.StructuredPlanHealthListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                GetFieldComponents(convertView);

                if (_health != null)
                {
                    if (_aspect != null)
                        _aspect.Text = _health[position].Aspect.Trim();
                    if (_importance != null)
                        _importance.Text = _health[position].Importance.ToString();
                    if (_reaction != null)
                        _reaction.Text = StringHelper.ReactionTypeForConstant(_health[position].Type);
                }

                var parentHeldPosition = ((StructuredPlanHealth)_activity).GetSelectedItemIndex();
                if (position == parentHeldPosition)
                {
                    convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_aspect != null)
                        _aspect.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_importance != null)
                        _importance.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_reaction != null)
                        _reaction.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_strengthLabel != null)
                        _strengthLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_reactionLabel != null)
                        _reactionLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                }

                return convertView;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanHealthListGetView), "StructuredPlanHealthListAdapter.GetView");
                return convertView;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _aspect = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanHealthAspect);
                _importance = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanHealthImportanceText);
                _reaction = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanHealthReactionText);

                _strengthLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanHealthStrengthLabel);
                _reactionLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanHealthReactionLabel);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanHealthListGetComponents), "StructuredPlanHealthListAdapter.GetFieldComponents");
            }
        }
    }
}
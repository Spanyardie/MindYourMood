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
    public class StructuredPlanFantasiesListAdapter : BaseAdapter
    {
        public const string TAG = "M:StructuredPlanFantasiesListAdapter";

        List<Fantasies> _fantasies;

        Activity _activity;

        private TextView _ofWhat = null;
        private TextView _strength = null;
        private TextView _reaction = null;

        private TextView _strengthLabel;
        private TextView _reactionLabel;

        public StructuredPlanFantasiesListAdapter(Activity activity)
        {
            _activity = activity;
            _fantasies = new List<Fantasies>();
            GetAllFantasiesData();
        }

        private void GetAllFantasiesData()
        {
            if (GlobalData.StructuredPlanFantasies != null)
            {
                Log.Info(TAG, "GetAllFantasiesData: Getting all Fantasies Items...");
                _fantasies = GlobalData.StructuredPlanFantasies;
                Log.Info(TAG, "GetAllFantasiesData: Retrieved " + _fantasies.Count.ToString() + " items");
            }
        }

        public override int Count
        {
            get
            {
                return _fantasies.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_fantasies != null)
            {
                if (_fantasies.Count > 0)
                    return _fantasies[position].FantasiesID;
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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.StructuredPlanFantasiesListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.StructuredPlanFantasiesListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                GetFieldComponents(convertView);

                if (_fantasies != null)
                {
                    if (_ofWhat != null)
                        _ofWhat.Text = _fantasies[position].OfWhat.Trim();
                    if (_strength != null)
                        _strength.Text = _fantasies[position].Strength.ToString();
                    if (_reaction != null)
                        _reaction.Text = StringHelper.ReactionTypeForConstant(_fantasies[position].Type);
                }

                var parentHeldPosition = ((StructuredPlanFantasies)_activity).GetSelectedItemIndex();
                if (position == parentHeldPosition)
                {
                    convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_ofWhat != null)
                        _ofWhat.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_strength != null)
                        _strength.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
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
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanFantasiesListGetView), "StructuredPlanFantasiesListAdapter.GetView");
                return convertView;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _ofWhat = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanFantasiesOfWhat);
                _strength = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanFantasiesStrengthText);
                _reaction = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanFantasiesReactionText);

                _strengthLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanFantasiesStrengthLabel);
                _reactionLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanFantasiesReactionLabel);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanFantasiesListGetComponents), "StructuredPlanFantasiesListAdapter.GetFieldComponents");
            }
        }
    }
}
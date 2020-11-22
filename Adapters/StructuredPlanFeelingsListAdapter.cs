using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.StructuredPlan;
using Android.Content;
using Android.App;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class StructuredPlanFeelingsListAdapter : BaseAdapter
    {
        public const string TAG = "M:StructuredPlanFeelingsListAdapter";

        List<Feelings> _feelings;

        Activity _activity;

        private TextView _aboutWhat = null;
        private TextView _strength = null;
        private TextView _reaction = null;

        private TextView _strengthLabel;
        private TextView _reactionLabel;

        public StructuredPlanFeelingsListAdapter(Activity activity)
        {
            _activity = activity;
            _feelings = new List<Feelings>();
            GetAllFeelingsData();
        }

        private void GetAllFeelingsData()
        {
            if(GlobalData.StructuredPlanFeelings != null)
            {
                Log.Info(TAG, "GetAllFeelingsData: Getting all Feelings items...");
                _feelings = GlobalData.StructuredPlanFeelings;
                Log.Info(TAG, "GetAllFeelingsData: Retrieved " + _feelings.Count.ToString() + " items");
            }
        }

        public override int Count
        {
            get
            {
                return _feelings.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if(_feelings != null)
            {
                if (_feelings.Count > 0)
                    return _feelings[position].FeelingsID;
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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.StructuredPlanFeelingsListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.StructuredPlanFeelingsListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                GetFieldComponents(convertView);

                if(_feelings != null)
                {
                    if (_aboutWhat != null)
                        _aboutWhat.Text = _feelings[position].AboutWhat.Trim();
                    if (_strength != null)
                        _strength.Text = _feelings[position].Strength.ToString();
                    if (_reaction != null)
                        _reaction.Text = StringHelper.ReactionTypeForConstant(_feelings[position].Type);
                }

                var parentHeldPosition = ((StructuredPlanFeelings)_activity).GetSelectedItemIndex();
                Log.Info(TAG, "GetView: Parent selected position - " + parentHeldPosition.ToString());
                if (position == parentHeldPosition)
                {
                    convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_aboutWhat != null)
                        _aboutWhat.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
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
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanFeelingListGetView), "StructuredPlanFeelingsListAdapter.GetView");
                return convertView;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _aboutWhat = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanFeelingsAboutWhat);
                _strength = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanFeelingsStrengthText);
                _reaction = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanFeelingsReactionText);

                _strengthLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanFeelingsStrengthLabel);
                _reactionLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanFeelingsReactionLabel);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanFeelingListGetComponents), "StructuredPlanFeelingsListAdapter.GetFieldComponents");
            }
        }
    }
}
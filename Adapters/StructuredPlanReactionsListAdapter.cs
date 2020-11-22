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
    public class StructuredPlanReactionsListAdapter : BaseAdapter
    {
        public const string TAG = "M:StructuredPlanReactionsListAdapter";

        List<Reactions> _reactions;

        Activity _activity;

        private TextView _toWhat = null;
        private TextView _strength = null;
        private TextView _reaction = null;

        private TextView _strengthLabel;
        private TextView _reactionLabel;

        public StructuredPlanReactionsListAdapter(Activity activity)
        {
            _activity = activity;
            _reactions = new List<Reactions>();
            GetAllReactionsData();
        }

        private void GetAllReactionsData()
        {
            if(GlobalData.StructuredPlanReactions != null)
            {
                Log.Info(TAG, "GetAllReactionsData: Getting all Reaction Items...");
                _reactions = GlobalData.StructuredPlanReactions;
                Log.Info(TAG, "GetAllReactionsData: Retrieved " + _reactions.Count.ToString() + " items");
            }
        }

        public override int Count
        {
            get
            {
                return _reactions.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_reactions != null)
            {
                if (_reactions.Count > 0)
                    return _reactions[position].ReactionsID;
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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.StructuredPlanReactionsListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.StructuredPlanReactionsListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                GetFieldComponents(convertView);

                if (_reactions != null)
                {
                    if (_toWhat != null)
                        _toWhat.Text = _reactions[position].ToWhat.Trim();
                    if (_strength != null)
                        _strength.Text = _reactions[position].Strength.ToString();
                    if (_reaction != null)
                        _reaction.Text = StringHelper.ReactionTypeForConstant(_reactions[position].Type);
                }

                var parentHeldPosition = ((StructuredPlanReactions)_activity).GetSelectedItemIndex();
                if (position == parentHeldPosition)
                {
                    convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_toWhat != null)
                        _toWhat.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
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
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanReactionsListGetView), "StructuredPlanReactionsListAdapter.GetView");
                return convertView;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _toWhat = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanReactionsToWhat);
                _strength = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanReactionsStrengthText);
                _reaction = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanReactionsReactionText);

                _strengthLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanReactionsStrengthLabel);
                _reactionLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanReactionsReactionLabel);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanReactionsListGetComponents), "StructuredPlanReactionsListAdapter.GetFieldComponents");
            }
        }
    }
}
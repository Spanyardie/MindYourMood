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
    public class StructuredPlanRelationshipsListAdapter : BaseAdapter
    {
        public const string TAG = "M:StructuredPlanRelationshipsListAdapter";

        List<Relationships> _relationships;

        Activity _activity;

        private TextView _withWhom = null;
        private TextView _type = null;
        private TextView _strength = null;

        private TextView _typeLabel;
        private TextView _strengthLabel;

        public StructuredPlanRelationshipsListAdapter(Activity activity)
        {
            _activity = activity;
            _relationships = new List<Relationships>();
            GetAllRelationshipsData();
        }

        private void GetAllRelationshipsData()
        {
            if (GlobalData.StructuredPlanRelationships != null)
            {
                Log.Info(TAG, "GetAllRelationshipsData: Getting all Relationships Items...");
                _relationships = GlobalData.StructuredPlanRelationships;
                Log.Info(TAG, "GetAllRelationshipsData: Retrieved " + _relationships.Count.ToString() + " items");
            }
        }

        public override int Count
        {
            get
            {
                return _relationships.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_relationships != null)
            {
                if (_relationships.Count > 0)
                    return _relationships[position].RelationshipsID;
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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.StructuredPlanRelationshipsListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.StructuredPlanRelationshipsListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }
                GetFieldComponents(convertView);

                if (_relationships != null)
                {
                    if (_withWhom != null)
                        _withWhom.Text = _relationships[position].WithWhom.Trim();
                    if (_type != null)
                        _type.Text = StringHelper.RelationshipTypeForConstant((ConstantsAndTypes.RELATIONSHIP_TYPE)_relationships[position].Type);
                    if (_strength != null)
                        _strength.Text = _relationships[position].Strength.ToString();
                }

                var parentHeldPosition = ((StructuredPlanRelationships)_activity).GetSelectedItemIndex();
                if (position == parentHeldPosition)
                {
                    convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_withWhom != null)
                        _withWhom.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_type != null)
                        _type.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_strength != null)
                        _strength.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_strengthLabel != null)
                        _strengthLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_typeLabel != null)
                        _typeLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                }

                return convertView;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanRelationshipsListGetView), "StructuredPlanRelationshipsListAdapter.GetView");
                return convertView;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _withWhom = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanRelationshipsWithWhom);
                _type = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanRelationshipsTypeText);
                _strength = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanRelationshipsStrengthText);

                _typeLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanRelationshipsStrengthLabel);
                _strengthLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanRelationshipsReactionLabel);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanRelationshipsListGetComponents), "StructuredPlanRelationshipsListAdapter.GetFieldComponents");
            }
        }
    }
}
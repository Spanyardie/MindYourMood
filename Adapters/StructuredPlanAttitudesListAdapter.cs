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
    public class StructuredPlanAttitudesListAdapter : BaseAdapter
    {
        public const string TAG = "M:StructuredPlanAttitudesListAdapter";

        List<Attitudes> _attitudes;

        Activity _activity;

        private TextView _toWhat = null;
        private TextView _type = null;
        private TextView _belief = null;

        private TextView _typeLabel;
        private TextView _beliefLabel;

        public StructuredPlanAttitudesListAdapter(Activity activity)
        {
            _activity = activity;
            _attitudes = new List<Attitudes>();
            GetAllAttitudesData();
        }

        private void GetAllAttitudesData()
        {
            if (GlobalData.StructuredPlanAttitudes != null)
            {
                Log.Info(TAG, "GetAllAttitudesData: Getting all Attitudes Items...");
                _attitudes = GlobalData.StructuredPlanAttitudes;
                Log.Info(TAG, "GetAllAttitudesData: Retrieved " + _attitudes.Count.ToString() + " items");
            }
        }

        public override int Count
        {
            get
            {
                return _attitudes.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_attitudes != null)
            {
                if (_attitudes.Count > 0)
                    return _attitudes[position].AttitudesID;
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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.StructuredPlanAttitudesListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.StructuredPlanAttitudesListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                GetFieldComponents(convertView);

                if (_attitudes != null)
                {
                    if (_toWhat != null)
                        _toWhat.Text = _attitudes[position].ToWhat.Trim();
                    if (_type != null)
                        _type.Text = StringHelper.AttitudeTypeForConstant((ConstantsAndTypes.ATTITUDE_TYPES)_attitudes[position].TypeOf);
                    if (_belief != null)
                        _belief.Text = _attitudes[position].Belief.ToString();
                }

                var parentHeldPosition = ((StructuredPlanAttitudes)_activity).GetSelectedItemIndex();
                if (position == parentHeldPosition)
                {
                    convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_toWhat != null)
                        _toWhat.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_type != null)
                        _type.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_belief != null)
                        _belief.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_typeLabel != null)
                        _typeLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_beliefLabel != null)
                        _beliefLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                }

                return convertView;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanAttitudesListGetView), "StructuredPlanAttitudesListAdapter.GetView");
                return convertView;
            }
        }


        private void GetFieldComponents(View view)
        {
            try
            {
                _toWhat = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanAttitudesToWhat);
                _type = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanAttitudesTypeText);
                _belief = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanAttitudesBeliefText);

                _typeLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanAttitudesStrengthLabel);
                _beliefLabel = view.FindViewById<TextView>(Resource.Id.txtStructuredPlanAttitudesReactionLabel);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorStructuredPlanAttitudesListGetComponents), "StructuredPlanAttitudesListAdapter.GetFieldComponents");
            }
        }
    }
}
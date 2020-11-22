using System.Collections.Generic;
using System;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using Android.Graphics;
using com.spanyardie.MindYourMood.Helpers;
using Android.Content;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class SafetyPlanCardsStackAdapter : BaseAdapter
    {
        public const string TAG = "M:SafetyPlanCardsStackAdapter";

        List<SafetyPlanCard> _safetyPlanCards;

        Activity _activity;

        private TextView _calmMyself;
        private TextView _tellMyself;
        private TextView _willCall;
        private TextView _willGoTo;

        public SafetyPlanCardsStackAdapter(Activity activity)
        {
            _activity = activity;

            GetAllSafetyPlanCardData();
        }

        private void GetAllSafetyPlanCardData()
        {
            if(GlobalData.SafetyPlanCardsItems != null)
            {
                Log.Info(TAG, "GetAllSafetyPlanCardData: Getting all Safety Plan Card Items...");
                _safetyPlanCards = GlobalData.SafetyPlanCardsItems;
                Log.Info(TAG, "GetAllSafetyPlanCardData: Retrieved " + _safetyPlanCards.Count.ToString() + " items");
            }
        }

        public override int Count
        {
            get
            {
                if (_safetyPlanCards != null)
                    return _safetyPlanCards.Count;
                return -1;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if(_safetyPlanCards != null)
            {
                if (_safetyPlanCards.Count > 0)
                    return _safetyPlanCards[position].ID; 
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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.SafetyPlanCardsStackItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.SafetyPlanCardsStackItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                _calmMyself = convertView.FindViewById<TextView>(Resource.Id.txtCalmMyselfText);
                _tellMyself = convertView.FindViewById<TextView>(Resource.Id.txtTellMyselfText);
                _willCall = convertView.FindViewById<TextView>(Resource.Id.txtCallText);
                _willGoTo = convertView.FindViewById<TextView>(Resource.Id.txtGoToText);

                var parentHeldPosition = ((SafetyPlanCardsActivity)_activity).GetSelectedItemIndex();
                if (position == parentHeldPosition)
                {
                    convertView.SetBackgroundColor(Color.Blue);
                    Log.Info(TAG, "GetView: - Set background colour to Blue for item " + position.ToString());
                }
                else
                {
                    convertView.SetBackgroundColor(Color.DarkRed);
                    Log.Info(TAG, "GetView: - Set background colour to Dark Red for item " + position.ToString());
                }
                if (_calmMyself != null)
                {
                    _calmMyself.Text = _safetyPlanCards[position].CalmMyself.Trim();
                    Log.Info(TAG, "GetView: Calm myself text '" + _calmMyself.Text.Trim() + "'");
                }
                if (_tellMyself != null)
                {
                    _tellMyself.Text = _safetyPlanCards[position].TellMyself.Trim();
                    Log.Info(TAG, "GetView: Tell myself text '" + _tellMyself.Text.Trim() + "'");
                }
                if (_willCall != null)
                {
                    _willCall.Text = _safetyPlanCards[position].WillCall.Trim();
                    Log.Info(TAG, "GetView: Will Call text '" + _willCall.Text.Trim() + "'");
                }
                if (_willGoTo != null)
                {
                    _willGoTo.Text = _safetyPlanCards[position].WillGoTo.Trim();
                    Log.Info(TAG, "GetView: Will Go To text '" + _willGoTo.Text.Trim() + "'");
                }

                return convertView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Safety Plan Card Items View", "GenericTextListAdapter.GetView");
                return convertView;
            }
        }
    }
}
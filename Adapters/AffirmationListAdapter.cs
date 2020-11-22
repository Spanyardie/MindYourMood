using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using Android.Graphics;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class AffirmationListAdapter : BaseAdapter
    {
        public const string TAG = "M:AffirmationListAdapter";

        Activity _activity;

        private List<Affirmation> _affirmations = null;

        private TextView _affirmationText;

        public AffirmationListAdapter(Activity activity)
        {
            _activity = activity;

            _affirmations = new List<Affirmation>();

            GetAffirmationData();
        }

        private void GetAffirmationData()
        {
            if(GlobalData.AffirmationListItems != null)
            {
                _affirmations = GlobalData.AffirmationListItems;
            }
        }

        public override int Count
        {
            get
            {
                return _affirmations.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if(_affirmations != null)
            {
                if(position <= _affirmations.Count)
                {
                    return _affirmations[position].AffirmationID;
                }
            }
            return -1;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.AffirmationListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.AffirmationListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                if (convertView != null)
                {
                    GetFieldComponents(convertView);

                    if (_affirmationText != null)
                    {
                        _affirmationText.Text = _affirmations[position].AffirmationText.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _affirmationText is NULL!");
                    }

                    if (position == ((AffirmationsActivity)_activity).GetSelectedItemIndex())
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_affirmationText != null)
                            _affirmationText.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                }
                else
                {
                    Log.Error(TAG, "GetView: view is NULL!");
                }

                return convertView;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorAffirmationListGetView), "AffirmationListAdapter.GetView");
                return null;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if(view != null)
                {
                    _affirmationText = view.FindViewById<TextView>(Resource.Id.txtAffirmationListItem);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: view is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorAffirmationListGetComponents), "AffirmationListAdapter.GetFieldComponents");
            }
        }
    }
}
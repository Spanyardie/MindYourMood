using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Graphics;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;


namespace com.spanyardie.MindYourMood.Wizards
{
    public class AlternativeThoughtItemsAdapter : BaseAdapter
    {
        public const string TAG = "M:AlternativeThoughtItemsAdapter";

        List<AlternativeThoughts> _alternativeThoughtEntries;
        Activity _activity;

        //private int _selectedPosition;

        public AlternativeThoughtItemsAdapter(Activity activity)
        {
            _activity = activity;
            GetAlternativeThoughts();
        }

        private void GetAlternativeThoughts()
        {
            _alternativeThoughtEntries = GlobalData.AlternativeThoughtsItems;
        }

        public override int Count
        {
            get
            {
                if (_alternativeThoughtEntries != null)
                {
                    return _alternativeThoughtEntries.Count;
                }
                else
                {
                    _alternativeThoughtEntries = new List<AlternativeThoughts>();
                    return 0;
                }
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _alternativeThoughtEntries.ElementAt(position).AlternativeThoughtsId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.AlternativeThoughtListItem, parent, false);

                TextView thoughtText = view.FindViewById<TextView>(Resource.Id.txtAlternativeThoughtListItem);

                TextView thoughtBelief = view.FindViewById<TextView>(Resource.Id.txtAlternativeThoughtBeliefListItem);

                thoughtText.Text = _alternativeThoughtEntries.ElementAt(position).Alternative.Trim();
                thoughtBelief.Text = _alternativeThoughtEntries.ElementAt(position).BeliefRating.ToString() + "%";

                var parentHeldSelectedItemIndex = ((ThoughtRecordWizardAlternativeThoughtStep)_activity).GetSelectedItem();
                if (position == parentHeldSelectedItemIndex)
                {
                    var viewColor = Color.Argb(255, 19, 75, 127);
                    view.SetBackgroundColor(viewColor);

                    thoughtText.SetTextColor(Color.White);
                    thoughtText.SetBackgroundColor(viewColor);
                    thoughtBelief.SetTextColor(Color.White);
                    thoughtBelief.SetBackgroundColor(viewColor);
                }
                return view;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetAltThoughtAdapterView), "AlternativeThoughtItemsAdapter.GetView");
                return null;
            }
        }
    }
}
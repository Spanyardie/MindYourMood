using System.Collections.Generic;
using System.Linq;
using System;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Graphics;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;


namespace com.spanyardie.MindYourMood.Wizards
{
    public class AutomaticThoughtItemsAdapter : BaseAdapter
    {
        public const string TAG = "M:AutomaticThoughtItemsAdapter";

        List<AutomaticThoughts> _automaticThoughtEntries;
        Activity _activity;

        //private int _selectedPosition;

        public AutomaticThoughtItemsAdapter(Activity activity)
        {
            _activity = activity;
            GetAutomaticThoughts();
        }

        private void GetAutomaticThoughts()
        {
            _automaticThoughtEntries = GlobalData.AutomaticThoughtsItems;
        }

        public override int Count
        {
            get
            {
                if (_automaticThoughtEntries != null)
                {
                    return _automaticThoughtEntries.Count;
                }
                else
                {
                    return -1;
                }
            }
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _automaticThoughtEntries.ElementAt(position).AutomaticThoughtsId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.AutomaticThoughtsListItem, parent, false);

                var thought = view.FindViewById<TextView>(Resource.Id.txtAutomaticThought);

                AutomaticThoughts thoughtEntry = _automaticThoughtEntries.ElementAt(position);
                thought.Text = thoughtEntry.Thought.Trim() + (thoughtEntry.IsHotThought ? "  " + _activity.GetString(Resource.String.HotThoughtTag): "");

                var parentHeldSelectedItemIndex = ((ThoughtRecordWizardAutomaticThoughtsStep)_activity).GetSelectedItem();
                if (position == parentHeldSelectedItemIndex)
                {
                    view.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    thought.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                }
                else
                {
                    view.SetBackgroundDrawable(null);
                    thought.SetBackgroundDrawable(null);
                }
                return view;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetAutoThoughtAdapterView), "AutomaticThoughtItemsAdapter.GetView");
                return null;
            }
        }
    }
}
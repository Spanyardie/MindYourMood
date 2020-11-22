using System.Collections.Generic;
using System.Linq;
using System;
using Android.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Graphics;
using com.spanyardie.MindYourMood.Helpers;


namespace com.spanyardie.MindYourMood.Wizards
{
    public class EvidenceAgainstHotThoughtItemsAdapter : BaseAdapter
    {
        public const string TAG = "M:EvidenceAgainstHotThoughtItemsAdapter";

        List<EvidenceAgainstHotThought> _evidenceAgainstHotThoughtEntries;
        Activity _activity;

        //private int _selectedPosition;

        public EvidenceAgainstHotThoughtItemsAdapter(Activity activity)
        {
            _activity = activity;
            GetEvidenceAgainstHotThoughts();
        }

        private void GetEvidenceAgainstHotThoughts()
        {
            _evidenceAgainstHotThoughtEntries = GlobalData.EvidenceAgainstHotThoughtItems;
        }

        public override int Count
        {
            get
            {
                if (_evidenceAgainstHotThoughtEntries != null)
                {
                    return _evidenceAgainstHotThoughtEntries.Count;
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
            return _evidenceAgainstHotThoughtEntries.ElementAt(position).EvidenceAgainstHotThoughtId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.EvidenceAgainstHotThoughtListItem, parent, false);

                var thought = view.FindViewById<TextView>(Resource.Id.txtEvidenceAgainstHotThought);

                EvidenceAgainstHotThought thoughtEntry = _evidenceAgainstHotThoughtEntries.ElementAt(position);
                thought.Text = thoughtEntry.Evidence.Trim();

                var parentHeldSelectedItemIndex = ((ThoughtRecordWizardEvidenceAgainstHotThoughtStep)_activity).GetSelectedItem();
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetEvidenceAgainstAdapterView), "EvidenceAgainstHotThoughtItemsAdapter.GetView");
                return null;
            }
        }
    }
}
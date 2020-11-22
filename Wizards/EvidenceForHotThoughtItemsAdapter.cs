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
    public class EvidenceForHotThoughtItemsAdapter : BaseAdapter
    {
        public const string TAG = "M:EvidenceForHotThoughtItemsAdapter";

        List<EvidenceForHotThought> _evidenceForHotThoughtEntries;
        Activity _activity;

        //private int _selectedPosition;

        public EvidenceForHotThoughtItemsAdapter(Activity activity)
        {
            _activity = activity;
            GetEvidenceForHotThoughts();
        }

        private void GetEvidenceForHotThoughts()
        {
            _evidenceForHotThoughtEntries = GlobalData.EvidenceForHotThoughtItems;
        }

        public override int Count
        {
            get
            {
                if (_evidenceForHotThoughtEntries != null)
                {
                    return _evidenceForHotThoughtEntries.Count;
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
            return _evidenceForHotThoughtEntries.ElementAt(position).EvidenceForHotThoughtId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.EvidenceForHotThoughtListItem, parent, false);

                var thought = view.FindViewById<TextView>(Resource.Id.txtEvidenceForHotThought);

                EvidenceForHotThought thoughtEntry = _evidenceForHotThoughtEntries.ElementAt(position);
                thought.Text = thoughtEntry.Evidence.Trim();

                var parentHeldSelectedItemIndex = ((ThoughtRecordWizardEvidenceForHotThoughtStep)_activity).GetSelectedItem();
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
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetEvidenceForAdapterView), "EvidenceForHotThoughtItemsAdapter.GetView");
                return null;
            }
        }
    }
}
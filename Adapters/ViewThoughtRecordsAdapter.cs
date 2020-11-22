using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using static Android.Views.ViewGroup;
using Java.Lang;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Content;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class ViewThoughtRecordsAdapter : BaseAdapter
    {
        public const string TAG = "M:ViewThoughtRecordsAdapter";

        List<ThoughtRecord> _thoughtRecords;
        Activity _activity;

        private TextView _situationWhat;
        private TextView _situationWhen;
        private TextView _situationWhere;
        private TextView _situationWho;

        private LinearLayout _linMoodsItem;
        private LinearLayout _linAutomaticItem;
        private LinearLayout _linEvidenceForItem;
        private LinearLayout _linEvidenceAgainstItem;
        private LinearLayout _linAlternativeItem;
        private LinearLayout _linRerateItem;

        public ViewThoughtRecordsAdapter(Activity activity)
        {
            _activity = activity;
            GetAllThoughtRecordData();
        }

        public override int Count
        {
            get
            {
                if(_thoughtRecords != null)
                {
                    return _thoughtRecords.Count;
                }
                return -1;
            }
        }

        public override Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _thoughtRecords.ElementAt(position).ThoughtRecordId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.thoughtRecordListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.thoughtRecordListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                GetFieldComponents(convertView);
                SetFieldData(convertView, position);

                return convertView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Thought Records View", "ViewThoughtRecordsAdapter.GetView");
                return convertView;
            }
        }

        private void GetAllThoughtRecordData()
        {
            _thoughtRecords = GlobalData.ThoughtRecordsItems;
            Log.Info(TAG, "GetAllThoughtRecordData: Thought record data indicates " + _thoughtRecords.Count.ToString() + " items");
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _situationWhat = view.FindViewById<TextView>(Resource.Id.txtSituationWhatItemValue);
                _situationWho = view.FindViewById<TextView>(Resource.Id.txtSituationWhoItemValue);
                _situationWhere = view.FindViewById<TextView>(Resource.Id.txtSituationWhereItemValue);
                _situationWhen = view.FindViewById<TextView>(Resource.Id.txtSituationWhenItemValue);
                _linMoodsItem = view.FindViewById<LinearLayout>(Resource.Id.linMoodsItemPlaceholder);
                _linAutomaticItem = view.FindViewById<LinearLayout>(Resource.Id.linAutomaticItemPlaceholder);
                _linEvidenceForItem = view.FindViewById<LinearLayout>(Resource.Id.linEvidenceForItemPlaceholder);
                _linEvidenceAgainstItem = view.FindViewById<LinearLayout>(Resource.Id.linEvidenceAgainstItemPlaceholder);
                _linAlternativeItem = view.FindViewById<LinearLayout>(Resource.Id.linAlternativeItemPlaceholder);
                _linRerateItem = view.FindViewById<LinearLayout>(Resource.Id.linRerateItemPlaceholder);

            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Thought Records Field Components", "ViewThoughtRecordsAdapter.GetFieldComponents");
            }
        }

        private void SetFieldData(View view, int position)
        {
            try
            { 
                var thoughtRecord = _thoughtRecords[position];

                if (thoughtRecord != null)
                {
                    if (_situationWhat != null)
                        _situationWhat.Text = thoughtRecord.Situation.What.Trim();
                    if (_situationWho != null)
                        _situationWho.Text = thoughtRecord.Situation.Who.Trim();
                    if (_situationWhere != null)
                        _situationWhere.Text = thoughtRecord.Situation.Where.Trim();
                    if (_situationWhen != null)
                        _situationWhen.Text = thoughtRecord.Situation.When.Trim();

                    SetMoodViewData(thoughtRecord);
                    SetAutomaticThoughtData(thoughtRecord);
                    SetEvidenceForData(thoughtRecord);
                    SetEvidenceAgainstData(thoughtRecord);
                    SetAlternativeThoughtData(thoughtRecord);
                    SetRerateMoodViewData(thoughtRecord);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetFieldData: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting Thought Records Field Data", "ViewThoughtRecordsAdapter.SetFieldData");
            }
        }

        private void SetMoodViewData(ThoughtRecord thoughtRecord)
        {
            try
            {
                if (_linMoodsItem != null)
                {
                    _linMoodsItem.RemoveAllViews();
                    foreach (var mood in thoughtRecord.Moods)
                    {
                        TextView moodName = (TextView)_activity.LayoutInflater.Inflate(Resource.Layout.VTextViewThought, null);
                        var itemText = GetMoodName(mood.MoodListId) + ", " + mood.MoodRating.ToString() + "%";
                        SetTextItem(moodName, itemText);
                        _linMoodsItem.AddView(moodName);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetMoodViewData: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting Mood View Data", "ViewThoughtRecordsAdapter.SetMoodViewData");
            }
        }

        private void SetAutomaticThoughtData(ThoughtRecord thoughtRecord)
        {
            try
            {
                if (_linAutomaticItem != null)
                {
                    _linAutomaticItem.RemoveAllViews();
                    foreach (var automaticThoughts in thoughtRecord.AutomaticThoughtsList)
                    {
                        LayoutParams layoutParams = new TableLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
                        TextView automaticThought = (TextView)_activity.LayoutInflater.Inflate(Resource.Layout.VTextViewThought, null);
                        automaticThought.LayoutParameters = layoutParams;
                        automaticThought.SetPadding(0, 0, 5, 0);
                        var autoText = automaticThoughts.Thought + (automaticThoughts.IsHotThought ? " " + _activity.GetString(Resource.String.HotThoughtTag) : "");
                        SetTextItem(automaticThought, autoText);
                        _linAutomaticItem.AddView(automaticThought);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetAutomaticThoughtData: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting Automatic Thought Data", "ViewThoughtRecordsAdapter.SetAutomaticThoughtData");
            }
        }

        private void SetEvidenceForData(ThoughtRecord thoughtRecord)
        {
            try
            {
                if (_linEvidenceForItem != null)
                {
                    _linEvidenceForItem.RemoveAllViews();
                    foreach (var evidenceFor in thoughtRecord.EvidenceForHotThoughtList)
                    {
                        TextView evidenceForValue = (TextView)_activity.LayoutInflater.Inflate(Resource.Layout.VTextViewThought, null);
                        var evidenceForText = evidenceFor.Evidence;
                        SetTextItem(evidenceForValue, evidenceForText);
                        _linEvidenceForItem.AddView(evidenceForValue);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetEvidenceForData: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting Evidence For Thought Data", "ViewThoughtRecordsAdapter.SetEvidenceForData");
            }
        }

        private void SetEvidenceAgainstData(ThoughtRecord thoughtRecord)
        {
            try
            {
                if (_linEvidenceAgainstItem != null)
                {
                    _linEvidenceAgainstItem.RemoveAllViews();
                    foreach (var evidenceAgainst in thoughtRecord.EvidenceAgainstHotThoughtList)
                    {
                        TextView evidenceAgainstValue = (TextView)_activity.LayoutInflater.Inflate(Resource.Layout.VTextViewThought, null);
                        var evidenceAgainstText = evidenceAgainst.Evidence;
                        SetTextItem(evidenceAgainstValue, evidenceAgainstText);
                        _linEvidenceAgainstItem.AddView(evidenceAgainstValue);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetEvidenceAgainstData: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting Evidence Against Thought Data", "ViewThoughtRecordsAdapter.SetEvidenceAgainstData");
            }
        }

        private void SetAlternativeThoughtData(ThoughtRecord thoughtRecord)
        {
            try
            {
                if (_linAlternativeItem != null)
                {
                    _linAlternativeItem.RemoveAllViews();
                    foreach (var alternativeThoughts in thoughtRecord.AlternativeThoughtsList)
                    {
                        TextView alternativeThought = (TextView)_activity.LayoutInflater.Inflate(Resource.Layout.VTextViewThought, null);
                        var alternativeThoughtText = alternativeThoughts.Alternative + ", " + alternativeThoughts.BeliefRating.ToString() + "%";
                        SetTextItem(alternativeThought, alternativeThoughtText);
                        _linAlternativeItem.AddView(alternativeThought);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetAlternativeThoughtData: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting Alternative Thought Records Data", "ViewThoughtRecordsAdapter.SetAlternativeThoughtData");
            }
        }

        private void SetRerateMoodViewData(ThoughtRecord thoughtRecord)
        {
            try
            {
                if (_linRerateItem != null)
                {
                    _linRerateItem.RemoveAllViews();
                    foreach (var mood in thoughtRecord.RerateMoodList)
                    {
                        TextView rerateMoodName = (TextView)_activity.LayoutInflater.Inflate(Resource.Layout.VTextViewThought, null);
                        var rerateMoodNameText = GetMoodName(mood.MoodListId) + ", " + mood.MoodRating.ToString() + "%";
                        SetTextItem(rerateMoodName, rerateMoodNameText);
                        _linRerateItem.AddView(rerateMoodName);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetRerateMoodViewData: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting Rerate Mood Data", "ViewThoughtRecordsAdapter.SetRerateMoodViewData");
            }
        }

        private string GetMoodName(long moodListId)
        {
            string retVal = "";

            foreach (var moodListItem in GlobalData.MoodListItems)
            {
                if (moodListItem.MoodId == moodListId)
                {
                    retVal = moodListItem.MoodName.Trim();
                    break;
                }
            }

            return retVal;
        }

        private void SetTextItem(TextView textView, string itemText)
        {
            ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

            textView.Text = itemText;

            switch (screenSize)
            {
                case ConstantsAndTypes.ScreenSize.Normal:
                    textView.SetTextSize(ComplexUnitType.Dip, 14);
                    break;
                case ConstantsAndTypes.ScreenSize.Large:
                    textView.SetTextSize(ComplexUnitType.Dip, 18);
                    break;
                case ConstantsAndTypes.ScreenSize.ExtraLarge:
                    textView.SetTextSize(ComplexUnitType.Dip, 22);
                    break;
            }
        }
    }
}
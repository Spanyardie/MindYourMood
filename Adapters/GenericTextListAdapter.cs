using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Graphics;
using Android.Util;
using Android.Content;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class GenericTextListAdapter : BaseAdapter
    {
        public const string TAG = "M:GenericTextListAdapter";

        private ConstantsAndTypes.GENERIC_TEXT_TYPE _textType;

        Activity _activity;

        private List<GenericText> _genericTextList;
        private ImageView _genericTextType;
        private TextView _textValue;

        public GenericTextListAdapter() { }

        public GenericTextListAdapter(Activity activity, ConstantsAndTypes.GENERIC_TEXT_TYPE textType)
        {
            _activity = activity;
            _textType = textType;

            GetGenericTextEntries();
        }

        private void GetGenericTextEntries()
        {
            try
            {
                if (_genericTextList == null)
                {
                    _genericTextList = new List<GenericText>();
                }

                if (GlobalData.GenericTextItemsList != null)
                {
                    _genericTextList =
                        (from genericTextItem in GlobalData.GenericTextItemsList
                         where genericTextItem.TextType == _textType
                         select genericTextItem).ToList();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetGenericTextEntries: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Generic Text Items", "GenericTextListAdapter.GetGenericTextEntries");
            }
        }

        public override int Count
        {
            get
            {
                if(_genericTextList != null)
                    return _genericTextList.Count;
                return -1;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            try
            {
                if (_genericTextList != null)
                {
                    if (_genericTextList.Count > 0)
                    {
                        return _genericTextList[position].ID;
                    }
                }
                return position;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetItemId: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Generic Text Item ID", "GenericTextListAdapter.GetItemId");
                return -1;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.GenericTextListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.GenericTextListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                _genericTextType = convertView.FindViewById<ImageView>(Resource.Id.imgGenericListItemTextType);
                _textValue = convertView.FindViewById<TextView>(Resource.Id.txtGenericListItemTextValue);

                if (_genericTextType != null)
                {
                    SetImageForType(_genericTextList[position].TextType);
                    _genericTextType.SetMaxHeight(48);
                    _genericTextType.SetMaxWidth(48);
                    _genericTextType.Focusable = false;
                }

                if (_textValue != null)
                {
                    _textValue.Text = _genericTextList[position].TextValue.Trim();
                }

                var parentHeldSelectedItemIndex = ((IGenericTextCallback)_activity).SelectedItemIndex;
                if (parentHeldSelectedItemIndex != -1)
                {
                    if (position == parentHeldSelectedItemIndex)
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        _genericTextType.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        _textValue.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                }

                return convertView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Generic Text View", "GenericTextListAdapter.GetView");
                return null;
            }
        }

        private void SetImageForType(ConstantsAndTypes.GENERIC_TEXT_TYPE textType)
        {
            try
            {
                if (_genericTextType != null)
                {
                    switch (textType)
                    {
                        case ConstantsAndTypes.GENERIC_TEXT_TYPE.KeepCalm:
                            _genericTextType.SetImageResource(Resource.Drawable.calm);
                            break;
                        case ConstantsAndTypes.GENERIC_TEXT_TYPE.MethodsOfCoping:
                            _genericTextType.SetImageResource(Resource.Drawable.thoughts);
                            break;
                        case ConstantsAndTypes.GENERIC_TEXT_TYPE.OthersCanDo:
                            _genericTextType.SetImageResource(Resource.Drawable.othersdo);
                            break;
                        case ConstantsAndTypes.GENERIC_TEXT_TYPE.StopSuicidalThoughts:
                            _genericTextType.SetImageResource(Resource.Drawable.suicidal);
                            break;
                        case ConstantsAndTypes.GENERIC_TEXT_TYPE.WarningSigns:
                            _genericTextType.SetImageResource(Resource.Drawable.warningsign);
                            break;
                        case ConstantsAndTypes.GENERIC_TEXT_TYPE.SafePlaces:
                            _genericTextType.SetImageResource(Resource.Drawable.house);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetImageForType: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting Generic Text Image for Type", "GenericTextListAdapter.SetImageForType");
            }
        }

        public GenericText GetItemAtPosition(int position)
        {
            if(_genericTextList != null)
            {
                return _genericTextList[position];
            }
            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Wizards
{
    public class MoodSpinnerDropdownListItemsAdapter : BaseAdapter
    {
        public const string TAG = "M:MoodSpinnerDropdownListItemsAdapter";

        string[] _moodTypes;
        AppCompatActivity _activity;

        private TextView _typeName;

        public MoodSpinnerDropdownListItemsAdapter(AppCompatActivity activity, string[] moods)
        {
            _activity = activity;
            _moodTypes = moods;
        }

        public override int Count
        {
            get
            {
                if (_moodTypes != null)
                {
                    return _moodTypes.Length;
                }
                return -1;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.SpinnerGeneralDropdownItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.SpinnerGeneralDropdownItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                _typeName = convertView.FindViewById<TextView>(Resource.Id.txtSpinnerSelected);

                if (_typeName != null)
                    _typeName.Text = _moodTypes[position].Trim();

                return convertView;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMoodTypeAdapterGetView), "MoodSpinnerDropdownListItemsAdapter.GetView");
                return convertView;
            }
        }
    }
}
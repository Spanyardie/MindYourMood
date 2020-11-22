using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using System;
using Android.Content;
using Android.App;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class PlayListsGridAdapter : BaseAdapter
    {
        public const string TAG = "M:PlayListsGridAdapter";

        Activity _activity;

        private List<PlayList> _playLists = null;

        public PlayListsGridAdapter(Activity activity)
        {
            try
            {
                _activity = activity;

                _playLists = new List<PlayList>();

                GetPlayListData();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Constructor: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorPlayListGridCreateAdapter), "PlayListsGridAdapter.Constructor");
            }
        }

        private void GetPlayListData()
        {
            if (GlobalData.PlayListItems != null)
            {
                _playLists = GlobalData.PlayListItems;
            }
        }

        public override int Count
        {
            get
            {
                return _playLists.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if(_playLists != null)
            {
                if(position <= _playLists.Count)
                {
                    return _playLists[position].PlayListID;
                }
            }
            return -1;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            TextView textPlayListName = null;
            try
            {
                bool isSelected = ((MusicTherapyActivity)_activity).GetSelectedItemIndex() == position;

                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.PlayListGridItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.PlayListGridItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }
                if (convertView != null)
                {
                    textPlayListName = convertView.FindViewById<TextView>(Resource.Id.txtGridListItemPlayListName);
                    if (textPlayListName != null)
                    {
                        textPlayListName.Text = _playLists[position].PlayListName.Trim();
                    }
                    if (isSelected)
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                    else
                    {
                        convertView.SetBackgroundColor(Color.DarkGray);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorPlayListGridGetView), "PlayListsGridAdapter.GetView");
            }
            return convertView;
        }
    }
}
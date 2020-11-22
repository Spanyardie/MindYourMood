using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Database;
using Android.Content;
using Android.App;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class MusicPickerTrackListAdapter : BaseAdapter
    {
        public const string TAG = "M:MusicPickerTrackListAdapter";

        public List<ExtendedTrack> _tracksOnDevice;

        private Activity _activity;

        private int _playListID;

        private TextView _trackTitle;
        private CheckBox _trackSelected;
        private bool _codeSetting = false;

        public MusicPickerTrackListAdapter(Activity activity, int playListID)
        {
            try
            {
                _activity = activity;
                _playListID = playListID;
                _tracksOnDevice = new List<ExtendedTrack>();
                GetTrackData();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Constructor: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMusicPickerCreateAdapter), "MusicPickerTrackListAdapter.Constructor");
            }
        }

        private void GetTrackData()
        {
            try
            {
                Android.Net.Uri musicUri = Android.Provider.MediaStore.Audio.Media.ExternalContentUri;
                ICursor musicCursor = _activity.ContentResolver.Query(musicUri, null, null, null, null);

                if (musicCursor != null && musicCursor.Count > 0)
                {
                    musicCursor.MoveToFirst();
                    do
                    {
                        var track = new ExtendedTrack();
                        track.PlayListID = _playListID;
                        track.TrackName = musicCursor.GetString(musicCursor.GetColumnIndex(Android.Provider.MediaStore.Audio.AudioColumns.Title)).Trim();
                        track.TrackArtist = musicCursor.GetString(musicCursor.GetColumnIndex(Android.Provider.MediaStore.Audio.AudioColumns.Artist));
                        track.TrackDuration = Convert.ToInt32(musicCursor.GetString(musicCursor.GetColumnIndex(Android.Provider.MediaStore.Audio.AudioColumns.Duration)));
                        var uri = ContentUris.WithAppendedId(musicUri, (long)musicCursor.GetLong(musicCursor.GetColumnIndex(Android.Provider.MediaStore.Audio.AudioColumns.Id)));
                        track.TrackUri = uri.ToString();
                        _tracksOnDevice.Add(track);
                    }
                    while (musicCursor.MoveToNext());
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetTrackData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMusicPickerGetData), "MusicPickerTrackListAdapter.GetTrackData");
            }
        }

        public override int Count
        {
            get
            {
                return _tracksOnDevice.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if(position < _tracksOnDevice.Count)
            {
                return _tracksOnDevice[position].TrackID;
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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.MusicPickerDialogListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.MusicPickerDialogListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                if(convertView != null)
                {
                    GetFieldComponents(convertView);
                    SetupCallbacks();

                    if(_trackSelected != null)
                    {
                        _trackSelected.Tag = position;
                        _codeSetting = true;
                        _trackSelected.Checked = _tracksOnDevice[position].TrackSelected;
                        _codeSetting = false;
                    }
                    if(_trackTitle != null)
                    {
                        _trackTitle.Text = _tracksOnDevice[position].TrackName.Trim();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMusicPickerGetView), "MusicPickerTrackListAdapter.GetView");
            }
            return convertView;
        }

        private void SetupCallbacks()
        {
            if(_trackSelected != null)
                _trackSelected.CheckedChange += TrackSelected_CheckedChange;
        }

        private void TrackSelected_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (!_codeSetting)
            {
                var trackPosition = Convert.ToInt32(((CheckBox)sender).Tag);
                _tracksOnDevice[trackPosition].TrackSelected = e.IsChecked;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _trackTitle = view.FindViewById<TextView>(Resource.Id.txtTrackItem);
                    _trackSelected = view.FindViewById<CheckBox>(Resource.Id.chkTrackItem);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMusicPickerGetComponents), "MusicPickerTrackListAdapter.GetFieldComponents");
            }
        }




        public class ExtendedTrack : Track
        {
            public bool TrackSelected { get; set; }
        }
    }
}
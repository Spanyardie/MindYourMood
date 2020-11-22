using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Content;
using Android.App;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class MusicPlayListTracksListAdapter : BaseAdapter
    {
        public const string TAG = "M:MusicPlayListTracksListAdapter";

        private Activity _activity;
        private List<Track> _tracksList;

        private TextView _trackTitle;
        private TextView _artist;
        private TextView _duration;
        private ImageView _wave;
        private LinearLayout _mainLayout;

        private int _playListID;

        public MusicPlayListTracksListAdapter(Activity activity, int playListID)
        {
            try
            {
                _activity = activity;
                _playListID = playListID;

                _tracksList = new List<Track>();

                GetTrackData();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Constructor: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMusicPlayListTracksCreateAdapter), "MusicPlayListTracksListAdapter.Constructor");
            }
        }

        private void GetTrackData()
        {
            try
            {
                if (GlobalData.PlayListItems != null)
                {
                    PlayList playList = GlobalData.PlayListItems.Find(list => list.PlayListID == _playListID);
                    if (playList != null)
                    {
                        if (playList.PlayListTracks != null)
                            _tracksList = playList.PlayListTracks;
                    }
                    else
                    {
                        Log.Error(TAG, "GetTrackData: playList is NULL!");
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetTrackData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMusicPlayListTracksGetData), "MusicPlayListTracksListAdapter.GetTrackData");
            }
        }

        public override int Count
        {
            get
            {
                return _tracksList.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _tracksList[position].TrackID;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.MusicPlayListTrackListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.MusicPlayListTrackListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                if(convertView != null)
                {
                    GetFieldComponents(convertView);

                    if (_trackTitle != null)
                        _trackTitle.Text = _tracksList[position].TrackName.Trim();
                    if (_artist != null)
                        _artist.Text = _tracksList[position].TrackArtist.Trim();
                    if(_duration != null)
                    {
                        DurationHelper.Duration duration = DurationHelper.ConvertMillisToDuration((long)_tracksList[position].TrackDuration);
                        _duration.Text = string.Format("{0:00}:{1:00}:{2:00}", duration.Hours, duration.Minutes, duration.Seconds);
                    }

                    if (((MusicPlayListTracksActivity)_activity).GetSelectedListItemIndex() == position)
                    {
                        Log.Info(TAG, "GetView: Determined selected track at position - " + position.ToString());
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_trackTitle != null)
                            _trackTitle.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_artist != null)
                            _artist.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_duration != null)
                            _duration.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_mainLayout != null)
                            _mainLayout.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        Log.Info(TAG, "GetView: Detected selected item, set background dark");
                    }
                    else
                    {
                        convertView.SetBackgroundDrawable(null);
                        if (_trackTitle != null)
                            _trackTitle.SetBackgroundDrawable(null);
                        if (_artist != null)
                            _artist.SetBackgroundDrawable(null);
                        if (_duration != null)
                            _duration.SetBackgroundDrawable(null);
                        if (_mainLayout != null)
                            _mainLayout.SetBackgroundDrawable(null);
                    }

                    var isPlaying = ((MusicPlayListTracksActivity)_activity).IsPlaying;
                    var isPaused = ((MusicPlayListTracksActivity)_activity).IsPaused;
                    var playingIndex = ((MusicPlayListTracksActivity)_activity).GetCurrentlyPlayingIndex;
                    if (isPlaying || isPaused)
                    {
                        Log.Info(TAG, "GetView: Playing or paused, position - " + position.ToString() + ", playingIndex - " + playingIndex.ToString());
                        if(playingIndex == position)
                        {
                            if(_wave != null)
                            {
                                Log.Info(TAG, "GetView: Setting animation...");
                                _wave.SetBackgroundResource(Resource.Drawable.wave);
                                AnimationDrawable background = (AnimationDrawable)_wave.Background;
                                _wave.Visibility = ViewStates.Visible;
                                if (isPaused)
                                {
                                    Log.Info(TAG, "GetView: Paused, stopping animation playback");
                                    background.Stop();
                                }
                                if (isPlaying && !isPaused)
                                {
                                    Log.Info(TAG, "GetView: Playing, starting animation playback");
                                    background.Stop();
                                    background.Start();
                                }
                            }
                        }
                        else
                        {
                            if(_wave != null)
                            {
                                _wave.Visibility = ViewStates.Invisible;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMusicPlayListTracksGetView), "MusicPlayListTracksListAdapter.GetView");
            }
            return convertView;
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _trackTitle = view.FindViewById<TextView>(Resource.Id.txtTrackListItemTitle);
                    _artist = view.FindViewById<TextView>(Resource.Id.txtTrackListItemArtist);
                    _duration = view.FindViewById<TextView>(Resource.Id.txtTrackListItemDuration);
                    _wave = view.FindViewById<ImageView>(Resource.Id.imgWave);
                    _mainLayout = view.FindViewById<LinearLayout>(Resource.Id.linTrackListItemMain);
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMusicPlayListTracksGetComponents), "MusicPlayListTracksListAdapter.GetFieldComponents");
            }
        }
    }
}
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Adapters;
using Android.Media;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Model;
using System.Collections.Generic;
using Java.Lang;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views;
using com.spanyardie.MindYourMood.SubActivities.Help.Media;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class MusicPlayListTracksActivity : AppCompatActivity, IMusicPickerCallback, IAlertCallback
    {
        public const string TAG = "M:MusicPlayListTracksActivity";

        private Toolbar _toolbar;
        private TextView _playListName;

        private ListView _trackList;

        private ImageButton _previous;
        private ImageButton _stop;
        private ImageButton _pause;
        private ImageButton _play;
        private ImageButton _next;
        private ProgressBar _trackProgress;


        private MediaPlayer _mediaPlayer;

        private int _selectedListItemIndex = -1;
        private int _playListID = -1;
        private int _currentlyPlayingTrack = -1;
        private int _currentTrackPosition = -1;
        private bool _isPaused = false;
        private bool _isPlaying = false;

        private bool _mediaPlayerConfigured = false;

        //timer for progressbar update
        Handler _timerHandler = null;
        Runnable _timerRunnable = null;

        private IMenu _menu;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("selectedItemIndex", _selectedListItemIndex);
                outState.PutInt("playListID", _playListID);
                outState.PutBoolean("isPaused", _isPaused);
                outState.PutBoolean("isPlaying", _isPlaying);
                if (_isPlaying || _isPaused)
                {
                    outState.PutInt("trackPosition", _mediaPlayer.CurrentPosition);
                }
            }
            base.OnSaveInstanceState(outState);
        }

        public int GetSelectedListItemIndex()
        {
            return _selectedListItemIndex;
        }

        public int GetCurrentlyPlayingIndex
        {
            get
            {
                return _currentlyPlayingTrack;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
        }

        public bool IsPaused
        {
            get
            {
                return _isPaused;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    GoBack();
                    return true;
                }
                switch (item.ItemId)
                {
                    case Resource.Id.musicplaylisttracksActionAdd:
                        Add();
                        return true;
                    case Resource.Id.musicplaylisttracksActionRemove:
                        Remove();
                        return true;
                    case Resource.Id.musicplaylisttracksActionHelp:
                        Intent intent = new Intent(this, typeof(MediaMusicTherapyHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            bool configurationChange = false;

            SetContentView(Resource.Layout.MusicPlayListTracksLayout);
            Log.Info(TAG, "OnCreate: - checking for ReadExternalStorage permission");
            CheckReadExternalStoragePermission();

            try
            {
                if(savedInstanceState != null)
                {
                    _selectedListItemIndex = savedInstanceState.GetInt("selectedItemIndex");
                    Log.Info(TAG, "OnCreate: - savedInstanceState value - selectedItemIndex - " + _selectedListItemIndex.ToString());
                    _playListID = savedInstanceState.GetInt("playListID");
                    Log.Info(TAG, "OnCreate: - savedInstanceState value - playListID - " + _playListID.ToString());
                    _isPaused = savedInstanceState.GetBoolean("isPaused");
                    Log.Info(TAG, "OnCreate: - savedInstanceState value - isPaused - " + (_isPaused?"True":"False"));
                    _isPlaying = savedInstanceState.GetBoolean("isPlaying");
                    Log.Info(TAG, "OnCreate: - savedInstanceState value - isPlaying - " + (_isPlaying?"True":"False"));
                    if (_isPlaying || _isPaused)
                    {
                        _currentTrackPosition = savedInstanceState.GetInt("trackPosition");
                        Log.Info(TAG, "OnCreate: isPlaying, currentTrackPosition - " + _currentTrackPosition.ToString());
                    }
                    configurationChange = true;
                }
                if ((Intent != null && Intent.HasExtra("selectedIndex")))
                {
                    _selectedListItemIndex = Intent.GetIntExtra("selectedIndex", -1);
                }
                Log.Info(TAG, "OnCreate: _selectedListItemIndex is " + _selectedListItemIndex.ToString());
                GetFieldComponents();

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.cds,
                    new ImageLoadingListener
                    (
                        loadingComplete: (imageUri, view, loadedImage) =>
                        {
                            var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                            ImageLoader_LoadingComplete(null, args);
                        }
                    )
                );

                SetupCallbacks();

                if (Intent != null && Intent.HasExtra("playListID"))
                {
                    _playListID = Intent.GetIntExtra("playListID", -1);
                    Log.Info(TAG, "OnCreate: playListID - " + _playListID.ToString());
                }

                PlayList playList = GlobalData.PlayListItems.Find(play => play.PlayListID == _playListID);
                if (playList != null)
                    _playListName.Text = playList.PlayListName.Trim();


                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.musicTrackListToolbar, Resource.String.MusicPlayListTracksToolbarTitle, Color.White);

                if(_trackProgress != null)
                {
                    //change the colour of the progressbar (primary progress is on layer 2, layer 0 is background, layer 1 is secondary progress)
                    LayerDrawable layers = (LayerDrawable)_trackProgress.ProgressDrawable;
                    layers.GetDrawable(0).SetColorFilter(Color.LightBlue, PorterDuff.Mode.SrcIn);
                    layers.GetDrawable(2).SetColorFilter(Color.DarkBlue, PorterDuff.Mode.SrcIn);
                }

                UpdateAdapter();

                SetupMediaPlayer();

                //do we have permission to play at this time?
                if (PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.ReadExternalStorage))
                {
                    Log.Info(TAG, "OnCreate: Permission to read external storage is true");
                    if (_selectedListItemIndex != -1)
                    {
                        Log.Info(TAG, "OnCreate: isPlaying, calling Play_Click, selectedListItemIndex - " + _selectedListItemIndex.ToString());
                        Play_Click(null, null);
                    }
                    else
                    {
                        Log.Info(TAG, "OnCreate: No selected index (-1), configurationChange is " + (configurationChange ? "True" : "False") + ", _isPlaying is " + (_isPlaying ? "True" : "False") + ", _isPaused is " + (_isPaused ? "True" : "False"));
                        if (configurationChange && (_isPlaying || _isPaused))
                            Play_Click(null, null);
                    }
                }
                else
                {
                    Log.Info(TAG, "OnCreate: Permission to read external storage is FALSE");
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMusicPlayListTracksActivityCreate), "MusicPlayListTracksActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_trackList != null)
                _trackList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupMediaPlayer()
        {
            Log.Info(TAG, "SetupMediaPlayer: Checking if media player is configured...");
            if (!_mediaPlayerConfigured)
            {
                Log.Info(TAG, "SetupMediaPlayer: Media player is NOT configured - creating...");
                _mediaPlayer = new MediaPlayer();
                if (_mediaPlayer != null)
                {
                    Log.Info(TAG, "SetupMediaPlayer: Setting callbacks and audio stream");
                    _mediaPlayer.Prepared += MediaPlayer_Prepared;
                    _mediaPlayer.Completion += MediaPlayer_Completion;
                    _mediaPlayer.SetAudioStreamType(Stream.Music);
                    _mediaPlayerConfigured = true;
                }
            }
            else
            {
                Log.Info(TAG, "SetupMediaPlayer: Media player is configured");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MusicPlayListTracksMenu, menu);

            SetActionIcons(menu);

            _menu = menu;

            return true;
        }

        private void MediaPlayer_Completion(object sender, EventArgs e)
        {
            try
            {
                if (_mediaPlayer != null)
                {
                    if (_mediaPlayer.IsPlaying)
                    {
                        _mediaPlayer.Stop();
                        _isPlaying = false;
                    }
                    if(_timerHandler != null)
                    {
                        _timerHandler.RemoveCallbacks(_timerRunnable);
                    }
                    _mediaPlayer.Reset();
                    var playList = GlobalData.PlayListItems.Find(list => list.PlayListID == _playListID);
                    if (playList != null)
                    {
                        if (_currentlyPlayingTrack < (playList.PlayListTracks.Count - 1))
                        {
                            _currentlyPlayingTrack++;
                            _selectedListItemIndex = _currentlyPlayingTrack;
                            UpdateAdapter();
                            SetSelectedTrack(_selectedListItemIndex);
                            _trackList.SetSelection(_selectedListItemIndex);
                            _mediaPlayer.PrepareAsync();
                        }
                        else
                        {
                            //we've reached the end of the track list so reset the currently playing item
                            _currentlyPlayingTrack = -1;
                            _isPaused = false;
                            _isPlaying = false;
                            SetActionButtonsAvailability(true);
                            UpdateAdapter();
                        }
                    }
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "MediaPlayer_Completion: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicPlayListTracksActivityCompletion), "MusicPlayListTracksActivity.MediaPlayer_Completion");
            }
        }

        private void MediaPlayer_Prepared(object sender, EventArgs e)
        {
            try
            {
                Log.Info(TAG, "MediaPlayer_Prepared: Checking selected list item index");
                if (_selectedListItemIndex != -1)
                {
                    Log.Info(TAG, "MediaPlayer_Prepared: _selectedListItemIndex is " + _selectedListItemIndex.ToString());
                    if (_trackProgress != null)
                    {
                        var playList = GlobalData.PlayListItems.Find(play => play.PlayListID == _playListID);
                        if(playList != null)
                        {
                            Log.Info(TAG, "MediaPlayer_Prepared: Found playlist '" + playList.PlayListName);
                            _trackProgress.Max = (int)playList.PlayListTracks[_selectedListItemIndex].TrackDuration;
                            _trackProgress.Progress = 0;
                        }
                         
                        _timerHandler = new Handler();
                        _timerRunnable = new Runnable(new Action(UpdateProgressBar));
                        //start the timer immediately
                        _timerHandler.PostDelayed(_timerRunnable, 0);
                    }
                    _isPlaying = true;
                    //Merely calling UpdateAdapter here will not work (on ICS anyway) as it re-creates the datasource and won't start the play animation
                    //however, if we refresh using NotifyDataSetChanged it will work!
                    if (_trackList != null && _trackList.Adapter != null)
                        ((BaseAdapter)_trackList.Adapter).NotifyDataSetChanged();
                    Log.Info(TAG, "MediaPlayer_Prepared: Setting tracklist selection to " + _selectedListItemIndex.ToString());
                    _trackList.SetSelection(_selectedListItemIndex);
                    SetActionButtonsAvailability(false);
                    Log.Info(TAG, "MediaPlayer_Prepared: Starting Media Player");
                    _mediaPlayer.Start();
                    if(_currentTrackPosition != -1)
                    {
                        _mediaPlayer.SeekTo(_currentTrackPosition);
                        _currentTrackPosition = -1;
                        if (_isPaused)
                            _mediaPlayer.Pause();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "MediaPlayer_Prepared: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicPlayListTracksActivityPrepared), "MusicPlayListTracksActivity.MediaPlayer_Prepared");
            }
        }

        private void UpdateProgressBar()
        {
            if(_trackProgress != null)
            {
                _trackProgress.Progress = _mediaPlayer.CurrentPosition;
                _timerHandler.PostDelayed(_timerRunnable, 500);
            }
        }

        private void UpdateAdapter()
        {
            Log.Info(TAG, "UpdateAdapter: Updating!");
            try
            {
                MusicPlayListTracksListAdapter trackList = new MusicPlayListTracksListAdapter(this, _playListID);
                if (_trackList != null)
                    _trackList.Adapter = trackList;
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMusicPlayListTracksActivityUpdateAdapter), "MusicPlayListTracksActivity.UpdateAdapter");
            }
        }

        private void SetupCallbacks()
        {
            if(_trackList != null)
                _trackList.ItemClick += TrackList_ItemClick;
            if(_previous != null)
                _previous.Click += Previous_Click;
            if(_stop != null)
                _stop.Click += Stop_Click;
            if(_pause != null)
                _pause.Click += Pause_Click;
            if(_play != null)
                _play.Click += Play_Click;
            if(_next != null)
                _next.Click += Next_Click;
        }

        private void Next_Click(object sender, EventArgs e)
        {
            //Stop current if playing, if pos < count-1, set selected index to next item, updateadapter then start playing
            try
            {
                if (_mediaPlayer != null)
                {
                    if (_mediaPlayer.IsPlaying || _isPaused)
                    {
                        if(_timerHandler != null)
                        {
                            _timerHandler.RemoveCallbacks(_timerRunnable);
                        }
                        _mediaPlayer.Stop();
                        _mediaPlayer.Reset();
                        _isPaused = false;
                        _isPlaying = false;
                    }

                    var playList = GlobalData.PlayListItems.Find(list => list.PlayListID == _playListID);
                    if (playList != null)
                    {
                        if (_selectedListItemIndex < (playList.PlayListTracks.Count - 1))
                        {
                            _selectedListItemIndex++;
                            SetSelectedTrack(_selectedListItemIndex);
                            _currentlyPlayingTrack = _selectedListItemIndex;
                            UpdateAdapter();
                            _trackList.SetSelection(_selectedListItemIndex);
                            _mediaPlayer.PrepareAsync();
                        }
                        else
                        {
                            _currentlyPlayingTrack = -1;
                            SetActionButtonsAvailability(true);
                            UpdateAdapter();
                        }
                    }
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "Next_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicPlayListTracksActivityNext), "MusicPlayListTracksActivity.Next_Click");
            }
        }

        private void Play_Click(object sender, EventArgs e)
        {
            //if there is a current selection and it is not playing, then play it
            try
            {
                if (PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.ReadExternalStorage))
                {
                    if (_mediaPlayer != null)
                    {
                        if (!_mediaPlayerConfigured)
                        {
                            Log.Info(TAG, "Play_Click: Player not configured - calling SetupMediaPlayer...");
                            SetupMediaPlayer();
                        }
                        Log.Info(TAG, "Play_Click: currentlyPlayingTrack - " + _currentlyPlayingTrack.ToString());
                        Log.Info(TAG, "Play_Click: selectedItemIndex - " + _selectedListItemIndex.ToString());
                        if (_currentlyPlayingTrack != _selectedListItemIndex)
                        {
                            Log.Info(TAG, "Play_Click: isPlaying - " + (_isPlaying ? "True" : "False"));
                            Log.Info(TAG, "Play_Click: isPaused - " + (_isPaused ? "True" : "False"));
                            if (_mediaPlayer.IsPlaying || _isPaused)
                            {
                                if (_timerHandler != null)
                                {
                                    _timerHandler.RemoveCallbacks(_timerRunnable);
                                }
                                else
                                {
                                    Log.Info(TAG, "Play_Click: timerHandler is NULL!");
                                }
                                _mediaPlayer.Stop();
                                _mediaPlayer.Reset();
                                if (_currentTrackPosition == -1)
                                {
                                    _isPaused = false;
                                    _isPlaying = false;
                                }
                            }
                            SetSelectedTrack(_selectedListItemIndex);
                            _mediaPlayer.PrepareAsync();
                        }
                    }
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "Play_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicPlayListTracksActivityPlay), "MusicPlayListTracksActivity.Play_Click");
            }
        }

        private void Pause_Click(object sender, EventArgs e)
        {
            //if there is a track playing, then pause it
            try
            {
                if (_mediaPlayer != null)
                {
                    if (_mediaPlayer.IsPlaying)
                    {
                        _mediaPlayer.Pause();
                        _isPaused = true;
                        _isPlaying = false;
                    }
                    else
                    {
                        if (_isPaused)
                        {
                            _mediaPlayer.Start();
                            _isPaused = false;
                            _isPlaying = true;
                        }
                    }
                    UpdateAdapter();
                    if(_selectedListItemIndex != -1)
                    {
                        if (_trackList != null)
                            _trackList.SetSelection(_selectedListItemIndex);
                    }
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "Pause_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicPlayListTracksActivityPause), "MusicPlayListTracksActivity.Pause_Click");
            }
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            //if there is a track playing, stop it
            try
            {
                if (_mediaPlayer != null)
                {
                    if (_mediaPlayer.IsPlaying || _isPaused)
                    {
                        if(_timerHandler != null)
                        {
                            _timerHandler.RemoveCallbacks(_timerRunnable);
                        }
                        _mediaPlayer.Stop();
                        _mediaPlayer.Reset();
                        _isPaused = false;
                        _currentlyPlayingTrack = -1;
                        _isPlaying = false;
                        UpdateAdapter();
                        SetActionButtonsAvailability(true);
                        if(_selectedListItemIndex != -1)
                        {
                            if (_trackList != null)
                                _trackList.SetSelection(_selectedListItemIndex);
                        }
                    }
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "Stop_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicPlayListTracksActivityStop), "MusicPlayListTracksActivity.Stop_Click");
            }
        }

        private void Previous_Click(object sender, EventArgs e)
        {
            //if there is a track playing, then stop it
            //if the position is greater than 0, then set selected to --
            //update adapter
            //play track
            try
            {
                if (_mediaPlayer != null)
                {
                    if (_mediaPlayer.IsPlaying || _isPaused)
                    {
                        if(_timerHandler != null)
                        {
                            _timerHandler.RemoveCallbacks(_timerRunnable);
                        }
                        _mediaPlayer.Stop();
                        _mediaPlayer.Reset();
                        _isPaused = false;
                        _isPlaying = false;
                    }

                    if (_selectedListItemIndex > 0)
                    {
                        _selectedListItemIndex--;
                        SetSelectedTrack(_selectedListItemIndex);
                        _currentlyPlayingTrack = _selectedListItemIndex;
                        UpdateAdapter();
                        _trackList.SetSelection(_selectedListItemIndex);
                        _mediaPlayer.PrepareAsync();
                    }
                    else
                    {
                        _currentlyPlayingTrack = -1;
                        SetActionButtonsAvailability(true);
                        UpdateAdapter();
                    }
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "Previous_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicPlayListTracksActivityPrevious), "MusicPlayListTracksActivity.Previous_Click");
            }
        }

        private void Add()
        {
            try
            {
                if(!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.ReadExternalStorage) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.ReadExternalStorage)))
                {
                    Toast.MakeText(this, Resource.String.ReadExternalStoragePermissionDenialToast, ToastLength.Short).Show();
                    return;
                }

                //go to music picker dialog
                MusicPickerDialogFragment musicDialogFragment = new MusicPickerDialogFragment(this, _playListID, "Add Tracks");
                var transaction = FragmentManager.BeginTransaction();
                musicDialogFragment.Show(transaction, musicDialogFragment.Tag);
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicPlayListTracksActivityAdd), "MusicPlayListTracksActivity.Add_Click");
            }
        }

        private void Remove()
        {
            try
            {
                //remove selected track from the playlist
                if (_mediaPlayer != null)
                {
                    if (_mediaPlayer.IsPlaying || _isPaused)
                    {
                        if(_timerHandler != null)
                        {
                            _timerHandler.RemoveCallbacks(_timerRunnable);
                        }
                        _mediaPlayer.Stop();
                        _mediaPlayer.Reset();
                        _isPaused = false;
                        _isPlaying = false;
                    }

                    var playList = GlobalData.PlayListItems.Find(list => list.PlayListID == _playListID);
                    if (playList != null)
                    {
                        if (_selectedListItemIndex == -1)
                        {
                            if (playList.PlayListTracks.Count == 0)
                            {
                                Toast.MakeText(this, Resource.String.MusicPlayListTracksActivityRemoveWarning1, ToastLength.Short).Show();
                            }
                            else
                            {
                                Toast.MakeText(this, Resource.String.MusicPlayListTracksActivityRemoveWarning2, ToastLength.Short).Show();
                            }
                            return;
                        }

                        AlertHelper alertHelper = new AlertHelper(this);
                        alertHelper.AlertIconResourceID = Resource.Drawable.notes;
                        alertHelper.AlertTitle = GetString(Resource.String.MusicPlayListTrackRemoveAlertTitle);
                        alertHelper.AlertMessage = GetString(Resource.String.MusicPlayListTrackRemoveAlertMessage);
                        alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                        alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                        alertHelper.InstanceId = "999";
                        alertHelper.ShowAlert();
                    }
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "Remove_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicPlayListTracksActivityRemove), "MusicPlayListTracksActivity.Remove_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.musicplaylisttracksActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.musicplaylisttracksActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.musicplaylisttracksActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "MusicPlayListTracksActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            //if a track is playing, stop it and nav back to playlist activity
            if(_mediaPlayer != null)
            {
                if (_mediaPlayer.IsPlaying || _isPaused)
                {
                    if(_timerHandler != null)
                    {
                        _timerHandler.RemoveCallbacks(_timerRunnable);
                    }
                    _mediaPlayer.Stop();
                    _mediaPlayer.Reset();
                    _isPaused = false;
                    _isPlaying = false;
                }
            }

            Intent intent = new Intent(this, typeof(MusicTherapyActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(_mediaPlayer != null)
            {
                if (_mediaPlayer.IsPlaying || _isPaused)
                {
                    if(_timerHandler != null)
                    {
                        _timerHandler.RemoveCallbacks(_timerRunnable);
                    }
                    _mediaPlayer.Stop();
                    _mediaPlayer.Reset();
                    _isPlaying = false;
                }

                _mediaPlayer.Release();
                _mediaPlayer = null;
            }
        }

        private void TrackList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedListItemIndex = e.Position;
            UpdateAdapter();
            _trackList.SetSelection(_selectedListItemIndex);
        }

        private void GetFieldComponents()
        {
            try
            {
                _playListName = FindViewById<TextView>(Resource.Id.txtMusicTrackListPlayListText);
                _trackList = FindViewById<ListView>(Resource.Id.lstMusicTrackList);
                _previous = FindViewById<ImageButton>(Resource.Id.imgbtnMusicTrackListPrevious);
                _stop = FindViewById<ImageButton>(Resource.Id.imgbtnMusicTrackListStop);
                _pause = FindViewById<ImageButton>(Resource.Id.imgbtnMusicTrackListPause);
                _play = FindViewById<ImageButton>(Resource.Id.imgbtnMusicTrackListPlay);
                _next = FindViewById<ImageButton>(Resource.Id.imgbtnMusicTrackListNext);
                _trackProgress = FindViewById<ProgressBar>(Resource.Id.prgTrackProgress);
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMusicPlayListTracksActivityGetComponents), "MusicPlayListTracksActivity.GetFieldComponents");
            }
        }

        public void ConfirmAddition(int playListID, List<Track> _tracks)
        {
            try
            {
                //we need to make sure that tracks are not being duplicated, so we will check via the Uri
                var currentTrackList = GlobalData.PlayListItems.Find(list => list.PlayListID == playListID).PlayListTracks;

                if (currentTrackList != null)
                {
                    foreach (Track track in _tracks)
                    {
                        var theTrack = currentTrackList.Find(trk => trk.TrackUri == track.TrackUri);
                        if (theTrack == null)
                        {
                            //we don't have this track, so add it
                            Track newTrack = new Track();
                            newTrack.PlayListID = playListID;
                            newTrack.TrackArtist = track.TrackArtist;
                            newTrack.TrackDuration = track.TrackDuration;
                            newTrack.TrackName = track.TrackName;
                            newTrack.TrackOrderNumber = track.TrackOrderNumber;
                            newTrack.TrackUri = track.TrackUri;
                            newTrack.Save();
                            var playList = GlobalData.PlayListItems.Find(list => list.PlayListID == _playListID);
                            if (playList != null)
                            {
                                playList.PlayListTracks.Add(newTrack);
                            }
                        }
                    }
                    _selectedListItemIndex = -1;
                    UpdateAdapter();
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "ConfirmAddition: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMusicPlayListTracksActivityConfirm), "MusicPlayListTracksActivity.ConfirmAddition");
            }
        }

        public void CancelAddition()
        {
            Toast.MakeText(this, GetString(Resource.String.MusicPlayListTracksActivityCancelToast), ToastLength.Short).Show();
        }

        private void SetSelectedTrack(int selectedTrack)
        {
            try
            {
                if (PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.ReadExternalStorage))
                {
                    var playList = GlobalData.PlayListItems.Find(list => list.PlayListID == _playListID);
                    if (playList != null)
                    {
                        Log.Info(TAG, "SetSelectedTrack: Found playlist - " + playList.PlayListName.Trim());
                        Android.Net.Uri trackUri = Android.Net.Uri.Parse(playList.PlayListTracks[selectedTrack].TrackUri);
                        Log.Info(TAG, "SetSelectedTrack: parameter selectedTrack has value of - " + selectedTrack.ToString());
                        Log.Info(TAG, "SetSelectedTrack: Uri is '" + playList.PlayListTracks[selectedTrack].TrackUri + "'");
                        _mediaPlayer.SetDataSource(this, trackUri);

                        _currentlyPlayingTrack = selectedTrack;
                    }
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "SetSelectedTrack: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMusicPlaylistTracksSetSelected), "MusicPlayListTracksActivity.SetSelectedTrack");
            }
        }

        private void SetActionButtonsAvailability(bool availability)
        {
            try
            {
                if (_menu != null)
                {
                    var itemAdd = _menu.FindItem(Resource.Id.musicplaylisttracksActionAdd);
                    var itemRemove = _menu.FindItem(Resource.Id.musicplaylisttracksActionRemove);

                    switch (availability)
                    {
                        case false:
                            if (itemAdd != null)
                            {
                                itemAdd.SetEnabled(false);
                            }
                            if (itemRemove != null)
                            {
                                itemRemove.SetEnabled(false);
                            }
                            break;
                        case true:
                        default:
                            if (itemAdd != null)
                            {
                                itemAdd.SetEnabled(true);
                            }
                            if (itemRemove != null)
                            {
                                itemRemove.SetEnabled(true);
                            }
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "SetActionButtonsAvailability: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorTellMyselfSettingActionButtons), "MusicPlayListTracksActivity.SetActionButtonsAvailability");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "readExternalStorage")
            {
                Log.Info(TAG, "AlertPositiveButtonSelect: Instance id is 'readExternalStorage', requesting application permission");
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.ReadExternalStorage);
                return;
            }

            try
            {
                var playList = GlobalData.PlayListItems.Find(list => list.PlayListID == _playListID);
                if (playList != null)
                {
                    var track = playList.PlayListTracks[_selectedListItemIndex];

                    var trackList = GlobalData.PlayListItems.Find(tlst => tlst.PlayListID == _playListID);
                    if (trackList != null)
                    {
                        var globalTrack = trackList.PlayListTracks.Find(trk => trk.TrackID == track.TrackID);
                        if (globalTrack != null)
                            trackList.PlayListTracks.Remove(globalTrack);
                    }
                    track.Remove();

                    _selectedListItemIndex = -1;

                    UpdateAdapter();
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMusicPlaylistTracksRemoveTrack), "MusicPlayListTracksActivity.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "readExternalStorage")
            {
                Toast.MakeText(this, Resource.String.ReadExternalStoragePermissionDenialToast, ToastLength.Short).Show();
                return;
            }

            Toast.MakeText(this, Resource.String.MusicPlayListTrackRemoveNoActionToast, ToastLength.Short).Show();
        }

        private void CheckReadExternalStoragePermission()
        {
            try
            {
                if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.ReadExternalStorage) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.ReadExternalStorage)))
                {
                    Log.Info(TAG, "CheckReadExternalStoragePermission: Attempting permission request");
                    AttemptPermissionRequest();
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "CheckReadExternalStoragePermission: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCheckingApplicationPermission), "MusicPlayListTracksActivity.CheckReadExternalStoragePermission");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            try
            {
                if (requestCode == ConstantsAndTypes.REQUEST_CODE_PERMISSION_READ_EXTERNAL_STORAGE)
                {
                    //only the newer android will use this
                    if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                    {
                        GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == ConstantsAndTypes.AppPermission.ReadExternalStorage).PermissionGranted = Permission.Granted;
                        return;
                    }

                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //now update the global permission
                        if (GlobalData.ApplicationPermissions == null)
                        {
                            //if null then we can go get permissions
                            PermissionsHelper.SetupDefaultPermissionList(this);
                        }
                        else
                        {
                            //we need to update the existing permission
                            if (PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.ReadExternalStorage))
                            {
                                GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == ConstantsAndTypes.AppPermission.ReadExternalStorage).PermissionGranted = Permission.Granted;
                            }
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.ReadExternalStoragePermissionDenialToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedicationListProcessResponse), "MusicPlayListTracksActivity.OnRequestPermissionsResult");
            }
        }

        private void ShowPermissionRationale()
        {
            try
            {
                if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagReadExternalStorage").SettingValue == "True") return;

                AlertHelper alertHelper = new AlertHelper(this);

                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolInformation;
                alertHelper.AlertMessage = GetString(Resource.String.RequestPermissionReadExternalStorageAlertMessage);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertTitle = GetString(Resource.String.RequestPermissionReadExternalStorageAlertTitle);
                alertHelper.InstanceId = "readExternalStorage";
                alertHelper.ShowAlert();
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "ShowPermissionRationale: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorReadExternalStoragePermissionShowRationaleAlert), "MusicPlayListTracksActivity.ShowPermissionRationale");
            }
        }

        public void AttemptPermissionRequest()
        {
            try
            {
                Log.Info(TAG, "AttemptPermissionRequest: Determining whether to show Permission Rationale...");
                if (PermissionsHelper.ShouldShowPermissionRationale(this, ConstantsAndTypes.AppPermission.ReadExternalStorage))
                {
                    Log.Info(TAG, "AttemptPermissionRequest: Showing Permission Rationale");
                    ShowPermissionRationale();
                    return;
                }
                else
                {
                    //just request the permission
                    Log.Info(TAG, "AttemptPermissionRequest: Not showing Rationale, requesting application permission ReadExternalStorage");
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.ReadExternalStorage);
                    return;
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "AttemptPermissionRequest: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "MusicPlayListTracksActivity.AttemptPermissionRequest");
            }
        }
    }
}
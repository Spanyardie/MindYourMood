using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class MusicPickerDialogFragment : DialogFragment
    {
        public const string TAG = "M:MusicPickerDialogFragment";

        private MusicPickerTrackListAdapter _trackListAdapter;

        private Activity _activity;

        private TextView _playListName;
        private ListView _playListTracks;
        private Button _goBack;
        private Button _add;

        private int _PlayListID = -1;

        private string _dialogTitle = "";

        private ImageLoader _imageLoader = null;

        public MusicPickerDialogFragment()
        {

        }

        public MusicPickerDialogFragment(Activity activity, int playListID, string title)
        {
            try
            {
                _activity = activity;
                _PlayListID = playListID;
                _trackListAdapter = new MusicPickerTrackListAdapter(_activity, playListID);
                _dialogTitle = title;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Constructor: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMusicPickerDialogFragmentCreate), ".Constructor");
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutInt("playListID", _PlayListID);
                outState.PutString("dialogTitle", _dialogTitle);
            }

            base.OnSaveInstanceState(outState);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            if (context != null)
                _activity = (Activity)context;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = null;
            try
            {
                if(savedInstanceState != null)
                {
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                    _PlayListID = savedInstanceState.GetInt("playListID");
                    _trackListAdapter = new MusicPickerTrackListAdapter(Activity, _PlayListID);
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                view = inflater.Inflate(Resource.Layout.MusicPickerDialogLayout, container, false);
                GetFieldComponents(view);

                if(_playListName != null)
                {
                    var playlist = GlobalData.PlayListItems.Find(play => play.PlayListID == _PlayListID);
                    if(playlist != null)
                    {
                        _playListName.Text = playlist.PlayListName.Trim();
                    }
                }
                if(_playListTracks != null)
                {
                    _playListTracks.Adapter = _trackListAdapter;
                }

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.musicdj,
                    new ImageLoadingListener
                    (
                        loadingComplete: (imageUri, viewImg, loadedImage) =>
                        {
                            var args = new LoadingCompleteEventArgs(imageUri, viewImg, loadedImage);
                            ImageLoader_LoadingComplete(null, args);
                        }
                    )
                );

                SetupCallbacks();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMusicPickerDialogFragmentCreateView), "MusicPickerDialogFragment.OnCreateView");
            }
            return view;
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_playListTracks != null)
                _playListTracks.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            if(_goBack != null)
            {
                _goBack.Click += GoBack_Click;
            }
            if(_add != null)
            {
                _add.Click += Add_Click;
            }
        }

        public override void OnResume()
        {
            var attrs = Dialog.Window.Attributes;
            attrs.Width = LinearLayout.LayoutParams.MatchParent;
            Dialog.Window.Attributes = attrs;

            base.OnResume();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_trackListAdapter != null)
                {
                    var selectedTracks =
                    (from eachTrack in _trackListAdapter._tracksOnDevice
                     where eachTrack.TrackSelected
                     select eachTrack).ToList();

                    List<Track> addedTracks = new List<Model.Track>();
                    foreach (MusicPickerTrackListAdapter.ExtendedTrack track in selectedTracks)
                    {
                        Track newTrack = new Track();
                        newTrack.PlayListID = _PlayListID;
                        newTrack.TrackArtist = track.TrackArtist;
                        newTrack.TrackDuration = track.TrackDuration;
                        newTrack.TrackName = track.TrackName;
                        newTrack.TrackOrderNumber = track.TrackOrderNumber;
                        newTrack.TrackUri = track.TrackUri;
                        addedTracks.Add(newTrack);
                    }

                    ((IMusicPickerCallback)Activity).ConfirmAddition(_PlayListID, addedTracks);
                    Dismiss();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorMusicPickerDialogFragmentAdd), "MusicPickerDialogFragment.Add_Click");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if(view != null)
                {
                    _goBack = view.FindViewById<Button>(Resource.Id.btnMusicPickerGoBack);
                    _add = view.FindViewById<Button>(Resource.Id.btnMusicPickerAdd);
                    _playListTracks = view.FindViewById<ListView>(Resource.Id.lstMusicPicker);
                    _playListName = view.FindViewById<TextView>(Resource.Id.txtMusicPickerPlayListText);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMusicPickerDialogFragmentGetComponents), "MusicPickerDialogFragment.GetFieldComponents");
            }
        }

        private void UpdateAdapter()
        {
            if(_playListTracks != null)
            {
                _trackListAdapter.NotifyDataSetChanged();
            }
        }
    }
}
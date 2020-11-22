using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MikePhil.Charting.Charts;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class SummaryActivity : AppCompatActivity
    {
        public const string TAG = "M:SummaryActivity";

        private Toolbar _toolbar;
        private ImageButton _pictureOfTheDay;
        private LinearLayout _weeksProgress;
        private LinearLayout _notificationContainer;
        private LinearLayout _summaryMain;
        private ImageButton _musicImage;
        private TextView _musicTrack;
        private TextView _trackLabel;
        private TextView _happyPicture;
        private TextView _weeksProgressLabel;
        private Button _done;
        private int _selectedTrackIndex = -1;
        private string _currentPictureUri;
        private int _playListID = -1;
        private int _trackID = -1;

        private ImageLoader _imageLoader = null;

        private bool _isLoadingBackground = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SummaryLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.summarytoolbar, Resource.String.SummaryTitle, Color.White);

            GetFieldComponents();

            _imageLoader = ImageLoader.Instance;

            _isLoadingBackground = true;
            _imageLoader.LoadImage
            (
                "drawable://" + Resource.Drawable.summarymain, 
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

            GetPositivePictureForToday();
            if(_currentPictureUri == "")
            {
                if (_happyPicture != null)
                    _happyPicture.Text = GetString(Resource.String.SummaryNoImages);
            }

            SetupWeeksProgress();

            SetupRandomNotification();

            GetRandomMusicTrack();
        }

        private void MusicImage_LongClick(object sender, View.LongClickEventArgs e)
        {
            if (_selectedTrackIndex != -1)
            {
                Intent intent = new Intent(this, typeof(MusicPlayListTracksActivity));
                intent.PutExtra("playListID", _playListID);
                intent.PutExtra("selectedIndex", _selectedTrackIndex);
                StartActivity(intent);
            }
        }

        private void PictureOfTheDay_LongClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentPictureUri))
            {
                Intent intent = new Intent(this, typeof(ImageDetailActivity));
                intent.PutExtra("imageUri", _currentPictureUri);
                intent.PutExtra("imageComment", GetString(Resource.String.MainActivityPositivePictureText));

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this, _pictureOfTheDay, "imageTransition");
                    Log.Info(TAG, "ImageGrid_ItemClick: Created activity options for image transition as Version code >= Lollipop");
                    StartActivity(intent, options.ToBundle());
                }
                else
                {
                    Log.Info(TAG, "ImageGrid_ItemClick: Starting regular activity");
                    StartActivity(intent);
                }
            }
        }

        private void GetFieldComponents()
        {
            try
            {

                _pictureOfTheDay = FindViewById<ImageButton>(Resource.Id.imgbtnMainActivityPositivePicture);
                _weeksProgress = FindViewById<LinearLayout>(Resource.Id.linMainActivityProgressContainer);


                _notificationContainer = FindViewById<LinearLayout>(Resource.Id.linMainActivityRecentAchievement);

                _musicImage = FindViewById<ImageButton>(Resource.Id.imgbtnMainActivityHappyMusic);
                _musicTrack = FindViewById<TextView>(Resource.Id.txtHappyMusic);
                _trackLabel = FindViewById<TextView>(Resource.Id.txtHappyTrackLabel);

                _happyPicture = FindViewById<TextView>(Resource.Id.txtMainActivityImageryTitle);
                _weeksProgressLabel = FindViewById<TextView>(Resource.Id.txtWeeksProgressLabel);

                _done = FindViewById<Button>(Resource.Id.btnDone);

                _summaryMain = FindViewById<LinearLayout>(Resource.Id.linSummaryMain);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMainActivityGetComponents), "SummaryActivity.GetFieldComponents");
            }
        }
        private void SetupCallbacks()
        {
            try
            {
                if(_pictureOfTheDay != null)
                    _pictureOfTheDay.LongClick += PictureOfTheDay_LongClick;

                if(_musicImage != null)
                    _musicImage.LongClick += MusicImage_LongClick;
                if(_musicTrack != null)
                    _musicTrack.LongClick += MusicImage_LongClick;

                if(_done != null)
                    _done.Click += Done_Click;

                if(_imageLoader != null)
                    _imageLoader.LoadingComplete += ImageLoader_LoadingComplete;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMainSetupCallbacks), "SummaryActivity.SetupCallbacks");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            if (_isLoadingBackground)
            {
                var bitmap = e.LoadedImage;

                if (_summaryMain != null)
                    _summaryMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
                _isLoadingBackground = false;
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void GetPositivePictureForToday()
        {
            try
            {
                ImageryHelper imageHelper = new ImageryHelper(this, _pictureOfTheDay);
                imageHelper.GetPositivePictureOfTheDay();
                _currentPictureUri = imageHelper.PictureOfTheDayUri;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetPositivePictureForToday: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMainActivityPositivePic), "SummaryActivity.GetPositivePictureForToday");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SummaryMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    Finish();
                    return true;
                }
                if(item.ItemId == Resource.Id.summaryActionHelp)
                {
                    Intent intent = new Intent(this, typeof(SummaryHelpActivity));
                    StartActivity(intent);
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void SetupWeeksProgress()
        {
            LineChart lineChart = null;
            try
            {
                if (_weeksProgress != null)
                {
                    var fromDate = DateTime.Now.AddDays(-7);
                    var toDate = DateTime.Now;
                    lineChart = new LineChart(this);
                    var progressHelper = new ProgressChartHelper(this, _weeksProgress, lineChart);
                    _weeksProgress = progressHelper.SetupLineChart();
                    lineChart = progressHelper.SetupChartData(fromDate, toDate);
                }

                if (_weeksProgressLabel != null)
                {
                    if (lineChart != null && lineChart.LineData.YMax < 0.002)
                    {
                        _weeksProgressLabel.Text = GetString(Resource.String.SummaryNoThoughtData);
                    }
                    else
                    {
                        _weeksProgressLabel.Text = GetString(Resource.String.MainActivityMyProgressLabel);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupWeeksProgress: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMainActivityWeeksProgress), "SummaryActivity.SetupWeeksProgress");
            }
        }

        private void GetRandomMusicTrack()
        {
            try
            {
                if (GlobalData.PlayListItems != null)
                {
                    if (GlobalData.PlayListItems.Count > 0)
                    {
                        string trackName = GetString(Resource.String.MainActivityTrackDefault);
                        string trackLabel = GetString(Resource.String.SummaryNoMusic);
                        Random random = new Random();
                        var index = random.Next(GlobalData.PlayListItems.Count);
                        PlayList playList = GlobalData.PlayListItems[index];
                        if (playList.PlayListTracks != null && playList.PlayListTracks.Count > 0)
                        {
                            var trackIndex = random.Next(playList.PlayListTracks.Count);
                            var track = playList.PlayListTracks[trackIndex];
                            trackName = track.TrackName.Trim() + "\n" + track.TrackArtist.Trim();
                            _playListID = track.PlayListID;
                            _trackID = track.TrackID;
                            _selectedTrackIndex = trackIndex;
                                trackLabel = GetString(Resource.String.MainActivityMusicSelect);
                        }
                        if(_musicTrack != null)
                            _musicTrack.Text = trackName.Trim();
                        if (_trackLabel != null)
                            _trackLabel.Text = trackLabel.Trim();
                    }
                }
                else
                {
                    Log.Error(TAG, "GetRandomMusicTrack: PlayListItems is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetRandomMusicTrack: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "", "SummaryActivity.GetRandomMusicTrack");
            }
        }

        private void SetupRandomNotification()
        {
            if (_notificationContainer != null)
            {
                MainNotificationHelper notifyHelp = new MainNotificationHelper(this);
                LinearLayout randomView = notifyHelp.GetRandomNotification();
                if (randomView != null)
                    _notificationContainer.AddView(randomView);
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHelp = menu.FindItem(Resource.Id.summaryActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SummaryActivity.SetActionIcons");
            }
        }
    }
}
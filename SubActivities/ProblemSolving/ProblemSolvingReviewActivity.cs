using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment.ProblemSolving.Review;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.SubActivities.ProblemSolving
{
    [Activity()]
    public class ProblemSolvingReviewActivity : AppCompatActivity
    {
        public const string TAG = "M:ProblemSolvingReviewActivity";

        private Toolbar _toolbar;

        private LinearLayout _reviewMainBody;
        private LinearLayout _problemSolvingReviewScrollerContainer;
        private Button _btnDone;

        private Problem _problem;
        private int _problemID = -1;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("problemID", _problemID);

            base.OnSaveInstanceState(outState);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.ProblemSolvingReview);
                Log.Info(TAG, "OnCreate: Set content view successfully");

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.problemSolvingReviewToolbar, Resource.String.ProblemSolvingActionBarTitle, Color.White);

                if (savedInstanceState != null)
                {
                    _problemID = savedInstanceState.GetInt("problemID");
                }
                else
                {
                    _problemID = Intent.GetIntExtra("problemID", -1);
                }

                GetFieldComponents();

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.problemsolving,
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

                GetProblemData();

                CreateViews(ProblemSolvingReviewHelper.ClearView.False);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingReviewActivityCreateView), "ProblemSolvingReviewActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_problemSolvingReviewScrollerContainer != null)
                _problemSolvingReviewScrollerContainer.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        protected override void OnResume()
        {
            base.OnResume();
            CreateViews(ProblemSolvingReviewHelper.ClearView.True);
        }

        private void CreateViews(ProblemSolvingReviewHelper.ClearView removeExistingChild = ProblemSolvingReviewHelper.ClearView.False)
        {
            try
            {
                if (_problemID != -1)
                {
                    ProblemSolvingReviewHelper reviewHelper = new ProblemSolvingReviewHelper(this, this);
                    LinearLayout linProblem = reviewHelper.GetProblemDisplay(_problemID);
                    if (_reviewMainBody != null)
                    {
                        if (removeExistingChild == ProblemSolvingReviewHelper.ClearView.True)
                            _reviewMainBody.RemoveAllViews();
                        _reviewMainBody.AddView(linProblem);
                    }
                }
                else
                {
                    Log.Error(TAG, "CreateViews: _problemID is INVALID!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "CreateViews: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingReviewActivityCreateViews), "ProblemSolvingReviewActivity.CreateViews");
            }
        }

        private void GetProblemData()
        {
            if (_problemID != -1)
            {
                _problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);
            }
            else
            {
                Log.Error(TAG, "GetProblemData: _problemID is INVALID!");
            }
        }

        private void GetFieldComponents()
        {
            _reviewMainBody = FindViewById<LinearLayout>(Resource.Id.linProblemSolvingReviewMainBody);
            _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            _problemSolvingReviewScrollerContainer = FindViewById<LinearLayout>(Resource.Id.linProblemSolvingReviewScrollerContainer);
        }

        private void SetupCallbacks()
        {
            if(_btnDone != null)
                _btnDone.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ProblemSolvingReviewMenu, menu);

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
                switch(item.ItemId)
                {
                    case Resource.Id.problemSolvingReviewActionHelp:
                        Intent intent = new Intent(this, typeof(ProblemSolvingReviewHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHelp = menu.FindItem(Resource.Id.problemSolvingReviewActionHelp);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ProblemSolvingReviewActivity.SetActionIcons");
            }
        }
    }
}
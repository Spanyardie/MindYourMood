using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Helpers;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment.ProblemSolving;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.SubActivities.ProblemSolving
{
    [Activity()]
    public class ProblemSolvingIdeasActivity : AppCompatActivity, IProblemSolvingIdeasCallback, IAlertCallback
    {
        public const string TAG = "M:ProblemSolvingIdeasActivity";

        private Toolbar _toolbar;

        private TextView _problemStepText;
        private ListView _problemIdeaList;
        private Button _btnDone;

        private int _problemStepID;
        private int _problemID;
        private string _stepText;
        private int _selectedItemIndex = -1;

        private ImageLoader _imageLoader = null;

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutInt("selectedItemIndex", _selectedItemIndex);
                outState.PutInt("problemID", _problemID);
                outState.PutInt("problemStepID", _problemStepID);
                outState.PutString("stepText", _stepText);
            }

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ProblemSolvingIdeasMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                {
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");
                    _problemID = savedInstanceState.GetInt("problemID");
                    _problemStepID = savedInstanceState.GetInt("problemStepID");
                    _stepText = savedInstanceState.GetString("stepText");
                }
                else
                {
                    _problemStepID = Intent.GetIntExtra("problemStepID", -1);
                    _problemID = Intent.GetIntExtra("problemID", -1);
                    _stepText = Intent.GetStringExtra("problemStepText");
                }
                SetContentView(Resource.Layout.ProblemSolvingIdeasLayout);
                Log.Info(TAG, "OnCreate: Set content view successfully, problemID - " + _problemID.ToString() + ", problemStepID - " + _problemStepID.ToString() + ", problemStepText - " + _stepText);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.problemSolvingIdeasToolbar, Resource.String.ProblemSolvingIdeasToolbarTitle, Color.White);

                GetFieldComponents();

                CheckMicPermission();

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.problemsteps,
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

                if (_problemStepID != -1)
                    _problemStepText.Text = _stepText.Trim();

                if (IsProblemSolved())
                    InflateResolved();

                UpdateAdapter();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingIdeasActivityCreateView), "ProblemSolvingIdeasActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_problemIdeaList != null)
                _problemIdeaList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
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
                    case Resource.Id.problemsolvingideasActionAdd:
                        Add();
                        return true;

                    case Resource.Id.problemsolvingideasActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.problemsolvingideasActionReview:
                        Review();
                        return true;

                    case Resource.Id.problemsolvingideasActionHelp:
                        Intent intent = new Intent(this, typeof(ProblemSolvingStepIdeasHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_problemIdeaList != null)
                {
                    _problemIdeaList.ItemClick += ProblemIdeaList_ItemClick;
                    _problemIdeaList.ItemLongClick += ProblemIdeaList_ItemLongClick;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _problemIdeaList is NULL!");
                }
                if(_btnDone != null)
                    _btnDone.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingIdeasActivitySetCallbacks), "ProblemSolvingIdeasActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void Review()
        {
            Intent intent = new Intent(this, typeof(ProblemSolvingReviewActivity));
            intent.PutExtra("problemID", _problemID);
            StartActivity(intent);
        }

        private void ProblemIdeaList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            ProblemIdea problemIdea = null;
            ProblemSolvingIdeasDialogFragment ideaFragment = null;

            Log.Info(TAG, "ProblemIdeaList_ItemLongClick: selected item - " + e.Position.ToString() + ", problemID - " + _problemID.ToString());
            problemIdea = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID).ProblemSteps.Find(step => step.ProblemStepID == _problemStepID).ProblemStepIdeas[e.Position];
            if (problemIdea != null)
            {
                ideaFragment = new ProblemSolvingIdeasDialogFragment(this, "Edit Idea", _problemID, problemIdea.ProblemStepID, problemIdea.ProblemIdeaID, problemIdea.ProblemIdeaText.Trim());
            }
            var fragmentTransaction = FragmentManager.BeginTransaction();
            if (fragmentTransaction != null)
            {
                ideaFragment.Show(fragmentTransaction, ideaFragment.Tag);
            }
        }

        private void ProblemIdeaList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateAdapter();
            _problemIdeaList.SetSelection(_selectedItemIndex);
        }

        private void Add()
        {
            try
            {
                ProblemSolvingIdeasDialogFragment ideaFragment = null;

                Log.Info(TAG, "Add_Click: passing -1 for ideaID, stepID is " + _problemStepID.ToString() + ", problemID is " + _problemID.ToString());
                ideaFragment = new ProblemSolvingIdeasDialogFragment(this, "Add Idea", _problemID, _problemStepID, -1);

                var fragmentTransaction = FragmentManager.BeginTransaction();
                if (fragmentTransaction != null)
                {
                    ideaFragment.Show(fragmentTransaction, ideaFragment.Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingIdeasActivityAdd), "ProblemSolvingIdeasActivity.Add_Click");
            }
        }

        private void Remove()
        {
            try
            {
                if (_selectedItemIndex != -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.ProblemSolvingIdeasActivityRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.ProblemSolvingIdeasActivityRemoveAlertMessage);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                else
                {
                    Toast.MakeText(this, Resource.String.ProblemSolvingIdeasActivityRemoveToast, ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Remove: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingIdeasActivityRemove), "ProblemSolvingIdeasActivity.Remove");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.problemsolvingideasActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.problemsolvingideasActionRemove);
                var itemReview = menu.FindItem(Resource.Id.problemsolvingideasActionReview);
                var itemHelp = menu.FindItem(Resource.Id.problemsolvingideasActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if(itemReview != null)
                            itemReview.SetIcon(Resource.Drawable.ic_visibility_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemReview != null)
                            itemReview.SetIcon(Resource.Drawable.ic_visibility_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
                        if (itemReview != null)
                            itemReview.SetIcon(Resource.Drawable.ic_visibility_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ProblemSolvingIdeasActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(ProblemSolvingStepsActivity));

            //going back, the step needs its original data
            var problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);
            if(problem != null)
            {
                intent.PutExtra("problemID", _problemID);
                intent.PutExtra("problemText", problem.ProblemText.Trim());
            }
            else
            {
                Log.Error(TAG, "GoBack_Click: problem is NULL!");
            }
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void GetFieldComponents()
        {
            try
            {
                _problemStepText = FindViewById<TextView>(Resource.Id.txtProblemIdeasStepText);
                _problemIdeaList = FindViewById<ListView>(Resource.Id.lstProblemIdeas);
                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingIdeasActivityGetComponents), "ProblemSolvingIdeasActivity.GetFieldComponents");
            }
        }

        public void ConfirmAddition(int problemID, int problemStepID, int problemIdeaID, string problemIdeaText)
        {
            Log.Info(TAG, "ConfirmAddition: problemIdeaID - " + problemIdeaID.ToString() + ", problemStepID - " + problemStepID.ToString() + ", problemID - " + problemID.ToString() + ", problemIdeaText - " + problemIdeaText);

            ProblemIdea idea = null;
            if (problemIdeaID == -1)
            {
                Log.Info(TAG, "ConfirmAddition: New step idea detected");
                idea = new ProblemIdea();
                idea.IsNew = true;
                idea.IsDirty = false;
                idea.ProblemID = problemID;
                idea.ProblemStepID = problemStepID;
            }
            else
            {
                idea = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == problemID).ProblemSteps.Find(probStep => probStep.ProblemStepID == problemStepID).ProblemStepIdeas.Find(stepIdea => stepIdea.ProblemIdeaID == problemIdeaID);
                idea.IsNew = false;
                idea.IsDirty = true;
            }

            idea.ProblemIdeaText = problemIdeaText;

            idea.Save();
            if (problemIdeaID == -1)
                GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == problemID).ProblemSteps.Find(step => step.ProblemStepID == problemStepID).ProblemStepIdeas.Add(idea);

            UpdateAdapter();
        }

        private void UpdateAdapter()
        {
            ProblemSolvingIdeasListAdapter adapter = new ProblemSolvingIdeasListAdapter(this, _problemID, _problemStepID);

            if (_problemIdeaList != null)
                _problemIdeaList.Adapter = adapter;
        }

        public void CancelAddition()
        {
            Toast.MakeText(this, "No changes made", ToastLength.Short).Show();
        }
        private bool IsProblemSolved()
        {
            bool retVal = false;

            var problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);

            if (problem != null)
            {
                retVal = problem.IsProblemSolved();
            }

            return retVal;
        }

        private void InflateResolved()
        {
            try
            {
                var view = LayoutInflater.Inflate(Resource.Layout.ProblemSolvingResolvedItemLayout, null);

                if (view != null)
                {
                    var mainView = view.FindViewById<LinearLayout>(Resource.Id.linProblemSolvingStepsResolvedMain);
                    if (mainView != null)
                        mainView.SetBackgroundColor(Color.Argb(255, 113, 196, 228));
                    var solveText = view.FindViewById<TextView>(Resource.Id.txtProblemSolvingStepsResolvedText);
                    if (solveText != null)
                        solveText.SetBackgroundColor(Color.Argb(255, 113, 196, 228));

                    //view will be part of the header/footer, so apply that style
                    view.SetBackgroundColor(Color.Argb(255, 113, 196, 228));
                    var placeholder = FindViewById<LinearLayout>(Resource.Id.linProblemIdeasResolved);
                    if (placeholder != null)
                    {
                        placeholder.AddView(view);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "InflateResolved: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingIdeasActivityInflate), "ProblemSolvingIdeasActivity.InflateResolved");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                return;
            }

            var problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);

            var step = problem.ProblemSteps.Find(stp => stp.ProblemStepID == _problemStepID);

            var idea = step.ProblemStepIdeas[_selectedItemIndex];

            idea.Remove();

            step.ProblemStepIdeas.Remove(idea);

            _selectedItemIndex = -1;

            UpdateAdapter();
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                //PermissionResultUpdate(Permission.Denied);
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                return;
            }

            Toast.MakeText(this, Resource.String.ProblemSolvingIdeasActivityRemoveCancelToast, ToastLength.Short).Show();
        }

        private void CheckMicPermission()
        {
            try
            {
                if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.UseMicrophone)))
                {
                    AttemptPermissionRequest();
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "CheckMicPermission: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ProblemSolvingIdeasActivity.CheckMicPermission");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            try
            {
                if (requestCode == ConstantsAndTypes.REQUEST_CODE_PERMISSION_USE_MICROPHONE)
                {
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
                            if (PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone))
                            {
                                GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == ConstantsAndTypes.AppPermission.UseMicrophone).PermissionGranted = Permission.Granted;
                            }
                        }
                        //PermissionResultUpdate(Permission.Granted);
                    }
                    else
                    {
                        //PermissionResultUpdate(Permission.Denied);
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ProblemSolvingIdeasActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {

            }
        }

        private void ShowPermissionRationale()
        {
            try
            {
                if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagMic").SettingValue == "True") return;

                AlertHelper alertHelper = new AlertHelper(this);

                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolInformation;
                alertHelper.AlertMessage = GetString(Resource.String.RequestPermissionUseMicrophoneAlertMessage);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertTitle = GetString(Resource.String.RequestPermissionUseMicrophoneAlertTitle);
                alertHelper.InstanceId = "useMic";
                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ShowPermissionRationale: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ProblemSolvingIdeasActivity.ShowPermissionRationale");
            }
        }

        public void AttemptPermissionRequest()
        {
            try
            {
                if (PermissionsHelper.ShouldShowPermissionRationale(this, ConstantsAndTypes.AppPermission.UseMicrophone))
                {
                    ShowPermissionRationale();
                    return;
                }
                else
                {
                    //just request the permission
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "AttemptPermissionRequest: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "ProblemSolvingIdeasActivity.AttemptPermissionRequest");
            }
        }
    }
}
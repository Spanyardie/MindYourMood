using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Adapters;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Model;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.SubActivities.ProblemSolving
{
    [Activity(Label = "ProblemSolvingActivity")]
    public class ProblemSolvingActivity : AppCompatActivity, IProblemSolvingCallback, IAlertCallback
    {
        public const string TAG = "M:ProblemSolvingActivity";

        private ListView _problemList = null;
        private Toolbar _toolbar;
        private Button _btnDone;

        private int _selectedListItemIndex = -1;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedListItemIndex", _selectedListItemIndex);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ProblemSolvingMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedListItemIndex = savedInstanceState.GetInt("selectedListItemIndex");

                SetContentView(Resource.Layout.ProblemSolvingLayout);
                Log.Info(TAG, "OnCreate: Set content view successfully");

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.problemSolvingToolbar, Resource.String.ProblemSolvingActionBarTitle, Color.White);

                GetFieldComponents();

                CheckMicPermission();

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.solution,
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

                UpdateAdapter();

            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsDialogSetActSpin), "ProblemSolvingActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_problemList != null)
                _problemList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateAdapter();
        }

        public int GetSelectedItemIndex()
        {
            return _selectedListItemIndex;
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_problemList != null)
                {
                    _problemList.ItemClick += ProblemList_ItemClick;
                    _problemList.ItemLongClick += ProblemList_ItemLongClick;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _problemList is NULL!");
                }
                if(_btnDone != null)
                    _btnDone.Click += Done_Click;
                Log.Info(TAG, "SetupCallbacks: Successfully set up callbacks");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingActivitySetCallbacks), "ProblemSolvingActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void Review()
        {
            if(_selectedListItemIndex == -1)
            {
                Toast.MakeText(this, "Please select a problem to review.", ToastLength.Short).Show();
                return;
            }
            Intent intent = new Intent(this, typeof(ProblemSolvingReviewActivity));
            intent.PutExtra("problemID", GlobalData.ProblemSolvingItems[_selectedListItemIndex].ProblemID);
            StartActivity(intent);
        }

        private void ProblemList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            Problem problem = null;
            ProblemSolvingDialogFragment problemFragment = null;

            problem = GlobalData.ProblemSolvingItems[e.Position];
            problemFragment = new ProblemSolvingDialogFragment(this, "Edit Problem", problem.ProblemID);
            Log.Info(TAG, "Add_Click: Existing problem detected with ID " + problem.ProblemID.ToString());
            var fragmentTransaction = FragmentManager.BeginTransaction();
            if (fragmentTransaction != null)
            {
                Log.Info(TAG, "Add_Click: Showing dialog Fragment");
                problemFragment.Show(fragmentTransaction, problemFragment.Tag);
            }
        }

        private void ProblemList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedListItemIndex = e.Position;
                Log.Info(TAG, "ProblemList_ItemClick: Detected click at position " + _selectedListItemIndex.ToString());
                UpdateAdapter();
                _problemList.SetSelection(_selectedListItemIndex);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ProblemList_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingActivitySelectProblem), "ProblemSolvingActivity.ProblemList_ItemClick");
            }
        }

        private void Remove()
        {
            try
            {
                if (_selectedListItemIndex != -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.ProblemSolvingActivityRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.ProblemSolvingActivityRemoveAlertMessage);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                else
                {
                    Toast.MakeText(this, Resource.String.ProblemSolvingActivityRemoveToast, ToastLength.Short).Show();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Remove_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingActivityRemove), "ProblemSolvingActivity.Remove_Click");
            }
        }

        private void Add()
        {
            try
            {
                ProblemSolvingDialogFragment problemFragment = null;

                problemFragment = new ProblemSolvingDialogFragment(this, "Add Problem", -1);
                Log.Info(TAG, "Add_Click: New problem, passing ID -1 to dialog fragment");

                var fragmentTransaction = FragmentManager.BeginTransaction();
                if (fragmentTransaction != null)
                {
                    Log.Info(TAG, "Add_Click: Showing dialog Fragment");
                    problemFragment.Show(fragmentTransaction, problemFragment.Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingActivityAdd), "ProblemSolvingActivity.Add");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.problemsolvingActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.problemsolvingActionRemove);
                var itemReview = menu.FindItem(Resource.Id.problemsolvingActionReview);
                var itemHelp = menu.FindItem(Resource.Id.problemsolvingActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ProblemSolvingActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            try
            {
                Log.Info(TAG, "Navigating to TreatmentPlan");
                Intent intent = new Intent(this, typeof(TreatmentPlanActivity));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "GoBack_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingActivityGoBack), "ProblemSolvingActivity.GoBack_Click");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _problemList = FindViewById<ListView>(Resource.Id.lstProblemSolving);
                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
                Log.Info(TAG, "GetFieldComponents: Retrieved components successfully");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingActivityGetComponents), "ProblemSolvingActivity.GetFieldComponents");
            }
        }

        private void UpdateAdapter()
        {
            try
            {
                ProblemSolvingListAdapter adapter = new ProblemSolvingListAdapter(this);
                if (_problemList != null)
                {
                    _problemList.Adapter = adapter;
                    Log.Info(TAG, "UpdateAdapter: Updated adapter successfully");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingActivityUpdatingAdapter), "ProblemSolvingActivity.UpdateAdapter");
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
                    case Resource.Id.problemsolvingActionAdd:
                        Add();
                        return true;

                    case Resource.Id.problemsolvingActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.problemsolvingActionReview:
                        Review();
                        return true;

                    case Resource.Id.problemsolvingActionHelp:
                        Intent intent = new Intent(this, typeof(TreatmentProblemSolvingHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        public void ConfirmAddition(int problemID, string problemText)
        {
            Problem problem = null;
            if(problemID == -1)
            {
                problem = new Problem();
                problem.IsNew = true;
                problem.IsDirty = false;
                Log.Info(TAG, "ConfirmAddition: Adding a new problem...");
            }
            else
            {
                problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == problemID);
                problem.IsNew = false;
                problem.IsDirty = true;
            }

            problem.ProblemText = problemText.Trim();
            problem.Save();
            Log.Info(TAG, "ConfirmAddition: Saved problem with ID " + problem.ProblemID);

            if (problemID == -1)
            {
                GlobalData.ProblemSolvingItems.Add(problem);
                Log.Info(TAG, "ConfirmAddition: Added item to global cache as it is new!");
            }

            UpdateAdapter();
        }

        public void CancelAddition()
        {
            Toast.MakeText(this, Resource.String.ProblemSolvingActivityCancel, ToastLength.Short).Show();
            Log.Info(TAG, "CancelAddition: Nothing to do but make toast!");
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                return;
            }

            var problem = GlobalData.ProblemSolvingItems[_selectedListItemIndex];
            Log.Info(TAG, "AlertPositiveButtonSelect: Found problem with ID " + problem.ProblemID.ToString());

            problem.Remove();
            Log.Info(TAG, "AlertPositiveButtonSelect: Removed successfully");
            GlobalData.ProblemSolvingItems.Remove(problem);

            _selectedListItemIndex = -1;

            UpdateAdapter();
            Log.Info(TAG, "AlertPositiveButtonSelect: Updated adapter");
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                //PermissionResultUpdate(Permission.Denied);
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                return;
            }

            Toast.MakeText(this, Resource.String.ProblemSolvingActivityRemoveCancelToast, ToastLength.Short).Show();
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ProblemSolvingActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ProblemSolvingActivity.OnRequestPermissionsResult");
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ProblemSolvingActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "ProblemSolvingActivity.AttemptPermissionRequest");
            }
        }
    }
}
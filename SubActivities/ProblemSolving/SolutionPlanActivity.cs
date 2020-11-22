using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Adapters;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using System.Collections.Generic;
using Android.Graphics;
using Android.Content.PM;
using Android.Runtime;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment.ProblemSolving.Review;

namespace com.spanyardie.MindYourMood.SubActivities.ProblemSolving
{
    [Activity(Label = "SolutionPlanActivity")]
    public class SolutionPlanActivity : AppCompatActivity, ISolutionPlanStepsCallback, IAlertCallback
    {
        public const string TAG = "M:SolutionPlanActivity";

        private Toolbar _toolbar;

        private TextView _problemIdeaText;
        private ListView _solutionStepList;

        private int _selectedItemIndex = -1;
        private int _problemIdeaID;
        private string _ideaText;

        private Button _done;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("selectedItemIndex", _selectedItemIndex);
                outState.PutInt("problemIdeaID", _problemIdeaID);
                outState.PutString("ideaText", _ideaText);
            }
            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SolutionPlanmenu, menu);

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
                    _problemIdeaID = savedInstanceState.GetInt("problemIdeaID");
                    _ideaText = savedInstanceState.GetString("ideaText");
                }
                else
                {
                    _problemIdeaID = Intent.GetIntExtra("problemIdeaID", -1);
                    _ideaText = Intent.GetStringExtra("problemIdeaText");
                }

                SetContentView(Resource.Layout.SolutionPlanLayout);
                Log.Info(TAG, "OnCreate: Set content view successfully, problemIdeaID - " + _problemIdeaID.ToString() + ", problemIdeaText - " + _ideaText);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.solutionPlanToolbar, Resource.String.SolutionReviewToolbarTitle, Color.White);

                GetFieldComponents();

                CheckMicPermission();

                SetupCallbacks();

                if (_problemIdeaID != -1)
                    _problemIdeaText.Text = _ideaText.Trim();

                UpdateAdapter();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSolutionPlanActivityCreateView), "SolutionPlanActivity.OnCreate");
            }
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
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
                    case Resource.Id.solutionPlanActionAdd:
                        Add();
                        return true;

                    case Resource.Id.solutionPlanActionRemove:
                        Remove();
                        return true;
                    case Resource.Id.solutionPlanActionHelp:
                        Intent intent = new Intent(this, typeof(ProblemSolvingReviewHelpActivity));
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
                if (_solutionStepList != null)
                {
                    _solutionStepList.ItemClick += SolutionPlanList_ItemClick;
                    _solutionStepList.ItemLongClick += SolutionStepList_ItemLongClick;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _problemStepList is NULL!");
                }
                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSolutionPlanActivitySetCallbacks), "SolutionPlanActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void SolutionStepList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            SolutionPlan solutionStep = null;
            SolutionStepsDialogFragment stepFragment = null;

            Log.Info(TAG, "SolutionStepList_ItemLongClick: selected item - " + e.Position.ToString() + ", problemIdeaID - " + _problemIdeaID.ToString());
            solutionStep = GetSolutionPlanStepsForIdea()[e.Position];
            if (solutionStep != null)
            {
                Log.Info(TAG, "SolutionStepList_ItemLongClick: Found step with ID - " + solutionStep.SolutionPlanID + ", stepText - " + solutionStep.SolutionStep + ", priorityOrder - " + solutionStep.PriorityOrder.ToString());
                stepFragment = new SolutionStepsDialogFragment(this, "Edit Solution Step", _problemIdeaID, solutionStep.SolutionPlanID, solutionStep.SolutionStep.Trim(), solutionStep.PriorityOrder);
            }
            var fragmentTransaction = FragmentManager.BeginTransaction();
            if (fragmentTransaction != null)
            {
                stepFragment.Show(fragmentTransaction, stepFragment.Tag);
            }
        }

        private List<SolutionPlan> GetSolutionPlanStepsForIdea()
        {
            List<SolutionPlan> solutionList = null;
            solutionList =
            (from eachStep in GlobalData.SolutionPlansItems
                where eachStep.ProblemIdeaID == _problemIdeaID
                select eachStep).ToList();

            return solutionList;
        }

        private void SolutionPlanList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateAdapter();
            _solutionStepList.SetSelection(_selectedItemIndex);
        }

        private void Add()
        {
            try
            {
                SolutionStepsDialogFragment stepFragment = null;

                Log.Info(TAG, "Add_Click: passing -1 for stepID, problemIdeaID is " + _problemIdeaID.ToString());
                stepFragment = new SolutionStepsDialogFragment(this, "Adding Solution", _problemIdeaID, -1);

                var fragmentTransaction = FragmentManager.BeginTransaction();
                if (fragmentTransaction != null)
                {
                    stepFragment.Show(fragmentTransaction, stepFragment.Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSolutionPlanActivityAdd), "SolutionPlanActivity.Add_Click");
            }
        }

        private void Remove()
        {
            try
            {
                if (_selectedItemIndex != -1)
                {
                    var step = GetSolutionPlanStepsForIdea()[_selectedItemIndex];

                    if (step != null)
                    {
                        step.Remove();
                        GlobalData.SolutionPlansItems.Remove(step);
                    }
                    _selectedItemIndex = -1;

                    UpdateAdapter();
                }
                else
                {
                    Toast.MakeText(this, Resource.String.ErrorSolutionPlanActivityRemoveToast, ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Remove_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSolutionPlanActivityRemove), "SolutionPlanActivity.Remove_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.solutionPlanActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.solutionPlanActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.solutionPlanActionHelp);

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
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SolutionPlanActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            try
            {
                Finish();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "GoBack_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSolutionPlanActivityGoBack), "SolutionPlanActivity.GoBack_Click");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _solutionStepList = FindViewById<ListView>(Resource.Id.lstSolutionPlans);
                _problemIdeaText = FindViewById<TextView>(Resource.Id.txtSolutionPlanIdeaText);
                _done = FindViewById<Button>(Resource.Id.btnDone);

                Log.Info(TAG, "GetFieldComponents: Retrieved components successfully");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSolutionPlanActivityGetComponents), "SolutionPlanActivity.GetFieldComponents");
            }
        }

        private void UpdateAdapter()
        {
            try
            {
                SolutionPlanStepsListAdapter adapter = new SolutionPlanStepsListAdapter(this, _problemIdeaID);
                if (_solutionStepList != null)
                {
                    _solutionStepList.Adapter = adapter;
                    Log.Info(TAG, "UpdateAdapter: Updated adapter successfully");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSolutionPlanActivityUpdateList), "SolutionPlanActivity.UpdateAdapter");
            }
        }

        public void ConfirmAddition(int solutionPlanStepID, int problemIdeaID, string solutionStepText, int priorityOrder)
        {
            SolutionPlan solution = null;
            if (solutionPlanStepID == -1)
            {
                solution = new SolutionPlan();
                solution.IsNew = true;
                solution.IsDirty = false;
                Log.Info(TAG, "ConfirmAddition: Adding a new Solution...");
            }
            else
            {
                solution = GlobalData.SolutionPlansItems.Find(sol => sol.SolutionPlanID == solutionPlanStepID);
                solution.IsNew = false;
                solution.IsDirty = true;
            }

            solution.ProblemIdeaID = problemIdeaID;
            solution.SolutionStep = solutionStepText.Trim();
            solution.PriorityOrder = priorityOrder;
            solution.Save();
            Log.Info(TAG, "ConfirmAddition: Saved solution with ID " + solution.SolutionPlanID);

            if (solutionPlanStepID == -1)
            {
                GlobalData.SolutionPlansItems.Add(solution);
                Log.Info(TAG, "ConfirmAddition: Added item to global cache as it is new!");
            }

            UpdateAdapter();
        }

        public void CancelAddition()
        {
            Toast.MakeText(this, Resource.String.SolutionPlanActivityCancelToast, ToastLength.Short).Show();
            Log.Info(TAG, "CancelAddition: Nothing to do but make toast!");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "SolutionReviewActivity.CheckMicPermission");
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
                        PermissionResultUpdate(Permission.Granted);
                    }
                    else
                    {
                        PermissionResultUpdate(Permission.Denied);
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "SolutionReviewActivity.OnRequestPermissionsResult");
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
                if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagMic").SettingValue == "True")
                {
                    if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.UseMicrophone) == true))
                    {
                        PermissionResultUpdate(Permission.Denied);
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                    return;
                }

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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "SolutionReviewActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "SolutionReviewActivity.AttemptPermissionRequest");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                PermissionResultUpdate(Permission.Denied);
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
            }
        }
    }
}
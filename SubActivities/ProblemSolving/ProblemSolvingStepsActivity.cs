using System;

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
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment.ProblemSolving;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.SubActivities.ProblemSolving
{
    [Activity(Label = "ProblemSolvingStepsActivity")]
    public class ProblemSolvingStepsActivity : AppCompatActivity, IProblemSolvingStepsCallback, IAlertCallback
    {
        public const string TAG = "M:ProblemSolvingStepsActivity";

        private Toolbar _toolbar;

        private TextView _problemText;
        private ListView _problemStepList;
        private Button _btnDone;

        private int _selectedItemIndex = -1;
        private string _probText;
        private int _problemID;

        private ImageLoader _imageLoader = null;

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("selectedItemIndex", _selectedItemIndex);
                outState.PutInt("problemID", _problemID);
                outState.PutString("probText", _probText);
            }
            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ProblemSolvingStepsMenu, menu);

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
                    _probText = savedInstanceState.GetString("probText");
                }
                else
                {
                    _problemID = Intent.GetIntExtra("problemID", -1);
                    _probText = Intent.GetStringExtra("problemText");
                }

                SetContentView(Resource.Layout.ProblemSolvingStepsLayout);
                Log.Info(TAG, "OnCreate: Set content view successfully, problemID - " + _problemID.ToString() + ", problemText - " + _probText);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.problemSolvingStepsToolbar, Resource.String.ProblemSolvingStepsToolbarTitle, Color.White);

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

                if(_problemID != -1)
                    _problemText.Text = _probText.Trim();

                if(IsProblemSolved())
                {
                    InflateResolved();
                }

                UpdateAdapter();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingStepsActivityCreateView), "ProblemSolvingStepsActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_problemStepList != null)
                _problemStepList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
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
                    case Resource.Id.problemsolvingstepsActionAdd:
                        Add();
                        return true;

                    case Resource.Id.problemsolvingstepsActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.problemsolvingstepsActionReview:
                        Review();
                        return true;

                    case Resource.Id.problemsolvingstepsActionHelp:
                        Intent intent = new Intent(this, typeof(ProblemSolvingProblemStepsHelpActivity));
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
                if(_problemStepList != null)
                {
                    _problemStepList.ItemClick += ProblemStepList_ItemClick;
                    _problemStepList.ItemLongClick += ProblemStepList_ItemLongClick;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _problemStepList is NULL!");
                }
                if(_btnDone != null)
                    _btnDone.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingStepsActivitySetCallbacks), "ProblemSolvingStepsActivity.SetupCallbacks");
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

        private void ProblemStepList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            ProblemStep problemStep = null;
            ProblemSolvingStepsFragment stepFragment = null;

            Log.Info(TAG, "ProblemStepList_ItemLongClick: selected item - " + e.Position.ToString() + ", problemID - " + _problemID.ToString());
            problemStep = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID).ProblemSteps[e.Position];
            if (problemStep != null)
            {
                stepFragment = new ProblemSolvingStepsFragment(this, "Edit Step",_problemID, problemStep.ProblemStepID, problemStep.ProblemStep.Trim(), problemStep.PriorityOrder);
            }
            var fragmentTransaction = FragmentManager.BeginTransaction();
            if (fragmentTransaction != null)
            {
                stepFragment.Show(fragmentTransaction, stepFragment.Tag);
            }
        }

        private void ProblemStepList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateAdapter();
            _problemStepList.SetSelection(_selectedItemIndex);
        }

        private void Add()
        {
            try
            {
                ProblemSolvingStepsFragment stepFragment = null;

                Log.Info(TAG, "Add_Click: passing -1 for stepID, problemID is " + _problemID.ToString());
                stepFragment = new ProblemSolvingStepsFragment(this, "Add Step", _problemID, -1);

                var fragmentTransaction = FragmentManager.BeginTransaction();
                if(fragmentTransaction != null)
                {
                    stepFragment.Show(fragmentTransaction, stepFragment.Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingStepsActivityAdd), "ProblemSolvingStepsActivity.Add_Click");
            }
        }

        private void Remove()
        {
            try
            {
                if(_selectedItemIndex != -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.ProblemSolvingStepsActivityRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.ProblemSolvingStepsActivityRemoveAlertMessage);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                else
                {
                    Toast.MakeText(this, Resource.String.ProblemSolvingStepsActivityRemoveToast, ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Remove_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingStepsActivityRemove), "ProblemSolvingStepsActivity.Remove_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.problemsolvingstepsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.problemsolvingstepsActionRemove);
                var itemReview = menu.FindItem(Resource.Id.problemsolvingstepsActionReview);
                var itemHelp = menu.FindItem(Resource.Id.problemsolvingstepsActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ProblemSolvingStepsActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(ProblemSolvingActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void GetFieldComponents()
        {
            try
            {
                _problemText = FindViewById<TextView>(Resource.Id.txtProblemStepsProblemText);
                _problemStepList = FindViewById<ListView>(Resource.Id.lstProblemSteps);
                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingStepsActivityGetComponents), "ProblemSolvingStepsActivity.GetFieldComponents");
            }
        }

        public void ConfirmAddition(int problemStepID, int problemID, string problemStepText, int priorityOrder)
        {
            Log.Info(TAG, "ConfirmAddition: problemStepID - " + problemStepID.ToString() + ", problemID - " + problemID.ToString() + ", problemStepText - " + problemStepText + ", priorityOrder - " + priorityOrder.ToString());

            ProblemStep step = null;
            if(problemStepID == -1)
            {
                Log.Info(TAG, "ConfirmAddition: New problem step detected");
                step = new ProblemStep();
                step.IsNew = true;
                step.IsDirty = false;
                step.ProblemID = problemID;
            }
            else
            {
                step = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == problemID).ProblemSteps.Find(probStep => probStep.ProblemStepID == problemStepID);
                step.IsNew = false;
                step.IsDirty = true;
            }

            step.ProblemStep = problemStepText;
            step.PriorityOrder = priorityOrder;

            step.Save();
            if (problemStepID == -1)
                GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == problemID).ProblemSteps.Add(step);

            UpdateAdapter();
        }

        private void UpdateAdapter()
        {
            ProblemSolvingStepsListAdapter adapter = new ProblemSolvingStepsListAdapter(this, _problemID);

            if (_problemStepList != null)
                _problemStepList.Adapter = adapter;
        }

        public void CancelAddition()
        {
            Toast.MakeText(this, Resource.String.ProblemSolvingStepsActivityCancelToast, ToastLength.Short).Show();
        }

        private bool IsProblemSolved()
        {
            bool retVal = false;

            var problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);

            if(problem != null)
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

                if(view != null)
                {
                    var mainView = view.FindViewById<LinearLayout>(Resource.Id.linProblemSolvingStepsResolvedMain);
                    if(mainView != null)
                        mainView.SetBackgroundColor(Color.Argb(255, 113, 196, 228));
                    var solveText = view.FindViewById<TextView>(Resource.Id.txtProblemSolvingStepsResolvedText);
                    if(solveText != null)
                        solveText.SetBackgroundColor(Color.Argb(255, 113, 196, 228));

                    //view will be part of the header/footer, so apply that style
                    view.SetBackgroundColor(Color.Argb(255, 113, 196, 228));
                    var placeholder = FindViewById<LinearLayout>(Resource.Id.linProblemStepsResolved);
                    if(placeholder != null)
                    {
                        placeholder.SetBackgroundColor(Color.Argb(255, 113, 196, 228));
                        placeholder.AddView(view);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "InflateResolved: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingStepsActivityInflate), "ProblemSolvingStepsActivity.InflateResolved");
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
            var step = problem.ProblemSteps[_selectedItemIndex];

            step.Remove();

            problem.ProblemSteps.Remove(step);

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

            Toast.MakeText(this, Resource.String.ProblemSolvingStepsActivityRemoveCancelToast, ToastLength.Short).Show();
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ProblemSolvingProsAndConsActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ProblemSolvingProsAndConsActivity.OnRequestPermissionsResult");
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ProblemSolvingProsAndConsActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "ProblemSolvingProsAndConsActivity.AttemptPermissionRequest");
            }
        }
    }
}
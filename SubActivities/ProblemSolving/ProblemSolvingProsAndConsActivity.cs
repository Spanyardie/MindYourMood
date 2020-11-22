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
    [Activity(Label = "ProblemSolvingProsAndConsActivity")]
    public class ProblemSolvingProsAndConsActivity : AppCompatActivity, IProblemSolvingProsAndConsCallback,IAlertCallback
    {
        public const string TAG = "M:ProblemSolvingProsAndConsActivity";

        private Toolbar _toolbar;

        private TextView _problemIdeaText;
        private ListView _problemProsAndConsList;
        private Button _btnDone;
        
        private int _problemIdeaID;
        private int _problemStepID;
        private int _problemID;
        private string _ideaText;
        private int _selectedItemIndex = -1;

        private ImageLoader _imageLoader = null;

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("problemIdeaID", _problemIdeaID);
                outState.PutInt("problemStepID", _problemStepID);
                outState.PutInt("problemID", _problemID);
                outState.PutString("ideaText", _ideaText);
                outState.PutInt("selectedItemIndex", _selectedItemIndex);
            }
            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ProblemSolvingProsAndConsMenu, menu);

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
                    _problemIdeaID = savedInstanceState.GetInt("problemIdeaID");
                    _problemStepID = savedInstanceState.GetInt("problemStepID");
                    _problemID = savedInstanceState.GetInt("problemID");
                    _ideaText = savedInstanceState.GetString("ideaText");
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");
                }
                else
                {
                    _problemIdeaID = Intent.GetIntExtra("problemIdeaID", -1);
                    _problemStepID = Intent.GetIntExtra("problemStepID", -1);
                    _problemID = Intent.GetIntExtra("problemID", -1);
                    _ideaText = Intent.GetStringExtra("problemIdeaText");
                }

                SetContentView(Resource.Layout.ProblemSolvingProsAndConsLayout);
                Log.Info(TAG, "OnCreate: Set content view successfully, problemID - " + _problemID.ToString() + ", problemStepID - " + _problemStepID.ToString() + ", problemIdeaID - " + _problemIdeaID.ToString() + ", problemIdeaText - " + _ideaText);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.problemSolvingProsAndConsToolbar, Resource.String.ProblemSolvingProConToolbarTitle, Color.White);

                GetFieldComponents();

                CheckMicPermission();

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.procontra,
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

                if (_problemIdeaID != -1)
                    _problemIdeaText.Text = _ideaText.Trim();

                if (IsProblemSolved())
                    InflateResolved();

                UpdateAdapter();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingProConActivityCreateView), "ProblemSolvingProsAndConsActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_problemProsAndConsList != null)
                _problemProsAndConsList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
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
                    case Resource.Id.problemsolvingprosandconsActionAdd:
                        Add();
                        return true;

                    case Resource.Id.problemsolvingprosandconsActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.problemsolvingprosandconsActionReview:
                        Review();
                        return true;

                    case Resource.Id.problemsolvingprosandconsActionHelp:
                        Intent intent = new Intent(this, typeof(ProblemSolvingProsAndConsHelpActivity));
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
                if (_problemProsAndConsList != null)
                {
                    _problemProsAndConsList.ItemClick += ProblemProAndConList_ItemClick;
                    _problemProsAndConsList.ItemLongClick += ProblemProAndConList_ItemLongClick;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _problemProsAndConsList is NULL!");
                }
                if(_btnDone != null)
                    _btnDone.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingProConActivitySetCallbacks), "ProblemSolvingProsAndConsActivity.SetupCallbacks");
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

        private void ProblemProAndConList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            try
            {
                Problem problem = null;
                ProblemStep step = null;
                ProblemIdea idea = null;
                ProblemProAndCon problemProAndCon = null;

                ProblemSolvingProsAndConsDialogFragment proAndConFragment = null;
                Log.Info(TAG, "ProblemProAndConList_ItemLongClick: selected item - " + e.Position.ToString() + ", problemID - " + _problemID.ToString());

                problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);
                if (problem != null)
                {
                    step = problem.ProblemSteps.Find(stp => stp.ProblemStepID == _problemStepID);
                    if (step != null)
                    {
                        idea = step.ProblemStepIdeas.Find(ida => ida.ProblemIdeaID == _problemIdeaID);
                        if (idea != null)
                        {
                            problemProAndCon = idea.ProsAndCons[e.Position];
                        }
                    }
                }
                if (problemProAndCon != null)
                {
                    proAndConFragment = new ProblemSolvingProsAndConsDialogFragment(this, "Edit Pro or Con", _problemID, problemProAndCon.ProblemStepID, problemProAndCon.ProblemIdeaID, problemProAndCon.ProblemProAndConID,
                    problemProAndCon.ProblemProAndConType, problemProAndCon.ProblemProAndConText.Trim());
                }
                var fragmentTransaction = FragmentManager.BeginTransaction();
                if (fragmentTransaction != null)
                {
                    proAndConFragment.Show(fragmentTransaction, proAndConFragment.Tag);
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ProblemProAndConList_ItemLongClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingProConActivitySelect), "ProblemSolvingProsAndConsActivity.ProblemProAndConList_ItemLongClick");
            }
        }

        private void ProblemProAndConList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateAdapter();
            _problemProsAndConsList.SetSelection(_selectedItemIndex);
        }

        private void Add()
        {
            try
            {
                ProblemSolvingProsAndConsDialogFragment proAndConFragment = null;

                Log.Info(TAG, "Add_Click: passing -1 for proAndConID, problemIdeaID is " + _problemIdeaID.ToString() + ", stepID is " + _problemStepID.ToString() + ", problemID is " + _problemID.ToString());
                proAndConFragment = new ProblemSolvingProsAndConsDialogFragment(this, "Add Pro or Con", _problemID, _problemStepID, _problemIdeaID);

                var fragmentTransaction = FragmentManager.BeginTransaction();
                if (fragmentTransaction != null)
                {
                    proAndConFragment.Show(fragmentTransaction, proAndConFragment.Tag);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingProConActivityAdd), "ProblemSolvingProsAndConsActivity.Add_Click");
            }
        }

        private void Remove()
        {
            try
            {
                if (_selectedItemIndex != -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.ProblemSolvingProConActivityRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.ProblemSolvingProConActivityRemoveAlertMessage);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                else
                {
                    Toast.MakeText(this, Resource.String.ProblemSolvingProConActivityRemoveToast, ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Remove_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingProConActivityRemove), "ProblemSolvingProsAndConsActivity.Remove_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.problemsolvingprosandconsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.problemsolvingprosandconsActionRemove);
                var itemReview = menu.FindItem(Resource.Id.problemsolvingprosandconsActionReview);
                var itemHelp = menu.FindItem(Resource.Id.problemsolvingprosandconsActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ProblemSolvingProsAndConsActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            try
            {
                Intent intent = new Intent(this, typeof(ProblemSolvingIdeasActivity));

                //going back, the idea needs its original data
                var problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);
                var problemStep = problem.ProblemSteps.Find(step => step.ProblemStepID == _problemStepID);
                if (problem != null)
                {
                    intent.PutExtra("problemStepID", _problemStepID);
                    intent.PutExtra("problemID", _problemID);
                    if (problemStep != null)
                    {
                        intent.PutExtra("problemStepText", problemStep.ProblemStep.Trim());
                    }
                    else
                    {
                        Log.Error(TAG, "GoBack_Click: problem is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "GoBack_Click: problemStep is NULL!");
                }
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "GoBack_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorProblemSolvingProConActivityGoBack), "ProblemSolvingProsAndConsActivity.GoBack_Click");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _problemIdeaText = FindViewById<TextView>(Resource.Id.txtProblemProsAndConsIdeaText);
                _problemProsAndConsList = FindViewById<ListView>(Resource.Id.lstProblemProsAndCons);
                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingProConActivityGetComponents), "ProblemSolvingProsAndConsActivity.GetFieldComponents");
            }
        }

        public void ConfirmAddition(int problemID, int problemStepID, int problemIdeaID, int problemProAndConID, string problemProAndConText, ConstantsAndTypes.PROCON_TYPES problemProAndConType)
        {
            try
            {
                Log.Info(TAG, "ConfirmAddition: problemIdeaID - " + problemIdeaID.ToString() + ", problemStepID - " + problemStepID.ToString() + ", problemID - " + problemID.ToString() + ", problemIdeaText - " + problemProAndConText);

                ProblemProAndCon proAndCon = null;
                if (problemProAndConID == -1)
                {
                    Log.Info(TAG, "ConfirmAddition: New step idea detected");
                    proAndCon = new ProblemProAndCon();
                    proAndCon.IsNew = true;
                    proAndCon.IsDirty = false;
                    proAndCon.ProblemID = problemID;
                    proAndCon.ProblemStepID = problemStepID;
                    proAndCon.ProblemIdeaID = problemIdeaID;
                }
                else
                {
                    proAndCon = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == problemID).ProblemSteps.Find(probStep => probStep.ProblemStepID == problemStepID).ProblemStepIdeas.Find(stepIdea => stepIdea.ProblemIdeaID == problemIdeaID).ProsAndCons.Find(pro => pro.ProblemProAndConID == problemProAndConID);
                    proAndCon.IsNew = false;
                    proAndCon.IsDirty = true;
                }

                proAndCon.ProblemProAndConText = problemProAndConText.Trim();
                proAndCon.ProblemProAndConType = problemProAndConType;

                proAndCon.Save();
                if (problemProAndConID == -1)
                    GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == problemID).ProblemSteps.Find(step => step.ProblemStepID == problemStepID).ProblemStepIdeas.Find(step => step.ProblemIdeaID == problemIdeaID).ProsAndCons.Add(proAndCon);

                UpdateAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "ConfirmAddition: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingProConActivityAdd), "ProblemSolvingProsAndConsActivity.ConfirmAddition");
            }
        }

        private void UpdateAdapter()
        {
            try
            {
                ProblemSolvingProsAndConsListAdapter adapter = new ProblemSolvingProsAndConsListAdapter(this, _problemID, _problemStepID, _problemIdeaID);

                if (_problemProsAndConsList != null)
                    _problemProsAndConsList.Adapter = adapter;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingProConActivityUpdateAdapter), "ProblemSolvingProsAndConsActivity.UpdateAdapter");
            }
        }

        public void CancelAddition()
        {
            Toast.MakeText(this, Resource.String.ProblemSolvingProConActivityCancelToast, ToastLength.Short).Show();
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
                    var placeholder = FindViewById<LinearLayout>(Resource.Id.linProblemProsAndConsResolved);
                    if (placeholder != null)
                    {
                        placeholder.AddView(view);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "InflateResolved: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorProblemSolvingProConActivityInflate), "ProblemSolvingProsAndConsActivity.InflateResolved");
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
            if (problem != null)
            {
                var step = problem.ProblemSteps.Find(stp => stp.ProblemStepID == _problemStepID);

                if (step != null)
                {
                    var idea = step.ProblemStepIdeas.Find(ida => ida.ProblemIdeaID == _problemIdeaID);

                    if (idea != null)
                    {
                        var proAndCon = idea.ProsAndCons[_selectedItemIndex];

                        if (proAndCon != null)
                        {
                            proAndCon.Remove();

                            idea.ProsAndCons.Remove(proAndCon);

                            _selectedItemIndex = -1;

                            UpdateAdapter();
                        }
                        else
                        {
                            Log.Error(TAG, "AlertPositiveButtonSelect: proAndCon is NULL!");
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "AlertPositiveButtonSelect: idea is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "AlertPositiveButtonSelect: step is NULL!");
                }
            }
            else
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: problem is NULL!");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                //PermissionResultUpdate(Permission.Denied);
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                return;
            }

            Toast.MakeText(this, Resource.String.ProblemSolvingProConActivityRemoveCancelToast, ToastLength.Short).Show();
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
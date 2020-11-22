using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Adapters;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Model;
using Android.Graphics;
using Android.Content;
using com.spanyardie.MindYourMood.SubActivities.Help;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class ActivitiesActivity : AppCompatActivity, IActivityTimeCallback, IAlertCallback
    {
        public const string TAG = "M:ActivitiesActivity";

        private Toolbar _toolbar;

        private Button _btnDone;

        private ExpandableListView _activitiesWeek;

        private int _selectedGroupPosition;

        public ActivityTimeDialogFragment _timeFragment;

        private ImageLoader _imageLoader = null;

        public int SelectedGroupPosition
        {
            get
            {
                return _selectedGroupPosition;
            }

            set
            {
                _selectedGroupPosition = value;
            }
        }

        private int[,] _selectedChildPositions;

        public int GetSelectedChildPosition(int groupPosition)
        {
            var count = GlobalData.ActivitiesForWeek[groupPosition].ActivityTimes.Count;
            int returnPosition = -1;

            for(var a = 0; a < count; a++)
            {
                if(_selectedChildPositions[groupPosition, a] != -1)
                {
                    returnPosition = a;
                    break;
                }
            }

            return returnPosition;
        }

        public void SetSelectedChildPosition(int groupPosition, int childPosition)
        {
            try
            {
                var currentChildPosition = GetSelectedChildPosition(groupPosition);

                if (currentChildPosition != -1)
                {
                    _selectedChildPositions[groupPosition, currentChildPosition] = -1;
                }

                _selectedChildPositions[groupPosition, childPosition] = 1;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetSelectedChildPosition: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSettingSelectedChildPosition), "ActivitiesActivity.SetSelectedChildPosition");
            }
        }

        public ActivitiesActivity()
        {
            try
            {
                var groupCount = GlobalData.ActivitiesForWeek.Count;
                var childCount = GlobalData.ActivitiesForWeek[0].ActivityTimes.Count;

                _selectedChildPositions = new int[groupCount, childCount];
                for (var grp = 0; grp < groupCount; grp++)
                {
                    for (var a = 0; a < childCount; a++)
                    {
                        _selectedChildPositions[grp, a] = -1;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Constructor: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorConstructorActivities), "ActivitiesActivity.Constructor");
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("selectedGroupPosition", _selectedGroupPosition);
            }

            base.OnSaveInstanceState(outState);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedGroupPosition = savedInstanceState.GetInt("selectedGroupPosition");

                SetContentView(Resource.Layout.ActivitiesLayout);

                GetFieldComponents();

                CheckMicPermission();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.activitiesToolbar, Resource.String.ActivitiesActionBarTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.activities,
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorActivityCreate), "ActivitiesActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_activitiesWeek != null)
                _activitiesWeek.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ActivitiesMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void GetFieldComponents()
        {
            try
            {
                _activitiesWeek = FindViewById<ExpandableListView>(Resource.Id.expWeeksActivities);

                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorActivityGetComponents), "ActivitiesActivity.GetFieldComponents");
            }
        }

        private void SetNavigationIcon()
        {
            ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

            switch(screenSize)
            {
                case ConstantsAndTypes.ScreenSize.Normal:
                    _toolbar.SetNavigationIcon(Resource.Drawable.ic_arrow_back_white_24dp);
                    break;
                case ConstantsAndTypes.ScreenSize.Large:
                    _toolbar.SetNavigationIcon(Resource.Drawable.ic_arrow_back_white_36dp);
                    break;
                case ConstantsAndTypes.ScreenSize.ExtraLarge:
                    _toolbar.SetNavigationIcon(Resource.Drawable.ic_arrow_back_white_48dp);
                    break;
            }
        }

        private void UpdateAdapter()
        {
            try
            {
                ActivitiesExpandableListAdapter adapter = new ActivitiesExpandableListAdapter(this);
                if (_activitiesWeek != null)
                    _activitiesWeek.SetAdapter(adapter);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorActivityUpdatingAdapter), "ActivitiesActivity.UpdateAdapter");
            }
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
                    case Resource.Id.ActivitiesActionExpand:
                        Expand();
                        return true;
                    case Resource.Id.ActivitiesActionCollapse:
                        Collapse();
                        return true;
                    case Resource.Id.ActivitiesActionHelp:
                        Intent intent = new Intent(this, typeof(ActivitiesHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SetupCallbacks()
        {
            if(_activitiesWeek != null)
            {
                _activitiesWeek.GroupClick += ActivitiesWeek_GroupClick;
                _activitiesWeek.ChildClick += ActivitiesWeek_ChildClick;
                _activitiesWeek.ItemLongClick += ActivitiesWeek_ItemLongClick;
            }

            if(_btnDone != null)
                _btnDone.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void Collapse()
        {
            try
            {
                if (_activitiesWeek != null)
                {
                    for (var a = 0; a < GlobalData.ActivitiesForWeek.Count; a++)
                    {
                        _activitiesWeek.CollapseGroup(a);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Collapse: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorActivitiesCollapse), "ActivitiesActivity.Collapse");
            }
        }

        private void Expand()
        {
            try
            {
                if (_activitiesWeek != null)
                {
                    for (var a = 0; a < GlobalData.ActivitiesForWeek.Count; a++)
                    {
                        _activitiesWeek.ExpandGroup(a);
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Expand: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorActivitiesCollapse), "ActivitiesActivity.Expand");
            }
        }

        private void ActivitiesWeek_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            try
            {
                long packedPosition = _activitiesWeek.GetExpandableListPosition(e.Position);

                if (ExpandableListView.GetPackedPositionType(packedPosition) == PackedPositionType.Child)
                {
                    // get item ID's
                    int groupPosition = ExpandableListView.GetPackedPositionGroup(packedPosition);
                    int childPosition = ExpandableListView.GetPackedPositionChild(packedPosition);

                    Activities activity = GlobalData.ActivitiesForWeek[groupPosition];
                    ActivityTime activityTime = activity.ActivityTimes[childPosition];

                    string startTime, endTime;

                    StringHelper.ActivityTimeBeginEndForConstant(activityTime.ActivityTime, out startTime, out endTime);

                    _timeFragment = new ActivityTimeDialogFragment(this, activity.ActivityDate, activityTime.ActivityTime, startTime, endTime, groupPosition, childPosition, "Add Activity");

                    var fragmentTransaction = FragmentManager.BeginTransaction();
                    _timeFragment.Show(fragmentTransaction, _timeFragment.Tag);

                    Log.Info(TAG, "ActivitiesWeek_ItemLongClick: groupPosition - " + groupPosition.ToString() + ", childPosition - " + childPosition.ToString());

                    e.Handled = true;
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ActivitiesWeek_ItemLongClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorActivitySelectChildItem), "ActivitiesActivity.ActivitiesWeek_ItemLongClick");
            }
        }

        private void ActivitiesWeek_ChildClick(object sender, ExpandableListView.ChildClickEventArgs e)
        {
            e.Handled = false;

            SetSelectedChildPosition(e.GroupPosition, e.ChildPosition);

            Log.Info(TAG, "ActivitiesWeek_ChildClick: groupPosition - " + e.GroupPosition.ToString() + ", childPosition - " + e.ChildPosition.ToString() + ", ID - " + e.Id.ToString());
        }

        private void ActivitiesWeek_GroupClick(object sender, ExpandableListView.GroupClickEventArgs e)
        {
            e.Handled = false;

            _selectedGroupPosition = e.GroupPosition;

            Log.Info(TAG, "ActivitiesWeek_GroupClick: groupPosition - " + e.GroupPosition.ToString() + ", ID - " + e.Id.ToString());
        }

        public void CompletedActivityTime(int groupPosition, int childPosition)
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();

                if (sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    Log.Info(TAG, "CompletedActivityTime: Calling Save for Activity with ID " + GlobalData.ActivitiesForWeek[groupPosition].ActivityID.ToString());
                    GlobalData.ActivitiesForWeek[groupPosition].Save(sqlDatabase);
                    sqlDatabase.Close();
                    UpdateAdapter();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "CompletedActivityTime: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorActivitySaving), "ActivitiesActivity.CompletedActivityTime");
            }
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ActivitiesActivity.CheckMicPermission");
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
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ActivitiesActivity.OnRequestPermissionsResult");
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ActivitiesActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "ActivitiesActivity.AttemptPermissionRequest");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            try
            {
                if (instanceId == "useMic")
                {
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRequestingApplicationPermission), "ActivitiesActivity.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemExpand = menu.FindItem(Resource.Id.ActivitiesActionExpand);
                var itemCollapse = menu.FindItem(Resource.Id.ActivitiesActionCollapse);
                var itemHelp = menu.FindItem(Resource.Id.ActivitiesActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemExpand != null)
                            itemExpand.SetIcon(Resource.Drawable.expand);
                        if (itemCollapse != null)
                            itemCollapse.SetIcon(Resource.Drawable.collapse);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemExpand != null)
                            itemExpand.SetIcon(Resource.Drawable.expand);
                        if (itemCollapse != null)
                            itemCollapse.SetIcon(Resource.Drawable.collapse);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemExpand != null)
                            itemExpand.SetIcon(Resource.Drawable.expand);
                        if (itemCollapse != null)
                            itemCollapse.SetIcon(Resource.Drawable.collapse);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "MedicationActivity.SetActionIcons");
            }
        }

    }
}
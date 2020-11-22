using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Adapters;
using Android.Util;
using Android.Content;
using Android.Database.Sqlite;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class AchievementChartActivity : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:AchievementChartActivity";

        private TextView _selectedDate;
        private ListView _achievementList;

        public AchievementChartDialogFragment _achievement;

        private Toolbar _toolbar;

        private Button _btnDone;

        private int _selectedItemIndex = -1;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemIndex", _selectedItemIndex);

            base.OnSaveInstanceState(outState);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");

                SetContentView(Resource.Layout.AchievementChartLayout);

                GetFieldComponents();

                CheckMicPermission();

                SetupToolbar();

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.achievement,
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

                if (_selectedDate != null)
                    _selectedDate.Text = DateTime.Now.ToShortDateString();

                GetDataForDate();

                UpdateAdapter();

                if (_selectedItemIndex != -1)
                    _achievementList.SetSelection(_selectedItemIndex);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateChuffChartActivity), "AchievementChartActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_achievementList != null)
                _achievementList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void GetDataForDate()
        {
            try
            {
                if (_selectedDate != null)
                {
                    DateTime theDate = Convert.ToDateTime(_selectedDate.Text.Trim());
                    GlobalData.AchievementChartItems.Clear();
                    GlobalData.GetAchievementChartItemsForDate(theDate);
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetDataForDate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateChuffChartActivity), "AchievementChartActivity.GetDataForDate");
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                if (item != null)
                {
                    if (item.ItemId == Android.Resource.Id.Home)
                    {
                        Finish();
                        return true;
                    }

                    switch (item.ItemId)
                    {
                        case Resource.Id.AchievementActionAdd:
                            EnterAchievement();
                            return true;
                        case Resource.Id.AchievementActionRemove:
                            Remove();
                            return true;
                        case Resource.Id.AchievementActionDate:
                            SelectDate();
                            return true;
                        case Resource.Id.AchievementActionHelp:
                            Intent intent = new Intent(this, typeof(AchievementsHelpActivity));
                            StartActivity(intent);
                            return true;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnOptionsItemSelected: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSelectingMenuOption), "AchievementChartActivity.OnOptionsItemSelected");
            }

            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            try
            {
                _selectedDate = FindViewById<TextView>(Resource.Id.txtSelectedDate);
                _achievementList = FindViewById<ListView>(Resource.Id.lstAchievements);
                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorChuffChartGetComponents), "AchievementChartActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_achievementList != null)
                {
                    _achievementList.ItemClick += AchievementList_ItemClick;
                    _achievementList.LongClick += AchievementList_LongClick;
                }
                if(_btnDone != null)
                    _btnDone.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorChuffChartSetupCallbacks), "AchievementChartActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void AchievementList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateAdapter();
        }

        private void AchievementList_LongClick(object sender, View.LongClickEventArgs e)
        {
            try
            {
                if (_selectedItemIndex == -1 && GlobalData.AchievementChartItems.Count > 0)
                {
                    Toast.MakeText(this, Resource.String.AchievementChartActivitySelectNoEdit, ToastLength.Short).Show();
                    return;
                }
                if (GlobalData.AchievementChartItems.Count == 0)
                {
                    Toast.MakeText(this, Resource.String.AchievementChartActivityEditNoItems, ToastLength.Short).Show();
                    return;
                }

                int achievementID = GlobalData.AchievementChartItems[_selectedItemIndex].AchievementId;
                AchievementChartDialogFragment chartFragment = new AchievementChartDialogFragment(this, "Enter an Achievement", achievementID);
                var transaction = FragmentManager.BeginTransaction();
                chartFragment.Show(transaction, chartFragment.Tag);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AchievementList_LongClick: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorCreatingAchievementChartFragment), "AchievementChartActivity.AchievementList_LongClick");
            }
        }

        private void Remove()
        {
            try
            {
                if (GlobalData.AchievementChartItems.Count > 0)
                {
                    if (_selectedItemIndex != -1)
                    {
                        AlertHelper alertHelper = new AlertHelper(this);

                        alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                        alertHelper.AlertTitle = GetString(Resource.String.AchievementChartActivityRemoveAlertTitle);
                        alertHelper.AlertMessage = GetString(Resource.String.AchievementChartActivityRemoveAlertText);
                        alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                        alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                        alertHelper.InstanceId = "remove";

                        alertHelper.ShowAlert();
                    }
                    else
                    {
                        Toast.MakeText(this, "Please select an item", ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, "There are no items currently in the list", ToastLength.Short).Show();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Remove: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRemovingAchievementItem), "AchievementChartActivity.Remove");
            }
        }

        private void EnterAchievement()
        {
            try
            {
                _achievement = new AchievementChartDialogFragment(this, "Enter Achievement");

                var fragmentTransaction = FragmentManager.BeginTransaction();
                _achievement.Show(fragmentTransaction, _achievement.Tag);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "EnterAchievement_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorEnterAchievement), "AchievementChartActivity.EnterAchievement_Click");
            }
        }

        private void SelectDate()
        {
            try
            {
                DatePickerFragment datePicker = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    _selectedDate.Text = time.ToShortDateString();
                    GetDataForDate();
                    UpdateAdapter();
                });
                datePicker.Show(FragmentManager, DatePickerFragment.TAG);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SelectDate_Click: exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSelectingDateChuffChart), "AchievementChartActivity.SelectDate_Click");
            }
        }

        public void ConfirmClicked(int achievementID, string achievement, AchievementChart.ACHIEVEMENTCHART_TYPE achievmentType)
        {
            try
            {
                if (_selectedDate != null)
                {
                    if (!string.IsNullOrEmpty(achievement))
                    {
                        AchievementChart newAchievement = new AchievementChart();
                        if (achievementID != -1)
                        {
                            newAchievement.IsNew = false;
                            newAchievement.IsDirty = true;
                        }
                        newAchievement.Achievement = achievement.Trim();
                        newAchievement.AchievementDate = Convert.ToDateTime(_selectedDate.Text.Trim());
                        newAchievement.AchievementChartType = achievmentType;
                        Globals dbHelp = new Globals();
                        dbHelp.OpenDatabase();
                        var sqlDatabase = dbHelp.GetSQLiteDatabase();
                        if (sqlDatabase != null)
                        {
                            newAchievement.Save(sqlDatabase);
                        }
                        dbHelp.CloseDatabase();
                        GetDataForDate();
                        _selectedItemIndex = -1;
                        UpdateAdapter();
                    }
                }
                Toast.MakeText(this, Resource.String.chuffChartAchievementRecorded, ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ConfirmClicked: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorEnterAchievement), "AchievementChartActivity.ConfirmClicked");
            }
        }

        public void CancelClicked()
        {
            Toast.MakeText(this, Resource.String.chuffChartFragmentQuestionCancel, ToastLength.Long).Show();
        }

        private void UpdateAdapter()
        {
            try
            {
                AchievementChartRecordsAdapter adapter = new AchievementChartRecordsAdapter(this);
                if (_achievementList != null)
                {
                    _achievementList.Adapter = adapter;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorChuffChartUpdateAdapter), "AchievementChartActivity.UpdateAdapter");
            }
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "remove")
            {
                int achievementID = -1;
                SQLiteDatabase sqlDatabase = null;
                try
                {
                    if (_selectedItemIndex != -1)
                    {
                        var achievement = GlobalData.AchievementChartItems[_selectedItemIndex];
                        achievementID = achievement.AchievementId;

                        Globals dbHelp = new Globals();
                        dbHelp.OpenDatabase();
                        sqlDatabase = dbHelp.GetSQLiteDatabase();
                        if (sqlDatabase != null && sqlDatabase.IsOpen)
                        {
                            achievement.Remove(sqlDatabase);

                            GlobalData.AchievementChartItems.Remove(achievement);
                            sqlDatabase.Close();
                            _selectedItemIndex = -1;
                            UpdateAdapter();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                    if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAchievementChartActivityRemove), "AchievementChartActivity.AlertPositiveButtonSelect");
                    if (sqlDatabase != null && sqlDatabase.IsOpen)
                        sqlDatabase.Close();
                }
            }
            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
            }

            if (instanceId == "remove")
            {
                Toast.MakeText(this, Resource.String.AchievementChartActivityRemoveToast, ToastLength.Short).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.AchievementMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void SetupToolbar()
        {
            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.chuffChartToolbar, Resource.String.AchievementChartTitle, Color.White);
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.AchievementActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.AchievementActionRemove);
                var itemDate = menu.FindItem(Resource.Id.AchievementActionDate);
                var itemHelp = menu.FindItem(Resource.Id.AchievementActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if (itemDate != null)
                            itemDate.SetIcon(Resource.Drawable.ic_date_range_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemDate != null)
                            itemDate.SetIcon(Resource.Drawable.ic_date_range_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
                        if (itemDate != null)
                            itemDate.SetIcon(Resource.Drawable.ic_date_range_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "AchievementChartActivity.SetActionIcons");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "AchievementChartActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "AchievementChartActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {

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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "AchievementChartActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "AchievementChartActivity.AttemptPermissionRequest");
            }
        }
    }
}
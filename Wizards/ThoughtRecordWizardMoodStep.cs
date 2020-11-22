using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using V7Sup = Android.Support.V7.App;
using com.spanyardie.MindYourMood.Model;
using Android.Content;
using Android.App;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.SubActivities.Help.ThoughtsHelp;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.Wizards
{
    [Activity]
    public class ThoughtRecordWizardMoodStep : V7Sup.AppCompatActivity, IAlertCallback
    {
        public static string TAG = "M:ThoughtRecordWizardMoodStep";

        private LinearLayout _rootMoods;

        private ListView _moodList;
        private Spinner _spinnerMood;
        private SeekBar _moodStrengthSeekBar;
        private TextView _percentageLabel;
        private Toolbar _toolbar;

        private bool _validated;
        private int _selectedItemIndex = -1;

        private bool _setupCallbacksComplete = false;

        private Button _continue;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemIndex", _selectedItemIndex);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MoodMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");

                SetContentView(Resource.Layout.Moods);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.moodsToolbar, Resource.String.MoodsHeading, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.moodsDream,
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

                SetupSpinner();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateMoodActivity), "ThoughtRecordWizardMoodStep.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_rootMoods != null)
                _rootMoods.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                GetFieldComponents();
                SetupCallbacks();
                SetupSpinner();
                UpdateAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnResume: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorResumeMoodActivity), "ThoughtRecordWizardMoodStep.OnResume");
            }
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            try
            {
                if (e.KeyCode == Keycode.Back)
                {
                    Log.Info(TAG, "DispatchKeyEvent: Removing database Moods...");
                    GlobalData.RemoveMoods();
                    Log.Info(TAG, "DispatchKeyEvent: Removing Global Mood Items...");
                    GlobalData.MoodItems.Clear();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "DispatchKeyEvent: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMoodDispatchkeyEvent), "ThoughtRecordWizardMoodStep.DispatchKeyEvent");
            }
            return base.DispatchKeyEvent(e);
        }

        private void SetupSpinner()
        {
            try
            {
                if (_spinnerMood != null)
                {
                    Globals dbHelp = new Globals();
                    dbHelp.OpenDatabase();
                    List<string> moods = dbHelp.GetAllMoodsForAdapter();
                    dbHelp.CloseDatabase();

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, moods.ToArray());

                    adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                    _spinnerMood.Adapter = adapter;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupSpinner: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMoodSetupSpinner), "ThoughtRecordWizardMoodStep.SetupSpinner");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (!_setupCallbacksComplete)
                {
                    if(_moodStrengthSeekBar != null)
                        _moodStrengthSeekBar.ProgressChanged += MoodStrengthSeekBar_ProgressChanged; ;
                    if(_moodList != null)
                        _moodList.ItemClick += MoodList_ItemClick;
                    if(_continue != null)
                        _continue.Click += Continue_Click;
                    _setupCallbacksComplete = true;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                _setupCallbacksComplete = false;
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMoodSetupCallbacks), "ThoughtRecordWizardMoodStep.SetupCallbacks");
            }
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void RemoveMoodImageButton()
        {
            try
            {
                if (_selectedItemIndex > -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertMessage = GetString(Resource.String.ThoughtRecordWizardMoodDeleteConfirm);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonCancelCaption);
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonOKCaption);
                    alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardMoodDeleteTitle);
                    alertHelper.InstanceId = "moodRemove";

                    alertHelper.ShowAlert();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RemoveMoodImageButton_Click: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRemovingMood), "ThoughtRecordWizardMoodStep.RemoveMoodImageButton_Click");
            }
        }

        private void MoodList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;
                UpdateAdapter();
                _moodList.SetSelection(_selectedItemIndex);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "MoodList_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSelectingMood), "ThoughtRecordWizardMoodStep.MoodList_ItemClick");
            }
        }

        private void MoodStrengthSeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            try
            {
                var val = e.Progress;
                if (_percentageLabel != null)
                    _percentageLabel.Text = val.ToString() + "%";
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "MoodStrengthSeekBar_ProgressChanged: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMoodSeekbarChanged), "ThoughtRecordWizardMoodStep.MoodStrengthSeekBar_ProgressChanged");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.moodsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.moodsActionRemove);
                var itemCancel = menu.FindItem(Resource.Id.moodsActionCancel);
                var itemHelp = menu.FindItem(Resource.Id.moodsActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if(itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
                        if (itemCancel != null)
                            itemCancel.SetIcon(Resource.Drawable.ic_cancel_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ThoughtRecordWizardMoodStep.SetActionIcons");
            }
        }

        private void Previous()
        {
            try
            {
                //if we are going back to the situation then clear out the Moods beforehand
                GlobalData.MoodItems.Clear();
                Intent intent = new Intent(this, typeof(ThoughtRecordWizardSituationStep));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                Finish();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Previous: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMoodPreviousButton), "ThoughtRecordWizardMoodStep.Previous");
            }
        }

        private void Cancel()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertMessage = GetString(Resource.String.ThoughtRecordWizardAutomaticThoughtConfirm);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonCancelCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonOKCaption);
                alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardAutomaticThoughtCancel);
                alertHelper.InstanceId = "moodCancel";

                alertHelper.ShowAlert();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "CancelButton_Click: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCancelMoodAddition), "ThoughtRecordWizardMoodStep.CancelButton_Click");
            }
        }

        private void Next()
        {
            try
            {
                Validate();
                if (!_validated)
                    return;

                if (GlobalData.MoodItems.Count > 0 && GlobalData.MoodItems[0].MoodsId == 0)
                {
                    StoreMoods();
                }

                Intent intent = new Intent(this, typeof(ThoughtRecordWizardAutomaticThoughtsStep));
                StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Next: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorNextButtonMood), "ThoughtRecordWizardMoodStep.Next");
            }
        }

        public void StoreMoods()
        {
            Globals dbHelp = new Globals();

            try
            {
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var mood in GlobalData.MoodItems)
                    {
                        if (mood.MoodsId == 0)
                            mood.Save(sqlDatabase);
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "StoreMoods: Exception - " + e.Message);
                if (dbHelp != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStoringMoods), "ThoughtRecordWizardMoodStep.StoreMoods");
            }
        }

        private void AddMoodImageButton()
        {
            try
            {
                if (MoodAlreadySelected())
                {
                    Toast.MakeText(this, Resource.String.moodAlreadyAdded, ToastLength.Short).Show();
                    return;
                }

                Mood mood = new Mood();
                mood.ThoughtRecordId = GlobalData.ThoughtRecordId;
                mood.MoodListId = GlobalData.MoodListItems[(int)_spinnerMood.SelectedItemId].MoodId;
                mood.MoodRating = Convert.ToInt32(_percentageLabel.Text.Trim().Replace("%", ""));

                GlobalData.MoodItems.Add(mood);

                UpdateAdapter();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AddMoodImageButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAddingMood), "ThoughtRecordWizardMoodStep.AddMoodImageButton_Click");
            }
        }

        private void UpdateAdapter()
        {
            try
            {
                MoodItemsAdapter moodAdapter = new MoodItemsAdapter(this);
                _moodList.Adapter = moodAdapter;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMoodUpdateAdapter), "ThoughtRecordWizardMoodStep.UpdateAdapter");
            }
        }

        private bool MoodAlreadySelected()
        {
            //get the current selected mood from the spinner
            try
            {
                if (_spinnerMood != null)
                {
                    var selectedId = _spinnerMood.SelectedItemId;
                    Log.Info(TAG, "MoodAlreadySelected: spinner selected index - " + selectedId.ToString());
                    foreach (Mood mood in GlobalData.MoodItems)
                    {
                        if (mood.MoodListId == GlobalData.MoodListItems[(int)selectedId].MoodId)
                        {
                            return true;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "MoodAlreadySelected: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSelectingMood), "ThoughtRecordWizardMoodStep.MoodAlreadySelected");
                //return True to stop any further processing
                return true;
            }
            return false;
        }

        private void GetFieldComponents()
        {
            try
            {
                _moodList = FindViewById<ListView>(Resource.Id.lstMoodList);
                _moodStrengthSeekBar = FindViewById<SeekBar>(Resource.Id.skbRateMood);
                _percentageLabel = FindViewById<TextView>(Resource.Id.txtPercentageLabel);
                _spinnerMood = FindViewById<Spinner>(Resource.Id.spnMoodList);
                _continue = FindViewById<Button>(Resource.Id.btnContinue);
                _rootMoods = FindViewById<LinearLayout>(Resource.Id.rootMoods);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMoodGetComponents), "ThoughtRecordWizardMoodStep.GetFieldComponents");
            }
        }

        private void Validate()
        {
            try
            {
                _validated = true;

                if (_moodList != null)
                {
                    if (_moodList.Adapter != null)
                    {
                        if (_moodList.Adapter.Count == 0)
                        {
                            var resourceString = GetString(Resource.String.moodInvalidData);
                            Toast.MakeText(this, resourceString, ToastLength.Short).Show();
                            _validated = false;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Validate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorValidatingMood), "ThoughtRecordWizardMoodStep.Validate");
            }
        }

        public int GetSelectedItem()
        {
            return _selectedItemIndex;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    Previous();
                    return true;
                }

                switch (item.ItemId)
                {
                    case Resource.Id.moodsActionAdd:
                        AddMoodImageButton();
                        return true;

                    case Resource.Id.moodsActionRemove:
                        RemoveMoodImageButton();
                        return true;

                    case Resource.Id.moodsActionCancel:
                        Cancel();
                        return true;

                    case Resource.Id.moodsActionHelp:
                        Intent intent = new Intent(this, typeof(MoodsHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void Cleanup()
        {
            try
            {
                GlobalData.RemoveThoughtRecord();
                GlobalData.SituationItem.What = "";
                GlobalData.SituationItem.When = "";
                GlobalData.SituationItem.Where = "";
                GlobalData.SituationItem.Who = "";
                GlobalData.RemoveSituation();
                GlobalData.MoodItems.Clear();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Cleanup: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMoodCleanup), "ThoughtRecordWizardMoodStep.Cleanup");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if(instanceId == "moodCancel")
            {
                Cleanup();
                Intent intent = new Intent(this, typeof(ThoughtRecordsActivity));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
            }
            if(instanceId == "moodRemove")
            {
                var item = GlobalData.MoodItems[_selectedItemIndex];
                GlobalData.MoodItems.Remove(item);
                UpdateAdapter();
                _selectedItemIndex = -1;
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            
        }
    }
}
using System;
using System.Collections.Generic;
using Android.OS;
using Android.Widget;
using V7Sup = Android.Support.V7.App;
using com.spanyardie.MindYourMood.Model;
using Android.Content;
using Android.App;
using Android.Views;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.SubActivities.Help.ThoughtsHelp;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.Wizards
{
    [Activity]
    public class ThoughtRecordWizardRerateMoodStep : V7Sup.AppCompatActivity, IAlertCallback
    {
        public static string TAG = "M:ThoughtRecordWizardRerateMoodStep";
        private ListView _moodList;
        private Spinner _spinnerMood;
        private SeekBar _moodRerateSeekBar;
        private SeekBar _moodAdditionalRateSeekbar;
        private TextView _percentageRerateLabel;
        private TextView _percentageRateAdditionalLabel;
        private Toolbar _toolbar;
        private LinearLayout _linRerateMoodsStepMain;

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
            MenuInflater.Inflate(Resource.Menu.RerateMoodsMenu, menu);

            SetActionIcons(menu);

            return true;
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
                    case Resource.Id.reratemoodsActionAdd:
                        AddMoodImageButton();
                        return true;

                    case Resource.Id.reratemoodsActionRemove:
                        RemoveMoodImageButton();
                        return true;

                    case Resource.Id.reratemoodsActionCancel:
                        Cancel();
                        return true;

                    case Resource.Id.reratemoodsActionHelp:
                        Intent intent = new Intent(this, typeof(RerateMoodsHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");

                SetContentView(Resource.Layout.RerateMoods);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.reratemoodsToolbar, Resource.String.rerateHeading, Color.White);

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

                if (savedInstanceState != null)
                {
                    UpdateAdapter();
                }
                else
                {
                    UpdateAdapter(true);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateRerateMoodActivity), "ThoughtRecordWizardRerateMoodStep.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linRerateMoodsStepMain != null)
                _linRerateMoodsStepMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
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
                Log.Error(TAG, "OnResume");
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorResumeRerateMoodActivity), "ThoughtRecordWizardRerateMoodStep.OnResume");
            }
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            try
            {
                if (e.KeyCode == Keycode.Back)
                {
                    GlobalData.RemoveReratedMoods();
                    GlobalData.RerateMoodsItems.Clear();
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "DispatchKeyEvent: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRerateMoodDispatchkeyEvent), "ThoughtRecordWizardRerateMoodStep.DispatchKeyEvent");
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

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, moods);

                    adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                    _spinnerMood.Adapter = adapter;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupSpinner: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRerateMoodSetupSpinner), "ThoughtRecordWizardRerateMoodStep.SetupSpinner");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (!_setupCallbacksComplete)
                {
                    if(_moodRerateSeekBar != null)
                        _moodRerateSeekBar.ProgressChanged += MoodRerateSeekBar_ProgressChanged; ;
                    if(_moodAdditionalRateSeekbar != null)
                        _moodAdditionalRateSeekbar.ProgressChanged += MoodAdditionalRateSeekbar_ProgressChanged;
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRerateMoodSetupCallbacks), "ThoughtRecordWizardRerateMoodStep.SetupCallbacks");
            }
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void MoodAdditionalRateSeekbar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            try
            {
                var val = e.Progress;
                if (_percentageRateAdditionalLabel != null)
                    _percentageRateAdditionalLabel.Text = val.ToString() + "%";

                if (_selectedItemIndex != -1)
                {
                    //update the list
                    if (GlobalData.RerateMoodsItems[_selectedItemIndex] != null)
                    {
                        if (GlobalData.RerateMoodsItems[_selectedItemIndex].FromMood == false)
                        {
                            GlobalData.RerateMoodsItems[_selectedItemIndex].MoodRating = val;
                            UpdateAdapter();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "MoodAdditionalRateSeekbar_ProgressChanged: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRerateMoodSeekbarChanged), "ThoughtRecordWizardRerateMoodStep.MoodAdditionalRateSeekbar_ProgressChanged");
            }
        }

        private void RemoveMoodImageButton()
        {
            try
            {
                if (_selectedItemIndex > -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertMessage = GetString(Resource.String.ThoughtRecordWizardRerateMoodDeleteConfirm);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonCancelCaption);
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonOKCaption);
                    alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardRerateMoodDeleteTitle);
                    alertHelper.InstanceId = "rerateRemove";

                    alertHelper.ShowAlert();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RemoveMoodImageButton_Click: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRemovingRerateMood), "ThoughtRecordWizardRerateMoodStep.RemoveMoodImageButton_Click");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorSelectingRerateMood), "ThoughtRecordWizardRerateMoodStep.MoodList_ItemClick");
            }
        }

        private void MoodRerateSeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            try
            {
                //we need to have a mood selected
                if (_selectedItemIndex != -1)
                {
                    var val = e.Progress;
                    if (_percentageRerateLabel != null)
                        _percentageRerateLabel.Text = val.ToString() + "%";

                    //update the list
                    if (GlobalData.RerateMoodsItems[_selectedItemIndex] != null)
                    {
                        if (GlobalData.RerateMoodsItems[_selectedItemIndex].FromMood)
                        {
                            GlobalData.RerateMoodsItems[_selectedItemIndex].MoodRating = val;
                            UpdateAdapter();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "MoodRerateSeekBar_ProgressChanged: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRerateMoodSeekbarChanged), "ThoughtRecordWizardRerateMoodStep.MoodRerateSeekBar_ProgressChanged");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.reratemoodsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.reratemoodsActionRemove);
                var itemCancel = menu.FindItem(Resource.Id.reratemoodsActionCancel);
                var itemHelp = menu.FindItem(Resource.Id.reratemoodsActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ThoughtRecordWizardRerateMoodStep.SetActionIcons");
            }
        }

        private void Previous()
        {
            try
            {
                //if we are going back to the situation then clear out the Moods beforehand
                GlobalData.RemoveReratedMoods();
                GlobalData.RerateMoodsItems.Clear();
                Intent intent = new Intent(this, typeof(ThoughtRecordWizardAlternativeThoughtStep));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                Finish();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "PreviousButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRerateMoodPreviousButton), "ThoughtRecordWizardRerateMoodStep.PreviousButton_Click");
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
                alertHelper.AlertTitle = GetString(Resource.String.ThoughtRecordWizardRerateMoodCancel);
                alertHelper.InstanceId = "rerateCancel";

                alertHelper.ShowAlert();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Cancel: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCancelRerateMoodAddition), "ThoughtRecordWizardRerateMoodStep.Cancel");
            }
        }

        private void Next()
        {
            try
            {
                Validate();
                if (!_validated)
                    return;

                //we are now, finally, finished
                var resourceString = GetString(Resource.String.thoughtRecordEntryCompleted);
                Toast.MakeText(this, resourceString, ToastLength.Long).Show();

                StoreReratedMoods();


                GlobalData.SituationItem.SituationId = -1;
                GlobalData.SituationItem.What = "";
                GlobalData.SituationItem.When = "";
                GlobalData.SituationItem.Where = "";
                GlobalData.SituationItem.Who = "";

                GlobalData.MoodItems.Clear();
                GlobalData.AutomaticThoughtsItems.Clear();
                GlobalData.EvidenceForHotThoughtItems.Clear();
                GlobalData.EvidenceAgainstHotThoughtItems.Clear();
                GlobalData.AlternativeThoughtsItems.Clear();
                GlobalData.RerateMoodsItems.Clear();

                Intent intent = new Intent(this, typeof(ThoughtRecordsActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                StartActivity(intent);
                Finish();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "NextButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorNextButtonRerateMood), "ThoughtRecordWizardRerateMoodStep.NextButton_Click");
            }
        }

        private void StoreReratedMoods()
        {
            Globals dbHelp = new Globals();

            try
            {
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var thought in GlobalData.RerateMoodsItems)
                    {
                        if (thought.RerateMoodId == 0)
                        {
                            thought.Save(sqlDatabase);
                        }
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "StoreReratedMoods: Exception - " + e.Message);
                if (dbHelp != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStoringRerateMoods), "ThoughtRecordWizardRerateMoodStep.StoreReratedMoods");
            }
        }

        private void AddMoodImageButton()
        {
            try
            {
                if (RerateMoodAlreadySelected())
                {
                    var resourceString = GetString(Resource.String.moodAlreadyAdded);
                    Toast.MakeText(this, resourceString, ToastLength.Short).Show();
                    return;
                }

                RerateMood mood = new RerateMood();
                mood.ThoughtRecordId = GlobalData.ThoughtRecordId;
                mood.MoodListId = (int)GlobalData.MoodListItems[(int)_spinnerMood.SelectedItemId].MoodId;
                mood.MoodRating = Convert.ToInt32(_percentageRateAdditionalLabel.Text.Trim().Replace("%", ""));

                GlobalData.RerateMoodsItems.Add(mood);

                UpdateAdapter();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AddMoodImageButton_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAddingRerateMood), "ThoughtRecordWizardRerateMoodStep.AddMoodImageButton_Click");
            }
        }

        private void UpdateAdapter(bool firstTimeInit = false)
        {
            try
            {
                RerateMoodItemsAdapter moodAdapter = new RerateMoodItemsAdapter(this, firstTimeInit);
                _moodList.Adapter = moodAdapter;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRerateMoodUpdateAdapter), "ThoughtRecordWizardRerateMoodStep.UpdateAdapter");
            }
        }

        private bool RerateMoodAlreadySelected()
        {
            try
            {
                //get the current selected mood from the spinner
                if (_spinnerMood != null)
                {
                    var selectedId = _spinnerMood.SelectedItemId;
                    foreach (RerateMood mood in GlobalData.RerateMoodsItems)
                    {
                        if (mood.MoodListId == GlobalData.MoodListItems[(int)selectedId].MoodId)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RerateMoodAlreadySelected: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSelectingRerateMood), "ThoughtRecordWizardRerateMoodStep.RerateMoodAlreadySelected");
                return false;
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _moodList = FindViewById<ListView>(Resource.Id.lstRerateMoods);
                _moodRerateSeekBar = FindViewById<SeekBar>(Resource.Id.skbRerateMood);
                _moodAdditionalRateSeekbar = FindViewById<SeekBar>(Resource.Id.skbRateAdditionalMood);
                _percentageRerateLabel = FindViewById<TextView>(Resource.Id.txtReratePercentageLabel);
                _percentageRateAdditionalLabel = FindViewById<TextView>(Resource.Id.txtAdditionalMoodPercentageLabel);
                _spinnerMood = FindViewById<Spinner>(Resource.Id.spnRerateMoodList);
                _continue = FindViewById<Button>(Resource.Id.btnContinue);
                _linRerateMoodsStepMain = FindViewById<LinearLayout>(Resource.Id.linRerateMoodsStepMain);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRerateMoodGetComponents), "ThoughtRecordWizardRerateMoodStep.GetFieldComponents");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorValidatingRerateMood), "ThoughtRecordWizardRerateMoodStep.Validate");
            }
        }

        public int GetSelectedItem()
        {
            return _selectedItemIndex;
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
                GlobalData.RemoveMoods();
                GlobalData.RemoveAutomaticThoughts();
                GlobalData.RemoveEvidenceForHotThought();
                GlobalData.RemoveEvidenceAgainstHotThought();
                GlobalData.RemoveAlternativeThoughts();
                GlobalData.RemoveReratedMoods();
                GlobalData.MoodItems.Clear();
                GlobalData.AutomaticThoughtsItems.Clear();
                GlobalData.EvidenceForHotThoughtItems.Clear();
                GlobalData.EvidenceAgainstHotThoughtItems.Clear();
                GlobalData.AlternativeThoughtsItems.Clear();
                GlobalData.RerateMoodsItems.Clear();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Cleanup: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorRerateMoodCleanup), "ThoughtRecordWizardRerateMoodStep.Cleanup");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if(instanceId == "rerateCancel")
            {
                Cleanup();
                Intent intent = new Intent(this, typeof(ThoughtRecordsActivity));
                Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
            }
            if(instanceId == "rerateRemove")
            {
                var item = GlobalData.RerateMoodsItems[_selectedItemIndex];
                GlobalData.RerateMoodsItems.Remove(item);
                UpdateAdapter();
                _selectedItemIndex = -1;
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            
        }
    }
}
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Java.Lang;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class MedicationPagerAdapter : PagerAdapter, IMedicationTime, IAlertCallback
    {
        public const string TAG = "M:MedicationPagerAdapter";

        public enum MedicationView
        {
            MedicationName = 0,
            MedicationInterval,
            MedicationTimes
        }
        private MedicationView _currentView;

        private ImageButton _speakMedicationName;
        private ImageButton _speakMedicationDosage;
        private ImageButton _speakMonthDay;
        private bool _speakPermission = false;
        private EditText _medicationNameText;
        private EditText _medicationDosageText;

        private Spinner _medicationIntervalType;
        private Spinner _prescriptionWeekDay;
        private EditText _prescriptionMonthDay;

        private TextView _txtMedListPrescWeekDayLabel;
        private TextView _txtMedListPrescMonthDayLabel;

        private Context _context;

        //the Medication passed in
        private Medication _medication;

        private ListView _medicationTimeList;
        private Button _medicationTimeAdd;
        private Button _medicationTimeRemove;
        private Button _medicationAlarm;
        private int _selectedItemIndex = -1;
        private int _selectedMedicationTimeID = -1;

        private bool _isSpeakingName = false;
        private bool _isSpeakingDosage = false;
        private bool _isSpeakingMonthDay = false;

        private bool _isNew = true;
        private bool _isDirty = false;

        private bool _isLoading = true;

        public MedicationPagerAdapter(Context context, Medication medication, bool speakPermission)
        {
            _context = context;
            _medication = medication;
            if(_medication != null)
            {
                _isNew = _medication.IsNew;
                _isDirty = _medication.IsDirty;
            }
            _speakPermission = speakPermission;
        }

        public Context GetContext()
        {
            return _context;
        }

        public int GetSelectedTimeItemIndex()
        {
            return SelectedItemIndex;
        }

        public override int Count
        {
            get
            {
                return 3;
            }
        }

        public Medication Medication
        {
            get
            {
                return _medication;
            }

            set
            {
                _medication = value;
            }
        }

        public bool IsNew
        {
            get
            {
                return _isNew;
            }

            set
            {
                _isNew = value;
            }
        }

        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }

            set
            {
                _isDirty = value;
            }
        }

        public int SelectedItemIndex
        {
            get
            {
                return _selectedItemIndex;
            }

            set
            {
                _selectedItemIndex = value;
            }
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
        {
            return view == objectValue;
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object objectValue)
        {
            var viewPager = container.JavaCast<ViewPager>();
            if(viewPager != null)
                viewPager.RemoveView(objectValue as View);
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            Java.Lang.String title = new Java.Lang.String("");
            try
            {
                MedicationView view = (MedicationView)position;

                switch (view)
                {
                    case MedicationView.MedicationName:
                        title = new Java.Lang.String(_context.GetString(Resource.String.MedicationStepOneTitle));
                        break;
                    case MedicationView.MedicationInterval:
                        title = new Java.Lang.String(_context.GetString(Resource.String.MedicationStepTwoTitle));
                        break;
                    case MedicationView.MedicationTimes:
                        title = new Java.Lang.String(_context.GetString(Resource.String.MedicationStepThreeTitle));
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "GetPageTitleFormatted: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed getting formatted title string", "MedicationPagerAdapter.GetPageTitleFormatted");
            }
            return title;
        }

        private void GetSpeakView(View currentView, int position)
        {
            MedicationView view = (MedicationView)position;

            try
            {
                if (currentView != null)
                {
                    switch (view)
                    {
                        case MedicationView.MedicationName:
                            _speakMedicationName = currentView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakMedicationName);
                            _speakMedicationDosage = currentView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakMedicationDosage);
                            break;
                        case MedicationView.MedicationInterval:
                            _speakMonthDay = currentView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakMonthDay);
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "GetSpeakView: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed getting correct Speak Image Button", "MedicationPagerAdapter.GetSpeakView");
            }
        }

        private void GetTextView(View currentView, int position)
        {
            MedicationView view = (MedicationView)position;

            try
            {
                if (currentView != null)
                {
                    switch (view)
                    {
                        case MedicationView.MedicationName:
                            _medicationNameText = currentView.FindViewById<EditText>(Resource.Id.edtMedicationName);
                            _medicationDosageText = currentView.FindViewById<EditText>(Resource.Id.edtMedListMainDailyDose);
                            break;
                        case MedicationView.MedicationInterval:
                            _prescriptionMonthDay = currentView.FindViewById<EditText>(Resource.Id.edtMedListPrescMonthDay);
                            _txtMedListPrescWeekDayLabel = currentView.FindViewById<TextView>(Resource.Id.txtMedListPrescWeekDayLabel);
                            _txtMedListPrescMonthDayLabel = currentView.FindViewById<TextView>(Resource.Id.txtMedListPrescMonthDayLabel);
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "GetTextView: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed getting correct Text or Edit view", "MedicationPagerAdapter.GetTextView");
            }
        }

        private void GetSpinnerView(View currentView, int position)
        {
            MedicationView view = (MedicationView)position;

            try
            {
                if (currentView != null)
                {
                    switch (view)
                    {
                        case MedicationView.MedicationInterval:
                            _prescriptionWeekDay = currentView.FindViewById<Spinner>(Resource.Id.spnMedListPrescWeekDay);
                            _medicationIntervalType = currentView.FindViewById<Spinner>(Resource.Id.spnMedListPrescType);
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "GetSpinnerView: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed getting correct Spinner view", "MedicationPagerAdapter.GetSpinnerView");
            }
        }

        private void GetButtonView(View currentView, int position)
        {
            MedicationView view = (MedicationView)position;

            try
            {
                if (currentView != null)
                {
                    switch (view)
                    {
                        case MedicationView.MedicationTimes:
                            _medicationTimeAdd = currentView.FindViewById<Button>(Resource.Id.btnMedListTimeAdd);
                            _medicationTimeRemove = currentView.FindViewById<Button>(Resource.Id.btnMedListTimeRemove);
                            _medicationAlarm = currentView.FindViewById<Button>(Resource.Id.btnMedListTimeAlarm);
                            _medicationTimeList = currentView.FindViewById<ListView>(Resource.Id.lstMedListTime);
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "GetSpinnerView: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed getting correct Spinner view", "MedicationPagerAdapter.GetSpinnerView");
            }
        }

        private void HandleMicPermission()
        {
            try
            {
                if (!(PermissionsHelper.HasPermission(_context, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(_context, ConstantsAndTypes.AppPermission.UseMicrophone)))
                {
                    if (_speakMedicationName != null)
                        _speakMedicationName.Enabled = false;

                    if (_speakMedicationDosage != null)
                        _speakMedicationDosage.Enabled = false;
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "HandleMicPermission: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Unable to determine Microphone permission", "MedicationPagerAdapter.HandleMicPermission");
            }
        }

        private View GetViewOnPosition(MedicationView position)
        {
            View view = null;

            try
            {
                switch (position)
                {
                    case MedicationView.MedicationName:
                        view = ((Activity)_context).LayoutInflater.Inflate(Resource.Layout.MedicationStepMedicationLayout, null);
                        _currentView = MedicationView.MedicationName;
                        break;

                    case MedicationView.MedicationInterval:
                        view = ((Activity)_context).LayoutInflater.Inflate(Resource.Layout.MedicationStepIntervalLayout, null);
                        _currentView = MedicationView.MedicationInterval;
                        break;

                    case MedicationView.MedicationTimes:
                        view = ((Activity)_context).LayoutInflater.Inflate(Resource.Layout.MedicationStepTimeLayout, null);
                        _currentView = MedicationView.MedicationTimes;
                        break;
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "GetViewOnPosition: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed to inflate relevant view", "MedicationPagerAdapter.GetViewOnPosition");
            }
            return view;
        }

        public override Java.Lang.Object InstantiateItem(View container, int position)
        {
            _isLoading = true;

            //get our View based on position
            View currentView = null;
            try
            {
                currentView = GetViewOnPosition((MedicationView)position);

                GetFieldComponents(currentView, position);

                if (_speakMedicationName != null)
                    _speakMedicationName.Enabled = _speakPermission;
                if (_speakMedicationDosage != null)
                    _speakMedicationDosage.Enabled = _speakPermission;
                if (_speakMonthDay != null)
                    _speakMonthDay.Enabled = _speakPermission;

                //add the correct callbacks for this view
                SetupCallbacks(currentView, position);

                SetupSpinners();

                AssignTexts(currentView, position);

                var containerView = container.JavaCast<ViewPager>();

                containerView.AddView(currentView);

                _isLoading = false;

            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "InstantiateItem: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Unable to instantiate item", "MedicationPagerAdapter.InstantiateItem");
            }
            return currentView;
        }

        private void AssignTexts(View currentView, int position)
        {
            try
            {
                if (currentView != null)
                {
                    switch ((MedicationView)position)
                    {
                        case MedicationView.MedicationName:
                            if (_medicationNameText != null)
                                _medicationNameText.Text = Medication.MedicationName;
                            if (_medicationDosageText != null)
                                _medicationDosageText.Text = Medication.TotalDailyDosage.ToString();
                            break;
                        case MedicationView.MedicationInterval:
                            if (_prescriptionWeekDay != null)
                                _prescriptionWeekDay.SetSelection((int)Medication.PrescriptionType.WeeklyDay);
                            break;
                        case MedicationView.MedicationTimes:
                            UpdateTimeAdapter();
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "AssignTexts: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Unable to assign text values", "MedicationPagerAdapter.AssignTexts");
            }
        }

        private void GetFieldComponents(View currentView, int position)
        {
            try
            {
                GetSpeakView(currentView, position);
                GetTextView(currentView, position);
                GetSpinnerView(currentView, position);
                GetButtonView(currentView, position);
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed retrieving field components", "MedicationPagerAdapter.GetFieldComponents");
            }
        }

        private void SetupCallbacks(View currentView, int position)
        {
            try
            {
                if (currentView != null)
                {
                    if (_speakMedicationName != null)
                    {
                        _speakMedicationName.Click += SpeakMedicationName_Click;
                        _speakMedicationName.Tag = (int)_currentView;
                    }

                    if (_speakMedicationDosage != null)
                    {
                        _speakMedicationDosage.Click += SpeakMedicationDosage_Click;
                        _speakMedicationDosage.Tag = (int)_currentView;
                    }
                    if (_speakMonthDay != null)
                    {
                        _speakMonthDay.Click += SpeakMonthDay_Click;
                        _speakMonthDay.Tag = (int)_currentView;
                    }
                    if (_medicationNameText != null)
                    {
                        _medicationNameText.TextChanged += MedicationNameText_TextChanged;
                        _medicationNameText.Tag = (int)_currentView;
                    }
                    if (_medicationDosageText != null)
                    {
                        _medicationDosageText.TextChanged += MedicationDosageText_TextChanged;
                        _medicationDosageText.Tag = (int)_currentView;
                    }
                    if (_medicationIntervalType != null)
                    {
                        _medicationIntervalType.ItemSelected += MedicationIntervalType_ItemSelected;
                        _medicationIntervalType.Tag = (int)_currentView;
                    }
                    if (_medicationTimeAdd != null)
                    {
                        _medicationTimeAdd.Click += MedicationTimeAdd_Click;
                        _medicationTimeAdd.Tag = (int)_currentView;
                    }
                    if (_medicationTimeRemove != null)
                    {
                        _medicationTimeRemove.Click += MedicationTimeRemove_Click;
                        _medicationTimeRemove.Tag = (int)_currentView;
                    }
                    if (_medicationAlarm != null)
                    {
                        _medicationAlarm.Click += MedicationAlarm_Click;
                        _medicationAlarm.Tag = (int)_currentView;
                    }
                    if (_medicationTimeList != null)
                    {
                        _medicationTimeList.ItemClick += MedicationTimeList_ItemClick; ;
                        _medicationTimeList.Tag = (int)_currentView;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed setting up callbacks", "MedicationPagerAdapter.SetupCallbacks");
            }
        }

        private void MedicationTimeList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                SelectedItemIndex = e.Position;
                UpdateTimeAdapter();
                _medicationTimeList.SetSelection(SelectedItemIndex);
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "MedicationTimeList_ItemClick: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, "Failed selecting Medication Time", "MedicationPagerAdapter.MedicationTimeList_ItemClick");
            }
        }

        private void MedicationAlarm_Click(object sender, EventArgs e)
        {
            if (SelectedItemIndex == -1) return;

            try
            {

                var spread = _medication.MedicationSpread[_selectedItemIndex];

                if (spread == null) return;

                //if the alarm is set
                if (spread.MedicationTakeReminder != null)
                {
                    //cancel the alarm, use the Alert function without displaying an alert
                    AlertPositiveButtonSelect(null, null, "removeReminder");
                    return;
                }

                if (spread.MedicationTakeReminder == null)
                {
                    AlertPositiveButtonSelect(null, null, "addReminder");
                }
            }
            catch(System.Exception ex)
            {
                Log.Error(TAG, "MedicationAlarm_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, _context.GetString(Resource.String.ErrorMedListFragRemoveTime), "MedicationPagerAdapter.MedicationAlarm_Click");
            }
        }

        private void MedicationTimeRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedItemIndex == -1)
                {
                    Toast.MakeText(_context, Resource.String.MedListFragRemoveTimeToast, ToastLength.Short).Show();
                    return;
                }

                ConfirmTimeDeletion();
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "MedicationTimeRemove_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, _context.GetString(Resource.String.ErrorMedListFragRemoveTime), "MedicationPagerAdapter.MedicationTimeRemove_Click");
            }
        }

        private void ConfirmTimeDeletion()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper((Activity)_context);
                alertHelper.AlertTitle = _context.GetString(Resource.String.MedListFragConfirmDeleteDialogTitleTime);
                alertHelper.AlertMessage = _context.GetString(Resource.String.MedListFragConfirmDeleteDialogQuestionTime);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertPositiveCaption = _context.GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertNegativeCaption = _context.GetString(Resource.String.ButtonNoCaption);
                alertHelper.InstanceId = "removeTime";
                alertHelper.ShowAlert();
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "ConfirmTimeDeletion: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Removing Time", "MedicationPagerAdapter.ConfirmTimeDeletion");
            }
        }

        private void MedicationTimeAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validate())
                {
                    Log.Info(TAG, "MedicationTimeAdd_Click: Validation failed, name or dose missing");
                    Toast.MakeText((Activity)_context, Resource.String.MedListFragRemindAddToast, ToastLength.Long).Show();
                    return;
                }

                //we made it to here meaning validation checks have passed and
                //this is a new medication which needs to have a basic save
                //before attempting to add any list items for medication spread
                //this ensures we have a MedicationID to pass forward
                if (_selectedMedicationTimeID == -1)
                {
                    Log.Info(TAG, "MedicationTimeAdd_Click: _selectedMedicationTimeID - " + _selectedMedicationTimeID.ToString());
                    Log.Info(TAG, "MedicationTimeAdd_Click: Setting data to medication model...");
                    SetDataToMedicationModel();
                    Medication.IsNew = true;
                    Log.Info(TAG, "MedicationTimeAdd_Click: Saving Medication...");
                    SaveMedication();
                    _selectedMedicationTimeID = Medication.ID;
                    Log.Info(TAG, "MedicationTimeAdd_Click: Added medication, new ID - " + _selectedMedicationTimeID.ToString());
                }

                MedicationTimeDialogFragment timeAddFragment = new MedicationTimeDialogFragment((Activity)_context, this, "Add Medication Time");

                if (timeAddFragment != null)
                {
                    var fragmentTransaction = ((Activity)_context).FragmentManager.BeginTransaction();
                    if (fragmentTransaction != null)
                    {
                        timeAddFragment.Show(fragmentTransaction, timeAddFragment.Tag);
                    }
                    else
                    {
                        Log.Error(TAG, "MedicationTimeAdd_Click: Failed to create fragment Transaction!");
                    }
                }
                else
                {
                    Log.Error(TAG, "MedicationTimeAdd_Click: Failed to create Medication Time Dialog Fragment");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "MedicationTimeAdd_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, _context.GetString(Resource.String.ErrorMedListFragAddTime), "MedicationPagerAdapter.MedicationTimeAdd_Click");
            }
        }

        private void SetDataToMedicationModel()
        {
            try
            {
                if (Medication != null)
                {
                    if (_medicationNameText != null)
                    {
                        Medication.MedicationName = _medicationNameText.Text.Trim();
                        Log.Info(TAG, "SetDataToMedicationModel: Stored Medication Name - " + Medication.MedicationName);
                    }
                    else
                    {
                        Log.Error(TAG, "SetDataToMedicationModel: _medName is NULL!");
                    }
                    if (_medicationDosageText != null)
                    {
                        Medication.TotalDailyDosage = Convert.ToInt32(_medicationDosageText.Text.Trim());
                        Log.Info(TAG, "SetDataToMedicationModel: Stored Total daily dosage - " + Medication.TotalDailyDosage.ToString());
                    }
                    else
                    {
                        Log.Error(TAG, "SetDataToMedicationModel: _dailyDose is NULL!");
                    }
                    if (_medicationIntervalType != null)
                    {
                        Medication.PrescriptionType.PrescriptionType = (ConstantsAndTypes.PRESCRIPTION_TYPE)_medicationIntervalType.SelectedItemPosition;
                        Log.Info(TAG, "SetDataToMedicationModel: Stored Prescription type - " + StringHelper.PrescriptionStringForConstant(Medication.PrescriptionType.PrescriptionType));
                    }
                    else
                    {
                        Log.Error(TAG, "SetDataToMedicationModel: _prescriptionType is NULL!");
                    }
                    if (_prescriptionWeekDay != null)
                    {
                        Medication.PrescriptionType.WeeklyDay = (ConstantsAndTypes.DAYS_OF_THE_WEEK)_prescriptionWeekDay.SelectedItemPosition;
                        Log.Info(TAG, "SetDataToMedicationModel: Stored Prescription Weekly Day - " + StringHelper.DayStringForConstant(Medication.PrescriptionType.WeeklyDay));
                    }
                    else
                    {
                        Log.Error(TAG, "SetDataToMedicationModel: _prescriptionWeekDay is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "SetDataToMedicationModel: _medication is NULL!");
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "SetDataToMedicationModel: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, _context.GetString(Resource.String.ErrorMedListFragSetDataModel), "MedicationPagerAdapter.SetDataToMedicationModel");
            }
        }

        private void SaveMedication()
        {
            try
            {
                if (Medication != null)
                {
                    Log.Info(TAG, "SaveMedication: medication object exists!");
                    if (Validate())
                    {
                        Log.Info(TAG, "SaveMedication: medication passed validation! IsNew - " + (IsNew ? "TRUE" : "FALSE") + ", IsDirty - " + (IsDirty ? "TRUE" : "FALSE"));
                        Medication.IsNew = IsNew;
                        Medication.IsDirty = IsDirty;
                        Log.Info(TAG, "SaveMedication: calling Medication Save...");
                        Medication.Save();
                        Log.Info(TAG, "SaveMedication: Saved Medication with ID - " + Medication.ID.ToString());

                        if (IsNew)
                        {
                            GlobalData.MedicationItems.Add(Medication);
                            Log.Info(TAG, "SaveMedication: Added medication to Global cache, ID - " + Medication.ID.ToString());
                        }
                        IsNew = false;
                        IsDirty = false;
                        Log.Info(TAG, "SaveMedication: IsNew and IsDirty now FALSE");
                    }
                }
                else
                {
                    Log.Error(TAG, "SaveMedication: _medication is NULL!");
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "SaveMedication: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, _context.GetString(Resource.String.ErrorMedListFragSaveMedication), "MedicationPagerAdapter.SaveMedication");
            }
        }

        private bool Validate()
        {
            //we want at the very least the medication name and daily dose before we can save
            if (Medication != null)
            {
                if (Medication.MedicationName.Trim() == "")
                {
                    Log.Info(TAG, "Validate: Medication Name is empty!");
                    return false;
                }
            }
            else
            {
                Log.Error(TAG, "Validate: Medication object is NULL");
                return false;
            }
            if (Medication != null)
            {
                if (Medication.TotalDailyDosage == 0)
                {
                    Log.Info(TAG, "Validate: TotalDailyDosage is zero!");
                    return false;
                }
            }
            else
            {
                Log.Error(TAG, "Validate: Medication object is NULL!");
                return false;
            }

            return true;
        }

        private void MedicationIntervalType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                var selectedItem = (ConstantsAndTypes.PRESCRIPTION_TYPE)e.Position;

                if (!_isLoading)
                    _medication.IsDirty = true;

                switch (selectedItem)
                {
                    case ConstantsAndTypes.PRESCRIPTION_TYPE.Daily:
                        //ensure invisibility of un-needed items
                        if (_txtMedListPrescWeekDayLabel != null)
                        {
                            _txtMedListPrescWeekDayLabel.Visibility = ViewStates.Invisible;
                        }
                        if (_prescriptionWeekDay != null)
                        {
                            _prescriptionWeekDay.Visibility = ViewStates.Invisible;
                        }
                        if (_txtMedListPrescMonthDayLabel != null)
                        {
                            _txtMedListPrescMonthDayLabel.Visibility = ViewStates.Invisible;
                        }
                        if (_prescriptionMonthDay != null)
                        {
                            _prescriptionMonthDay.Visibility = ViewStates.Invisible;
                        }
                        if (_speakMonthDay != null)
                        {
                            _speakMonthDay.Visibility = ViewStates.Invisible;
                        }
                        break;
                    case ConstantsAndTypes.PRESCRIPTION_TYPE.Weekly:
                        if (_txtMedListPrescWeekDayLabel != null)
                        {
                            _txtMedListPrescWeekDayLabel.Visibility = ViewStates.Visible;
                        }
                        if (_prescriptionWeekDay != null)
                        {
                            _prescriptionWeekDay.Visibility = ViewStates.Visible;
                        }
                        if (_txtMedListPrescMonthDayLabel != null)
                        {
                            _txtMedListPrescMonthDayLabel.Visibility = ViewStates.Invisible;
                        }
                        if (_prescriptionMonthDay != null)
                        {
                            _prescriptionMonthDay.Visibility = ViewStates.Invisible;
                        }
                        if (_speakMonthDay != null)
                        {
                            _speakMonthDay.Visibility = ViewStates.Invisible;
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "MedicationIntervalType_ItemSelected: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, "Failed selecting interval type", "MedicationPagerAdapter.MedicationIntervalType_ItemSelected");
            }
        }

        private void MedicationDosageText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (Medication != null)
                {
                    int outInt;
                    if (int.TryParse(_medicationDosageText.Text.Trim(), out outInt))
                        Medication.TotalDailyDosage = outInt;
                    if (!_isLoading)
                        _medication.IsDirty = true;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "MedicationDosageText_TextChanged: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, "Failed whilst changing dosage text", "MedicationPagerAdapter.MedicationDosageText_TextChanged");
            }
        }

        private void MedicationNameText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (Medication != null)
                {
                    Medication.MedicationName = _medicationNameText.Text.Trim();
                    if (!_isLoading)
                        _medication.IsDirty = true;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "MedicationNameText_TextChanged: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, "Failed whilst changing medication name text", "MedicationPagerAdapter.MedicationNameText_TextChanged");
            }
        }

        private void SpeakMonthDay_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (_isSpeakingMonthDay) return;

                var button = (ImageButton)sender;

                if (button != null)
                {
                    int viewIndex = (int)button.Tag;
                    if (_context != null)
                    {
                        _isSpeakingMonthDay = true;
                        ((IMedicationSpeakCallback)_context).SpeakMonthDay();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "SpeakMonthDay_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, "Failed calling back to context", "MedicationPagerAdapter.SpeakMonthDay_Click");
            }
        }

        private void SpeakMedicationDosage_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (_isSpeakingDosage) return;

                var button = (ImageButton)sender;

                if (button != null)
                {
                    int viewIndex = (int)button.Tag;
                    if (_context != null)
                    {
                        _isSpeakingDosage = true;
                        ((IMedicationSpeakCallback)_context).SpeakMedicationDosage();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "SpeakMedicationDosage_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, "Failed calling back to context", "MedicationPagerAdapter.SpeakMedicationDosage_Click");
            }
        }

        private void SpeakMedicationName_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (_isSpeakingName) return;

                var button = (ImageButton)sender;

                if (button != null)
                {
                    int viewIndex = (int)button.Tag;
                    if (_context != null)
                    {
                        _isSpeakingName = true;
                        ((IMedicationSpeakCallback)_context).SpeakMedicationName();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "SpeakMedicationName_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, "Failed calling back to context", "MedicationPagerAdapter.SpeakMedicationName_Click");
            }
        }

        private void SetupSpinners()
        {
            try
            {
                SetupMedicationTypeSpinner();
                SetupMedicationWeekDaySpinner();
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "SetupSpinners: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed setting up spinners", "MedicationPagerAdapter.SetupSpinners");
            }
        }

        private void SetupMedicationTypeSpinner()
        {
            try
            {
                if (_medicationIntervalType != null)
                {
                    string[] medTypes = StringHelper.PrescriptionTypes();

                    ArrayAdapter adapter = new ArrayAdapter(_context, Resource.Layout.SpinnerGeneral, medTypes);

                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _medicationIntervalType.Adapter = adapter;
                        Log.Info(TAG, "SetupMedicationTypeSpinner: Set Prescription Type Adapter");
                        _medicationIntervalType.SetSelection((int)_medication.PrescriptionType.PrescriptionType);
                    }
                    else
                    {
                        Log.Error(TAG, "SetupMedicationTypeSpinner: Failed to create Adapter");
                    }
                }
                else
                {
                    Log.Error(TAG, "SetupMedicationTypeSpinner: _prescriptionType is NULL!");
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "SetupMedicationTypeSpinner: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, _context.GetString(Resource.String.ErrorMedListFragTypeSpin), "MedicationPagerAdapter.SetupMedicationTypeSpinner");
            }
        }

        private void SetupMedicationWeekDaySpinner()
        {
            try
            {
                if (_prescriptionWeekDay != null)
                {
                    string[] weekDays = StringHelper.DaysOfTheWeek();

                    ArrayAdapter adapter = new ArrayAdapter(_context, Resource.Layout.SpinnerGeneral, weekDays);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _prescriptionWeekDay.Adapter = adapter;
                        Log.Info(TAG, "SetupMedicationTypeSpinner: Set Prescription Week Day Adapter");
                        if(_medication.PrescriptionType.PrescriptionType == ConstantsAndTypes.PRESCRIPTION_TYPE.Weekly)
                        {
                            _prescriptionWeekDay.SetSelection((int)_medication.PrescriptionType.WeeklyDay);
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "SetupMedicationTypeSpinner: Failed to create Adapter");
                    }
                }
                else
                {
                    Log.Error(TAG, "SetupMedicationWeekDaySpinner: _prescriptionWeekDay = is NULL!");
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "SetupMedicationWeekDaySpinner: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, _context.GetString(Resource.String.ErrorMedListFragWeekDaySpin), "MedicationPagerAdapter.SetupMedicationWeekDaySpinner");
            }
        }

        private void OverDose()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper((Activity)_context);
                alertHelper.AlertTitle = _context.GetString(Resource.String.MedListFragOverDoseTitle);
                alertHelper.AlertMessage = _context.GetString(Resource.String.MedListFragOverDoseQuestion);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertPositiveCaption = _context.GetString(Resource.String.ButtonOKCaption);
                alertHelper.InstanceId = "overdose";
                alertHelper.ShowAlert();
            }
            catch (SystemException e)
            {
                Log.Error(TAG, "OverDose: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Showing dialog", "MedicationPagerAdapter.OverDose");
            }
        }

        public void MedicationTimeAdded(int dose, ConstantsAndTypes.MEDICATION_FOOD medFood, ConstantsAndTypes.MEDICATION_TIME medTime, DateTime taken)
        {
            try
            {
                //we cant add this spread if the dosage will take us over the total daily dosage
                var totalSpreadDosage = Medication.GetTotalSpreadDosage();
                if (totalSpreadDosage + dose > Medication.TotalDailyDosage)
                {
                    OverDose();
                    return;
                }

                MedicationSpread spread = new MedicationSpread();
                if (spread != null)
                {
                    spread.IsNew = true;
                    Log.Info(TAG, "MedicationTimeAdded: spread IsNew - TRUE");
                    Log.Info(TAG, "MedicationTimeAdded: Medication ID - " + Medication.ID.ToString());
                    spread.Dosage = dose;
                    spread.FoodRelevance = medFood;
                    Log.Info(TAG, "MedicationTimeAdded: Stored Dosage - " + spread.Dosage.ToString() + ", Food Relevance - " + StringHelper.MedicationFoodForConstant(medFood));
                    spread.Save(-1, Medication.ID);
                    MedicationTime timeTaken = new MedicationTime();
                    if (timeTaken != null)
                    {
                        Log.Info(TAG, "MedicationTimeAdded: Created Medication Time");
                        timeTaken.IsNew = true;
                        timeTaken.MedicationSpreadID = spread.ID;
                        timeTaken.MedicationTime = medTime;
                        timeTaken.TakenTime = taken;
                        Log.Info(TAG, "MedicationTimeAdded: Stored Spread ID - " + timeTaken.MedicationSpreadID.ToString() + ", Medication Time - " + StringHelper.MedicationTimeForConstant(medTime) + ", Taken Time - " + taken.ToShortTimeString());
                        timeTaken.Save(spread.ID);
                        spread.MedicationTakeTime = timeTaken;
                        Medication.MedicationSpread.Add(spread);
                        Log.Info(TAG, "MedicationTimeAdded: Add spread to Medication Spread list");
                        IsDirty = true;
                        Medication.IsDirty = true;
                        UpdateTimeAdapter();
                        Log.Info(TAG, "MedicationTimeAdded: Updated Time Adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "MedicationTimeAdded: Failed to create Medication Time");
                    }
                }
                else
                {
                    Log.Error(TAG, "MedicationTimeAdded: spread is NULL!");
                }
                DumpGlobal.Medication();
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "MedicationTimeAdded: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, _context.GetString(Resource.String.ErrorMedListFragAddTime), "MedicationPagerAdapter.MedicationTimeAdded");
            }
        }

        private void UpdateTimeAdapter()
        {
            try
            {
                var adapter = new MedicationTimeListAdapter(this, Medication.ID);
                Log.Info(TAG, "UpdateTimeAdapter: Parent - " + ((Activity)_context).ComponentName + ", Medication ID - " + Medication.ID.ToString());
                if (_medicationTimeList != null)
                {
                    _medicationTimeList.Adapter = adapter;
                    Log.Info(TAG, "UpdateReminderAdapter: Successfully updated Time List adapter");
                }
                else
                {
                    Log.Error(TAG, "UpdateTimeAdapter: _medicationTimeList is NULL!");
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "UpdateTimeAdapter: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, _context.GetString(Resource.String.ErrorMedListFragUpdateTimeList), "MedicationPagerAdapter.UpdateTimeAdapter");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            MedicationSpread spread = _medication.MedicationSpread[_selectedItemIndex];
            if (spread == null) return;
            try
            {
                if (instanceId == "removeTime")
                {
                    //start with getting the spread id of the selected item
                    var spreadID = spread.ID;

                    //is there a reminder set?
                    bool isSet = false;
                    if (spread.MedicationTakeReminder != null)
                    {
                        isSet = spread.MedicationTakeReminder.IsSet;
                        if (isSet)
                        {
                            CancelAlarm(spread.MedicationTakeReminder.ID);
                            spread.MedicationTakeReminder.Remove();
                            spread.MedicationTakeTime.Remove();
                        }
                    }

                    //finally, remove the spread
                    spread.Remove();
                    _medication.MedicationSpread.Remove(spread);

                    if (_medication.MedicationSpread.Count == 0)
                    {
                        _selectedMedicationTimeID = -1;
                        _selectedItemIndex = -1;
                    }
                    var globalMedication = GlobalData.MedicationItems.Find(med => med.ID == Medication.ID);

                    if (globalMedication != null)
                    {
                        var globalSpread = globalMedication.MedicationSpread.Find(spreadG => spreadG.ID == spreadID);
                        if (globalSpread != null)
                            globalMedication.MedicationSpread.Remove(globalSpread);
                        UpdateTimeAdapter();
                        IsDirty = true;
                    }
                }

                if (instanceId == "removeReminder")
                {
                    RemoveReminder();
                }

                if(instanceId == "addReminder")
                {
                    AddReminder(spread);
                }

                if (instanceId == "overdose")
                {
                    try
                    {
                        Toast.MakeText(_context, Resource.String.MedListFragOverdoseAbandonToast, ToastLength.Long).Show();
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                        if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, _context.GetString(Resource.String.ErrorMedListFragAddTime), "MedicationPagerAdapter.AlertPositiveButtonSelect");
                    }
                }

                if (instanceId == "useMic")
                {
                    PermissionsHelper.RequestApplicationPermission((Activity)_context, ConstantsAndTypes.AppPermission.UseMicrophone);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, _context.GetString(Resource.String.ErrorMedListFragRemoveTime), "MedicationPagerAdapter.AlertPositiveButtonSelect");
            }
        }

        private void RemoveReminder()
        {
            try
            {
                //grab the list set
                MedicationSpread medicationSpread;
                medicationSpread = _medication.MedicationSpread[SelectedItemIndex];

                if (medicationSpread != null)
                {
                    //now get the spread
                    var spreadID = medicationSpread.ID;
                    Log.Info(TAG, "RemoveReminder: Found reminder with ID " + spreadID.ToString());
                    var medication = GlobalData.MedicationItems.Find(med => med.ID == Medication.ID);
                    if (medication != null)
                    {
                        var rmd = medication.MedicationSpread.Find(spreadF => spreadF.ID == spreadID).MedicationTakeReminder;
                        if (rmd != null)
                        {
                            rmd.IsSet = false;
                            CancelAlarm(rmd.ID);
                            Log.Info(TAG, "RemoveReminder: Removed alarm with ID " + rmd.ID.ToString() + ", Dosage - " + medicationSpread.Dosage.ToString() + ", Medication name - " + Medication.MedicationName);
                        }
                        else
                        {
                            Log.Info(TAG, "RemoveReminder: Could not find reminder with Spread ID " + spreadID.ToString());
                        }
                    }
                    else
                    {
                        Log.Info(TAG, "RemoveReminder: Could not find Medication with ID " + Medication.ID.ToString() + ", Local medication ID is " + _medication.ID.ToString());
                    }
                    medicationSpread.MedicationTakeReminder.Remove();
                    medicationSpread.MedicationTakeReminder = null;
                    UpdateTimeAdapter();
                }
                IsDirty = true;
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "RemoveReminder: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, _context.GetString(Resource.String.ErrorMedListFragRemoveRemind), "MedicationPagerAdapter.RemoveReminder");
            }
        }

        private void AddReminder(MedicationSpread spread)
        {
            if (spread == null) return;

            if(_medication != null)
                MedicationReminderAdded(_medication.PrescriptionType.WeeklyDay, spread.MedicationTakeTime.TakenTime, spread.MedicationTakeTime.ID);
        }

        private void CancelAlarm(int reminderID)
        {
            try
            {
                new AlarmHelper((Activity)_context).CancelAlarm(reminderID);
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "CancelAlarm: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed to cancel alarm", "MedicationPagerAdapter.CancelAlarm");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            try
            {
                if (instanceId == "useMic")
                {
                    Toast.MakeText(_context, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    PermissionResultUpdate(Permission.Denied);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(TAG, "AlertNegativeButtonSelect: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, ex, "Failed selecting alert negative button", "MedicationPagerAdapter.AlertNegativeButtonSelect");
            }
        }

        public void MedicationReminderAdded(ConstantsAndTypes.DAYS_OF_THE_WEEK day, DateTime reminderTime, int medicationTimeID)
        {
            try
            {
                Log.Info(TAG, "MedicationReminderAdded: medicationTimeID - " + medicationTimeID.ToString());
                //there must be an existing Medication Time before having a reminder
                MedicationReminder reminder = new MedicationReminder();
                if (reminder != null)
                {
                    reminder.MedicationDay = day;
                    reminder.MedicationTime = reminderTime;
                    reminder.IsSet = true;
                    Log.Info(TAG, "MedicationReminderAdded: Stored Medication Day - " + StringHelper.DayStringForConstant(day) + ", Medication Time - " + reminderTime.ToShortTimeString());
                    var spreadHelp = _medication.MedicationSpread.Find(spread => spread.ID == medicationTimeID);
                    reminder.MedicationSpreadID = medicationTimeID;
                    reminder.Save(medicationTimeID);
                    spreadHelp.MedicationTakeReminder = reminder;
                    Log.Info(TAG, "MedicationReminderAdded: Stored Reminder in Spread");
                    _isDirty = true;
                    UpdateTimeAdapter();
                    Log.Info(TAG, "MedicationReminderAdded: Updated Reminder Adapter");
                    //now set the reminder
                    AlarmHelper alarm = new AlarmHelper((Activity)_context);

                    var ofText = "of ";
                    switch (GlobalData.CurrentIsoLanguageCode.ToLower())
                    {
                        case "eng":
                            ofText = "of ";
                            break;
                        case "spa":
                        case "fra":
                            ofText = "de ";
                            break;
                    }
                    switch (_medication.PrescriptionType.PrescriptionType)
                    {
                        case ConstantsAndTypes.PRESCRIPTION_TYPE.Daily:
                            alarm.SetAlarm(
                                (Activity)_context,
                                reminder.ID,
                                reminderTime,
                                spreadHelp.Dosage.ToString() + "mg " + ofText + _medication.MedicationName,
                                ConstantsAndTypes.ALARM_INTERVALS.Daily,
                                ConstantsAndTypes.DAYS_OF_THE_WEEK.Undefined,
                                true
                            );
                            break;
                        case ConstantsAndTypes.PRESCRIPTION_TYPE.Weekly:
                            alarm.SetAlarm(
                                (Activity)_context,
                                reminder.ID,
                                reminderTime,
                                spreadHelp.Dosage.ToString() + "mg " + ofText + _medication.MedicationName,
                                ConstantsAndTypes.ALARM_INTERVALS.Weekly,
                                reminder.MedicationDay,
                                true
                            );
                            break;
                        default:
                            alarm.SetAlarm(
                                (Activity)_context,
                                reminder.ID,
                                reminderTime,
                                spreadHelp.Dosage.ToString() + "mg " + ofText + _medication.MedicationName,
                                ConstantsAndTypes.ALARM_INTERVALS.EveryMinute,
                                ConstantsAndTypes.DAYS_OF_THE_WEEK.Undefined,
                                true
                            );
                            Log.Info(TAG, "MedicationReminderAdded: Set alarm with ID " + reminder.ID.ToString() + ", Dosage - " + spreadHelp.Dosage.ToString() + ", Medication name - " + _medication.MedicationName);
                            break;
                    }
                }
                else
                {
                    Log.Error(TAG, "MedicationReminderAdded: reminder is NULL!");
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "MedicationReminderAdded: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, ((Activity)_context).GetString(Resource.String.ErrorMedListFragAddRemind), "MedicationPagerAdapter.MedicationReminderAdded");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            try
            {
                if (permission == Permission.Denied)
                {
                    //find all the Mic image buttons and disable them
                    if (_speakMedicationName != null)
                        _speakMedicationName.SetImageResource(Resource.Drawable.micgreyscale);
                    if (_speakMedicationDosage != null)
                        _speakMedicationDosage.SetImageResource(Resource.Drawable.micgreyscale);
                    if (_speakMonthDay != null)
                        _speakMonthDay.SetImageResource(Resource.Drawable.micgreyscale);

                    if (_speakMedicationName != null)
                        _speakMedicationName.Enabled = false;
                    if (_speakMedicationDosage != null)
                        _speakMedicationDosage.Enabled = false;
                    if (_speakMonthDay != null)
                        _speakMonthDay.Enabled = false;
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "PermissionResultUpdate: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_context, e, "Failed during permission result update", "MedicationPagerAdapter.PermissionResultUpdate");
            }
        }
    }
}
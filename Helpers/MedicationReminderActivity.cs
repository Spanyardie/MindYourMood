using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model.Interfaces;


namespace com.spanyardie.MindYourMood.Helpers
{
    public class MedicationReminderDialogFragment : DialogFragment, ITimePickerCallback
    {
        public const string TAG = "M:MedicationReminderDialogFragment";

        private MedicationListActivity _fragment;
        private Activity _activity;
        private ConstantsAndTypes.DAYS_OF_THE_WEEK _dayOfWeek;

        private TextView _dayLabel;
        private Spinner _daySpinner;
        private TextView _timeText;
        private ImageButton _buttonSetTime;

        private Button _goBack;
        private Button _add;

        private int _medicationSpreadID = -1;

        private ConstantsAndTypes.PRESCRIPTION_TYPE _prescriptionType;
        private DateTime _reminderTime;
        private bool _firstTimeView = true;

        public const int REMINDER_REQUEST_CODE = 20002;

        private string _dialogTitle = "";

        public MedicationReminderDialogFragment()
        {

        }

        public MedicationReminderDialogFragment( Activity activity, MedicationListActivity fragment,  
            ConstantsAndTypes.PRESCRIPTION_TYPE prescriptionType,
            ConstantsAndTypes.DAYS_OF_THE_WEEK dayOfWeek,
            int medicationSpreadID,
            DateTime reminderTime, string title)
        {
            _activity = activity;
            _fragment = fragment;
            _prescriptionType = prescriptionType;
            _dayOfWeek = dayOfWeek;
            _medicationSpreadID = medicationSpreadID;
            _reminderTime = reminderTime;
            Log.Info(TAG, "Constructor, dayOfWeek - " + StringHelper.DayStringForConstant(dayOfWeek) + ", medicationSpreadID - " + medicationSpreadID.ToString());
            _dialogTitle = title;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("prescriptionType", (int)_prescriptionType);
                outState.PutInt("dayOfWeek", (int)_dayOfWeek);
                outState.PutInt("medicationSpreadID", _medicationSpreadID);
                outState.PutString("reminderTime", _reminderTime.ToString());
                outState.PutString("dialogTitle", _dialogTitle);
            }

            base.OnSaveInstanceState(outState);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            if (context != null)
                _activity = (Activity)context;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                View view = inflater.Inflate(Resource.Layout.MedicationReminderDialogFragmentLayout, container, false);

                if (view != null)
                {
                    GetFieldComponents(view);
                    Log.Info(TAG, "OnCreateView: Got field components");

                    SetupCallbacks();
                    Log.Info(TAG, "OnCreateView: Setup callbacks");

                    if(savedInstanceState != null)
                    {
                        _prescriptionType = (ConstantsAndTypes.PRESCRIPTION_TYPE)savedInstanceState.GetInt("prescriptionType", -1);
                        _dayOfWeek = (ConstantsAndTypes.DAYS_OF_THE_WEEK)savedInstanceState.GetInt("dayOfWeek", (int)ConstantsAndTypes.DAYS_OF_THE_WEEK.Monday);
                        _medicationSpreadID = savedInstanceState.GetInt("medicationSpreadID", -1);
                        _reminderTime = Convert.ToDateTime(savedInstanceState.GetString("reminderTime"));
                        _dialogTitle = savedInstanceState.GetString("dialogTitle");
                    }

                    _timeText.Text = _reminderTime.ToShortTimeString();

                    SetupSpinner();
                    Log.Info(TAG, "OnCreateView: Set up spinner");

                    if (_firstTimeView)
                    {
                        switch(_prescriptionType)
                        {
                            case ConstantsAndTypes.PRESCRIPTION_TYPE.Weekly:
                                if (_dayLabel != null)
                                    _dayLabel.Visibility = ViewStates.Visible;
                                if (_daySpinner != null)
                                {
                                    _daySpinner.Visibility = ViewStates.Visible;
                                    _daySpinner.SetSelection((int)_dayOfWeek);
                                }
                                if (_timeText != null)
                                {
                                    _timeText.Text = _reminderTime.ToShortTimeString();
                                }
                                break;
                            default:
                                if (_dayLabel != null)
                                    _dayLabel.Visibility = ViewStates.Invisible;
                                if (_daySpinner != null)
                                    _daySpinner.Visibility = ViewStates.Invisible;
                                if (_timeText != null)
                                {
                                    _timeText.Text = _reminderTime.ToShortTimeString();
                                }
                                break;
                        }
                        _firstTimeView = false;
                    }

                }
                else
                {
                    Log.Error(TAG, "OnCreateView: View is NULL!");
                }
                return view;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMedListFragCreateView), "MedicationreminderDialogFragment.OnCreateView");
                return null;
            }
        }

        private void SetupSpinner()
        {
            try
            {
                if (_daySpinner != null)
                {
                    string[] days = StringHelper.DaysOfTheWeek();
                    ArrayAdapter adapter = new ArrayAdapter(Activity, Resource.Layout.SpinnerGeneral, days);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _daySpinner.Adapter = adapter;
                        Log.Info(TAG, "SetupSpinner: Set Day spinner Adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupSpinner: Failed to create Adapter");
                    }
                }
                else
                {
                    Log.Error(TAG, "SetupSpinner: _daySpinner is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupSpinner: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMedListFragWeekDaySpin), "MedicationReminderDialogFragment.SetupSpinner");
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if(view != null)
                {
                    _dayLabel = view.FindViewById<TextView>(Resource.Id.txtMedListRemindSetDayLabel);
                    _daySpinner = view.FindViewById<Spinner>(Resource.Id.spnMedListRemindSetDay);
                    _timeText = view.FindViewById<TextView>(Resource.Id.txtMedListRemindSetTimeText);
                    _buttonSetTime = view.FindViewById<ImageButton>(Resource.Id.imgbtnMedListRemindSetTime);
                    _goBack = view.FindViewById<Button>(Resource.Id.btnMedListRemindGoBack);
                    _add = view.FindViewById<Button>(Resource.Id.btnMedListRemindAdd);
                    Log.Info(TAG, "GetFieldComponents: Succeeded retrieving components");
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: View is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMedListAdapterGetComponents), "MedicationReminderDialogFragment.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_buttonSetTime != null)
                {
                    _buttonSetTime.Click += ButtonSetTime_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _buttonSetTime is NULL!");
                }
                if(_goBack != null)
                {
                    _goBack.Click += GoBack_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _goBack is NULL!");
                }
                if(_add != null)
                {
                    _add.Click += Add_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _add is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMedActivitySetupCallbacks), "MedicationReminderDialogFragment.SetupCallbacks");
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                if(_fragment != null)
                {
                    ((IMedicationReminder)_fragment).MedicationReminderAdded(_dayOfWeek, Convert.ToDateTime(_timeText.Text.Trim()), _medicationSpreadID);
                    Log.Info(TAG, "Add_Click: Called back to MedicationReminderAdded");
                    Dismiss();
                }
                else
                {
                    Log.Error(TAG, "Add_Click: _fragment is NULL!");
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorMedListFragAddRemind), "MedicationReminderDialogFragment.Add_Click");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            Log.Info(TAG, "GoBack_Click: Going back from reminder Fragment");
            Dismiss();
        }

        private void ButtonSetTime_Click(object sender, EventArgs e)
        {
            try
            {
                TimePickerDialogFragment timeFragment = new TimePickerDialogFragment(Activity, this, DateTime.Now, ConstantsAndTypes.TIMEPICKER_CONTEXT.MedicationReminder, "Select Reminder Time");
                var transaction = FragmentManager.BeginTransaction();
                timeFragment.Show(transaction, timeFragment.Tag);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "ButtonSetTime_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorMedListFragSelTime), "MedicationReminderDialogFragment.ButtonSetTime_Click");
            }
        }

        public void TimePicked(DateTime timePicked, ConstantsAndTypes.TIMEPICKER_CONTEXT timeContext)
        {
            if (_timeText != null)
            {
                _timeText.Text = timePicked.ToShortTimeString();
                Log.Info(TAG, "TimePicked: Received time picked - " + timePicked.ToShortTimeString());
            }
            else
            {
                Log.Error(TAG, "TimePicked: _timeText is NULL!");
            }
        }
    }
}
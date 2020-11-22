using System;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Content;
using com.spanyardie.MindYourMood.Adapters;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class MedicationTimeDialogFragment : DialogFragment, ITimePickerCallback
    {
        public const string TAG = "M:MedicationTimeDialogFragment";

        private EditText _spreadDose;
        private Spinner _food;
        private Spinner _time;
        private TextView _takenText;
        private ImageButton _buttonTaken;

        private Button _goBack;
        private Button _add;

        private Activity _activity;
        private MedicationPagerAdapter _fragment;

        public const int TIME_REQUEST_CODE = 10001;

        private string _dialogTitle = "";

        public MedicationTimeDialogFragment()
        {

        }

        public MedicationTimeDialogFragment(Activity activity, MedicationPagerAdapter fragment, string title)
        {
            _activity = activity;
            _fragment = fragment;
            _dialogTitle = title;
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

                View view = inflater.Inflate(Resource.Layout.MedicationTimeDialogFragmentLayout, container, false);

                if (view != null)
                {
                    GetFieldComponents(view);
                    Log.Info(TAG, "OnCreateView: Got Field Components");

                    SetupCallbacks();
                    Log.Info(TAG, "OnCreateView:Set up Callbacks");

                    SetupSpinners();
                    Log.Info(TAG, "OnCreateView: Set up spinners");

                    return view;
                }
                else
                {
                    Log.Error(TAG, "OnCreateView: View is NULL!");
                    return null;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMedListFragCreateView), "MedicationTimeDialogFragment.OnCreateView");
                return null;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _spreadDose = view.FindViewById<EditText>(Resource.Id.edtMedListTimeDose);
                _food = view.FindViewById<Spinner>(Resource.Id.spnMedListTimeFood);
                _time = view.FindViewById<Spinner>(Resource.Id.spnMedListTimeTime);
                _takenText = view.FindViewById<TextView>(Resource.Id.txtMedListTimeTakenText);
                _buttonTaken = view.FindViewById<ImageButton>(Resource.Id.imgbtnMedListTimeTaken);
                _goBack = view.FindViewById<Button>(Resource.Id.btnTimeGoBack);
                _add = view.FindViewById<Button>(Resource.Id.btnTimeAdd);
                Log.Info(TAG, "GetFieldComponents: Succeeded getting Field Components");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMedListAdapterGetComponents), "MedicationTimeDialogFragment.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_buttonTaken != null)
                {
                    _buttonTaken.Click += ButtonTaken_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _buttonTaken is NULL!");
                }
                if(_goBack != null)
                {
                    _goBack.Click += GoBack_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _goBack is NULL");
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMedActivitySetupCallbacks), "MedicationTimeDialogFragment.SetupCallbacks");
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_spreadDose != null)
                {
                    if (string.IsNullOrEmpty(_spreadDose.Text))
                    {
                        _spreadDose.Error = Activity.GetString(Resource.String.MedTimeDialogFragEnterDose);
                        return;
                    }
                }

                if (_takenText != null)
                {
                    if(string.IsNullOrEmpty(_takenText.Text))
                    {
                        _takenText.Error = Activity.GetString(Resource.String.MedTimeDialogFragSelectTime);
                        return;
                    }
                }

                if (Activity != null)
                {
                    if (_spreadDose != null && _food != null && _time != null && _takenText != null)
                    {
                        if (_fragment != null)
                        {
                            ((IMedicationTime)_fragment).MedicationTimeAdded(Convert.ToInt32(_spreadDose.Text.Trim()), (ConstantsAndTypes.MEDICATION_FOOD)_food.SelectedItemPosition, (ConstantsAndTypes.MEDICATION_TIME)_time.SelectedItemPosition, Convert.ToDateTime(_takenText.Text.Trim()));
                            Log.Info(TAG, "Add_Click: Sent data to parent activity via interface");
                        }
                        else
                        {
                            Log.Error(TAG, "Add_Click: _fragment is NULL!");
                        }
                        Dismiss();
                    }
                    else
                    {
                        Log.Error(TAG, "Add_Click: A UI error occurred - one of the components is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "Add_Click: Activity is NULL!");
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorMedListFragAddTime), "MedicationTimeDialogFragment.Add_Click");
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            Log.Info(TAG, "GoBack_Click: User cancelled Time entry");
            Dismiss();
        }

        private void ButtonTaken_Click(object sender, EventArgs e)
        {
            try
            {
                TimePickerDialogFragment timeFragment = new TimePickerDialogFragment(Activity, this, DateTime.Now, ConstantsAndTypes.TIMEPICKER_CONTEXT.MedicationTime, "Select Time Taken");
                var transaction = FragmentManager.BeginTransaction();
                timeFragment.Show(transaction, timeFragment.Tag);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ButtonTaken_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorMedListFragTakenTime), "MedicationTimeDialogFragment.ButtonTaken_Click");
            }
        }

        private void SetupSpinners()
        {
            SetupFoodSpinner();
            SetupTimeSpinner();
        }

        private void SetupFoodSpinner()
        {
            try
            {
                if (_food != null)
                {
                    string[] foodTimes = StringHelper.MedicationFoodTimes();
                    ArrayAdapter adapter = new ArrayAdapter(Activity, Resource.Layout.SpinnerGeneral, foodTimes);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _food.Adapter = adapter;
                        Log.Info(TAG, "SetupFoodSpinner: Set Medication Food Adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupFoodSpinner: Failed to create Adapter");
                    }
                }
                else
                {
                    Log.Error(TAG, "SetupFoodSpinner: _food is NULL!");
                }

            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupFoodSpinner: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMedListFragFoodSpinner), "MedicationTimeDialogFragment.SetupFoodSpinner");
            }
        }

        private void SetupTimeSpinner()
        {
            try
            {
                if(_time != null)
                {
                    string[] times = StringHelper.MedicationTimes();
                    ArrayAdapter adapter = new ArrayAdapter(Activity, Resource.Layout.SpinnerGeneral, times);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _time.Adapter = adapter;
                        Log.Info(TAG, "SetupTimeSpinner: Set Medication Time Adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupTimeSpinner: Failed to create Adapter");
                    }
                }
                else
                {
                    Log.Error(TAG, "SetupTimeSpinner: _time is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupTimeSpinner: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorMedListFragTimeSpinner), "MedicationTimeDialogFragment.SetupTimeSpinner");
            }
        }

        public void TimePicked(DateTime timePicked, ConstantsAndTypes.TIMEPICKER_CONTEXT timeContext)
        {
            if (_takenText != null)
            {
                _takenText.Text = timePicked.ToShortTimeString();
                Log.Info(TAG, "TimePicked: Received time picked - " + timePicked.ToShortTimeString());
            }
            else
            {
                Log.Error(TAG, "TimePicked: _takenText is NULL!");
            }
        }
    }
}
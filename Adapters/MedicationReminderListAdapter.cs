using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using Android.Graphics;
using Android.Content;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class MedicationReminderListAdapter : BaseAdapter
    {
        public const string TAG = "M:MedicationReminderListAdapter";

        private TextView _reminderSetDayLabel;
        private TextView _day;
        private TextView _reminderSetTimeLabel;
        private TextView _time;
        private LinearLayout _linReminder;
        private LinearLayout _linReminderSet;

        private MedicationListActivity _activity;

        private List<MedicationSpread> _reminderItems;
        private int _medicationID;

        public MedicationReminderListAdapter(MedicationListActivity activity, int medicationID)
        {
            _activity = activity;

            _medicationID = medicationID;

            Log.Info(TAG, "Constructor: Medication ID - " + _medicationID.ToString());
            _reminderItems = new List<MedicationSpread>();

            GetReminderData();
        }

        private void GetReminderData()
        {
            try
            {
                var medication = GlobalData.MedicationItems.Find(med => med.ID == _medicationID);

                if (medication != null)
                {
                    if (_reminderItems == null)
                        _reminderItems = new List<MedicationSpread>();

                    _reminderItems.Clear();

                    foreach (var spread in medication.MedicationSpread)
                    {
                        if (spread.MedicationTakeReminder != null)
                        {
                            if (spread.MedicationTakeReminder.IsSet)
                            {
                                _reminderItems.Add(spread);
                                Log.Info(TAG, "GetReminderData: Found Reminder with ID - " + spread.MedicationTakeReminder.ID.ToString() + ", Reminder Time - " + spread.MedicationTakeReminder.MedicationTime.ToShortTimeString());
                            }
                        }
                    }

                    Log.Info(TAG, "GetReminderData: Medication with ID " + _medicationID.ToString() + " has " + _reminderItems.Count.ToString() + " items");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetReminderData: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Failed to retrieve Reminder data", "MedicationReminderListAdapter.GetReminderData");
            }
        }

        public override int Count
        {
            get
            {
                if (_reminderItems != null)
                {
                    return _reminderItems.Count;
                }
                return 0;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_reminderItems != null && _reminderItems.Count > 0)
            {
                return _reminderItems[position].ID;
            }
            return -1;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.MedicationReminderListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.MedicationReminderListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                if (convertView != null)
                {
                    if (_reminderItems[position].MedicationTakeReminder != null)
                    {
                        if (!_reminderItems[position].MedicationTakeReminder.IsSet) return null;

                        GetFieldComponents(convertView);

                        Log.Info(TAG, "GetView: Position - " + position.ToString());
                        if (_day != null)
                        {
                            var medication = GlobalData.MedicationItems.Find(med => med.ID == _medicationID);
                            if (medication != null)
                            {
                                if (medication.PrescriptionType.PrescriptionType == ConstantsAndTypes.PRESCRIPTION_TYPE.Daily)
                                {
                                    _day.Text = _activity.GetString(Resource.String.MedListReminderAdapterDayTextDaily);
                                    Log.Info(TAG, @"GetView: Set 'Daily' text");
                                }
                                else
                                {
                                    _day.Text = StringHelper.DayStringForConstant(_reminderItems[position].MedicationTakeReminder.MedicationDay);
                                    Log.Info(TAG, "GetView: Set day text - " + _day.Text);
                                }
                            }
                            else
                            {
                                Log.Error(TAG, "GetView: medication is NULL!");
                            }
                        }
                        else
                        {
                            Log.Error(TAG, "GetView: _day is NULL!");
                        }

                        if (_time != null)
                        {
                            _time.Text = _reminderItems[position].MedicationTakeReminder.MedicationTime.ToShortTimeString();
                            Log.Info(TAG, "GetView: Time - " + _time.Text);
                        }
                        else
                        {
                            Log.Error(TAG, "GetView: _time is NULL!");
                        }
                    }

                    if(position == _activity.GetSelectedReminderItemIndex())
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_reminderSetDayLabel != null)
                            _reminderSetDayLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_day != null)
                            _day.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_reminderSetTimeLabel != null)
                            _reminderSetTimeLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_time != null)
                            _time.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_linReminder != null)
                            _linReminder.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_linReminderSet != null)
                            _linReminderSet.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                }
                else
                {
                    Log.Error(TAG, "GetView: View is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMedListAdapterGetView), "MedicationReminderListAdapter.GetView");
            }
            return convertView;
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _reminderSetDayLabel = view.FindViewById<TextView>(Resource.Id.txtMedListRemindSetDayLabel);
                    _day = view.FindViewById<TextView>(Resource.Id.txtMedListRemindSetDayText);
                    _reminderSetTimeLabel = view.FindViewById<TextView>(Resource.Id.txtMedListRemindSetTimeLabel);
                    _time = view.FindViewById<TextView>(Resource.Id.txtMedListRemindSetTimeText);
                    _linReminder = view.FindViewById<LinearLayout>(Resource.Id.linMedListReminder);
                    _linReminderSet = view.FindViewById<LinearLayout>(Resource.Id.linMedListRemindSet);
                    Log.Info(TAG, "GetFieldComponents: Successfully retrieved field components");
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: View is NULL");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMedListAdapterGetComponents), "MedicationReminderListAdapter.GetFieldComponents");
            }
        }
    }
}
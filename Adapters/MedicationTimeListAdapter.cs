using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Android.Content;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class MedicationTimeListAdapter : BaseAdapter
    {
        public const string TAG = "M:MedicationTimeListAdapter";

        private MedicationPagerAdapter _parent;

        private int _medicationID;

        private TextView _timeDose;
        private TextView _timeOfDay;
        private TextView _timeTakenText;
        private TextView _itemAt1;
        private TextView _itemAt2;
        private LinearLayout _medicationSpread;
        private LinearLayout _doseAndFood;
        private ImageView _alarmNotify;

        private List<MedicationSpread> _medicationTimeItems;

        public MedicationTimeListAdapter(MedicationPagerAdapter activity, int medicationID)
        {
            _parent = activity;
            _medicationID = medicationID;
            Log.Info(TAG, "Constructor: Medication ID - " + _medicationID.ToString());
            GetMedicationTimeData();
        }

        private void GetMedicationTimeData()
        {
            var medication = GlobalData.MedicationItems.Find(med => med.ID == _medicationID);

            if (medication != null)
            {
                _medicationTimeItems = medication.MedicationSpread;
                Log.Info(TAG, "GetMedicationTimeData: _medicationTimeItems has " + _medicationTimeItems.Count.ToString() + " items");
            }
            else
            {
                Log.Info(TAG, "GetMedicationTimeData: medication is NULL!");
            }
        }

        public override int Count
        {
            get
            {
                if (_medicationTimeItems != null)
                {
                    return _medicationTimeItems.Count;
                }
                return -1;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_medicationTimeItems != null && _medicationTimeItems.Count > 0)
            {
                return _medicationTimeItems[position].ID;
            }
            return -1;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                if (_parent != null)
                {
                    convertView =((Activity) _parent.GetContext()).LayoutInflater.Inflate(Resource.Layout.MedicationTimeListItem, parent, false);
                }
                else if (parent != null)
                {
                    LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                    convertView = inflater.Inflate(Resource.Layout.MedicationTimeListItem, parent, false);
                }
                else
                {
                    return convertView;
                }
            }

            if (convertView != null)
            {
                try
                {
                    GetFieldComponents(convertView);

                    if (_medicationTimeItems[position].MedicationTakeTime != null)
                    {
                        if (_timeDose != null)
                        {
                            _timeDose.Text = _medicationTimeItems[position].Dosage.ToString() + "mg";
                            Log.Info(TAG, "GetView: Time dose - " + _timeDose.Text);
                        }
                        else
                        {
                            Log.Error(TAG, "GetView: _timeDose is NULL!");
                        }
                        if (_timeOfDay != null)
                        {
                            _timeOfDay.Text = StringHelper.MedicationTimeForConstant(_medicationTimeItems[position].MedicationTakeTime.MedicationTime);
                            Log.Info(TAG, "GetView: Time of day - " + _timeOfDay.Text);
                        }
                        else
                        {
                            Log.Error(TAG, "GetView: _timeOfDay is NULL!");
                        }
                        if (_timeTakenText != null)
                        {
                            _timeTakenText.Text = _medicationTimeItems[position].MedicationTakeTime.TakenTime.ToShortTimeString();
                            Log.Info(TAG, "GetView: Time Taken - " + _timeTakenText.Text);
                        }
                        else
                        {
                            Log.Error(TAG, "GetView: _timeTakenText is NULL!");
                        }
                        if(_alarmNotify != null)
                        {
                            if(_medicationTimeItems[position].MedicationTakeReminder != null)
                            {
                                _alarmNotify.Visibility = ViewStates.Visible;
                            }
                            else
                            {
                                _alarmNotify.Visibility = ViewStates.Invisible;
                            }
                        }
                        if (position == _parent.GetSelectedTimeItemIndex())
                        {
                            convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                            _timeDose.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                            _timeOfDay.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                            _timeTakenText.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                            _itemAt1.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                            _itemAt2.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                            _medicationSpread.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                            _doseAndFood.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                            _alarmNotify.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        }
                        else
                        {
                            convertView.SetBackgroundDrawable(null);
                            _timeDose.SetBackgroundDrawable(null);
                            _timeOfDay.SetBackgroundDrawable(null);
                            _timeTakenText.SetBackgroundDrawable(null);
                            _itemAt1.SetBackgroundDrawable(null);
                            _itemAt2.SetBackgroundDrawable(null);
                            _medicationSpread.SetBackgroundDrawable(null);
                            _doseAndFood.SetBackgroundDrawable(null);
                            _alarmNotify.SetBackgroundDrawable(null);
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Error(TAG, "GetView: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.GetContext(), e, ((Activity)_parent.GetContext()).GetString(Resource.String.ErrorMedListAdapterGetView), "MedicationTimeListAdapter.GetView");
                }
            }
            else
            {
                Log.Error(TAG, "GetView: View is NULL!");
            }
            return convertView;
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _timeDose = view.FindViewById<TextView>(Resource.Id.txtMedListTimeDoseLabel);
                    _timeOfDay = view.FindViewById<TextView>(Resource.Id.txtMedListTimeListItemTime);
                    _timeTakenText = view.FindViewById<TextView>(Resource.Id.txtMedListTimeTakenText);
                    _itemAt1 = view.FindViewById<TextView>(Resource.Id.txtMedListTimeListItemAt);
                    _itemAt2 = view.FindViewById<TextView>(Resource.Id.txtMedListTimeListItemAt2);
                    _medicationSpread = view.FindViewById<LinearLayout>(Resource.Id.linMedListMedicationSpread);
                    _doseAndFood = view.FindViewById<LinearLayout>(Resource.Id.linMedListTimeDoseAndFood);
                    _alarmNotify = view.FindViewById<ImageView>(Resource.Id.imgAlarmNotify);
                    Log.Info(TAG, "GetFieldComponents: Successfully retrieved field components");
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: View is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.GetContext(), e, ((Activity)_parent.GetContext()).GetString(Resource.String.ErrorMedListAdapterGetComponents), "MedicationSpreadAdapter.GetFieldComponents");
            }
        }
    }
}
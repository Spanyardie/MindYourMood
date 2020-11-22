using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using Android.Graphics;
using Android.Content;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class MedicationListAdapter : BaseAdapter
    {
        public const string TAG = "M:MedicationListAdapter";

        private Activity _parent = null;

        private List<Medication> _medicationList;

        private TextView _medName;
        private TextView _dose;
        private TextView _range;
        private TextView _mgText;
        private ImageView _medIcon;

        public MedicationListAdapter(Activity activity)
        {
            _parent = activity;
            GetMedicationData();
        }

        private void GetMedicationData()
        {
            _medicationList = GlobalData.MedicationItems;
            if(_medicationList != null)
            {
                Log.Info(TAG, "GetMedicationData: Retrieved cached global data - there are " + _medicationList.Count.ToString() + " items.");
            }
            else
            {
                Log.Info(TAG, "GetMedicationData: _medicationList is NULL!");
            }
        }

        public override int Count
        {
            get
            {
                if (_medicationList != null)
                {
                    return _medicationList.Count;
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
            if (_medicationList != null && _medicationList.Count > 0)
                return _medicationList[position].ID;

            return -1;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (_parent != null)
                {
                    Log.Info(TAG, "GetView: Attempting to retrieve View...");
                    if (convertView == null)
                    {
                        if (_parent != null)
                        {
                            convertView = _parent.LayoutInflater.Inflate(Resource.Layout.MedicationListItem, parent, false);
                        }
                        else if (parent != null)
                        {
                            LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                            convertView = inflater.Inflate(Resource.Layout.MedicationListItem, parent, false);
                        }
                        else
                        {
                            return convertView;
                        }
                    }
                    if (convertView != null)
                    {
                        GetFieldComponents(convertView);
                        if (_medicationList != null && _medicationList.Count > 0)
                        {
                            if(((MedicationActivity)_parent).GetSelectedItemIndex() != -1)
                            {
                                if(position == ((MedicationActivity)_parent).GetSelectedItemIndex())
                                {
                                    convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                                    if (_medName != null)
                                        _medName.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                                    if (_dose != null)
                                        _dose.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                                    if (_range != null)
                                        _range.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                                    if(_mgText != null)
                                        _mgText.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                                    if(_medIcon != null)
                                        _medIcon.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                                }
                                else
                                {
                                    convertView.SetBackgroundDrawable(null);
                                    if (_medName != null)
                                        _medName.SetBackgroundDrawable(null);
                                    if (_dose != null)
                                        _dose.SetBackgroundDrawable(null);
                                    if (_range != null)
                                        _range.SetBackgroundDrawable(null);
                                    if (_mgText != null)
                                        _mgText.SetBackgroundDrawable(null);
                                    if (_medIcon != null)
                                        _medIcon.SetBackgroundDrawable(null);
                                }
                            }

                            if (_medName != null)
                            {
                                _medName.Text = _medicationList[position].MedicationName;
                                Log.Info(TAG, "GetView: Medication name - " + _medName.Text);
                            }
                            else
                            {
                                Log.Error(TAG, "GetView: _medName is NULL!");
                            }
                            if (_dose != null)
                            {
                                _dose.Text = _medicationList[position].TotalDailyDosage.ToString();
                                Log.Info(TAG, "GetView: Dose - " + _dose.Text);
                            }
                            else
                            {
                                Log.Error(TAG, "GetView: _dose is NULL!");
                            }
                            if (_range != null)
                            {
                                if (_medicationList[position].PrescriptionType != null)
                                {
                                    _range.Text = StringHelper.PrescriptionStringForConstant(_medicationList[position].PrescriptionType.PrescriptionType);
                                    Log.Info(TAG, "GetView: Range - " + _range.Text);
                                }
                                else
                                {
                                    Log.Info(TAG, "GetView: PrescriptionType is NULL!");
                                }
                            }
                            else
                            {
                                Log.Error(TAG, "GetView: _range is NULL!");
                            }
                        }
                        else
                        {
                            Log.Error(TAG, "GetView: _medicationList is NULL!");
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: View is NULL!");
                    }
                    return convertView;
                }
                else
                {
                    Log.Error(TAG, "GetView: _parent is NULL!");
                    return null;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_parent, e, _parent.GetString(Resource.String.ErrorMedListAdapterGetView), "MedicationListAdapter.GetView");
                return null;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if(view != null)
                {
                    _medIcon = view.FindViewById<ImageView>(Resource.Id.imgMedListMedicine);
                    _medName = view.FindViewById<TextView>(Resource.Id.txtMedListMedName);
                    _dose = view.FindViewById<TextView>(Resource.Id.txtMedListDose);
                    _range = view.FindViewById<TextView>(Resource.Id.txtMedListRange);
                    _mgText = view.FindViewById<TextView>(Resource.Id.txtMedListMgText);
                    Log.Info(TAG, "GetFieldComponents: Successfully retrieved field components");
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: View is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_parent, e, _parent.GetString(Resource.String.ErrorMedListAdapterGetComponents), "MedicationListAdapter.GetFieldComponents");
            }
        }
    }
}
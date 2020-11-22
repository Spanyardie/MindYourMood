using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.SubActivities.Resources;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.Interfaces;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class AppointmentPopupAdapter : BaseAdapter
    {
        public const string TAG = "M:AppointmentPopupAdapter";

        private List<Appointments> _appointments = null;
        private Activity _activity = null;

        private const int STATUS_NEW = 1;
        private const int STATUS_EDIT = 0;

        private TextView _appointmentWithWhom = null;
        private TextView _appointmentLocation = null;
        private TextView _appointmentTime = null;
        private ImageButton _appointmentEdit = null;
        private ImageButton _appointmentRemove = null;
        private PopupWindow _appointmentPopup = null;
        private AppointmentPopupHelper _helper = null;

        public AppointmentPopupAdapter(Activity activity, List<Appointments> appointments, AppointmentPopupHelper helper, PopupWindow appointmentPopup = null)
        {
            Log.Info(TAG, "Constructor: Received appointment list with " + appointments.Count.ToString() + " appointments");
            _activity = activity;
            _appointments = appointments;
            _appointmentPopup = appointmentPopup;
            _helper = helper;
        }

        public override int Count
        {
            get
            {
                return _appointments.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _appointments[position].AppointmentID;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.AppointmentPopupListItem, parent, false);
                    }
                }

                GetFieldComponents(convertView, position);

                SetupCallbacks();

                if (_appointmentWithWhom != null)
                    _appointmentWithWhom.Text = _appointments[position].WithWhom;
                if (_appointmentTime != null)
                    _appointmentTime.Text = _appointments[position].AppointmentTime.ToShortTimeString();
                if (_appointmentLocation != null)
                    _appointmentLocation.Text = _appointments[position].Location;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Attempting to get view", "AppointmentPopupAdapter.GetView");
            }
            return convertView;
        }

        private void GetFieldComponents(View convertView, int position)
        {
            try
            {
                if (convertView != null)
                {
                    _appointmentWithWhom = convertView.FindViewById<TextView>(Resource.Id.txtAppointmentWithWhom);
                    _appointmentTime = convertView.FindViewById<TextView>(Resource.Id.txtAppointmentTime);
                    _appointmentLocation = convertView.FindViewById<TextView>(Resource.Id.txtAppointmentLocation);
                    _appointmentEdit = convertView.FindViewById<ImageButton>(Resource.Id.imgbtnAppointmentEdit);
                    if (_appointmentEdit != null)
                        _appointmentEdit.Tag = position;
                    _appointmentRemove = convertView.FindViewById<ImageButton>(Resource.Id.imgbtnAppointmentRemove);
                    if (_appointmentRemove != null)
                        _appointmentRemove.Tag = position;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting field components", "AppointmentPopupAdapter.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            if(_appointmentEdit != null)
                _appointmentEdit.Click += AppointmentEdit_Click;
            if(_appointmentRemove != null)
                _appointmentRemove.Click += AppointmentRemove_Click;
        }

        private void AppointmentRemove_Click(object sender, EventArgs e)
        {
            SQLiteDatabase sqlDatabase = null;
            try
            {
                ImageButton editSender = (ImageButton)sender;
                if (editSender != null)
                {
                    int index = Convert.ToInt32(editSender.Tag);
                    Appointments removeAppointment = _appointments[index];
                    Log.Info(TAG, "AppointmentRemove_Click: Attempting to remove appointment with ID - " + removeAppointment.AppointmentID.ToString());
                    Globals dbHelp = new Globals();
                    dbHelp.OpenDatabase();
                    sqlDatabase = dbHelp.GetSQLiteDatabase();
                    if (sqlDatabase != null && sqlDatabase.IsOpen)
                    {
                        removeAppointment.Remove(sqlDatabase);
                        _appointments.Remove(removeAppointment);
                        Log.Info(TAG, "AppointmentRemove_Click: Removed appointment!");
                        sqlDatabase.Close();
                        sqlDatabase = null;
                        Log.Info(TAG, "AppointmentRemove_Click: Calling AppointmentPopupHelper.AppointmentRemoved with index - " + index.ToString());
                        ((IPopupAdapterCallback)_helper).AppointmentRemoved(index);
                    }
                }
            }
            catch (Exception ex)
            {
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    sqlDatabase.Close();
                    sqlDatabase = null;
                }
                Log.Error(TAG, "AppointmentRemove_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ex, "Attempting to remove Appointment", "AppointmentPopupAdapter.AppointmentRemove_Click");
            }
        }

        private void AppointmentEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ImageButton editSender = (ImageButton)sender;
                if (editSender != null)
                {
                    int index = Convert.ToInt32(editSender.Tag);
                    Appointments editAppointment = _appointments[index];
                    if (editAppointment != null)
                    {
                        Intent intent = new Intent(_activity, typeof(ResourcesAppointmentItemActivity));
                        intent.PutExtra("appointmentDate", editAppointment.AppointmentDate.ToLongDateString());
                        intent.PutExtra("appointmentStatus", STATUS_EDIT);
                        intent.PutExtra("appointmentID", editAppointment.AppointmentID);
                        intent.PutExtra("appointmentTime", editAppointment.AppointmentTime.ToShortTimeString());

                        _activity.StartActivity(intent);
                        if (GlobalData.AppointmentPopupWindow != null)
                        {
                            GlobalData.AppointmentPopupWindow.RequiresParentUpdate = false;
                            GlobalData.AppointmentPopupWindow.Dismiss();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AppointmentEdit_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ex, "Attempting to edit Appointment", "AppointmentPopupAdapter.AppointmentEdit_Click");
            }
        }
    }
}
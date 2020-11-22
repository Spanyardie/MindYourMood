using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Graphics;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Util;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class AppointmentPopupHelper : IPopupAdapterCallback
    {
        public const string TAG = "M:AppointmentPopupHelper";

        private List<Appointments> _appointments = null;
        private Activity _activity = null;
        private LinearLayout _calendarItem = null;

        private LinearLayout _popupLayout = null;
        private LinearLayout _popupTopPointer = null;
        private LinearLayout _popupBottomPointer = null;
        private LinearLayout _popupLeftPointer = null;
        private LinearLayout _popupRightPointer = null;
        private TextView _popupAppointmentDate = null;
        private ImageButton _popupAppointmentDone = null;
        private ListView _appointmentList = null;
        private DateTime _appointmentDate = DateTime.Today;

        private const int POINT_X = 0;
        private const int POINT_Y = 1;

        public AppointmentPopupHelper(Activity activity, LinearLayout calendarItem, List<Appointments> appointments, DateTime appointmentDate)
        {
            if (activity == null)
                throw new Exception("Expected Activity is null");
            if (calendarItem == null)
                throw new Exception("Expected Calendar item is null");
            if (appointments == null)
                throw new Exception("Expected Appointments object is null");
            if (appointments.Count == 0)
                throw new Exception("No appointments were provided");

            _activity = activity;
            _calendarItem = calendarItem;
            _appointments = appointments;
            _appointmentDate = appointmentDate;
        }

        public void ShowPopup()
        {
            GlobalData.AppointmentPopupWindow = new ExtendedPopupWindow();
            GlobalData.AppointmentPopupWindow.Touchable = true;
            GlobalData.AppointmentPopupWindow.Focusable = false;
            GlobalData.AppointmentPopupWindow.OutsideTouchable = false;

            Point windowDimensions = new Point();
            _activity.WindowManager.DefaultDisplay.GetSize(windowDimensions);

            int[] itemPosition = new int[2];
            _calendarItem.GetLocationOnScreen(itemPosition);

            Point itemPoint = new Point(itemPosition[POINT_X], itemPosition[POINT_Y]);

            int itemHeight = _calendarItem.Height;
            int itemWidth = _calendarItem.Width;

            //start orientation detection
            if(_activity.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
            {
                AdjustPopupInPortraitMode(itemPoint, windowDimensions, itemHeight);
            }
            else
            {
                AdjustPopupInLandscapeMode(itemPoint, windowDimensions, itemWidth);
            }

            UpdateAdapter();
        }

        private void UpdateAdapter()
        {
            Log.Info(TAG, "UpdateAdapter: Creating new AppointmentPopupAdapter...");
            var adapter = new AppointmentPopupAdapter(_activity, _appointments, this);
            Log.Info(TAG, "UpdateAdapter: ... created Adapter, sent " + _appointments.Count.ToString() + " appointments");
            if (adapter != null)
            {
                if (_appointmentList != null)
                    _appointmentList.Adapter = adapter;
            }
        }

        private void GetFieldComponents()
        {
            if(_popupLayout != null)
            {
                _popupAppointmentDate = _popupLayout.FindViewById<TextView>(Resource.Id.txtAppointmentDate);
                _appointmentList = _popupLayout.FindViewById<ListView>(Resource.Id.lstAppointments);
                _popupAppointmentDone = _popupLayout.FindViewById<ImageButton>(Resource.Id.imgbtnPopupDone);

                if (_activity.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
                {
                    _popupTopPointer = _popupLayout.FindViewById<LinearLayout>(Resource.Id.linPointerTop);
                    _popupBottomPointer = _popupLayout.FindViewById<LinearLayout>(Resource.Id.linPointerBottom);
                }
                else
                {
                    _popupLeftPointer = _popupLayout.FindViewById<LinearLayout>(Resource.Id.linPointerLeft);
                    _popupRightPointer = _popupLayout.FindViewById<LinearLayout>(Resource.Id.linPointerRight);
                }
            }
        }

        private void SetupCallbacks()
        {
            if(_popupAppointmentDone != null)
                _popupAppointmentDone.Click += PopupAppointmentDone_Click;
        }

        private void PopupAppointmentDone_Click(object sender, EventArgs e)
        {
            GlobalData.AppointmentPopupWindow.RequiresParentUpdate = true;
            GlobalData.AppointmentPopupWindow.Dismiss();
        }

        private void SetupPopupLayout()
        {
            _popupLayout = (LinearLayout)_activity.LayoutInflater.Inflate(Resource.Layout.AppointmentPopupLayout, null, false);
            GetFieldComponents();
            if (_popupAppointmentDate != null)
                _popupAppointmentDate.Text = _appointmentDate.ToLongDateString();
            SetupCallbacks();
        }

        public void AppointmentRemoved(int index)
        {
            if (_appointments.Count == 0)
            {
                GlobalData.AppointmentPopupWindow.RequiresParentUpdate = true;
                GlobalData.AppointmentPopupWindow.Dismiss();
            }
            else
            {
                UpdateAdapter();
            }
        }

        public void PopupInstantiated()
        {
            
        }

        private void AdjustPopupInPortraitMode(Point itemPoint, Point windowDimensions, int itemHeight)
        {
            bool isTopHalf = false;
            if (itemPoint.Y <= (windowDimensions.Y / 2))
                isTopHalf = true;

            SetupPopupLayout();
            GlobalData.AppointmentPopupWindow.ContentView = _popupLayout;

            if (isTopHalf)
            {
                if (_popupTopPointer != null)
                    _popupTopPointer.Visibility = ViewStates.Visible;
                if (_popupBottomPointer != null)
                    _popupBottomPointer.Visibility = ViewStates.Gone;
            }
            else
            {
                if (_popupTopPointer != null)
                    _popupTopPointer.Visibility = ViewStates.Gone;
                if (_popupBottomPointer != null)
                    _popupBottomPointer.Visibility = ViewStates.Visible;
            }

            GlobalData.AppointmentPopupWindow.Height = (windowDimensions.Y / 2) - 50;
            GlobalData.AppointmentPopupWindow.Width = (windowDimensions.X - 50);

            int popupTop;
            int pointerLeft;

            pointerLeft = itemPoint.X;
            if (isTopHalf)
            {
                popupTop = itemPoint.Y + itemHeight;
                _popupTopPointer.SetX(pointerLeft);
                GlobalData.AppointmentPopupWindow.ShowAtLocation(_calendarItem, GravityFlags.NoGravity, 0, popupTop);
                GlobalData.AppointmentPopupWindow.Update(25, popupTop, GlobalData.AppointmentPopupWindow.Width, GlobalData.AppointmentPopupWindow.Height);
            }
            else
            {
                popupTop = itemPoint.Y - GlobalData.AppointmentPopupWindow.Height;
                _popupBottomPointer.SetX(pointerLeft);
                GlobalData.AppointmentPopupWindow.ShowAtLocation(_calendarItem, GravityFlags.NoGravity, 0, popupTop);
                GlobalData.AppointmentPopupWindow.Update(25, popupTop, GlobalData.AppointmentPopupWindow.Width, GlobalData.AppointmentPopupWindow.Height);
            }
        }

        private void AdjustPopupInLandscapeMode(Point itemPoint, Point windowDimensions, int itemWidth)
        {
            bool isLeftHalf = false;
            if (itemPoint.X <= (windowDimensions.X / 2))
                isLeftHalf = true;

            SetupPopupLayout();
            GlobalData.AppointmentPopupWindow.ContentView = _popupLayout;

            if (isLeftHalf)
            {
                if (_popupLeftPointer != null)
                    _popupLeftPointer.Visibility = ViewStates.Visible;
                if (_popupRightPointer != null)
                    _popupRightPointer.Visibility = ViewStates.Gone;
            }
            else
            {
                if (_popupLeftPointer != null)
                    _popupLeftPointer.Visibility = ViewStates.Gone;
                if (_popupRightPointer != null)
                    _popupRightPointer.Visibility = ViewStates.Visible;
            }

            GlobalData.AppointmentPopupWindow.Height = (windowDimensions.Y - 80);
            GlobalData.AppointmentPopupWindow.Width = (windowDimensions.X / 2);

            int popupLeft;
            int pointerTop;

            pointerTop = itemPoint.Y;
            if (isLeftHalf)
            {
                popupLeft = itemPoint.X + itemWidth;
                _popupLeftPointer.SetY(pointerTop);
                GlobalData.AppointmentPopupWindow.ShowAtLocation(_calendarItem, GravityFlags.NoGravity, popupLeft, 0);
                GlobalData.AppointmentPopupWindow.Update(popupLeft, 25, GlobalData.AppointmentPopupWindow.Width, GlobalData.AppointmentPopupWindow.Height);
            }
            else
            {
                popupLeft = itemPoint.X - GlobalData.AppointmentPopupWindow.Width;
                _popupRightPointer.SetY(pointerTop);
                GlobalData.AppointmentPopupWindow.ShowAtLocation(_calendarItem, GravityFlags.NoGravity, popupLeft, 0);
                GlobalData.AppointmentPopupWindow.Update(popupLeft, 25,  GlobalData.AppointmentPopupWindow.Width, GlobalData.AppointmentPopupWindow.Height);
            }
        }
    }
}
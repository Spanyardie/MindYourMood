using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Util;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.SubActivities.Help.Resources;

namespace com.spanyardie.MindYourMood.SubActivities.Resources
{
    [Activity]
    public class ResourcesAppointmentPlannerActivity : AppCompatActivity, IPopupAdapterCallback
    {
        public const string TAG = "M:ResourcesAppointmentPlannerActivity";

        private Toolbar _toolbar;
        private LinearLayout _gridCalendar;
        private ExtendedGridView _gridView;
        private ImageButton _previous;
        private ImageButton _next;
        private TextView _monthYear;

        private string[] _monthsFull = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};

        private bool _gridSetup = false;

        private bool _dataChanged = false;

        private int _currentMonth = 0;

        private bool _isOrientationChange = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            

            try
            {
                base.OnCreate(savedInstanceState);

                SetContentView(Resource.Layout.ResourceAppointmentPlanner);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.resourcesAppointmentPlannerToolbar, Resource.String.ResourcesAppointmentPlannerActionBarTitle, Color.White);

                SetupCallbacks();

                if(savedInstanceState != null)
                {
                    _currentMonth = savedInstanceState.GetInt("currentMonth");
                    _isOrientationChange = true;
                }
                ViewTreeObserver observer = _gridCalendar.ViewTreeObserver;
                observer.GlobalLayout += (sender, args) =>
                {
                    SetupGrid(_gridCalendar.Height);
                    SetMonthYear();
                };
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Creating Activity", "ResourcesAppointmentPlannerActivity.OnCreate");
            }
        }

        public void SetDataChanged(bool dataChanged)
        {
            _dataChanged = dataChanged;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.AppointmentPlannerMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.AppointmentActionAdd);
                var itemHelp = menu.FindItem(Resource.Id.AppointmentActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ResourcesAppointmentPlannerActivity.SetActionIcons");
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                if (item != null)
                {
                    if (GlobalData.AppointmentPopupWindow != null)
                    {
                        GlobalData.AppointmentPopupWindow.RequiresParentUpdate = false;
                        GlobalData.AppointmentPopupWindow.Dismiss();
                        GlobalData.AppointmentPopupWindow = null;
                    }
                    if (item.ItemId == Android.Resource.Id.Home)
                    {
                        Finish();
                        return true;
                    }

                    switch (item.ItemId)
                    {
                        case Resource.Id.AppointmentActionAdd:
                            EnterAppointment();
                            return true;

                        case Resource.Id.AppointmentActionHelp:
                            Intent intent = new Intent(this, typeof(ResourcesAppointmentPlannerHelpActivity));
                            StartActivity(intent);
                            return true;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnOptionsItemSelected: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Processing selected option", "ResourcesAppointmentPlannerActivity.OnOptionsItemSelected");
            }
            return base.OnOptionsItemSelected(item);
        }

        private void EnterAppointment()
        {
            try
            {
                //grab the selected date to pass to the activity
                var selectedDate = _gridView.GetCalendarDateSelected();
                //don't set appointments in the past!
                if (selectedDate < DateTime.Today)
                {
                    Toast.MakeText(this, "Cannot set appointments in the past!", ToastLength.Short).Show();
                    return;
                }

                Intent intent = new Intent(this, typeof(ResourcesAppointmentItemActivity));
                intent.PutExtra("appointmentDate", selectedDate.ToLongDateString());

                StartActivityForResult(intent, 99);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "EnterAppointment: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Attempting to enter Appointment", "ResourcesAppointmentPlannerActivity.EnterAppointment");
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if(requestCode == 99 && resultCode == Result.Ok)
            {
                _dataChanged = true;
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _gridCalendar = FindViewById<LinearLayout>(Resource.Id.linGridCalendar);
                _previous = FindViewById<ImageButton>(Resource.Id.imgbtnPreviousMonth);
                _next = FindViewById<ImageButton>(Resource.Id.imgbtnNextMonth);
                _monthYear = FindViewById<TextView>(Resource.Id.txtMonthYear);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "ResourcesAppointmentPlannerActivity.GetFieldComponents");
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                if (_gridView != null)
                    _gridView.AdjustCalendar(0);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnResume: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Resuming activity and adjusting calendar", "ResourcesAppointmentPlannerActivity.OnResume");
            }
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            try
            {
                base.OnWindowFocusChanged(hasFocus);

                if (hasFocus && _dataChanged)
                {
                    _dataChanged = false;
                    _gridView.UpdateAdapter();
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnWindowFocusChanged: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Updating adapter after focus change", "ResourcesAppointmentPlannerActivity.OnWindowFocusChanged");
            }
        }

        private void SetupGrid(int gridHeight)
        {
            try
            {
                if (_gridSetup) return;

                if (_gridCalendar != null)
                {
                    _gridView = new ExtendedGridView(this, gridHeight);
                    var newParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
                    _gridView.LayoutParameters = newParams;
                    if (_gridView != null)
                    {
                        _gridView.SetHorizontalSpacing(1);
                        _gridView.SetVerticalSpacing(1);
                        _gridView.SetPadding(1, 1, 0, 0);
                        _gridView.SetBackgroundColor(Color.Argb(100, 10, 80, 255));
                        _gridCalendar.AddView(_gridView);
                        _gridSetup = true;
                        if (_isOrientationChange)
                        {
                            Log.Info(TAG, "SetupGrid: Orientation has changed, setting grid view month to " + _currentMonth.ToString());
                            _gridView.CalendarInstance.Set(Java.Util.CalendarField.Month, _currentMonth);
                            _isOrientationChange = false;
                        }
                        _gridView.UpdateAdapter();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupGrid: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting up grid", "ResourcesAppointmentPlannerActivity.SetupGrid");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_previous != null)
                    _previous.Click += Previous_Click;
                if (_next != null)
                    _next.Click += Next_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting up callbacks", "ResourcesAppointmentPlannerActivity.SetupCallbacks");
            }
        }

        private void Next_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalData.AppointmentPopupWindow != null)
                {
                    GlobalData.AppointmentPopupWindow.RequiresParentUpdate = false;
                    GlobalData.AppointmentPopupWindow.Dismiss();
                    GlobalData.AppointmentPopupWindow = null;
                }
                _gridView.AdjustCalendar(1);
                SetMonthYear();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Next_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Moving to next month of calendar", "ResourcesAppointmentPlannerActivity.Next_Click");
            }
        }

        private void Previous_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalData.AppointmentPopupWindow != null)
                {
                    GlobalData.AppointmentPopupWindow.RequiresParentUpdate = false;
                    GlobalData.AppointmentPopupWindow.Dismiss();
                    GlobalData.AppointmentPopupWindow = null;
                }
                _gridView.AdjustCalendar(-1);
                SetMonthYear();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Previous_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Moving to previous month of calendar", "ResourcesAppointmentPlannerActivity.Previous_Click");
            }
        }

        private void SetMonthYear()
        {
            try
            {
                var calendarMonth = _gridView.CalendarInstance.Get(Java.Util.CalendarField.Month);
                _currentMonth = calendarMonth;
                Log.Info(TAG, "SetMonthYear: grid view calendar month is - " + calendarMonth.ToString() + "(current month setting is " + _currentMonth.ToString() + ")");
                string monthName = _monthsFull[calendarMonth];
                if (_monthYear != null)
                    _monthYear.Text = monthName + " " + _gridView.CalendarInstance.Get(Java.Util.CalendarField.Year);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SetMonthYear: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Setting up month and year", "ResourcesAppointmentPlannerActivity.SetMonthYear");
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                Log.Info(TAG, "OnSaveInstanceState: current month being saved - " + _currentMonth.ToString());
                outState.PutInt("currentMonth", _currentMonth);
            }
            base.OnSaveInstanceState(outState);
        }
        private void SetupAppointmentTypeSpinner()
        {

        }

        public void AppointmentRemoved(int index)
        {
            
        }

        public void PopupInstantiated()
        {
            try
            {
                if (GlobalData.AppointmentPopupWindow != null)
                {
                    GlobalData.AppointmentPopupWindow.DismissEvent += (sender, args) =>
                    {
                        if (GlobalData.AppointmentPopupWindow.RequiresParentUpdate)
                            _gridView.UpdateAdapter();
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "PopupInstantiated: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Processing dismiss event of popup", "ResourcesAppointmentPlannerActivity.PopupInstantiated");
            }
        }
    }
}
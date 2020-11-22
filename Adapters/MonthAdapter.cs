using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;
using Java.Util;
using Android.Graphics;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Model;
using Android.Database.Sqlite;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class MonthAdapter : BaseAdapter, IPopupAdapterCallback
    {
        public const string TAG = "M:MonthAdapter";

        private const string LINEAR_LAYOUT = "Android.Widget.LinearLayout";
        private const string TEXT_VIEW_COMPAT = "Android.Support.V7.Widget.AppCompatTextView";
        private const string TEXT_VIEW = "Android.Widget.TextView";

        private GregorianCalendar _calendar;
        private Calendar _calendarToday;
        private Context _context;
        private DisplayMetrics _displayMetrics;
        private List<string> _items;
        private List<Appointments> _displayAppointmentsPrevious;
        private List<Appointments> _displayAppointmentsCurrent;
        private List<Appointments> _displayAppointmentsNext;
        private int _month;
        private int _year;
        private int _daysShown;
        private int _daysLastMonth;
        private int _daysNextMonth;
        private int _titleHeight, _dayHeight;
        private string[] _days = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
	    private int[] _daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private int _containerHeight;
        public int _previousSelectedItem = -1;
        private ExtendedGridView _parent = null;
        private int _currentSelectedItem = -1;

        private const int DATE_DAY = 0;
        private const int DATE_MONTH = 1;
        private const int DATE_YEAR = 2;

        private enum DataMonth
        {
            Single = 0,
            Previous,
            Current,
            Next
        }

        private Color _otherMonthColor = Color.Argb(255, 152, 212, 235);
        private Color _currentMonthColor = Color.Argb(255, 113, 196, 228);
        private Color _headerColor = Color.Argb(100, 10, 80, 255);
        private Color _selectedItemColor = Color.Argb(100, 10, 80, 255);

        private TextView _dayNumber;
        private TextView _dayAppointment1;
        private TextView _dayAppointment2;

        private bool _completeRefresh = false;

        public bool CompleteRefresh
        {
            get
            {
                return _completeRefresh;
            }
            set
            {
                _completeRefresh = value;
            }
        }
        public GregorianCalendar CalendarInstance
        {
            get
            {
                return _calendar;
            }
        }

        private enum MonthType
        {
            Previous = 0,
            Current,
            Next
        }

        public MonthAdapter(Context context, int month, int year, DisplayMetrics metrics, int containerHeight, ExtendedGridView parent, int selectedDatePosition, int addToMonth = 0, bool completeRefresh = false)
        {
            SQLiteDatabase sqlDatabase = null;
            try
            {
                _context = context;
                _month = month;
                _year = year;
                _calendar = new GregorianCalendar(_year, _month, 1);
                _calendarToday = Calendar.Instance;
                AddToCalendar(addToMonth);
                _displayMetrics = metrics;
                _containerHeight = containerHeight;
                _parent = parent;
                _currentSelectedItem = addToMonth == 0 ? selectedDatePosition : 1;

                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();

                if (sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    GetAppointmentsForLastMonth(dbHelp);
                    GetAppointmentsForCurrentMonth(dbHelp);
                    GetAppointmentsForNextMonth(dbHelp);
                    sqlDatabase.Close();
                    sqlDatabase = null;
                }

                _items = new List<string>();

                PopulateMonth();
            }
            catch (Exception e)
            {
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    sqlDatabase.Close();
                    sqlDatabase = null;
                }
                Log.Error(TAG, "Constructor: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)context, e, "Creating adapter", "MonthAdapter.Ctor");
            }
        }

        public void PopulateMonth()
        {
            try
            {

                foreach (var day in _days)
                {
                    _items.Add(day);
                }

                var firstDay = GetDay(_calendar.Get(CalendarField.DayOfWeek));
                int previousDay;

                if (_month == 0)
                {
                    previousDay = DaysInMonth(11) - firstDay + 1;
                }
                else
                {
                    previousDay = DaysInMonth(_month - 1) - firstDay + 1;
                }

                var previousMonth = 0;
                var computedYear = _year;
                bool changedComputedYear = false;
                if (_month == 0)
                {
                    previousMonth = 11;
                    computedYear--;
                    changedComputedYear = true;
                }
                else
                {
                    previousMonth = _month - 1;
                }
                var nextMonth = 0;
                if (_month == 11)
                {
                    nextMonth = 0;
                    computedYear++;
                }
                else
                {
                    nextMonth = _month + 1;
                    if (!changedComputedYear) computedYear++;
                }

                for (var i = 0; i < firstDay; i++)
                {
                    _items.Add((previousDay + i).ToString());
                    _daysLastMonth++;
                    _daysShown++;
                }

                int daysInMonth = DaysInMonth(_month);
                for (int i = 1; i <= daysInMonth; i++)
                {
                    _items.Add(i.ToString());
                    _daysShown++;
                }

                _daysNextMonth = 1;
                while (_daysShown % 7 != 0)
                {
                    _items.Add(_daysNextMonth.ToString());
                    _daysShown++;
                    _daysNextMonth++;
                }

                _titleHeight = 80;
                int rows = (_daysShown / 7);
                _dayHeight = (_containerHeight - (rows * 8) - GetBarHeight()) / rows;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "PopulateMonth: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Populating month", "MonthAdapter.PopulateMonth");
            }
        }

        private void UpdateViewWithAppointments(TextView appt1, TextView appt2, MonthType monthType, int day)
        {
            try
            {
                if (appt1 == null || appt2 == null)
                {
                    Log.Error(TAG, "UpdateViewWithAppointments: View Appointment 1 or 2 is null!");
                    return;
                }
                List<Appointments> localList = null;

                switch (monthType)
                {
                    case MonthType.Previous:
                        localList = _displayAppointmentsPrevious;
                        break;
                    case MonthType.Current:
                        localList = _displayAppointmentsCurrent;
                        break;
                    case MonthType.Next:
                        localList = _displayAppointmentsNext;
                        break;
                }

                if (localList == null)
                {
                    Log.Error(TAG, "UpdateViewWithAppointments: localList is null!");
                    appt1.Text = "";
                    appt2.Text = "";
                    return;
                }

                var dayList =
                    (from eachAppt in localList
                     where eachAppt.AppointmentDate.Day == day
                     select eachAppt).ToList();

                if (dayList != null)
                {
                    if (dayList.Count > 0)
                    {
                        appt1.Text = dayList[0].WithWhom.Trim();
                        if (dayList.Count > 1)
                        {
                            appt2.Text = dayList[1].WithWhom.Trim();
                        }
                        else
                        {
                            appt2.Text = "";
                        }
                    }
                    else
                    {
                        appt1.Text = "";
                        appt2.Text = "";
                    }
                }
                else
                {
                    Log.Error(TAG, "UpdateViewWithAppointments: dayList is null!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateViewWithAppointments: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Updating view with Appointments", "MonthAdapter.UpdateViewWithAppointments");
            }
        }

        private List<Appointments> GetDayAppointmentData(DateTime plannerDay, DataMonth dataMonth)
        {
            try
            {
                List<Appointments> dayAppointments = null;
                int year = plannerDay.Year;

                //grab the month
                int month = plannerDay.Month - 1;

                if (dataMonth == DataMonth.Single)
                {
                    if (year < _year || month < _month) dataMonth = DataMonth.Previous;
                    if (year > _year || month > _month) dataMonth = DataMonth.Next;
                    if (year == _year && month == _month) dataMonth = DataMonth.Current;
                }
                if (dataMonth == DataMonth.Previous)
                {
                    dayAppointments =
                        (from eachAppt in _displayAppointmentsPrevious
                         where eachAppt.AppointmentDate.Day == plannerDay.Day
                         select eachAppt).ToList();
                }
                if (dataMonth == DataMonth.Current)
                {
                    dayAppointments =
                        (from eachAppt in _displayAppointmentsCurrent
                         where eachAppt.AppointmentDate.Day == plannerDay.Day
                         select eachAppt).ToList();
                }
                if (dataMonth == DataMonth.Next)
                {
                    dayAppointments =
                        (from eachAppt in _displayAppointmentsNext
                         where eachAppt.AppointmentDate.Day == plannerDay.Day
                         select eachAppt).ToList();
                }

                return dayAppointments;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetDayAppointmentData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Retreiving day Appointment data", "MonthAdapter.GetDayAppointmentData");
                return null;
            }
        }

        private int DaysInMonth(int month)
        {
            try
            {
                if (_daysInMonth != null)
                {
                    int daysInMonth = _daysInMonth[month];
                    if (_calendar != null)
                    {
                        if (month == 1 && _calendar.IsLeapYear(_year))
                            daysInMonth++;

                        return daysInMonth;
                    }
                    else
                    {
                        Log.Error(TAG, "DaysInMonth: _calendar is null!");
                        return 0;
                    }
                }
                else
                {
                    Log.Error(TAG, "DaysInMonth: _daysInMonth is null!");
                    return 0;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "DaysInMonth: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Getting days for month", "MonthAdapter.DaysInMonth");
                return 0;
            }
        }

        private int GetBarHeight()
        {
            if (_displayMetrics != null)
            {
                switch (_displayMetrics.DensityDpi)
                {
                    case DisplayMetricsDensity.High:
                        return 48;
                    case DisplayMetricsDensity.Medium:
                        return 32;
                    case DisplayMetricsDensity.Low:
                        return 24;
                    default:
                        return 48;
                }
            }
            else
            {
                Log.Error(TAG, "GetBarHeight: Unable to get Bar height because _displayMetrics is null!");
                return 48;
            }
        }

        private bool IsToday(int day, int month, int year)
        {
            if (_calendarToday != null)
            {
                if (_calendarToday.Get(CalendarField.Month) == month
                    && _calendarToday.Get(CalendarField.Year) == year
                    && _calendarToday.Get(CalendarField.DayOfMonth) == day)
                {
                    return true;
                }
            }
            else
            {
                Log.Error(TAG, "IsToday: Not checking for date being today because _calendarToday is null!");
            }
            return false;
        }

        private int GetDay(int day)
        {
            switch(day)
            {
                case Calendar.Monday:
                    return 0;
                case Calendar.Tuesday:
                    return 1;
                case Calendar.Wednesday:
                    return 2;
                case Calendar.Thursday:
                    return 3;
                case Calendar.Friday:
                    return 4;
                case Calendar.Saturday:
                    return 5;
                case Calendar.Sunday:
                    return 6;
                default:
                    return 0;
            }
        }

        private int[] GetDate(int position)
        {
            try
            {
                if(_items == null)
                {
                    Log.Error(TAG, "GetDate: Unable to get date because _items is null!");
                    return null;
                }

                var date = new int[3];

                if (position <= 6)
                {
                    return null;
                }
                else if (position <= _daysLastMonth + 6)
                {
                    //previous month
                    date[DATE_DAY] = int.Parse(_items[position]);
                    if (_month == 0)
                    {
                        date[DATE_MONTH] = 11;
                        date[DATE_YEAR] = _year - 1;
                    }
                    else
                    {
                        date[DATE_MONTH] = _month - 1;
                        date[DATE_YEAR] = _year;
                    }
                }
                else if (position <= _daysShown - (_daysNextMonth - 7))
                {
                    //current month
                    date[DATE_DAY] = position - (_daysLastMonth + 6);
                    date[DATE_MONTH] = _month;
                    date[DATE_YEAR] = _year;
                }
                else
                {
                    //next month
                    date[DATE_DAY] = int.Parse(_items[position]);
                    if (_month == 11)
                    {
                        date[DATE_MONTH] = 0;
                        date[DATE_YEAR] = _year + 1;
                    }
                    else
                    {
                        date[DATE_MONTH] = _month + 1;
                        date[DATE_YEAR] = _year;
                    }
                }
                return date;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetDate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Getting date", "MonthAdapter.Getdate");
                return null;
            }
        }

        public override int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                Activity activityContext = null;
                LinearLayout dayView = null;
                bool inflated = false;

                if (_parent != null)
                {
                    activityContext = (Activity)_parent.Context;
                }

                if (activityContext != null)
                {
                    if (convertView == null || _completeRefresh)
                    {
                        LayoutInflater inflater = (LayoutInflater)activityContext.GetSystemService(Context.LayoutInflaterService);
                        if (inflater != null)
                        {
                            dayView = (LinearLayout)inflater.Inflate(Resource.Layout.PlannerDayItem, parent, false);
                            inflated = true;
                        }
                        else
                        {
                            Log.Error(TAG, "GetView: Could not initialise inflater!");
                            return null;
                        }
                    }
                    else
                    {
                        dayView = (LinearLayout)convertView;
                    }

                    GetFieldComponents(dayView);

                    if (inflated)
                    {
                        if (_dayNumber != null)
                        {
                            _dayNumber.Text = _items[position].ToString();
                            _dayNumber.Tag = position;
                        }
                        else
                        {
                            Log.Error(TAG, "GetView: _dayNumber is null!");
                        }
                        if (_dayAppointment1 != null)
                        {
                            _dayAppointment1.Tag = position;
                        }
                        else
                        {
                            Log.Error(TAG, "GetView: _dayAppointment1 is null!");
                        }
                        if (_dayAppointment2 != null)
                        {
                            _dayAppointment2.Tag = position;
                        }
                        else
                        {
                            Log.Error(TAG, "GetView: _dayAppointment2 is null!");
                        }
                        dayView.Tag = position;

                        dayView.Click += View_Click;
                        if (_dayAppointment1 != null &&
                            _dayAppointment2 != null &&
                            _dayNumber != null)
                        {
                            _dayAppointment1.Click += View_Click;
                            _dayAppointment2.Click += View_Click;
                            _dayNumber.Click += View_Click;
                        }
                        dayView.LongClick += DayView_LongClick;
                        if (_dayAppointment1 != null &&
                            _dayAppointment2 != null &&
                            _dayNumber != null)
                        {
                            _dayAppointment1.LongClick += DayView_LongClick;
                            _dayAppointment2.LongClick += DayView_LongClick;
                            _dayNumber.LongClick += DayView_LongClick;
                        }

                        int[] date = GetDate(position);
                        if (date != null)
                        {
                            dayView.SetMinimumHeight(_dayHeight);
                            if (date[DATE_MONTH] != _month)
                            {
                                dayView.SetBackgroundColor(_otherMonthColor);
                                if (_dayNumber != null)
                                {
                                    _dayNumber.SetBackgroundColor(_otherMonthColor);
                                    _dayNumber.SetTextColor(Color.DarkBlue);
                                }
                                else
                                {
                                    Log.Error(TAG, "GetView: _dayNumber is null!");
                                }
                                if (_dayAppointment1 != null)
                                {
                                    _dayAppointment1.SetBackgroundColor(_otherMonthColor);
                                    _dayAppointment1.SetTextColor(Color.DarkBlue);
                                }
                                else
                                {
                                    Log.Error(TAG, "GetView: _dayAppointment1 is null!");
                                }
                                if (_dayAppointment2 != null)
                                {
                                    _dayAppointment2.SetBackgroundColor(_otherMonthColor);
                                    _dayAppointment2.SetTextColor(Color.DarkBlue);
                                }
                                else
                                {
                                    Log.Error(TAG, "GetView: _dayAppointment2 is null!");
                                }

                                if (date[DATE_YEAR] == _year)
                                {
                                    if (date[DATE_MONTH] < _month)
                                    {
                                        UpdateViewWithAppointments(_dayAppointment1, _dayAppointment2, MonthType.Previous, date[DATE_DAY]);
                                    }
                                    else
                                    {
                                        UpdateViewWithAppointments(_dayAppointment1, _dayAppointment2, MonthType.Next, date[DATE_DAY]);
                                    }
                                }
                                if(date[DATE_YEAR] < _year )
                                {
                                    //if the year is less than the current year, then it follows that the month is a previous month
                                    UpdateViewWithAppointments(_dayAppointment1, _dayAppointment2, MonthType.Previous, date[DATE_DAY]);
                                }
                                if(date[DATE_YEAR] > _year)
                                {
                                    //if the year is greater than the current year, then it follows that the month is the next month
                                    UpdateViewWithAppointments(_dayAppointment1, _dayAppointment2, MonthType.Next, date[DATE_DAY]);
                                }

                                if (_parent != null)
                                {
                                    if (position == _parent.SelectedDatePosition)
                                    {
                                        dayView.SetBackgroundColor(_selectedItemColor);
                                        if (_dayNumber != null)
                                        {
                                            _dayNumber.SetBackgroundColor(_selectedItemColor);
                                            _dayNumber.SetTextColor(Color.White);
                                        }
                                        else
                                        {
                                            Log.Error(TAG, "GetView: _dayNumber is null!");
                                        }
                                        if (_dayAppointment1 != null)
                                        {
                                            _dayAppointment1.SetBackgroundColor(_selectedItemColor);
                                            _dayAppointment1.SetTextColor(Color.White);
                                        }
                                        else
                                        {
                                            Log.Error(TAG, "GetView: _dayAppointment1 is null!");
                                        }
                                        if (_dayAppointment2 != null)
                                        {
                                            _dayAppointment2.SetBackgroundColor(_selectedItemColor);
                                            _dayAppointment2.SetTextColor(Color.White);
                                        }
                                        else
                                        {
                                            Log.Error(TAG, "GetView: _dayAppointment2 is null!");
                                        }
                                        _currentSelectedItem = position;
                                    }
                                }
                            }
                            else
                            {
                                dayView.SetBackgroundColor(_currentMonthColor);
                                if (_dayNumber != null)
                                {
                                    _dayNumber.SetBackgroundColor(_currentMonthColor);
                                    _dayNumber.SetTextColor(Color.DarkBlue);
                                }
                                else
                                {
                                    Log.Error(TAG, "GetView: _dayNumber is null!");
                                }
                                if (_dayAppointment1 != null)
                                {
                                    _dayAppointment1.SetBackgroundColor(_currentMonthColor);
                                    _dayAppointment1.SetTextColor(Color.DarkBlue);
                                }
                                else
                                {
                                    Log.Error(TAG, "GetView: _dayAppointment1 is null!");
                                }
                                if (_dayAppointment2 != null)
                                {
                                    _dayAppointment2.SetBackgroundColor(_currentMonthColor);
                                    _dayAppointment2.SetTextColor(Color.DarkBlue);
                                }
                                else
                                {
                                    Log.Error(TAG, "GetView: _dayAppointment2 is null!");
                                }

                                UpdateViewWithAppointments(_dayAppointment1, _dayAppointment2, MonthType.Current, date[DATE_DAY]);

                                if (_parent != null)
                                {
                                    if (position == _parent.SelectedDatePosition)
                                    {
                                        dayView.SetBackgroundColor(_selectedItemColor);
                                        if (_dayNumber != null)
                                        {
                                            _dayNumber.SetBackgroundColor(_selectedItemColor);
                                            _dayNumber.SetTextColor(Color.White);
                                        }
                                        else
                                        {
                                            Log.Error(TAG, "GetView: _dayNumber is null!");
                                        }
                                        if (_dayAppointment1 != null)
                                        {
                                            _dayAppointment1.SetBackgroundColor(_selectedItemColor);
                                            _dayAppointment1.SetTextColor(Color.White);
                                        }
                                        else
                                        {
                                            Log.Error(TAG, "GetView: _dayAppointment1 is null!");
                                        }
                                        if (_dayAppointment2 != null)
                                        {
                                            _dayAppointment2.SetBackgroundColor(_selectedItemColor);
                                            _dayAppointment2.SetTextColor(Color.White);
                                        }
                                        else
                                        {
                                            Log.Error(TAG, "GetView: _dayAppointment2 is null!");
                                        }
                                        _currentSelectedItem = position;
                                    }
                                }
                                if (IsToday(date[DATE_DAY], date[DATE_MONTH], date[DATE_YEAR]))
                                {
                                    if (_currentSelectedItem == -1)
                                    {
                                        dayView.SetBackgroundColor(_selectedItemColor);
                                        if (_dayNumber != null)
                                        {
                                            _dayNumber.SetBackgroundColor(_selectedItemColor);
                                        }
                                        else
                                        {
                                            Log.Error(TAG, "GetView: _dayNumber is null!");
                                        }
                                        if (_dayAppointment1 != null)
                                        {
                                            _dayAppointment1.SetBackgroundColor(_selectedItemColor);
                                            _dayAppointment1.SetTextColor(Color.Yellow);
                                        }
                                        else
                                        {
                                            Log.Error(TAG, "GetView: _dayAppointment1 is null!");
                                        }
                                        if (_dayAppointment2 != null)
                                        {
                                            _dayAppointment2.SetBackgroundColor(_selectedItemColor);
                                            _dayAppointment2.SetTextColor(Color.Yellow);
                                        }
                                        else
                                        {
                                            Log.Error(TAG, "GetView: _dayAppointment2 is null!");
                                        }
                                        _currentSelectedItem = position;
                                    }
                                    _dayNumber.SetTextColor(Color.Yellow);
                                }
                            }
                            ((IExtendedGridView)_parent).ItemChanged(_currentSelectedItem, new DateTime(date[DATE_YEAR], (date[DATE_MONTH] + 1), date[DATE_DAY]));
                        }
                        else
                        {
                            dayView.SetBackgroundColor(_headerColor);
                            dayView.SetMinimumHeight(_titleHeight);
                            if (_dayNumber != null)
                            {
                                _dayNumber.SetBackgroundColor(_headerColor);
                            }
                            else
                            {
                                Log.Error(TAG, "GetView: _dayNumber is null!");
                            }
                            if (_dayAppointment1 != null)
                            {
                                _dayAppointment1.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                Log.Error(TAG, "GetView: _dayAppointment1 is null!");
                            }
                            if (_dayAppointment2 != null)
                            {
                                _dayAppointment2.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                Log.Error(TAG, "GetView: _dayAppointment2 is null!");
                            }
                        }
                    }
                }
                else
                {
                    Log.Error(TAG, "GetView: activityContext is null!");
                }
                return dayView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Getting View", "MonthAdapter.GetView");
                return convertView;
            }
        }

        private void DayView_LongClick(object sender, View.LongClickEventArgs e)
        {
            try
            {
                if(GlobalData.AppointmentPopupWindow != null)
                {
                    GlobalData.AppointmentPopupWindow.RequiresParentUpdate = false;
                    GlobalData.AppointmentPopupWindow.Dismiss();
                    GlobalData.AppointmentPopupWindow = null;
                }

                View view = (View)sender;
                string viewType = view.GetType().FullName;
                LinearLayout dayView = null;

                //we want the dayView to be the encompassing LinearLayout
                if (viewType == TEXT_VIEW_COMPAT || viewType == TEXT_VIEW)
                {
                    dayView = (LinearLayout)view.Parent;
                }
                else
                {
                    dayView = (LinearLayout)view;
                }

                int[] date = GetDate(_currentSelectedItem);
                if (_currentSelectedItem != Convert.ToInt32(dayView.Tag))
                {
                    //loop the dayView's child elements and make them the selected colour
                    for (var loop = 0; loop < dayView.ChildCount; loop++)
                    {
                        var childView = dayView.GetChildAt(loop);
                        childView.SetBackgroundColor(_selectedItemColor);
                        var textItem = (TextView)childView;
                        if (textItem != null)
                        {
                            if (IsToday(date[DATE_DAY], date[DATE_MONTH], date[DATE_YEAR]))
                            {
                                textItem.SetTextColor(Color.Yellow);
                            }
                            else
                            {
                                textItem.SetTextColor(Color.DarkBlue);
                            }
                        }
                    }
                    dayView.SetBackgroundColor(_selectedItemColor);
                    //now we want to reset the previous item to its original colouring
                    LinearLayout previousItem = (LinearLayout)_parent.GetChildAt(_currentSelectedItem);
                    if (date != null)
                    {
                        if (date[1] != _month)
                        {
                            previousItem.SetBackgroundColor(_otherMonthColor);
                            for (var loop = 0; loop < previousItem.ChildCount; loop++)
                            {
                                var childView = previousItem.GetChildAt(loop);
                                childView.SetBackgroundColor(_otherMonthColor);
                                var textItem = (TextView)childView;
                                if (textItem != null)
                                {
                                    if (IsToday(date[DATE_DAY], date[DATE_MONTH], date[DATE_YEAR]))
                                    {
                                        textItem.SetTextColor(Color.Yellow);
                                    }
                                    else
                                    {
                                        textItem.SetTextColor(Color.DarkBlue);
                                    }
                                }
                            }
                        }
                        else
                        {
                            previousItem.SetBackgroundColor(_currentMonthColor);
                            for (var loop = 0; loop < previousItem.ChildCount; loop++)
                            {
                                var childView = previousItem.GetChildAt(loop);
                                childView.SetBackgroundColor(_currentMonthColor);
                                var textItem = (TextView)childView;
                                if (textItem != null)
                                    textItem.SetTextColor(Color.DarkBlue);
                            }
                        }
                    }
                }

                //finally, make the item we selected the current one
                _currentSelectedItem = Convert.ToInt32(dayView.Tag);
                date = GetDate(_currentSelectedItem);
                //and inform the parent
                DateTime dateSelected;
                if (date != null)
                {
                    dateSelected = new DateTime(date[DATE_YEAR], (date[DATE_MONTH] + 1), date[DATE_DAY]);
                    ((IExtendedGridView)_parent).ItemChanged(_currentSelectedItem, dateSelected);
                }
                else
                {
                    Log.Error(TAG, "DayView_LongClick: Unable to show popup because date is invalid!");
                    return;
                }

                //display the popup
                var itemAppointments = GetDayAppointmentData(dateSelected, DataMonth.Single);
                if (itemAppointments != null && itemAppointments.Count > 0)
                {
                    var popupHelper = new AppointmentPopupHelper((Activity)_context, dayView, itemAppointments, dateSelected);

                    popupHelper.ShowPopup();
                    ((IExtendedGridView)_parent).PopupInstantiated();
                }
                else
                {
                    Log.Info(TAG, "DayView_LongClick: No appointments retrieved for specified date");
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "DayView_LongClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, ex, "Updating parent with changed item", "MonthAdapter.DayView_LongClick");
            }
        }

        private void GetFieldComponents(View dayView)
        {
            try
            {
                if(dayView != null)
                {
                    _dayNumber = dayView.FindViewById<TextView>(Resource.Id.txtPlannerDayNumber);
                    _dayAppointment1 = dayView.FindViewById<TextView>(Resource.Id.txtDayAppointment1);
                    _dayAppointment2 = dayView.FindViewById<TextView>(Resource.Id.txtDayAppointment2);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: dayView is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Getting field components", "MonthAdapter.GetFieldComponents");
            }
        }

        private void View_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalData.AppointmentPopupWindow != null)
                {
                    GlobalData.AppointmentPopupWindow.RequiresParentUpdate = false;
                    GlobalData.AppointmentPopupWindow.Dismiss();
                    GlobalData.AppointmentPopupWindow = null;
                }
                View view = (View)sender;
                string viewType = view.GetType().FullName;
                LinearLayout dayView = null;

                //we want the dayView to be the encompassing LinearLayout
                if(viewType == TEXT_VIEW_COMPAT || viewType == TEXT_VIEW)
                {
                    dayView = (LinearLayout)view.Parent;
                }
                else
                {
                    dayView = (LinearLayout)view;
                }

                //do nothing if we have pressed the item that is the same as the current one
                if (_currentSelectedItem == Convert.ToInt32(dayView.Tag))
                {
                    return;
                }

                //loop the dayView's child elements and make them the selected colour
                dayView.SetBackgroundColor(_selectedItemColor);
                for(var loop = 0; loop < dayView.ChildCount; loop++)
                {
                    var childView = dayView.GetChildAt(loop);
                    childView.SetBackgroundColor(_selectedItemColor);
                    var textItem = (TextView)childView;
                    if (textItem != null)
                        textItem.SetTextColor(Color.White);
                }

                //now we want to reset the previous item to its original colouring
                LinearLayout previousItem = (LinearLayout)_parent.GetChildAt(_currentSelectedItem);
                int[] date = GetDate(_currentSelectedItem);
                if (date != null)
                {
                    if (date[1] != _month)
                    {
                        previousItem.SetBackgroundColor(_otherMonthColor);
                        for(var loop = 0; loop < previousItem.ChildCount; loop++)
                        {
                            var childView = previousItem.GetChildAt(loop);
                            childView.SetBackgroundColor(_otherMonthColor);
                            var textItem = (TextView)childView;
                            if (textItem != null)
                            {
                                if (IsToday(date[DATE_DAY], date[DATE_MONTH], date[DATE_YEAR]))
                                {
                                    textItem.SetTextColor(Color.Yellow);
                                }
                                else
                                {
                                    textItem.SetTextColor(Color.DarkBlue);
                                }
                            }
                        }
                    }
                    else
                    {
                        previousItem.SetBackgroundColor(_currentMonthColor);
                        for (var loop = 0; loop < previousItem.ChildCount; loop++)
                        {
                            var childView = previousItem.GetChildAt(loop);
                            childView.SetBackgroundColor(_currentMonthColor);
                            var textItem = (TextView)childView;
                            if (textItem != null)
                            {
                                if (IsToday(date[DATE_DAY], date[DATE_MONTH], date[DATE_YEAR]))
                                {
                                    textItem.SetTextColor(Color.Yellow);
                                }
                                else
                                {
                                    textItem.SetTextColor(Color.DarkBlue);
                                }
                            }
                        }
                    }
                }

                //finally, make the item we selected the current one
                _currentSelectedItem = Convert.ToInt32(dayView.Tag);
                //and inform the parent
                if(date != null)
                    ((IExtendedGridView)_parent).ItemChanged(_currentSelectedItem, new DateTime(date[DATE_YEAR], (date[DATE_MONTH] + 1), date[DATE_DAY]));


            }
            catch (Exception ex)
            {
                Log.Error(TAG, "View_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, ex, "Updating parent with changed item", "MonthAdapter.View_Click");
            }
        }

        public DateTime GetCalendarDateSelected()
        {
            try
            {
                if (_currentSelectedItem != -1)
                {
                    int[] selectedDate = GetDate(_currentSelectedItem);
                    if (selectedDate != null)
                        return new DateTime(selectedDate[DATE_YEAR], (selectedDate[DATE_MONTH] + 1), selectedDate[DATE_DAY]);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetCalendarDateSelected: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Getting selected calendar date", "MonthAdapter.GetCalendarDateSelected");
            }
            return DateTime.Today;
        }

        public void AddToCalendar(int amount)
        {
            if (_calendar != null)
            {
                _calendar.Add(CalendarField.Month, amount);
                _year = _calendar.Get(CalendarField.Year);
                _month = _calendar.Get(CalendarField.Month);
            }
        }

        public void AppointmentRemoved(int index)
        {
            //We don't care about the index in this instance
            PopulateMonth();
        }

        public void PopupInstantiated()
        {

        }

        private void GetAppointmentsForLastMonth(Globals dbHelp)
        {
            SQLiteDatabase sqlDatabase = dbHelp.GetSQLiteDatabase();
            int month = _month;
            int year = _year;
            if (month == 0)
            {
                month = 12;
                year--;
            }
            
            try
            {
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    _displayAppointmentsPrevious = dbHelp.GetAllAppointmentsForMonth(year, month);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetAppointmentsForLastMonth: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Getting Appointments for last month", "MonthAdapter.GetAppointmentsForLastMonth");
            }
        }

        private void GetAppointmentsForCurrentMonth(Globals dbHelp)
        {
            SQLiteDatabase sqlDatabase = dbHelp.GetSQLiteDatabase();
            try
            {
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    _displayAppointmentsCurrent = dbHelp.GetAllAppointmentsForMonth(_year, _month + 1);
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAppointmentsForCurrentMonth: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Getting Appointments for current month", "MonthAdapter.GetAppointmentsForCurrentMonth");
            }
        }

        private void GetAppointmentsForNextMonth(Globals dbHelp)
        {
            SQLiteDatabase sqlDatabase = dbHelp.GetSQLiteDatabase();

            int year = _year;
            int month = _month;
            if(month == 11)
            {
                month = 0;
                year++;
            }

            try
            {
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    _displayAppointmentsNext = dbHelp.GetAllAppointmentsForMonth(year, month + 2);
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAppointmentsForNextMonth: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert((Activity)_parent.Context, e, "Getting Appointments for next month", "MonthAdapter.GetAppointmentsForNextMonth");
            }
        }
    }
}
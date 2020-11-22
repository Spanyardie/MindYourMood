using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using TimeZone = Java.Util.TimeZone;
using Android.Util;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Model;
using Android.Database.Sqlite;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ExtendedGridView : GridView, IExtendedGridView
    {
        public const string TAG = "M:ExtendedGridView";

        private GregorianCalendar _calendar = null;
        private int _containerHeight = 0;
        private DateTime _calendarDate = DateTime.Now;
        private Context _context = null;

        private List<Appointments> _dayAppointments = null;

        private int _monthDelta = 0;

        public List<Appointments> GetAppointments
        {
            get
            {
                return _dayAppointments;
            }
        }

        private void GetDayAppointmentData(DateTime plannerDay)
        {
            Globals dbHelp = new Globals();
            dbHelp.OpenDatabase();

            SQLiteDatabase sqlDatabase = dbHelp.GetSQLiteDatabase();
            if(sqlDatabase != null && sqlDatabase.IsOpen)
            {
                dbHelp.GetAllAppointmentsForDate(plannerDay);
                _dayAppointments = GlobalData.Appointments;
            }

            dbHelp.CloseDatabase();
            sqlDatabase = null;
        }

        public int SelectedDatePosition { get; set; }

        public ExtendedGridView(Context context, int containerHeight):base(context)
        {
            _containerHeight = containerHeight;
            SelectedDatePosition = -1;
            _context = context;
            Initialise();
        }

        private void Initialise()
        {
            SetNumColumns(7);
            _calendar = (GregorianCalendar)Calendar.GetInstance(TimeZone.Default);

            if(_calendar != null)
            {
                _calendar.Lenient = false;
            }
            GetDayAppointmentData(DateTime.Today);
        }

        public void UpdateAdapter()
        {
            // get display metrics
            DisplayMetrics metrics = new DisplayMetrics();
            IWindowManager windowManager = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            windowManager.DefaultDisplay.GetMetrics(metrics);

            // set adapter
            Adapter = new MonthAdapter(Context, _calendar.Get(CalendarField.Month), _calendar.Get(CalendarField.Year), metrics, _containerHeight, this, SelectedDatePosition, _monthDelta, (_monthDelta == 0 ? true : false));

            ((MonthAdapter)Adapter).CompleteRefresh = false;
        }

        public void ItemChanged(int position, DateTime selectedDate)
        {
            SelectedDatePosition = position;

            _calendarDate = selectedDate;
        }

        public DateTime GetCalendarDateSelected()
        {
            if (Adapter != null)
            {
                _calendarDate = ((MonthAdapter)Adapter).GetCalendarDateSelected();
            }
            return _calendarDate;
        }

        public void AdjustCalendar(int monthDelta)
        {
            _monthDelta = monthDelta;
            Log.Info(TAG, "AdjustCalendar: Updating adapter after receiving delta - " + monthDelta.ToString());
            UpdateAdapter();
            _monthDelta = 0;
            GregorianCalendar tempCalendar = ((MonthAdapter)Adapter).CalendarInstance;
            _calendar.Set(tempCalendar.Get(CalendarField.Year), tempCalendar.Get(CalendarField.Month), tempCalendar.Get(CalendarField.DayOfMonth));
        }

        public void PopupInstantiated()
        {
            if (_context != null)
                ((IPopupAdapterCallback)_context).PopupInstantiated();
        }

        public GregorianCalendar CalendarInstance
        {
            get
            {
                return _calendar;
            }
        }

    }
}
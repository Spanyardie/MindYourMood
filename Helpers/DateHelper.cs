using System;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class DateHelper
    {
        public static DateTime FindDateForBeginningOfWeek()
        {
            var dayOfWeek = (int)ConversionHelper.ConvertFromDateTimeDaysToMYMDays(DateTime.Today.DayOfWeek);
            var dowMonday = (int)ConversionHelper.ConvertFromDateTimeDaysToMYMDays(DayOfWeek.Monday);

            var monday = DateTime.Today.AddDays(-dayOfWeek + dowMonday);

            return monday;
        }

        public static DateTime FindDateForEndOfWeek()
        {
            var monday = FindDateForBeginningOfWeek();
            var sunday = monday.AddDays(6);

            return sunday;
        }

        public static ConstantsAndTypes.NumericComparator CompareSpecifiedTimeWithActivityTimeRange(DateTime specifiedTime, ConstantsAndTypes.ACTIVITY_HOURS activityHours)
        {
            DateTime startHour = DateTime.Now;
            DateTime endHour = DateTime.Now;

            switch(activityHours)
            {
                case ConstantsAndTypes.ACTIVITY_HOURS.SixAMToEightAM:
                    startHour = new DateTime(1900, 1, 1, 6, 0, 0);
                    endHour = new DateTime(1900, 1, 1, 7, 59, 59);
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.EightAMToTenAM:
                    startHour = new DateTime(1900, 1, 1, 8, 0, 0);
                    endHour = new DateTime(1900, 1, 1, 9, 59, 59);
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TenAMToTwelvePM:
                    startHour = new DateTime(1900, 1, 1, 10, 0, 0);
                    endHour = new DateTime(1900, 1, 1, 11, 59, 59);
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TwelvePMToTwoPM:
                    startHour = new DateTime(1900, 1, 1, 12, 0, 0);
                    endHour = new DateTime(1900, 1, 1, 13, 59, 59);
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TwoPMToFourPM:
                    startHour = new DateTime(1900, 1, 1, 14, 0, 0);
                    endHour = new DateTime(1900, 1, 1, 15, 59, 59);
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.FourPMToSixPM:
                    startHour = new DateTime(1900, 1, 1, 16, 0, 0);
                    endHour = new DateTime(1900, 1, 1, 17, 59, 59);
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.SixPMToEightPM:
                    startHour = new DateTime(1900, 1, 1, 18, 0, 0);
                    endHour = new DateTime(1900, 1, 1, 19, 59, 59);
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.EightPMToTenPM:
                    startHour = new DateTime(1900, 1, 1, 20, 0, 0);
                    endHour = new DateTime(1900, 1, 1, 21, 59, 59);
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TenPMToTwelveAM:
                    startHour = new DateTime(1900, 1, 1, 22, 0, 0);
                    endHour = new DateTime(1900, 1, 1, 23, 59, 59);
                    break;
            }

            int currentHour = specifiedTime.Hour;

            if (currentHour >= startHour.Hour && currentHour <= endHour.Hour)
                return ConstantsAndTypes.NumericComparator.EqualTo;

            if (currentHour < startHour.Hour)
                return ConstantsAndTypes.NumericComparator.LessThan;

            if (currentHour > endHour.Hour)
                return ConstantsAndTypes.NumericComparator.GreaterThan;

            return ConstantsAndTypes.NumericComparator.LessThan;
        }

        public static int GetDaysInMonth(int month)
        {
            int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            return daysInMonth[month - 1];
        }
    }
}
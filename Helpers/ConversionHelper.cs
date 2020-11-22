using System;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class ConversionHelper
    {
        public static ConstantsAndTypes.DAYS_OF_THE_WEEK ConvertFromDateTimeDaysToMYMDays(DayOfWeek dayOfWeek)
        {
            switch(dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return ConstantsAndTypes.DAYS_OF_THE_WEEK.Monday;
                case DayOfWeek.Tuesday:
                    return ConstantsAndTypes.DAYS_OF_THE_WEEK.Tuesday;
                case DayOfWeek.Wednesday:
                    return ConstantsAndTypes.DAYS_OF_THE_WEEK.Wednesday;
                case DayOfWeek.Thursday:
                    return ConstantsAndTypes.DAYS_OF_THE_WEEK.Thursday;
                case DayOfWeek.Friday:
                    return ConstantsAndTypes.DAYS_OF_THE_WEEK.Friday;
                case DayOfWeek.Saturday:
                    return ConstantsAndTypes.DAYS_OF_THE_WEEK.Saturday;
                case DayOfWeek.Sunday:
                    return ConstantsAndTypes.DAYS_OF_THE_WEEK.Sunday;
                default:
                    return ConstantsAndTypes.DAYS_OF_THE_WEEK.Monday;
            }
        }
    }
}
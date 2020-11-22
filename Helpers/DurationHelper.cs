using System;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class DurationHelper
    {
        public struct Duration
        {
            public int Hours;
            public int Minutes;
            public int Seconds;
        }

        public static Duration ConvertMillisToDuration(long millis)
        {
            //convert millis to seconds
            int seconds = (int)(millis / 1000) % 60;
            int minutes = (int)(millis / (1000 * 60) % 60);
            int hours = (int)(millis / (1000 * 60 * 60) % 24);

            Duration duration = new Duration() { Hours = hours, Minutes = minutes, Seconds = seconds };

            return duration;
        }

        public static long ConvertDurationToMillis(Duration duration)
        {
            int hours = duration.Hours * 60 * 60 * 1000;
            int minutes = duration.Minutes * 60 * 1000;
            int seconds = duration.Seconds * 1000;

            return (hours + minutes + seconds);
        }
    }
}
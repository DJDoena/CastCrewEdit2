using System;

namespace DoenaSoft.DVDProfiler.FindMergedCastCrew.Main
{
    internal sealed class RemainingTimeCalculator : IRemainingTimeCalculator
    {
        private DateTime StartTime { get; set; }

        public RemainingTimeCalculator()
        {
            StartTime = new DateTime(0);
        }

        #region IRemainingTimeCalculator

        public void Start()
        {
            StartTime = DateTime.Now;
        }

        public string Get(int value
            , int max)
        {
            if (value == 0)
            {
                return (string.Empty);
            }

            var elapsed = DateTime.Now.Subtract(StartTime);

            var complete = (elapsed.TotalSeconds / value) * max;

            var remaining = TimeSpan.FromSeconds(complete - elapsed.TotalSeconds);

            var text = GetRemainingText(remaining);

            return (text);
        }

        #endregion

        private static string GetRemainingText(TimeSpan remaining)
        {
            var days = remaining.Days;

            var hours = remaining.Hours;

            var minutes = (remaining.Seconds >= 30) ? (remaining.Minutes + 1) : remaining.Minutes;

            if (minutes == 60)
            {
                hours++;

                minutes = 0;
            }

            if (hours == 24)
            {
                days++;

                hours = 0;
            }

            var dayString = (days > 0) ? $"{days}d " : string.Empty;

            var hourString = ((days > 0) || (hours > 0)) ? $"{hours}h " : string.Empty;

            var text = $" (est. {dayString}{hourString}{minutes}m remaining)";

            return (text);
        }
    }
}

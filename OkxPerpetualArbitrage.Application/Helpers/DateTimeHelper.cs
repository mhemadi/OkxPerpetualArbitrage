using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Helpers
{
    public static class DateTimeHelper
    {
        private static DateTime _epochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        public static DateTime GetDateFromUnix(long timestamp)
        {
            var dtDateTime = _epochTime.AddMilliseconds(timestamp);
            return dtDateTime;
        }
        public static long GetMillisecondsFromEpochStart(DateTime time)
        {
            return (long)(time - _epochTime).TotalMilliseconds;
        }

        public static TimeSpan TimeToFunding()
        {
            DateTime fundingTime;
            var currentTime = DateTime.UtcNow;
            if (currentTime.Hour >= 16)
                fundingTime = new DateTime(currentTime.AddDays(1).Year, currentTime.AddDays(1).Month, currentTime.AddDays(1).Day, 0, 0, 0);
            else
                if (currentTime.Hour >= 8)
                fundingTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 16, 0, 0);
            else
                fundingTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 8, 0, 0);

            var time = fundingTime - currentTime;
            return time;
        }
    }
}

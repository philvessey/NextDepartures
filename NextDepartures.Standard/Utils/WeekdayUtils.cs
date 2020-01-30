using NextDepartures.Standard.Models;
using System;
using System.Collections.Generic;

namespace NextDepartures.Standard.Utils
{
    public static class WeekdayUtils
    {
        private static readonly Dictionary<DayOfWeek, Func<Departure, bool>> _previousDays = new Dictionary<DayOfWeek, Func<Departure, bool>>()
        {
            { DayOfWeek.Monday, d => d.Sunday },
            { DayOfWeek.Tuesday, d => d.Monday },
            { DayOfWeek.Wednesday, d => d.Tuesday },
            { DayOfWeek.Thursday, d => d.Wednesday },
            { DayOfWeek.Friday, d => d.Thursday },
            { DayOfWeek.Saturday, d => d.Friday },
            { DayOfWeek.Sunday, d => d.Saturday },
        };

        private static readonly Dictionary<DayOfWeek, Func<Departure, bool>> _todayDays = new Dictionary<DayOfWeek, Func<Departure, bool>>()
        {
            { DayOfWeek.Monday, d => d.Monday },
            { DayOfWeek.Tuesday, d => d.Tuesday },
            { DayOfWeek.Wednesday, d => d.Wednesday },
            { DayOfWeek.Thursday, d => d.Thursday },
            { DayOfWeek.Friday, d => d.Friday },
            { DayOfWeek.Saturday, d => d.Saturday },
            { DayOfWeek.Sunday, d => d.Sunday },
        };

        private static readonly Dictionary<DayOfWeek, Func<Departure, bool>> _followingDays = new Dictionary<DayOfWeek, Func<Departure, bool>>()
        {
            { DayOfWeek.Monday, d => d.Tuesday },
            { DayOfWeek.Tuesday, d => d.Wednesday },
            { DayOfWeek.Wednesday, d => d.Thursday },
            { DayOfWeek.Thursday, d => d.Friday },
            { DayOfWeek.Friday, d => d.Saturday },
            { DayOfWeek.Saturday, d => d.Sunday },
            { DayOfWeek.Sunday, d => d.Monday },
        };

        public static bool GetPreviousDay(DayOfWeek dayOfWeek, Departure departure)
        {
            return _previousDays[dayOfWeek](departure);
        }

        public static bool GetTodayDay(DayOfWeek dayOfWeek, Departure departure)
        {
            return _todayDays[dayOfWeek](departure);
        }

        public static bool GetFollowingDay(DayOfWeek dayOfWeek, Departure departure)
        {
            return _followingDays[dayOfWeek](departure);
        }

        public static Func<DayOfWeek, Departure, bool> GetUtilByDayType(DayOffsetType dayOffsetType)
        {
            if (dayOffsetType == DayOffsetType.Yesterday)
            {
                return GetPreviousDay;
            }
            else if (dayOffsetType == DayOffsetType.Today)
            {
                return GetTodayDay;
            }
            else
            {
                return GetFollowingDay;
            }
        }
    }
}
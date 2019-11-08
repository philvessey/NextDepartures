﻿using NextDepartures.Standard.Models;
using System;
using System.Collections.Generic;

namespace NextDepartures.Standard.Utils
{
    public static class WeekdayUtils
    {
        private static readonly Dictionary<DayOfWeek, Func<Departure, string>> _previousDays = new Dictionary<DayOfWeek, Func<Departure, string>>()
        {
            { DayOfWeek.Monday, d => d.Sunday },
            { DayOfWeek.Tuesday, d => d.Monday },
            { DayOfWeek.Wednesday, d => d.Tuesday },
            { DayOfWeek.Thursday, d => d.Wednesday },
            { DayOfWeek.Friday, d => d.Thursday },
            { DayOfWeek.Saturday, d => d.Friday },
            { DayOfWeek.Sunday, d => d.Saturday },
        };

        private static readonly Dictionary<DayOfWeek, Func<Departure, string>> _todayDays = new Dictionary<DayOfWeek, Func<Departure, string>>()
        {
            { DayOfWeek.Monday, d => d.Monday },
            { DayOfWeek.Tuesday, d => d.Tuesday },
            { DayOfWeek.Wednesday, d => d.Wednesday },
            { DayOfWeek.Thursday, d => d.Thursday },
            { DayOfWeek.Friday, d => d.Friday },
            { DayOfWeek.Saturday, d => d.Saturday },
            { DayOfWeek.Sunday, d => d.Sunday },
        };

        private static readonly Dictionary<DayOfWeek, Func<Departure, string>> _followingDays = new Dictionary<DayOfWeek, Func<Departure, string>>()
        {
            { DayOfWeek.Monday, d => d.Tuesday },
            { DayOfWeek.Tuesday, d => d.Wednesday },
            { DayOfWeek.Wednesday, d => d.Thursday },
            { DayOfWeek.Thursday, d => d.Friday },
            { DayOfWeek.Friday, d => d.Saturday },
            { DayOfWeek.Saturday, d => d.Sunday },
            { DayOfWeek.Sunday, d => d.Monday },
        };

        public static string GetPreviousDay(DayOfWeek dayOfWeek, Departure departure)
        {
            return _previousDays[dayOfWeek](departure);
        }

        public static string GetTodayDay(DayOfWeek dayOfWeek, Departure departure)
        {
            return _todayDays[dayOfWeek](departure);
        }

        public static string GetFollowingDay(DayOfWeek dayOfWeek, Departure departure)
        {
            return _followingDays[dayOfWeek](departure);
        }
    }
}

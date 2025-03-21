﻿using System;
using System.Collections.Generic;
using NextDepartures.Standard.Models;
using NextDepartures.Standard.Types;

namespace NextDepartures.Standard.Utils;

public static class WeekdayUtils
{
    private static readonly Dictionary<DayOfWeek, Func<Departure, bool>> PreviousDays = new()
    {
        { DayOfWeek.Monday, d => d.Sunday },
        { DayOfWeek.Tuesday, d => d.Monday },
        { DayOfWeek.Wednesday, d => d.Tuesday },
        { DayOfWeek.Thursday, d => d.Wednesday },
        { DayOfWeek.Friday, d => d.Thursday },
        { DayOfWeek.Saturday, d => d.Friday },
        { DayOfWeek.Sunday, d => d.Saturday }
    };
    
    private static readonly Dictionary<DayOfWeek, Func<Departure, bool>> TodayDays = new()
    {
        { DayOfWeek.Monday, d => d.Monday },
        { DayOfWeek.Tuesday, d => d.Tuesday },
        { DayOfWeek.Wednesday, d => d.Wednesday },
        { DayOfWeek.Thursday, d => d.Thursday },
        { DayOfWeek.Friday, d => d.Friday },
        { DayOfWeek.Saturday, d => d.Saturday },
        { DayOfWeek.Sunday, d => d.Sunday }
    };
    
    private static readonly Dictionary<DayOfWeek, Func<Departure, bool>> FollowingDays = new()
    {
        { DayOfWeek.Monday, d => d.Tuesday },
        { DayOfWeek.Tuesday, d => d.Wednesday },
        { DayOfWeek.Wednesday, d => d.Thursday },
        { DayOfWeek.Thursday, d => d.Friday },
        { DayOfWeek.Friday, d => d.Saturday },
        { DayOfWeek.Saturday, d => d.Sunday },
        { DayOfWeek.Sunday, d => d.Monday }
    };
    
    public static Func<DayOfWeek, Departure, bool> GetFromOffset(DayOffsetType dayOffsetType)
    {
        return dayOffsetType switch
        {
            DayOffsetType.Yesterday => GetPreviousDay,
            DayOffsetType.Today => GetTodayDay,
            DayOffsetType.Tomorrow => GetFollowingDay,
            _ => GetTodayDay
        };
    }
    
    private static bool GetPreviousDay(
        DayOfWeek dayOfWeek,
        Departure departure) {
        
        return PreviousDays[dayOfWeek](departure);
    }
    
    private static bool GetTodayDay(
        DayOfWeek dayOfWeek,
        Departure departure) {
        
        return TodayDays[dayOfWeek](departure);
    }
    
    private static bool GetFollowingDay(
        DayOfWeek dayOfWeek,
        Departure departure) {
        
        return FollowingDays[dayOfWeek](departure);
    }
}
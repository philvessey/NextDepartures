﻿using System;

namespace NextDepartures.Standard.Extensions
{
    public static class DateTimeExtensions
    {
        public static int AsInteger(this DateTime dateTime)
        {
            return int.Parse(string.Format("{0}{1}{2}", dateTime.Year, dateTime.Month.ToString("00"), dateTime.Day.ToString("00")));
        }
    }
}
using System;

namespace NextDepartures.Storage.GTFS.Extensions;

public static class DoubleExtensions
{
    public static double ToRadians(this double baseDouble)
    {
        return baseDouble * (Math.PI / 180.0);
    }
}
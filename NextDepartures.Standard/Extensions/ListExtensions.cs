using System.Collections.Generic;
using System.Linq;

namespace NextDepartures.Standard.Extensions;

public static class ListExtensions
{
    public static void AddIfNotNull<T>(this List<T> list, T item) where T : class
    {
        if (item != null)
        {
            list.Add(item);
        }
    }

    public static IEnumerable<T> AddMultiple<T>(this IEnumerable<T> enumerable, IEnumerable<T> items) where T : class
    {
        return enumerable.Concat(items);
    }
}
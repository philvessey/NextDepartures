using System.Collections.Generic;
using System.Linq;

namespace NextDepartures.Standard.Extensions;

public static class ListExtensions
{
    public static void AddIfNotNull<T>(
        this List<T> baseList,
        T item) where T : class {
        
        if (item is not null)
            baseList.Add(item: item);
    }
    
    public static IEnumerable<T> AddMultiple<T>(
        this IEnumerable<T> baseEnumerable,
        IEnumerable<T> items) where T : class {
        
        return baseEnumerable.Concat(second: items);
    }
}
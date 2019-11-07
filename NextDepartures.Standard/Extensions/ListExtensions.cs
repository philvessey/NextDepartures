using System;
using System.Collections.Generic;

namespace NextDepartures.Standard.Extensions
{
    public static class ListExtensions
    {
        public static void AddIfNotNull<T>(this List<T> list, T item) where T : Object
        {
            if (item != null)
            {
                list.Add(item);
            }
        }
    }
}

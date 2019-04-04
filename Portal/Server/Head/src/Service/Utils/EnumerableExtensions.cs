using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Service
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToEmptyIfNull<T>(this IEnumerable<T> items)
        {
            if (items == null)
                return new T[0];

            return items;
        }
    }
}

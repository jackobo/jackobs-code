using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Service
{
    public static class ArrayUtils
    {
        public static T[] ToEmptyArrayIfNull<T>(this T[] items)
        {
            if (items == null)
                return new T[0];

            return items;
        }
    }
}

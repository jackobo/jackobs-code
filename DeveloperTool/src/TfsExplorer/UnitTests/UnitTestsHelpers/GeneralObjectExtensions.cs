using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.UnitTesting.Helpers
{
    public static class GeneralObjectExtensions
    {
        public static T[] WrapInArray<T>(this T obj)
        {
            if (obj == null)
                return new T[0];

            return new T[] { obj };
        }

        public static T[] MakeArray<T>(params T[] items)
        {
            return items;
        }
            

    }
}

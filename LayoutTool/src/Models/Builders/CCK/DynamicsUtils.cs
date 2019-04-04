using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Newtonsoft.Json.Linq;

namespace LayoutTool.Models.Builders.CCK
{
    public static class DynamicsUtils
    {
        public static T ConvertDynamicValue<T>(dynamic obj)
        {
            return ConvertDynamicValue<T>(obj, default(T));
        }

        public static T ConvertDynamicValue<T>(dynamic value, T defaultIfMissing)
        {
            if (value == null)
                return defaultIfMissing;

            var converter = TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromInvariantString((string)value);
        }


        
       
    }
}

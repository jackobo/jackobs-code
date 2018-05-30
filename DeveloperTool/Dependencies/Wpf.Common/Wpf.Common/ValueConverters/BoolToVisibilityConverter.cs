using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Spark.Wpf.Common
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = (bool)value;

            if (isVisible)
                return Visibility.Visible;

            

            if (parameter == null)
                return Visibility.Collapsed;

            return (Visibility)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility)value;

            return visibility == Visibility.Visible;
        }
    }

    public class InvertedBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = (bool)value;

            if (!isVisible)
                return Visibility.Visible;
            
            if (parameter == null)
                return Visibility.Collapsed;

            return (Visibility)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility)value;

            return visibility != Visibility.Visible;
        }
    }
}

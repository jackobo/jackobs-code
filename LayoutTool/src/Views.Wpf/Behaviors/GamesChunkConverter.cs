using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LayoutTool.Views.Wpf
{
    public class GamesChunkConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dataGridRow = (value as DataGridRow);
            if (dataGridRow == null)
                return false;

            var rowIndex = dataGridRow.GetIndex();

            if (rowIndex == 0)
                return false;

            return (rowIndex % 9) == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

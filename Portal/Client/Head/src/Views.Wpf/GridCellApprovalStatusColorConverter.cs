using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using GamesPortal.Client.Interfaces;

namespace GamesPortal.Client.Views.Wpf
{
    public class GridCellApprovalStatusColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var cell = values.FirstOrDefault(v => v is DataGridCell) as DataGridCell;
            if (cell == null)
            {
                return GetDefaultBrush(values);
            }

            var gridItem = values.FirstOrDefault(v => v is ViewModels.Workspace.GameVersionsWorkspaceItem.GridItem);
            if (gridItem == null)
                return GetDefaultBrush(values);

            var boundProperty = ((cell?.Column as DataGridBoundColumn)?.Binding as Binding)?.Path?.Path;
            if (string.IsNullOrEmpty(boundProperty))
                return GetDefaultBrush(values);

            var propertyDescriptor = TypeDescriptor.GetProperties(gridItem).Cast<PropertyDescriptor>().FirstOrDefault(pd => pd.Name == boundProperty);
            if (propertyDescriptor == null)
                return GetDefaultBrush(values);

            string value = propertyDescriptor.GetValue(gridItem)?.ToString();
            if (value == ApprovalStatusesDescriptions.QAApproved || value == ApprovalStatusesDescriptions.PMApproved)
                return Brushes.Brown;

            if (value == ApprovalStatusesDescriptions.ApprovedForProduction)
                return Brushes.Purple;

            if (value == ApprovalStatusesDescriptions.Production)
                return Brushes.DarkGreen;

            return GetDefaultBrush(values);
            
        }


        private object GetDefaultBrush(object[] values)
        {
            var b = values.FirstOrDefault(v => v is Brush);
            if (b != null)
                return b;

            return Brushes.Black;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

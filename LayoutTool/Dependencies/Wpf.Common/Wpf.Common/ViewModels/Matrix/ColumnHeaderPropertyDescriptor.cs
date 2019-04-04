using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels.Matrix
{
    internal class ColumnHeaderPropertyDescriptor<TItem> : MatrixRowPropertyDescriptor<TItem>
    {
        Func<IEnumerable<TItem>, object> _aggregate;
        Func<TItem, object> _columnHeaderSelector;
        object _value;
        public ColumnHeaderPropertyDescriptor(object value, Func<TItem, object> columnHeaderSelector, Func<IEnumerable<TItem>, object> aggregate)
            : base(value?.ToString() ?? "NULL")
        {
            _value = value;
            _columnHeaderSelector = columnHeaderSelector;
            _aggregate = aggregate;
        }

        public override object GetValue(object component)
        {
            var matrixRow = component as MatrixRow<TItem>;
            if (matrixRow == null)
                return null;
            
            return _aggregate(FilterItems(matrixRow));
        }

        private IEnumerable<TItem> FilterItems(MatrixRow<TItem> matrixRow)
        {
            return matrixRow.GetAggregatedItems().Where(item => AreEqual(_value, _columnHeaderSelector(item)));
        }

        private bool AreEqual(object val1, object val2)
        {
            if(object.ReferenceEquals(val1, null) 
                && object.ReferenceEquals(val2, null))
            {
                return true;
            }

            return val1?.Equals(val2) ?? false;
        }
    }
}

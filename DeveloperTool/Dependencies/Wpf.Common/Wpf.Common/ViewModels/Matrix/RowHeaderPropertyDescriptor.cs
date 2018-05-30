using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels.Matrix
{
    internal class RowHeaderPropertyDescriptor<TItem> : MatrixRowPropertyDescriptor<TItem>
    {
        RowHeader<TItem> _rowHeader;
        public RowHeaderPropertyDescriptor(RowHeader<TItem> rowHeader)
            : base(rowHeader.Caption)
        {
            _rowHeader = rowHeader;
        }
      
        public override object GetValue(object component)
        {
            var matrixRow = component as MatrixRow<TItem>;
            if (matrixRow == null)
                return null;

            return _rowHeader.Getter(matrixRow.GetAggregatedItems().First());
        }
        
    }

}

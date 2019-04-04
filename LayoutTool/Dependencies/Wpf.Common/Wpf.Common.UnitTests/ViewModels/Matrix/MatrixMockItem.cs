using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels.Matrix
{
    public class MatrixMockItem
    {
        public MatrixMockItem(string rowHeader1, string rowHeader2, string columnHeader, int value)
        {
            this.RowHeader1 = rowHeader1;
            this.RowHeader2 = rowHeader2;
            this.ColumnHeader = columnHeader;
            this.Value = value;

        }
        public string RowHeader1 { get; }
        public string RowHeader2 { get; }

        public string ColumnHeader { get; }

        public int Value { get; }
    }
}

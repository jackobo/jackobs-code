using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels.Matrix
{
    internal class RowHeader<TItem>
    {
        public RowHeader(string caption, Func<TItem, object> getter, bool addAfterColumnHeaders)
        {
            this.Getter = getter;
            this.Caption = caption;
            this.AddAfterColumnHeaders = addAfterColumnHeaders;
        }

        public Func<TItem, object> Getter { get; }
        public string Caption { get; }

        public bool AddAfterColumnHeaders { get; }
    }
}

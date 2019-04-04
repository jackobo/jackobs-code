using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels.Matrix
{
    internal class MatrixRowKey : List<object>, IComparable<MatrixRowKey>, IComparable
    {
        public MatrixRowKey()
        {

        }

       
        public override bool Equals(object obj)
        {
            var theOther = obj as MatrixRowKey;
            if (theOther == null)
                return false;

            if (this.Count != theOther.Count)
                return false;

            return this.Zip(theOther, (x, y) => AreEquals(x, y))
                .All(result => result == true);

        }

        public override int GetHashCode()
        {
            return this.Aggregate(0, (acumulatedHashCode, item) => acumulatedHashCode ^ (item?.GetHashCode() ?? 0));
        }


        private bool AreEquals(object x, object y)
        {
            if (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null))
                return true;

            return x?.Equals(y) ?? false;
        }

        int IComparable.CompareTo(object obj)
        {
            return CompareTo(obj as MatrixRowKey);
        }

        public int CompareTo(MatrixRowKey other)
        {
            if (other == null)
                return 1;

            for(int i = 0; i < this.Count; i++)
            {
                var result = ((IComparable)this[i]).CompareTo(other[i]);
                if (result != 0)
                    return result;
            }

            return 0;
        }
    }
}

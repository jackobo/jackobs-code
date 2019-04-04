using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels.Matrix
{

    public interface IFirstRowHeaderHolder<TItem>
    {
        IOptionalRowHeaderHolder<TItem> AddRowHeader<TPropertyType>(string caption, 
                                                                    Func<TItem, TPropertyType> propertySelector,
                                                                    bool addAfterColumnHeaders = false) where TPropertyType : IComparable;
    }


    public interface IOptionalRowHeaderHolder<TItem>
    {
        IOptionalRowHeaderHolder<TItem> AddRowHeader<TPropertyType>(string caption, Func<TItem, TPropertyType> propertySelector, bool addAfterColumnHeaders = false) where TPropertyType : IComparable;
        IColumnHeaderHolder<TItem> NoMoreRowHeaders();
    }

    public interface IColumnHeaderHolder<TItem>
    {
        IMatrixRowsAggregatorHolder<TItem> AddColumnHeader<TPropertyType>(Func<TItem, TPropertyType> propertySelector)
            where TPropertyType : IComparable;

        IMatrixBuilder<TItem> NoColumnHeaders();
    }


    public interface IMatrixRowsAggregatorHolder<TItem>
    {
        IMatrixBuilder<TItem> AggregateBy<TResult>(Func<IEnumerable<TItem>, TResult> aggregator);
    }
    
    public interface IMatrixBuilder<TItem>
    {
        MatrixViewModel<TItem> Build();

    }



    public class MatrixBuilder<TItem> : IFirstRowHeaderHolder<TItem>,
                                        IOptionalRowHeaderHolder<TItem>,
                                        IColumnHeaderHolder<TItem>,
                                        IMatrixRowsAggregatorHolder<TItem>,
                                        IMatrixBuilder<TItem>
    {
        MatrixViewModel<TItem> _matrix;
        private MatrixBuilder()
        {
            _matrix = new MatrixViewModel<TItem>();
        }
        
        public static IFirstRowHeaderHolder<TItem> Matrix()
        {
            return new MatrixBuilder<TItem>();
        }

        IOptionalRowHeaderHolder<TItem> IFirstRowHeaderHolder<TItem>.AddRowHeader<TPropertyType>(string caption, Func<TItem, TPropertyType> propertySelector, bool addAfterColumnHeaders)
        {
            _matrix.AddRowHeader(caption, propertySelector, addAfterColumnHeaders);
            return this;
        }

        IOptionalRowHeaderHolder<TItem> IOptionalRowHeaderHolder<TItem>.AddRowHeader<TPropertyType>(string caption, Func<TItem, TPropertyType> propertySelector, bool addAfterColumnHeaders)

        {
            _matrix.AddRowHeader(caption, propertySelector, addAfterColumnHeaders);
            return this;
        }
                
        IColumnHeaderHolder<TItem> IOptionalRowHeaderHolder<TItem>.NoMoreRowHeaders()
        {
            return this;
        }
        IMatrixRowsAggregatorHolder<TItem> IColumnHeaderHolder<TItem>.AddColumnHeader<TPropertyType>(Func<TItem, TPropertyType> propertySelector)
        {
            _matrix.SetColumnHeader(propertySelector);
            return this;
        }

        IMatrixBuilder<TItem> IColumnHeaderHolder<TItem>.NoColumnHeaders()
        {
            return this;
        }

        IMatrixBuilder<TItem> IMatrixRowsAggregatorHolder<TItem>.AggregateBy<TResult>(Func<IEnumerable<TItem>, TResult> aggregator)
        {
            _matrix.SetValuesAggregator(aggregator);
            return this;
        }

        MatrixViewModel<TItem> IMatrixBuilder<TItem>.Build()
        {
            return _matrix;
        }

       
    }
}

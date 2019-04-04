using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels.Matrix
{
    public class MatrixViewModel<TItem> : ObservableCollection<MatrixRow<TItem>>, ITypedList
    {
        private IEnumerable<TItem> OriginalItems { get; set; }

        internal MatrixViewModel()
        {
        }

        
        List<RowHeader<TItem>> _rowHeaders = new List<RowHeader<TItem>>();

        public void AddRowHeader<TPropertyType>(string caption, Func<TItem, TPropertyType> propertySelector, bool addAfterColumnHeaders)
            where TPropertyType : IComparable
        {
            _rowHeaders.Add(new RowHeader<TItem>(caption, (item) => propertySelector(item), addAfterColumnHeaders));
        }


        Func<TItem, object> _columnHeaderSelector;
        public void SetColumnHeader<TPropertyType>(Func<TItem, TPropertyType> propertySelector)
            where TPropertyType : IComparable
        {
            _columnHeaderSelector = (item) => propertySelector(item);
        }

        Func<IEnumerable<TItem>, object> _aggregator;

        public void SetValuesAggregator<TResult>(Func<IEnumerable<TItem>, TResult> aggregator)
        {
            _aggregator = (items) => aggregator(items);
        }

        PropertyDescriptorCollection _currentPropertyDescriptors;
        public void Load(IEnumerable<TItem> items)
        {
            var propertyDescriptors = BuildPropertyDescriptors(items);

            var newItems = items.GroupBy(item => BuildRowKey(item))
                           .Select(group => new MatrixRow<TItem>(group.Key, group.ToList(), propertyDescriptors))
                           .OrderBy(row => row);


            _currentPropertyDescriptors = propertyDescriptors;
            this.Clear();

            this.AddRange(newItems);

            this.OriginalItems = items;
        }

        private PropertyDescriptorCollection BuildPropertyDescriptors(IEnumerable<TItem> items)
        {
            var list = new List<PropertyDescriptor>();

            list.AddRange(BuildRowHeaderPropertiesDescriptors(_rowHeaders.Where(r => !r.AddAfterColumnHeaders)));
            list.AddRange(BuildColumnHeadersPropertiesDescriptors(items));
            list.AddRange(BuildRowHeaderPropertiesDescriptors(_rowHeaders.Where(r => r.AddAfterColumnHeaders)));

            return new PropertyDescriptorCollection(list.ToArray());
        }

        private IEnumerable<PropertyDescriptor> BuildRowHeaderPropertiesDescriptors(IEnumerable<RowHeader<TItem>> rowHeaders)
        {
            var list = new List<PropertyDescriptor>();
            foreach (var rowHeader in rowHeaders)
                list.Add(new RowHeaderPropertyDescriptor<TItem>(rowHeader));


            return list;
        }


        private IEnumerable<PropertyDescriptor> BuildColumnHeadersPropertiesDescriptors(IEnumerable<TItem> items)
        {
            var list = new List<PropertyDescriptor>();
            if (_columnHeaderSelector == null)
                return list;

            foreach(var distinctValue in items.Select(item => _columnHeaderSelector(item))
                                              .Distinct()
                                              .OrderBy(val => val))
            {
                list.Add(new ColumnHeaderPropertyDescriptor<TItem>(distinctValue, _columnHeaderSelector, _aggregator));
            }

            return list;
        }

        private MatrixRowKey BuildRowKey(TItem item)
        {
            return _rowHeaders.Aggregate(new MatrixRowKey(), (rowKey, rowHeader) =>
            {
                rowKey.Add(rowHeader.Getter(item));
                return rowKey;
            });
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return string.Empty;
        }

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return _currentPropertyDescriptors;
        }
    }


  



}

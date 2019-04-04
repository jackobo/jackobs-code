using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels.Matrix
{
    public class MatrixRow<TItem> : ICustomTypeDescriptor, IComparable<MatrixRow<TItem>>, IComparable
    {
        internal MatrixRow(MatrixRowKey key, IEnumerable<TItem> aggregatedItems, PropertyDescriptorCollection propertyDescriptors )
        {
            _aggregatedItems = aggregatedItems;
            _propertyDescriptors = propertyDescriptors;
            Key = key;
        }


        MatrixRowKey Key { get; set; }


        IEnumerable<TItem> _aggregatedItems;
        PropertyDescriptorCollection _propertyDescriptors;
        
        public IEnumerable<TItem> GetAggregatedItems()
        {
            return _aggregatedItems;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return new AttributeCollection();
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return new EventDescriptorCollection(new EventDescriptor[0]);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(new EventDescriptor[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return _propertyDescriptors;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return _propertyDescriptors;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public int CompareTo(MatrixRow<TItem> other)
        {
            if (other == null)
                return 1;

            return this.Key.CompareTo(other.Key);
        }

        int IComparable.CompareTo(object obj)
        {
            return this.CompareTo(obj as MatrixRow<TItem>);
        }
    }
}

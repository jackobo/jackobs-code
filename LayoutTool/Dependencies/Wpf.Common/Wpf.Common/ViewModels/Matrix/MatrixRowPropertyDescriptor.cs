using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels.Matrix
{
    internal abstract class MatrixRowPropertyDescriptor<TItem> : PropertyDescriptor
    {
        public MatrixRowPropertyDescriptor(string name)
            : base(name, new Attribute[0])
        {

        }

        public override Type ComponentType
        {
            get
            {
                return typeof(MatrixRow<TItem>);
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return typeof(object);
            }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }
        
        public override void ResetValue(object component)
        {
            
        }

        public override void SetValue(object component, object value)
        {
            
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}

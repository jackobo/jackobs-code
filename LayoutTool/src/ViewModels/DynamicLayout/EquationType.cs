using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace LayoutTool.ViewModels.DynamicLayout
{
    public abstract class EquationType : SmartEnum<string, EquationType>
    {
        public EquationType(string id, string name)
            : base(id, name)
        {

        }


        public static readonly EquationType Equal = new EqualEquationType();
        public static readonly EquationType LessThan = new LessThanEquationType();
        public static readonly EquationType GreaterThan = new GreaterThanEquationType();
        public static readonly EquationType GreaterThanOrEqual = new GreaterThanOrEqualEquationType();

        public abstract T GetPositiveTestValue<T>(T relativeValue, Func<T, T> decrement, Func<T, T> increment);
        public abstract T GetNegativeTestValue<T>(T relativeValue, Func<T, T> decrement, Func<T, T> increment);


    }


    internal class EqualEquationType : EquationType
    {
        public EqualEquationType()
            : base("equal", "Equal")
        {

        }

        public override T GetNegativeTestValue<T>(T relativeValue, Func<T, T> decrement, Func<T, T> increment)
        {
            return increment(relativeValue);
        }

        public override T GetPositiveTestValue<T>(T relativeValue, Func<T, T> decrement, Func<T, T> increment)
        {
            return relativeValue;
        }
    }

    internal class LessThanEquationType : EquationType
    {
        public LessThanEquationType()
            : base("lessThen", "Less than")
        {

        }

        public override T GetNegativeTestValue<T>(T relativeValue, Func<T, T> decrement, Func<T, T> increment)
        {
            return increment(relativeValue);
        }

        public override T GetPositiveTestValue<T>(T relativeValue, Func<T, T> decrement, Func<T, T> increment)
        {
            return decrement(relativeValue);
        }
    }

    internal class GreaterThanEquationType : EquationType
    {
        public GreaterThanEquationType()
            : base("greaterThen", "Greater than")
        {

        }

        public override T GetNegativeTestValue<T>(T relativeValue, Func<T, T> decrement, Func<T, T> increment)
        {
            return decrement(relativeValue);
        }

        public override T GetPositiveTestValue<T>(T relativeValue, Func<T, T> decrement, Func<T, T> increment)
        {
            return increment(relativeValue);
        }
    }

    internal class GreaterThanOrEqualEquationType : EquationType
    {
        public GreaterThanOrEqualEquationType()
            : base("greaterThenOrEqual", "Greater than or equal")
        {

        }

        public override T GetNegativeTestValue<T>(T relativeValue, Func<T, T> decrement, Func<T, T> increment)
        {
            return decrement(relativeValue);
        }

        public override T GetPositiveTestValue<T>(T relativeValue, Func<T, T> decrement, Func<T, T> increment)
        {
            return increment(relativeValue);
        }
    }
}

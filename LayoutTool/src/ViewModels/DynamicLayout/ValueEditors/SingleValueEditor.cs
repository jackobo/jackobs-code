using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout.ValueEditors
{
    public abstract class SingleValueEditor<TValue> : ViewModelBase, IConditionValueEditor 
        where TValue : struct
    {
        public SingleValueEditor()
        {

        }

        public SingleValueEditor(TValue value)
        {
            _value = value;
        }

        TValue? _value;
        public TValue? Value
        {
            get
            {
                return _value;
            }

            set
            {
                SetProperty(ref _value, value);
            }
        }


        protected abstract TValue Incrementer(TValue value);
        protected abstract TValue Decrementer(TValue value);
        public abstract IConditionValueEditor Clone();
        protected abstract string FormatValue(TValue value);

        public override string ToString()
        {
            if (_value == null)
                return string.Empty;

            return FormatValue(Value.Value);
        }

        

        public virtual object[] GetPositiveTestValues(EquationType equationType)
        {
            if (Value == null)
                return new object[0];

            return new object[] { equationType.GetPositiveTestValue(Value.Value, Incrementer, Decrementer) };
        }

        public virtual object GetNegativeTestValue(EquationType equationType, IEnumerable<object> positiveValues)
        {
            if (Value == null)
                return null;

            int currentTry = 0;

            TValue negativeValue = Value.Value;
            do
            {
                negativeValue = equationType.GetNegativeTestValue(negativeValue, Incrementer, Decrementer);
                currentTry++;
            }
            while (positiveValues.Contains(negativeValue) && currentTry < 1000);

            return negativeValue;

        }

        public virtual ConditionValue[] GetValues()
        {
            if (_value == null)
                return new ConditionValue[0];

            return new ConditionValue[]
            {
                new ConditionValue(FormatValue(_value.Value))
            };
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout.ValueEditors
{
    public class BooleanValueEditor : ViewModelBase, IConditionValueEditor
    {
        public BooleanValueEditor()
        {

        }

        public BooleanValueEditor(bool value)
        {
            _value = value;
        }

        private bool? _value;

        public bool? Value
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

        public ConditionValue[] GetValues()
        {
            if (_value == null)
                return new ConditionValue[0];

            return new ConditionValue[]
            {
                new ConditionValue(Value.ToString().ToLowerInvariant())
            };
        }

        public static BooleanValueEditor Parse(string val, BooleanValueEditor defaultValue)
        {
            if (string.IsNullOrEmpty(val))
                return defaultValue;


            return new BooleanValueEditor(bool.Parse(val));
        }

        public override string ToString()
        {
            return _value?.ToString();
        }

        public IConditionValueEditor Clone()
        {
            return new BooleanValueEditor() { Value = _value };
        }

        public object[] GetPositiveTestValues(EquationType equationType)
        {
            if (Value == null)
                return new object[0];
                
            return new object[] { Value.Value };
        }

        public object GetNegativeTestValue(EquationType equationType, IEnumerable<object> positiveValues)
        {
            if (Value == null)
                return null;

            return !Value.Value;
        }
    }
}

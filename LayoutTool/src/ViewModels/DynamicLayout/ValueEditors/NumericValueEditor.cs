using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout.ValueEditors
{
    public class NumericValueEditor : SingleValueEditor<int>
    {
        public NumericValueEditor()
        {

        }

        public NumericValueEditor(int value)
            : base(value)
        {
            
        }


        protected override string FormatValue(int value)
        {
            return value.ToString();
        }

        public static NumericValueEditor Parse(string val, NumericValueEditor defaultValue)
        {
            if (string.IsNullOrEmpty(val))
                return defaultValue;

            return new NumericValueEditor(int.Parse(val));
        }

   
        public override IConditionValueEditor Clone()
        {
            return new NumericValueEditor() { Value = this.Value };
        }

        protected override int Decrementer(int value)
        {
            return value - 1;
        }

        protected override int Incrementer(int value)
        {
            return value + 1;
        }
    }
}

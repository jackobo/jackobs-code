using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels.DynamicLayout
{
    public class TestValue
    {
        public TestValue(ConditionField field, object value)
        {
            Field = field;
            Value = value;
        }

        public ConditionField Field { get; private set; }

        public object Value { get; private set; }

        public void ApplyValue(IPlayerAttributesSimulatorViewModel playerAttributes)
        {
            Field.ApplyValue(playerAttributes, Value);
        }
    }
}

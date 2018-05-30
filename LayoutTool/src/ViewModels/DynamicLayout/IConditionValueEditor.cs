using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels.DynamicLayout
{
    public interface IConditionValueEditor : INotifyPropertyChanged
    {
        Interfaces.Entities.ConditionValue[] GetValues();
        IConditionValueEditor Clone();
        object[] GetPositiveTestValues(EquationType equationType);
        object GetNegativeTestValue(EquationType equationType, IEnumerable<object> positiveValues);
    }
}

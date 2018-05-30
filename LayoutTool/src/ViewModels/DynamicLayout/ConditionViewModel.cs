using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout
{
    public class ConditionViewModel : ViewModelBase
    {
        public ConditionViewModel(ConditionField field, EquationType equationType, IConditionValueEditor valueEditor)
        {
            Field = field;
            EquationType = equationType;
            ValueEditor = valueEditor;
            ValueEditor.PropertyChanged += ValueEditor_PropertyChanged;
        }

        private void ValueEditor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ValueEditor));
        }

        public ConditionField Field { get; private set; }

        public EquationType[] AvailableEquations
        {
            get { return Field.GetAvailableEquations(); }
        }

        private EquationType _equationType;
        public EquationType EquationType
        {
            get
            {
                return _equationType;
            }
            set
            {
                SetProperty(ref _equationType, value);
            }
        }

        public IConditionValueEditor ValueEditor { get; private set; }


        public override string ToString()
        {
            return $"{Field?.Name} {EquationType?.Name} {ValueEditor?.ToString()}";
        }

        public TestValue[] GetPositiveTestValues()
        {
            var values = ValueEditor.GetPositiveTestValues(EquationType);
            if(values == null)
            {
                return new TestValue[0];
            }
            
            return values.Select(v => new TestValue(Field, v)).ToArray();
        }

        public TestValue GetNegativeTestValue(IEnumerable<TestValue> positiveValues)
        {
            var value = ValueEditor.GetNegativeTestValue(EquationType, positiveValues.Select(v => v.Value));

            if (value == null)
            {
                return null;
            }

            return new TestValue(Field, value);
        }
    }

    

    public class ConditionViewModelCollection : ObservableCollectionExtended<ConditionViewModel>
    {

    }

    
}

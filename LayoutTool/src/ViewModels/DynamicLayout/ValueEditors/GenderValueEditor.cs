using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout.ValueEditors
{
    public class GenderValueEditor : ViewModelBase, IConditionValueEditor
    {

        public ConditionValue[] GetValues()
        {
            if (_selectedGender == null)
                return new ConditionValue[0];

            return new ConditionValue[] { new ConditionValue(_selectedGender.Id) };
        }


        private Gender _selectedGender;

        public Gender SelectedGender
        {
            get
            {
                return _selectedGender;
            }

            set
            {
                SetProperty(ref _selectedGender, value);
            }
        }

        public Gender[] Genders
        {
            get { return Gender.All; }
        }

        public override string ToString()
        {
            return SelectedGender?.ToString();
        }

        public IConditionValueEditor Clone()
        {
            return new GenderValueEditor() { SelectedGender = _selectedGender };
        }

        public object[] GetPositiveTestValues(EquationType equationType)
        {
            if (SelectedGender == null)
                return new object[0];
            return new object[] { SelectedGender };
        }

        public object GetNegativeTestValue(EquationType equationType, IEnumerable<object> positiveValues)
        {
            if (SelectedGender == null)
                return null;

            return Gender.All.FirstOrDefault(g => g.Id != SelectedGender.Id);
        }
    }

  
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout.ValueEditors
{
    public class AddNewConditionDialog : ViewModelBase, IOkCancelDialogBoxViewModel
    {
        public AddNewConditionDialog(TriggerViewModel trigger, IServiceLocator serviceLocator)
        {
            _trigger = trigger;
            _serviceLocator = serviceLocator;
        }

        TriggerViewModel _trigger;
        IServiceLocator _serviceLocator;
        

        public void ExecuteCancel()
        {
            
        }

        public void ExecuteOk()
        {
            _trigger.Conditions.Add(new ConditionViewModel(SelectedField, SelectedEquation, Value));
        }


        public ConditionField[] Fields
        {
            get
            {
                return ConditionField.All.Where(f => f.AllowMultipleInstances || !_trigger.Conditions.Select(c => c.Field).Contains(f))
                                           .OrderBy(f => f.Name)
                                           .ToArray();
            }
        }

        private ConditionField _selectedField;
        public ConditionField SelectedField
        {
            get
            {
                return _selectedField;
            }

            set
            {
                if(SetProperty(ref _selectedField , value))
                {
                    OnPropertyChanged(nameof(Equations));

                    this.SelectedEquation = Equations.FirstOrDefault();

                    if(_selectedField != null)
                    {
                        Value = _selectedField.GetDefaultValueEditor(_serviceLocator);
                    }
                    else
                    {
                        Value = null;
                    }
                    
                }
            }
        }


        public EquationType[] Equations
        {
            get
            {
                if (this.SelectedField == null)
                    return new EquationType[0];

                return this.SelectedField.GetAvailableEquations();
            }
        }

        private EquationType _selectedEquation;
        public EquationType SelectedEquation
        {
            get
            {
                return _selectedEquation;
            }

            set
            {
                SetProperty(ref _selectedEquation, value);
            }
        }


        IConditionValueEditor _value;
        public IConditionValueEditor Value
        {
            get { return _value; }
            private set
            {
                SetProperty(ref _value, value);
            }
        }

    }
}

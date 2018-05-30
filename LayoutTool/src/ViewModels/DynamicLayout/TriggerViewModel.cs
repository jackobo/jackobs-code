using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LayoutTool.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout
{
    public class TriggerViewModel : ViewModelBase, ISupportDynamicLayout<TriggerViewModel>
    {
        public TriggerViewModel(string name, PlayerStatusTypeViewModel playerStatus, IServiceLocator serviceLocator)
        {
            _name = name;
            PlayerStatus = playerStatus;

            if (string.IsNullOrEmpty(_name))
            {
                _name = playerStatus.Name;
            }

            
            _serviceLocator = serviceLocator;

            AddNewConditionCommand = new Command(AddNewCondition);

            _conditions.CollectionChanged += Conditions_CollectionChanged;
            
        }


        private void Conditions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ConditionsDescription));
            if(e.NewItems != null)
            {
                foreach(ConditionViewModel condition in e.NewItems)
                {
                    condition.PropertyChanged += Condition_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ConditionViewModel condition in e.OldItems)
                {
                    condition.PropertyChanged -= Condition_PropertyChanged;
                }
            }

        }


        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetProperty(ref _name, value);
            }
        }


        private void Condition_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ConditionsDescription));
        }

        IServiceLocator _serviceLocator;

        

        ConditionViewModelCollection _conditions = new ConditionViewModelCollection();
        public ConditionViewModelCollection Conditions
        {
            get { return _conditions; }
        }


        public TestValue[] GetAllPositiveTestValues()
        {
            var result = new List<TestValue>();

            foreach(var condition in Conditions)
            {
                var testValue = condition.GetPositiveTestValues();
                if(testValue != null)
                    result.AddRange(testValue);
            }

            return result.ToArray();
        }

        public void ApplyPositiveTestValues(IPlayerAttributesSimulatorViewModel playerAttributes)
        {
            foreach (var condition in Conditions)
            {
                var testValue = condition.GetPositiveTestValues()?.FirstOrDefault();
                if (testValue != null)
                    testValue.ApplyValue(playerAttributes);
            }
        }

        public void ApplyNegativeTestValues(IPlayerAttributesSimulatorViewModel playerAttributes, IEnumerable<TestValue> positiveValues)
        {
            var positiveValuesPerField = positiveValues.GroupBy(v => v.Field).ToDictionary(f => f.Key, f => f.ToArray());

           

            foreach (var condition in Conditions)
            {   
                var testValue = condition.GetNegativeTestValue(positiveValuesPerField.ContainsKey(condition.Field) ? positiveValuesPerField[condition.Field] : new TestValue[0]);
                if (testValue != null)
                    testValue.ApplyValue(playerAttributes);
            }

           
        }


        public string ConditionsDescription
        {
            get
            {
                return string.Join(Environment.NewLine + "AND" + Environment.NewLine, Conditions.Select(condition => $"({condition})"));
            }
        }
        public PlayerStatusTypeViewModel PlayerStatus { get; private set; }


        public ICommand AddNewConditionCommand { get; private set; }

        
        private void AddNewCondition()
        {
            _serviceLocator.GetInstance<IDialogServices>().ShowOkCancelDialogBox(new ValueEditors.AddNewConditionDialog(this, _serviceLocator));
        }
        
        public override string ToString()
        {
            return "Trigger for " + PlayerStatus.ToString();
        }

        public TriggerViewModel Clone(PlayerStatusTypeViewModel newPlayerStatus)
        {
            var clone = new TriggerViewModel(newPlayerStatus.Name, newPlayerStatus, _serviceLocator);

            foreach(var condition in this.Conditions)
            {
                clone.Conditions.Add(new ConditionViewModel(condition.Field, condition.EquationType, condition.ValueEditor.Clone()));
            }
            
            return clone;
        }

        
    }


}

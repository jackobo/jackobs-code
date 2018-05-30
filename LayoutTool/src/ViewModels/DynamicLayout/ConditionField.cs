using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout
{
    public abstract class ConditionField : SmartEnum<string, ConditionField>
    {
        public ConditionField(string id, string name)
            : base(id, name)
        {
            
        }


        public static readonly ConditionField IsVip = new IsVipField();
        public static readonly ConditionField RegistrationCountry = new RegistrationCountryField();
        public static readonly ConditionField RegistrationCurrency = new RegistrationCurrencyField();
        public static readonly ConditionField RegistrationDate = new RegistrationDateField();
        public static readonly ConditionField Gender = new GenderField();
        public static readonly ConditionField Age = new AgeField();
        //public static readonly ConditionField BirthDate = new BirthDateField();
        public static readonly ConditionField ExceededRoundsPlayed = new ExceededRoundsPlayedField();

        public virtual bool AllowMultipleInstances
        {
            get
            {
                return GetAvailableEquations().Length > 1;
            }
        }

        public virtual string UpdateType
        {
            get { return "single"; }
        }

        

        public abstract IConditionValueEditor GetDefaultValueEditor(IServiceLocator serviceLocator);

        public abstract EquationType[] GetAvailableEquations();

     
        

        public abstract ConditionViewModel BuildConditionViewModel(Condition condition, IServiceLocator serviceLocator);
        

        public virtual bool CanBuildViewModel(Condition condition)
        {
            return this.Id == condition.Type;
        }

        public virtual string ComputeFieldName(IConditionValueEditor valueEditor)
        {
            return Id;
        }

        public abstract void ApplyValue(IPlayerAttributesSimulatorViewModel playerAttributes, object value);
        
    }

    public class RegistrationDateField : ConditionField
    {
        public RegistrationDateField()
            : base("checkFirstRegistrationDate", "Registration date")
        {

        }

        public override void ApplyValue(IPlayerAttributesSimulatorViewModel playerAttributes, object value)
        {
            playerAttributes.RegistrationDate = (DateTime)value;
        }

        public override IConditionValueEditor GetDefaultValueEditor(IServiceLocator serviceLocator)
        {
            return BuildDefaultValueEditor();
        }

        private ValueEditors.DateTimeValueEditor BuildDefaultValueEditor()
        {
            return new ValueEditors.DateTimeValueEditor();
        }

        public override ConditionViewModel BuildConditionViewModel(Condition condition, IServiceLocator serviceLocator)
        {
            var valueEditor = ValueEditors.DateTimeValueEditor.TryParse(condition.Values.Select(x => x.Value).FirstOrDefault(), BuildDefaultValueEditor());

            return new ConditionViewModel(this, EquationType.FromId(condition.EquationType), valueEditor);
        }

        public override EquationType[] GetAvailableEquations()
        {
            return EquationType.All;
        }

    }

    public class RegistrationCountryField : ConditionField
    {
        public RegistrationCountryField()
            : base("checkCountryId", "Registration country")
        {
        }

        public override void ApplyValue(IPlayerAttributesSimulatorViewModel playerAttributes, object value)
        {
            string typeName = value?.GetType().FullName;
            var country = value as Country;

            playerAttributes.RegistrationCountry = country;
        }

        public override ConditionViewModel BuildConditionViewModel(Condition condition, IServiceLocator serviceLocator)
        {
            var valueEditor = BuildValueEditor(serviceLocator, country => condition.Values.Any(v => v.Value == country.Id.ToString()));
            
            return new ConditionViewModel(this, EquationType.FromId(condition.EquationType), valueEditor);
        }

        public override IConditionValueEditor GetDefaultValueEditor(IServiceLocator serviceLocator)
        {
            return BuildValueEditor(serviceLocator, c => false);
        }


        private IConditionValueEditor BuildValueEditor(IServiceLocator serviceLocator, Func<Country, bool> isSelected)
        {
            var valueEditor = new ValueEditors.MultiSelectValueEditor(serviceLocator.GetInstance<IDialogServices>());

            foreach (var country in serviceLocator.GetInstance<ICountryInformationProvider>().GetCountries().OrderBy(c => c.Name))
            {
                valueEditor.AddItem(country, country.Id.ToString(), country.Name, isSelected(country));
            }

            return valueEditor;
        }

        public override EquationType[] GetAvailableEquations()
        {
            return new EquationType[] { EquationType.Equal };
        }
    }

    public class RegistrationCurrencyField : ConditionField
    {
        public RegistrationCurrencyField()
            : base("checkCurrency", "Registration currency")
        {

        }

        public override void ApplyValue(IPlayerAttributesSimulatorViewModel playerAttributes, object value)
        {
            playerAttributes.RegistrationCurrency = value as CurrencyInfo;
        }
        public override ConditionViewModel BuildConditionViewModel(Condition condition, IServiceLocator serviceLocator)
        {
            ValueEditors.MultiSelectValueEditor valueEditor = BuildValueEditor(serviceLocator, currency => condition.Values.Any(v => v.Value == currency.Iso3.ToString()));

            return new ConditionViewModel(this, EquationType.FromId(condition.EquationType), valueEditor);
        }

        private ValueEditors.MultiSelectValueEditor BuildValueEditor(IServiceLocator serviceLocator, Func<CurrencyInfo, bool> isSelected)
        {
            var valueEditor = new ValueEditors.MultiSelectValueEditor(serviceLocator.GetInstance<IDialogServices>());

            foreach (var currency in serviceLocator.GetInstance<Interfaces.ICurrencyInformationProvider>().GetCurrencies().OrderBy(c => c.Name))
            {
                valueEditor.AddItem(currency, currency.Iso3, currency.Iso3 + " - " + currency.Name, isSelected(currency));
            }

            return valueEditor;
        }

        public override EquationType[] GetAvailableEquations()
        {
            return new EquationType[] { EquationType.Equal };
        }

        public override IConditionValueEditor GetDefaultValueEditor(IServiceLocator serviceLocator)
        {
            return BuildValueEditor(serviceLocator, currency => false);
        }
    }

    public class IsVipField : ConditionField
    {
        public IsVipField()
            : base("isVIP", "Is VIP")
        {

        }

        public override void ApplyValue(IPlayerAttributesSimulatorViewModel playerAttributes, object value)
        {
            var isVip = (bool)value;

            if (isVip)
                playerAttributes.VipLevel = VipLevel.VIP;
            else
                playerAttributes.VipLevel = VipLevel.Regular;
        }

        public override ConditionViewModel BuildConditionViewModel(Condition condition, IServiceLocator serviceLocator)
        {
            var valueEditor = ValueEditors.BooleanValueEditor.Parse(condition.Values.Select(v => v.Value).FirstOrDefault(), new ValueEditors.BooleanValueEditor());
            return new ConditionViewModel(this, EquationType.FromId(condition.EquationType), valueEditor);
        }

        public override EquationType[] GetAvailableEquations()
        {
            return new EquationType[] { EquationType.Equal };
        }

        public override IConditionValueEditor GetDefaultValueEditor(IServiceLocator serviceLocator)
        {
            return new ValueEditors.BooleanValueEditor();
        }
    }

    public class AgeField : ConditionField
    {
        public AgeField()
            : base("checkAge", "Age")
        {

        }


        public override void ApplyValue(IPlayerAttributesSimulatorViewModel playerAttributes, object value)
        {
            playerAttributes.Age = (int)value;
        }

        public override ConditionViewModel BuildConditionViewModel(Condition condition, IServiceLocator serviceLocator)
        {
            
            var valueEditor = ValueEditors.NumericValueEditor.Parse(condition.Values.Select(v => v.Value).FirstOrDefault(), 
                                                                 new ValueEditors.NumericValueEditor());


            return new ConditionViewModel(this, EquationType.FromId(condition.EquationType), valueEditor);
        }


        public override EquationType[] GetAvailableEquations()
        {
            return EquationType.All;
        }

        public override IConditionValueEditor GetDefaultValueEditor(IServiceLocator serviceLocator)
        {
            return new ValueEditors.NumericValueEditor();
        }
    }

    

    public class GenderField : ConditionField
    {
        public GenderField()
            : base("checkGender", "Gender")
        {

        }

        public override void ApplyValue(IPlayerAttributesSimulatorViewModel playerAttributes, object value)
        {
            playerAttributes.Gender = (Gender)value;
        }

        public override ConditionViewModel BuildConditionViewModel(Condition condition, IServiceLocator serviceLocator)
        {
            var valueEditor = new ValueEditors.GenderValueEditor();

            return new ConditionViewModel(this, EquationType.FromIdOrNull(condition.EquationType), valueEditor);
        }

        
        public override EquationType[] GetAvailableEquations()
        {
            return new EquationType[] { EquationType.Equal };
        }

        public override IConditionValueEditor GetDefaultValueEditor(IServiceLocator serviceLocator)
        {
            return new ValueEditors.GenderValueEditor();
        }
    }

    public class ExceededRoundsPlayedField : ConditionField
    {
        public ExceededRoundsPlayedField()
            : base("excedeedRoundsPlayed_", "Rounds played")
        {
        }

        public override string UpdateType
        {
            get
            {
                return "multi";
            }
        }

        public override void ApplyValue(IPlayerAttributesSimulatorViewModel playerAttributes, object value)
        {
#warning Not Implemented
        }

        public override string ComputeFieldName(IConditionValueEditor valueEditor)
        {
            var exceededRoundsPlayedValueEditor = valueEditor as ValueEditors.ExceededRoundsPlayedValueEditor;

            if (exceededRoundsPlayedValueEditor == null)
                throw new ArgumentException($"Value editor must be of type {typeof(ValueEditors.ExceededRoundsPlayedValueEditor).FullName}", nameof(valueEditor));

            return Id + exceededRoundsPlayedValueEditor.GameType;
        }

        public override ConditionViewModel BuildConditionViewModel(Condition condition, IServiceLocator serviceLocator)
        {
            var valueEditor = new ValueEditors.ExceededRoundsPlayedValueEditor();

            valueEditor.GameType = int.Parse(condition.Type.Split('_')[1]);
            valueEditor.NumberOfRounds = int.Parse(condition.Values.Select(v => v.Value).FirstOrDefault());

            return new ConditionViewModel(this, EquationType.FromId(condition.EquationType), valueEditor);

        }

        public override bool CanBuildViewModel(Condition condition)
        {
            return condition.Type.StartsWith(this.Id);
        }

        public override EquationType[] GetAvailableEquations()
        {
            return EquationType.All;
        }

        public override IConditionValueEditor GetDefaultValueEditor(IServiceLocator serviceLocator)
        {
            return new ValueEditors.ExceededRoundsPlayedValueEditor();
        }
    }

}

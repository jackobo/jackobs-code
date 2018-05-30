using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.MainProxyDataControlService;
using LayoutTool.ViewModels.DynamicLayout;
using LayoutTool.ViewModels.DynamicLayout.ValueEditors;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public interface IPlayerAttributesSimulatorViewModel
    {
        int? Age { get; set; }
        Gender Gender { get; set; }
        Country RegistrationCountry { get; set; }
        CurrencyInfo RegistrationCurrency { get; set; }
        DateTime? RegistrationDate { get; set; }
        VipLevel VipLevel { get; set; }
    }

    public class PlayerAttributesSimulatorViewModel : ViewModelBase, IPlayerAttributesSimulatorViewModel
    {
        public PlayerAttributesSimulatorViewModel(IServiceLocator serviceLocator, TriggerViewModelCollection triggers)
        {
            _serviceLocator = serviceLocator;
            _triggers = triggers;
            _playerData = MainProxyAdapter.GetPlayerData();

            LoadTestValuesProviders();

            _triggers.CollectionChanged += Triggers_CollectionChanged;
        }

        private void Triggers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (e.OldItems != null)
            {
                foreach (TriggerViewModel trigger in e.OldItems)
                {
                    trigger.PropertyChanged -= Trigger_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach(TriggerViewModel trigger in e.NewItems)
                {
                    trigger.PropertyChanged += Trigger_PropertyChanged;
                }
            }

            LoadTestValuesProviders();
        }

        private void Trigger_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LoadTestValuesProviders();
        }

        private void LoadTestValuesProviders()
        {
            var newTestValuesProviders = new ObservableCollection<IMockupViewTestValuesProvider>();
            newTestValuesProviders.Add(new MockupViewNagativeTestValuesProvider(_triggers));
            foreach(var t in _triggers)
            {
                newTestValuesProviders.Add(new MockupViewPositiveTestValuesProvider(t));
            }

            var oldTestValuesProviders = TestValuesProviders;

            TestValuesProviders = newTestValuesProviders;

            if (oldTestValuesProviders != null)
            {
                foreach (var vp in oldTestValuesProviders)
                    vp.Dispose();
            }

            SelectedTestValuesProvider = TestValuesProviders.FirstOrDefault();

        }

        TriggerViewModelCollection _triggers;
        protected override bool GlobalNotificationsEnabled
        {
            get
            {
                return false;
            }
        }

        IMockupViewTestValuesProvider _selectedTestValuesProvider;
        public IMockupViewTestValuesProvider SelectedTestValuesProvider
        {
            get { return _selectedTestValuesProvider; }
            set
            {
                _selectedTestValuesProvider = value;
                OnPropertyChanged();

                if (_selectedTestValuesProvider != null)
                    _selectedTestValuesProvider.ApplyTestValues(this);
            }
        }

        ObservableCollection<IMockupViewTestValuesProvider> _testValuesProviders;

        public ObservableCollection<IMockupViewTestValuesProvider> TestValuesProviders
        {
            get { return _testValuesProviders; }
            private set
            {
                SetProperty(ref _testValuesProviders, value);
            }
        }

        PlayerData _playerData;

        IServiceLocator _serviceLocator;

        IMainProxyAdapter MainProxyAdapter
        {
            get { return _serviceLocator.GetInstance<IMainProxyAdapter>(); }
        }

        ICountryInformationProvider CountryInformationProvider
        {
            get { return _serviceLocator.GetInstance<ICountryInformationProvider>(); }
        }

        ICurrencyInformationProvider CurrencyInformationProvider
        {
            get { return _serviceLocator.GetInstance<ICurrencyInformationProvider>(); }
        }

        public Country[] Countries
        {
            get
            {
                return CountryInformationProvider.GetCountries().OrderBy(c => c.Name).ToArray();
            }
        }

        public Country RegistrationCountry
        {
            get
            {
                var countryId = GetPlayerAttributeValue<int?>(PlayerAccountAttributeType.CountryID);
                if (countryId == null)
                    return null;

                return Countries.FirstOrDefault(c => c.Id == countryId.Value);
            }
            set
            {
                if (value != null)
                {
                    SetPlayerAttributeValue(PlayerAccountAttributeType.CountryID, value.Id);
                }
                else
                {
                    OnPropertyChanged();
                }

            }
        }


        public CurrencyInfo[] Currencies
        {
            get
            {
                return CurrencyInformationProvider.GetCurrencies().OrderBy(c => c.Name).ToArray();
            }
        }

        public CurrencyInfo RegistrationCurrency
        {
            get
            {
                return Currencies.FirstOrDefault(c => 0 == string.Compare(c.Iso3,  _playerData.BankrollCurrency, true));
            }
            set
            {
                if (value != null)
                {
                    var playerData = MainProxyAdapter.GetPlayerData();
                    playerData.BankrollCurrency = value.Iso3;
                    SetpLayerAttributeValue(PlayerAccountAttributeType.RegistrationCurrency, value.Iso3, playerData);
                    MainProxyAdapter.SetPlayerData(playerData);
                    _playerData = playerData;
                }
                OnPropertyChanged();
                
            }
        }


        public Gender[] Genders
        {
            get { return Gender.All; }
        }

        public Gender Gender
        {
            get
            {
                return Gender.FromId(GetPlayerAttributeValue<string>(PlayerAccountAttributeType.GenderId));
            }
            set
            {
                if (value != null)
                {
                    SetPlayerAttributeValue(PlayerAccountAttributeType.GenderId, value.Id);
                }
                else
                {
                    OnPropertyChanged();
                }
            }
        }

        public int? Age
        {
            get
            {
                var age = GetPlayerAttributeValue<int?>(PlayerAccountAttributeType.Age);

                return age;
            }
            set
            {
                if (value != null)
                {
                    var age = new PlayerAccountAttribute(PlayerAccountAttributeType.Age, value);
                    var birthDate = new PlayerAccountAttribute(PlayerAccountAttributeType.DateOfBirth, FormatDateTime(ComputeBirthDate(value)));
                    SetPlayerAttributes(new PlayerAccountAttribute[] { age, birthDate }, new string[] { nameof(Age) });
                }
                else
                {
                    OnPropertyChanged();
                }

            }
        }

        private DateTime? ComputeBirthDate(int? age)
        {
            if (age == null)
                return null;

            return DateTime.Today.AddYears(-age.Value);
        }




        public DateTime? RegistrationDate
        {
            get
            {
                return GetDateAttributeValue(PlayerAccountAttributeType.RegistrationDate);
            }
            set
            {
                if (value != null)
                {
                    var attributes = new PlayerAccountAttribute[]
                    {
                    new PlayerAccountAttribute(PlayerAccountAttributeType.RegistrationDate, FormatDateTime(value)),
                    new PlayerAccountAttribute(PlayerAccountAttributeType.PlayerJoinDate, FormatDateTime(value))
                    };

                    SetPlayerAttributes(attributes, new string[] { nameof(RegistrationDate) });
                }
                else
                {
                    OnPropertyChanged();
                }
                
            }
        }

        private string FormatDateTime(DateTime? value)
        {
            return DateTimeValueEditor.FormatDate(value.Value);
        }

        public VipLevel[] VipLevels
        {
            get
            {
                return new VipLevel[]
                {
                    VipLevel.Regular,
                    VipLevel.VIP,
                    VipLevel.Gold,
                    VipLevel.Diamond,
                    VipLevel.HR,
                    VipLevel.Platinum
                };
            }
        }

        public VipLevel VipLevel
        {
            get
            {
                var vipLevel = GetPlayerAttributeValue<VipLevel?>(PlayerAccountAttributeType.VipLogo);
                if (vipLevel == null)
                    return VipLevel.Regular;

                return vipLevel.Value;
            }

            set
            {
                SetPlayerAttributeValue(PlayerAccountAttributeType.VipLogo, (int)value);
            }
        }

        private T GetPlayerAttributeValue<T>(PlayerAccountAttributeType attributeType)
        {
            var attribute = _playerData.Attributes.FirstOrDefault(a => a.AttributeType == attributeType);

            if (attribute == null)
                return default(T);


            var converter = TypeDescriptor.GetConverter(typeof(T));

            
            return (T)converter.ConvertFromInvariantString(attribute.Value);

        }

        private DateTime? GetDateAttributeValue(PlayerAccountAttributeType attributeType)
        {
            return DateTimeValueEditor.TryParse(GetPlayerAttributeValue<string>(attributeType), new DateTimeValueEditor()).Value;
        }

        

        private void SetPlayerAttributeValue(PlayerAccountAttributeType attributeType, object value, [CallerMemberName] string propertyName = null)
        {
            SetPlayerAttributes(new PlayerAccountAttribute[] { new PlayerAccountAttribute(attributeType, value) }, new string[] { propertyName });
        }


        private void SetPlayerAttributes(PlayerAccountAttribute[] attributes, string[] properties)
        {
            var playerData = MainProxyAdapter.GetPlayerData();

            foreach (var a in attributes)
            {
                SetpLayerAttributeValue(a.AttributeType, a.Value, playerData);
            }

            MainProxyAdapter.SetPlayerData(playerData);

            _playerData = playerData;

            foreach(var propertyName in properties)
                OnPropertyChanged(propertyName);
        }

        private  void SetpLayerAttributeValue(PlayerAccountAttributeType attributeType, object value, PlayerData playerData)
        {
            var attribute = playerData.Attributes.FirstOrDefault(a => a.AttributeType == attributeType);

            if (attribute == null)
            {
                attribute = new PlayerAccountAttribute();
                attribute.AttributeType = attributeType;
                playerData.Attributes = playerData.Attributes.Concat(new PlayerAccountAttribute[] { attribute }).ToArray();
            }

            if (value != null)
            {
                attribute.Value = value.ToString();
            }
            else
            {
                attribute.Value = "";
            }
        }
    }

    public interface IMockupViewTestValuesProvider : IDisposable
    {
        string Name { get; }
        void ApplyTestValues(IPlayerAttributesSimulatorViewModel playerAttributes);
    }

    public class MockupViewNagativeTestValuesProvider : ViewModelBase, IMockupViewTestValuesProvider
    {
        public MockupViewNagativeTestValuesProvider(TriggerViewModelCollection triggers)
        {
            _triggers = triggers;
        }

        TriggerViewModelCollection _triggers;

        public string Name
        {
            get
            {
                return "Default layout";
            }
        }

        public void ApplyTestValues(IPlayerAttributesSimulatorViewModel playerAttributes)
        {
            var positiveValues = new List<TestValue>();

            foreach(var t in _triggers)
            {
                positiveValues.AddRange(t.GetAllPositiveTestValues());
            }

            foreach (var t in _triggers)
            {
                t.ApplyNegativeTestValues(playerAttributes, positiveValues);
            }
        }
        
    }

    public class MockupViewPositiveTestValuesProvider : ViewModelBase, IMockupViewTestValuesProvider
    {
        public MockupViewPositiveTestValuesProvider(TriggerViewModel trigger)
        {
            _trigger = trigger;
            _trigger.PropertyChanged += _trigger_PropertyChanged;
        }

        private void _trigger_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(TriggerViewModel.Name))
                OnPropertyChanged(nameof(Name));
        }

        TriggerViewModel _trigger;

        public void ApplyTestValues(IPlayerAttributesSimulatorViewModel playerAttributes)
        {
            _trigger.ApplyPositiveTestValues(playerAttributes);
        }

        public string Name
        {
            get { return _trigger.Name; }
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                _trigger.PropertyChanged -= _trigger_PropertyChanged;
            }
            base.Dispose(disposing);
        }

    }

    public enum VipLevel
    {
        Regular = 0,
        VIP = 1,
        Gold = 2,
        Diamond = 3,
        HR = 4,
        Platinum = 5
    }
}

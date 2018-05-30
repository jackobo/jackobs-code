using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class ClientUrlBuilderViewModel : ViewModelBase, IFiddlerOverrideProvider
    {
        public ClientUrlBuilderViewModel(BrandEntity brand,
                                         SkinEntity skin,
                                         ABTestCase abTest,
                                         IClientInformationProvider clientInformationProvider)
        {
            _brand = brand;
            _skin = skin;
            _abTest = abTest;
            _clientInformationProvider = clientInformationProvider;
            _clientUrlBuilder = clientInformationProvider.GetClientUrlBuilder(brand, skin);
            _environmentsConnections = clientInformationProvider.GetEnvironmentsConnections(brand, skin);
#warning maybe I should do something with this hard coded PRODUCTION value.
            _selectedEnvironmentConnection = _environmentsConnections.FirstOrDefault(e => e.Name == "PRODUCTION");
            if (_selectedEnvironmentConnection == null)
                _selectedEnvironmentConnection = _environmentsConnections.FirstOrDefault();
            UpdateSocketsSetupOverrideProvider();
        }

        BrandEntity _brand;
        SkinEntity _skin;
        IClientInformationProvider _clientInformationProvider;

        IClientUrlBuilder _clientUrlBuilder;
        ABTestCase _abTest;
        EnvironmentConnection[] _environmentsConnections = new EnvironmentConnection[0];

        protected override bool GlobalNotificationsEnabled
        {
            get
            {
                return false;
            }
        }

        public PathDescriptor BaseUrl
        {
            get
            {
                return _clientUrlBuilder.BuildBaseUrl(_abTest);
            }
        }
        public PathDescriptor LaunchUrl
        {
            get
            {
                if (EnvironmentsConnections.Length > 0 && SelectedEnvironmentConnection == null)
                {
                    return null;
                }

                return _clientUrlBuilder.BuildLaunchUrl(_abTest, SelectedEnvironmentConnection);
            }
        }

        private bool _useMainProxyMock = true;

        public bool UseMainProxyMock
        {
            get
            {
                return _useMainProxyMock;
            }

            set
            {
                if (SetProperty(ref _useMainProxyMock, value))
                {
                    OnPropertyChanged(nameof(EnforceEnvironmentSelection));
                }
            }
        }

        public EnvironmentConnection[] EnvironmentsConnections
        {
            get { return _environmentsConnections; }
        }

        private EnvironmentConnection _selectedEnvironmentConnection;
        public EnvironmentConnection SelectedEnvironmentConnection
        {
            get
            {
                return _selectedEnvironmentConnection;
            }

            set
            {
                SetProperty(ref _selectedEnvironmentConnection, value);
                OnPropertyChanged(nameof(LaunchUrl));

                UpdateSocketsSetupOverrideProvider();
            }
        }

        private void UpdateSocketsSetupOverrideProvider()
        {
            _socketsSetupOverrideProvider = _clientInformationProvider.GetSocketsSetupOverrideProvider(_brand, _skin, SelectedEnvironmentConnection);
        }

        IFiddlerOverrideProvider _socketsSetupOverrideProvider;

        public FiddlerOverrideMode GetOverrideMode(string url)
        {
            if (_socketsSetupOverrideProvider == null)
                return FiddlerOverrideMode.NoOverride;


            var overrideMode = _socketsSetupOverrideProvider.GetOverrideMode(url);

            if (UseMainProxyMock)
            {
                return overrideMode;
            }

            if (overrideMode == FiddlerOverrideMode.Normal)
            {
                return FiddlerOverrideMode.HeadersOnly;
            }

            return overrideMode;

        }

        public FiddlerOverrideContent GetOverrideContent(string url, string currentBodyContent)
        {
            if (_socketsSetupOverrideProvider == null)
                return new FiddlerOverrideContent(currentBodyContent);



            if (UseMainProxyMock)
            {
                return _socketsSetupOverrideProvider.GetOverrideContent(url, currentBodyContent);
            }
            else
            {
                return new FiddlerOverrideContent(currentBodyContent);
            }

        }


        public bool EnforceEnvironmentSelection
        {
            get
            {
                if (UseMainProxyMock)
                    return false;

                return EnvironmentsConnections.Length > 0;
            }
        }
        
    }
}

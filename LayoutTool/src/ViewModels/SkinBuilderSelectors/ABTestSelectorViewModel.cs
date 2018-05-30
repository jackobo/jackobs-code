using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class ABTestSelectorViewModel : ServicedViewModelBase
    {

        public ABTestSelectorViewModel(IServiceLocator serviceLocator, Func<IClientInformationProvider> createClientInformationProvider)
            : base(serviceLocator)
        {
            _createClientInformationProvider = createClientInformationProvider;
        }

        Func<IClientInformationProvider> _createClientInformationProvider;

        protected override bool GlobalNotificationsEnabled
        {
            get
            {
                return false;
            }
        }


        IClientInformationProvider ClientInformationProvider
        {
            get { return _createClientInformationProvider(); }
        }

        ABTestCaseSetViewModel[] _abTestCaseSets = new ABTestCaseSetViewModel[0];

        public ABTestCaseSetViewModel[] TestCaseSets
        {
            get { return _abTestCaseSets ?? new ABTestCaseSetViewModel[0]; }
            set
            {
                SetProperty(ref _abTestCaseSets, value);
            }
        }


        private ABTestCaseSetViewModel _selectedTestCaseSet;

        public ABTestCaseSetViewModel SelectedTestCaseSet
        {
            get
            {
                return _selectedTestCaseSet;
            }
            set
            {
                if(SetProperty(ref _selectedTestCaseSet, value))
                {
                    if (_selectedTestCaseSet == null)
                    {
                        this.SelectedTestCase = null;
                    }
                    else
                    {
                        if (_selectedTestCaseSet.TestCases.Length == 1)
                            this.SelectedTestCase = _selectedTestCaseSet.TestCases[0];
                        else
                            this.SelectedTestCase = null;
                    }

                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public ABTestCaseViewModel _selectedTestCase;
        public ABTestCaseViewModel SelectedTestCase
        {
            get
            {
                return _selectedTestCase;
            }

            set
            {
                if (SetProperty(ref _selectedTestCase, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                    OnPropertyChanged(nameof(SelectedTestCaseDescription));
                }
            }
        }

        public string SelectedTestCaseDescription
        {
            get
            {
                return this.SelectedTestCase?.Description;
            }
        }



        private bool _useDefaultLayout;

        public bool UseDefaultLayout
        {
            get
            {
                return _useDefaultLayout;
            }

            set
            {
                if(SetProperty(ref _useDefaultLayout, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }


        private bool _useABTest;
        public bool UseABTest
        {
            get
            {
                return _useABTest;
            }

            set
            {
                if(SetProperty(ref _useABTest, value))
                {
                    OnPropertyChanged(nameof(IsValid));
                }
                
            }
        }
        public override bool IsBusy
        {
            get
            {
                return base.IsBusy;
            }

            set
            {
                base.IsBusy = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid
        {
            get
            {
                if (IsBusy)
                    return false;

                if (this.UseDefaultLayout)
                    return true;

                if (TestCaseSets.Length > 0 && GetSelectedABTest() == null)
                    return false;


                return true;
            }
        }

        public ABTestCaseDescriptor GetSelectedABTest()
        {
            if (this.UseDefaultLayout)
                return null;

            if (SelectedTestCaseSet == null || SelectedTestCase == null)
                return null;

            return new ABTestCaseDescriptor(SelectedTestCaseSet.Id, SelectedTestCaseSet.BrandId, SelectedTestCaseSet.SkinId, SelectedTestCaseSet.Language, SelectedTestCase.GetABTest());

            
        }

        bool _showNoAbTestDetectedMessage = false;
        public bool ShowNoAbTestDetectedMessage
        {
            get
            {
                return _showNoAbTestDetectedMessage && this.TestCaseSets.Length == 0;
            }
            set
            {
                SetProperty(ref _showNoAbTestDetectedMessage, value);
            }
        }

        
        public void ReloadTestCases(BrandEntity brand, SkinEntity skin)
        {
            StartBusyAction(() => ReloadTestCasesAsync(brand, skin));
        }

        private void ReloadTestCasesAsync(BrandEntity brand, SkinEntity skin)
        {
            var newABTestCases = new ABTestCaseSetViewModel[0];
            if (brand != null && skin != null)
            {
                newABTestCases = ClientInformationProvider.GetABTestCases(brand, skin)
                                .Select(tc => new ABTestCaseSetViewModel(tc))
                                .ToArray();

                
            }


            ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(() =>
            {
                UbscribeFromTestsPropertyChanged(TestCaseSets);
                TestCaseSets = newABTestCases;
                SubscribeToTestsPropertyChanged(newABTestCases);

                if (brand != null && skin != null)
                {
                    this.ShowNoAbTestDetectedMessage = true;
                }
                OnPropertyChanged(nameof(IsValid));
            });
        }


        

        private void SubscribeToTestsPropertyChanged(ABTestCaseSetViewModel[] newABTestCases)
        {
            foreach (var testCase in newABTestCases ?? new ABTestCaseSetViewModel[0])
            {
                foreach (var test in testCase.TestCases ?? new ABTestCaseViewModel[0])
                {
                    test.PropertyChanged += Test_PropertyChanged;
                }

            }
        }

        private void UbscribeFromTestsPropertyChanged(ABTestCaseSetViewModel[] newABTestCases)
        {
            foreach (var testCase in newABTestCases)
            {
                foreach (var test in testCase.TestCases)
                {
                    test.PropertyChanged -= Test_PropertyChanged;
                }

            }
        }

        private void Test_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsValid));
        }
    }
}

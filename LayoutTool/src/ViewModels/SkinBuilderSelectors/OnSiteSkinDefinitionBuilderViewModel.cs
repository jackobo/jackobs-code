using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;

namespace LayoutTool.ViewModels
{
    public abstract class OnSiteSkinDefinitionBuilderViewModel : ServicedViewModelBase, ISkinDefinitionBuilderViewModel
    {
        public OnSiteSkinDefinitionBuilderViewModel(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            _abTestSelector = new ABTestSelectorViewModel(serviceLocator, () => ClientInformationProvider);
            _abTestSelector.PropertyChanged += ABTestSelector_PropertyChanged;
        }

        private void ABTestSelector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ABTestSelectorViewModel.IsValid))
                OnPropertyChanged(nameof(IsValid));
        }

        protected override bool GlobalNotificationsEnabled
        {
            get
            {
                return false;
            }
        }

        public event EventHandler StateRestored;

        public abstract Guid Id { get; }
        public abstract int Order { get; }
        public abstract string SourceName { get; }
        public abstract bool CanPublish { get; }

        public abstract void Publish(SkinDefinitionContext skinDefinitionContext);

        protected abstract IEnumerable<SkinSelectorIdentity.SkinSelectorProperty> SaveState();

        

        public void RestoreStateFrom(SkinIndentity skinIdentity)
        {
            RestoreStateCore(skinIdentity);
            ServiceLocator.GetInstance<IApplicationServices>().StartNewParallelTask(() =>
            {
                RestoreTestCase(skinIdentity.ABTestCase);
                ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(() => StateRestored?.Invoke(this, EventArgs.Empty));
            });
        }

        private void RestoreTestCase(ABTestIdentity aBTestCase)
        {
            
            while(ABTestSelector.IsBusy)
            {
                System.Threading.Thread.Sleep(10);
            }

            if(aBTestCase == null)
            {
                ABTestSelector.UseDefaultLayout = true;
                return;
            }
            
            foreach(var testCase in ABTestSelector.TestCaseSets)
            {
                if(testCase.Id == aBTestCase.TestCaseSetId)
                {
                    foreach(var test in testCase.TestCases)
                    {
                        if(test.Id == aBTestCase.TestCaseId)
                        {
                            ABTestSelector.UseABTest = true;
                            ABTestSelector.SelectedTestCaseSet = testCase;
                            ABTestSelector.SelectedTestCase = test;
                            return;
                        }
                    }
                }
            }

            ABTestSelector.UseDefaultLayout = true;



        }

        protected abstract void RestoreStateCore(SkinIndentity skinIdentity);
        

        protected virtual string GetSourceDescription()
        {
            return SourceName;
        }

        IClientInformationProvider ClientInformationProvider
        {
            get
            {
                return ServiceLocator.GetInstance<IClientInformationProviderFactory>().GetProvider(SelectedBrand, SelectedSkin);
            }
        }

        public virtual bool CanProvideClientUrl
        {
            get { return true; }
        }

        public ClientUrlBuilderViewModel GetClientUrlBuilder()
        {
            if (this.IsValid)
                return new ClientUrlBuilderViewModel(SelectedBrand, SelectedSkin, GetSelectedABTest()?.Test, ClientInformationProvider);
            else
                return null;
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                SetProperty(ref _isActive, value);
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                SetProperty(ref _isVisible, value);
            }
        }

        BrandEntity _selectedBrand;
        public BrandEntity SelectedBrand
        {
            get { return _selectedBrand; }
            set
            {
                SetProperty(ref _selectedBrand, value);
            }
        }

       

        private SkinEntity _selectedSkin;
        public SkinEntity SelectedSkin
        {
            get { return _selectedSkin; }
            set
            {
                if(SetProperty(ref _selectedSkin, value))
                {
                    this.ABTestSelector.ReloadTestCases(this.SelectedBrand, this.SelectedSkin);
                }
            }
        }


        


        private ISkinDefinitionBuilder GetBuilder()
        {
            if(!IsValid)
            {
                throw new InvalidOperationException("You must select a brand and skin");
            }
            return ClientInformationProvider.GetSkinDefinitionBuilder(SelectedBrand, SelectedSkin, GetSelectedABTest());
        }

        private ABTestCaseDescriptor GetSelectedABTest()
        {
            return ABTestSelector.GetSelectedABTest();
        }

      

        public SkinDefinitionContext Build()
        {
            var skinDefinitionContext = GetBuilder().Build();
            skinDefinitionContext.SourceSkin = GetSkinIdentity();
            skinDefinitionContext.DestinationSkin = skinDefinitionContext.SourceSkin;
            
            return skinDefinitionContext;
        }


        public SkinIndentity GetSkinIdentity()
        {
            return new SkinIndentity(new SkinSelectorIdentity(Id, GetSourceDescription(), SaveState()),
                                    GetClientVersion(),
                                    SelectedBrand.Id,
                                    SelectedBrand.Name,
                                    SelectedSkin.Id,
                                    SelectedSkin.Name,
                                    GetSelectedABIdentification());
        }

        protected virtual string GetClientVersion()
        {
            return GetBuilder().ClientVersion;
        }



        private ABTestIdentity GetSelectedABIdentification()
        {
            var testCaseDescriptor = GetSelectedABTest();
            if (testCaseDescriptor == null)
                return null;
            else
                return new ABTestIdentity(testCaseDescriptor.TestCaseSetId, testCaseDescriptor.Brand, testCaseDescriptor.Skin, testCaseDescriptor.Language, testCaseDescriptor.Test.Id, testCaseDescriptor.Test.Name);
        }
        

        public SkinConversionResult Apply(SkinDefinition skinDefinition)
        {
            return GetBuilder().Convert(skinDefinition);
        }


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);


            if(propertyName != nameof(ABTestSelector) && propertyName != nameof(IsValid))
            {
                _abTestSelector.ShowNoAbTestDetectedMessage = false;
            }
        }

        

        private readonly ABTestSelectorViewModel _abTestSelector;

        public ABTestSelectorViewModel ABTestSelector
        {
            get { return _abTestSelector; }
        }

       

      

        public virtual bool IsValid
        {
            get
            {
                if (IsBusy)
                    return false;

                if (SelectedBrand == null || SelectedSkin == null)
                    return false;

                if (!this.ABTestSelector.IsValid)
                    return false;
                
                return true;
            }
        }

        
        
    }
}

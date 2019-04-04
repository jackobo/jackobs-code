using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace LayoutTool.ViewModels
{
    public class QASkinDefinitionBuilderViewModel : OnSiteSkinDefinitionBuilderViewModel
    {
        public QASkinDefinitionBuilderViewModel(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.BrandInformationProvider = serviceLocator.GetInstance<IBrandInformationProvider>();
            LoadEnvironments();
        }

        public override Guid Id
        {
            get
            {
                
                return WellKnownSkinSourcesIds.QA;
            }
        }

        public override int Order
        {
            get { return 200; }
        }

        public override string SourceName
        {
            get
            {
                return "QA";
            }
        }

        public override bool CanPublish
        {
            get
            {
                return true;
            }
        }

        public override void Publish(SkinDefinitionContext skinDefinitionContext)
        {
            ServiceLocator.GetInstance<ISkinPublisher>().PublishForQA(skinDefinitionContext, this.SelectedEnvironment.Id);
        }
        protected override string GetSourceDescription()
        {
            if (!IsValid)
                return base.GetSourceDescription();

            return $"{SourceName} - {SelectedEnvironment.Name}";
        }

        protected override IEnumerable<SkinSelectorIdentity.SkinSelectorProperty> SaveState()
        {
            var properties = new List<SkinSelectorIdentity.SkinSelectorProperty>();

            properties.Add(new SkinSelectorIdentity.SkinSelectorProperty("SelectedEnvironment", SelectedEnvironment.Id));
            properties.Add(new SkinSelectorIdentity.SkinSelectorProperty("SelectedVersion", SelectedVersion.Id));

            return properties;
        }

        protected override void RestoreStateCore(SkinIndentity skinIdentity)
        {
            var property = skinIdentity.Selector.Properties.FirstOrDefault(p => p.Name == "SelectedEnvironment");

            if(property != null)
            {
                this.SelectedEnvironment = this.Environments.FirstOrDefault(e => e.Id == property.Value);
            }

            property = skinIdentity.Selector.Properties.FirstOrDefault(p => p.Name == "SelectedVersion");

            if (property != null)
            {
                this.SelectedVersion = this.Versions.FirstOrDefault(v => v.Id == property.Value);
            }

            this.SelectedBrand = this.Brands.FirstOrDefault(b => b.Id == skinIdentity.BrandId);
            this.SelectedSkin = this.Skins.FirstOrDefault(b => b.Id == skinIdentity.SkinId);
        }

        private void LoadEnvironments()
        {
            this.Environments = BrandInformationProvider.GetQAEnvironments();
        }

        IBrandInformationProvider BrandInformationProvider { get; set; }


        


        QAEnvironmentEntity[] _environments;

        public QAEnvironmentEntity[] Environments
        {
            get { return _environments; }
            set
            {
                SetProperty(ref _environments, value);
            }
        }

        QAEnvironmentEntity _selectedEnvironment;

        public QAEnvironmentEntity SelectedEnvironment
        {
            get { return _selectedEnvironment; }
            set
            {
                if (SetProperty(ref _selectedEnvironment, value))
                {
                    if(_selectedEnvironment == null)
                    {
                        this.SelectedVersion = null;
                        this.Versions = new ClientVersionEntity[0];
                        
                    }
                    else
                    {
                        this.Versions = _selectedEnvironment.ClientVersions.ToArray();
                        if (this.Versions.Length == 1)
                            this.SelectedVersion = this.Versions[0];
                        else
                            this.SelectedVersion = null;
                    }
                }

            }
        }


        private ClientVersionEntity[] _versions;

        public ClientVersionEntity[] Versions
        {
            get { return _versions; }
            set
            {
                SetProperty(ref _versions, value);
            }
        }


        private ClientVersionEntity _selectedVersion;

        public ClientVersionEntity SelectedVersion
        {
            get { return _selectedVersion; }
            set
            {
                if(SetProperty(ref _selectedVersion, value))
                {
                    if(_selectedVersion == null)
                    {
                        this.SelectedBrand = null;
                        this.Brands = new BrandEntity[0];
                    }
                    else
                    {
                        this.Brands = _selectedVersion.Brands.ToArray();
                        if (this.Brands.Length == 1)
                            this.SelectedBrand = this.Brands[0];
                        else
                            this.SelectedBrand = null;
                    }
                }
            }
        }

        private BrandEntity[] _brands;

        public BrandEntity[] Brands
        {
            get { return _brands; }
            set
            {
                SetProperty(ref _brands, value);
            }
        }


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if(propertyName == nameof(SelectedBrand))
            {
                OnSelectedBrandChanged();
            }
        }

        private void LoadBrands()
        {
            this.Brands = BrandInformationProvider.GetProductionBrands();
        }

        private void OnSelectedBrandChanged()
        {
            if (SelectedBrand != null)
            {
                this.Skins = SelectedBrand.Skins;
            }
            else
            {
                this.Skins = null;
            }

            if (this.Skins != null && this.Skins.Length == 1)
                this.SelectedSkin = this.Skins[0];
            else
                this.SelectedSkin = null;
        }

      


        SkinEntity[] _skins;
        public SkinEntity[] Skins
        {
            get { return _skins; }
            private set { SetProperty(ref _skins, value); }
        }

      
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class ProductionSkinDefinitionBuilderViewModel : OnSiteSkinDefinitionBuilderViewModel
    {
        public ProductionSkinDefinitionBuilderViewModel(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            LoadBrands(serviceLocator.GetInstance<IBrandInformationProvider>());
        }
        
        private void LoadBrands(IBrandInformationProvider brandInformationProvider)
        {
            this.Brands = brandInformationProvider.GetProductionBrands();
        }

        public override Guid Id
        {
            get
            {
                return WellKnownSkinSourcesIds.Production;
            }
        }

        public override int Order
        {
            get { return 100; }
        }
        public override string SourceName
        {
            get
            {
                return "Production";
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
            ServiceLocator.GetInstance<ISkinPublisher>().PublishForProduction(skinDefinitionContext);
        }

        protected override void RestoreStateCore(SkinIndentity skinIdentity)
        {
            this.SelectedBrand = this.Brands.FirstOrDefault(b => b.Id == skinIdentity.BrandId);
            this.SelectedSkin = this.Skins.FirstOrDefault(b => b.Id == skinIdentity.SkinId);
        }

        protected override IEnumerable<SkinSelectorIdentity.SkinSelectorProperty> SaveState()
        {
            return null;
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

            if (propertyName == nameof(SelectedBrand))
            {
                OnSelectedBrandChanged();
            }
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

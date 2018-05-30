using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class SkinToDesignSelectorViewModel : ServicedViewModelBase, INavigationAware
    {
        public SkinToDesignSelectorViewModel(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            

            this.SourceSkinSelector =  new SkinDefinitionBuilderSelectorViewModel(serviceLocator.GetAllInstances<ISkinDefinitionBuilderViewModel>().ToArray());
            this.DestinationSkinSelector = new SkinDefinitionBuilderSelectorViewModel(serviceLocator.GetAllInstances<ISkinDefinitionBuilderViewModel>().Where(s => s.CanPublish).ToArray());

            InitStartDesignCommand();


        }

        private void SubscribeToSkinSelectorEvents()
        {
            this.SourceSkinSelector.PropertyChanged += SkinSelector_PropertyChanged;
            this.SourceSkinSelector.SelectedBuilderPropertyChanged += SkinSelector_SelectedReaderPropertyChanged;
            this.DestinationSkinSelector.PropertyChanged += SkinSelector_PropertyChanged;
            this.DestinationSkinSelector.SelectedBuilderPropertyChanged += SkinSelector_SelectedReaderPropertyChanged;
        }

        private void UnsubscribeFromSkinSelectorEvents()
        {
            this.SourceSkinSelector.PropertyChanged -= SkinSelector_PropertyChanged;
            this.SourceSkinSelector.SelectedBuilderPropertyChanged -= SkinSelector_SelectedReaderPropertyChanged;
            this.DestinationSkinSelector.PropertyChanged -= SkinSelector_PropertyChanged;
            this.DestinationSkinSelector.SelectedBuilderPropertyChanged -= SkinSelector_SelectedReaderPropertyChanged;
        }



        private void SkinSelector_SelectedReaderPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateStartDesignCommand();
        }

        private void SkinSelector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SkinDefinitionBuilderSelectorViewModel.SelectedBuilder))
            {
                UpdateStartDesignCommand();
            }
        }

        private void SelectedReader_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateStartDesignCommand();
        }

        public SkinDefinitionBuilderSelectorViewModel SourceSkinSelector { get; private set; }

        public SkinDefinitionBuilderSelectorViewModel DestinationSkinSelector { get; private set; }

        private void InitStartDesignCommand()
        {
            _startDesignCommand = new Command(StartDesign, () => this.SourceSkinSelector != null 
                                                                 && this.SourceSkinSelector.SelectedBuilder != null 
                                                                 && this.SourceSkinSelector.SelectedBuilder.IsValid
                                                                 && this.DestinationSkinSelector.SelectedBuilder != null
                                                                 && this.DestinationSkinSelector.SelectedBuilder.IsValid);
        }

        private void StartDesign()
        {

            StartBusyAction(() =>
            {
                var designerBuilder = new SkinDesignerViewModelBuilder(ServiceLocator);

                var sourceSkinContext = SourceSkinSelector.SelectedBuilder.Build();
                var destinationSkinContext = DestinationSkinSelector.SelectedBuilder.Build();

                sourceSkinContext.AvailableGames = destinationSkinContext.AvailableGames;
                sourceSkinContext.DestinationSkin = destinationSkinContext.DestinationSkin;
                

                var designerViewModel = designerBuilder.Build(sourceSkinContext);

                ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(() => RegionManager.NavigateToMainContent(designerViewModel));

            });
        }


        private IRegionManager RegionManager
        {
            get
            {
                return this.ServiceLocator.GetInstance<IRegionManager>();
            }
        }
        

        private void UpdateStartDesignCommand()
        {
            if (_startDesignCommand != null)
            {
                _startDesignCommand.RaiseCanExecuteChanged();
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SubscribeToSkinSelectorEvents();
            this.SourceSkinSelector?.Activate();
            this.DestinationSkinSelector?.Activate();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            UnsubscribeFromSkinSelectorEvents();
        }

        private Command _startDesignCommand;
        public ICommand StartDesignCommand
        {
            get { return _startDesignCommand; }
        }
    }
}

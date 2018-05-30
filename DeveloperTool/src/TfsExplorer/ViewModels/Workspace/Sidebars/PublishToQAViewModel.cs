using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class PublishToQAViewModel : SidebarItemBase
    {
        public PublishToQAViewModel(ComponentsExplorerBar sourceComponents, IBranchPublisherViewModel branchPublisher, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            _sourceComponents = sourceComponents;
            _branchPublisher = branchPublisher;
            

            this.OkCommand = new Command(Publish, () => HasSomethingToPublish && !this.IsBusy, this);
            this.CancelCommand = new Command(Deactivate, () => !this.IsBusy, this);
            
            StartBusyAction(() => LoadComponentsPublishers(), "Detecting components to publish...");

            
        }

        bool _hasSomethingToPublish = false;
        public bool HasSomethingToPublish
        {
            get { return _hasSomethingToPublish; }
            private set
            {
                SetProperty(ref _hasSomethingToPublish, value);
            }
        }

        private void Publish()
        {
            StartBusyAction(() =>
            {
                _branchPublisher.Publish(GetPublishPayload());
                Deactivate();
            },
            "Publishing...");
        }
        
        public bool AllowCustomizedInstallerGeneration
        {
            get { return _branchPublisher.AllowCustomizedInstallerGeneration; }
        }


        private bool _generateCustomizedInstaller = true;

        public bool GenerateCustomizedInstaller
        {
            get { return _generateCustomizedInstaller; }
            set
            {
                if(SetProperty(ref _generateCustomizedInstaller, value))
                {
                    OnPropertyChanged(nameof(DoNotGenerateCustomizedInstaller));
                }
            }
        }

        public bool DoNotGenerateCustomizedInstaller
        {
            get { return !_generateCustomizedInstaller; }
            set
            {
                if(SetProperty(ref _generateCustomizedInstaller, !value))
                {
                    OnPropertyChanged(nameof(GenerateCustomizedInstaller));
                }
            }
        }

        private IPublishPayload GetPublishPayload()
        {

            var payloadBuilder = this.ServiceLocator.GetInstance<IPublishPayloadBuilder>();
            payloadBuilder.GenerateCustomizedInstaller = this.GenerateCustomizedInstaller && this.AllowCustomizedInstallerGeneration;
            AppendPublishPayload(this.ComponentsPublishers.Items, payloadBuilder);

            return payloadBuilder.Build();
        }

        private void AppendPublishPayload(ObservableCollection<IExplorerBarItem> items, IPublishPayloadBuilder payloadBuilder)
        {
            foreach(var item in items)
            {
                item.As<IComponentPublisherViewModel>().Do(cp => cp.Append(payloadBuilder));
                AppendPublishPayload(item.Items, payloadBuilder);
            }
        }

        private void LoadComponentsPublishers()
        {
            try
            {
                var publishableComponents = _branchPublisher.GetPublishableComponents();
                var explorerBar = new ComponentsPublishersExplorerBar(publishableComponents,
                                                                       this.ServiceLocator);
                explorerBar.ExpandAll();

                ApplicationServices.ExecuteOnUIThread(() =>
                {
                    this.ComponentsPublishers = explorerBar;
                    this.HasSomethingToPublish = publishableComponents.Any();
                });
            }
            catch
            {
                ApplicationServices.ExecuteOnUIThread(() =>
                {
                    this.HasSomethingToPublish = false;
                });
                throw;
            }
        }

        

        IApplicationServices ApplicationServices
        {
            get { return this.ServiceLocator.GetInstance<IApplicationServices>(); }
        }
        

        private ComponentsExplorerBar _sourceComponents;
        private IBranchPublisherViewModel _branchPublisher;

        ComponentsPublishersExplorerBar _componentsPublishers;
        public ComponentsPublishersExplorerBar ComponentsPublishers
        {
            get { return _componentsPublishers; }
            private set
            {
                SetProperty(ref _componentsPublishers, value);
            }
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
    }
}

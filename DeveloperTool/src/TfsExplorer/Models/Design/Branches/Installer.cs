using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.Models.Publish;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    public abstract class Installer<TLocation> : IInstaller
        where TLocation : Folders.IInstallerFolder
    {
        public Installer(TLocation location,
                        Guid installerId,
                        VersionNumber version,
                        IServiceLocator serviceLocator)
        {
            this.Location = location;
            this.InstallerId = installerId;
            this.Version = version;
            this.ServiceLocator = serviceLocator;
        }

        protected IServiceLocator ServiceLocator { get; private set; }

        protected TLocation Location { get; private set; }
        protected Guid InstallerId { get; private set; }
        public VersionNumber Version { get; private set; }
        
        public abstract string GetDescription();
        protected abstract void WriteHotfixTriggerContent();

        public void CreateBranch(Action<ProgressCallbackData> progressCallback = null)
        {

            if (IsBranched())
                throw new InvalidOperationException($"Installer branch {this.Location.GetServerPath().AsString()} already exists!");


            var installerBranchBuilder = new InstallerBranchBuilder(ServiceLocator.GetInstance<TFS.ITfsGateway>(),
                                                                    () => ServiceLocator.GetInstance<IPublishPayloadBuilder>(),
                                                                    ServiceLocator.GetInstance<IPublishPayloadSerializer>());

            installerBranchBuilder.ProgressCallback = progressCallback;

            installerBranchBuilder.Build(InstallerId, this.Location);

            ServiceLocator.GetInstance<IPubSubMediator>().Publish(new InstallerBranchedEventData(this));
        }

       


        public bool IsBranched()
        {
            return this.Location.Exists();
        }

        public IEnumerable<IComponentPublisher> GetHotfixPublishers()
        {
            if(!IsBranched())
            {
                throw new InvalidOperationException($"Hotfixes cannot be published for {this.Location.GetServerPath().AsString()} because it doesn't exists!");
            }
            
            return ServiceLocator.GetInstance<IComponentsPublishersReader>()
                                  .ReadPublishers(this.Location.PublishHistory,
                                                  this.Location.Components,
                                                  this.GetComponents());
        }

        public IEnumerable<ILogicalComponent> GetComponents()
        {

            return ComponentsReader.ReadComponents(this.Location.Components);


        }

        public bool IsPublishInProgress()
        {
            return ServiceLocator.GetInstance<IComponentsPublishersReader>()
                                 .IsPublishInProgress(this.Location.Trigger, this.Location.PublishHistory);


        }

        IComponentsReader ComponentsReader
        {
            get
            {
                return ServiceLocator.GetInstance<IComponentsReaderFactory>()
                                     .InstallerComponentsReader(InstallerId);
            }
        }


       

        public void Publish(IPublishPayload publishPayload)
        {
            var content = ServiceLocator.GetInstance<IPublishPayloadSerializer>().Serialize(publishPayload);
            this.Location.Trigger.PublishXml.SetBinaryContent(content);

            WriteHotfixTriggerContent();

         
        }
    }
}

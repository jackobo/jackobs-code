using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.Publish;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    public class QaBranch : EnvironmentBranch<Folders.QAFolder>, IQaBranch
    {
        public QaBranch(Folders.QAFolder qaFolder, IRootBranch owner, IServiceLocator serviceLocator)
            : base(qaFolder, owner, serviceLocator)
        {
        }

        protected override IBranchFolder GetMainFolder()
        {
            return this.Location.Main;
        }

        protected override IComponentsReader ComponentsReader
        {
            get
            {
                return ServiceLocator.GetInstance<IComponentsReaderFactory>().QAMainBranchComponentsReader(this.Owner.Version);
            }
        }
        

        public IEnumerable<IMergeSet> GetMergeSetsToDev()
        {
            return this.ServiceLocator.GetInstance<IMergeSetsReader>().ReadMergeSets(this.GetComponents(), Location.Parent.DEV.Main.Components);
        }
        
        public bool IsPublishInProgress()
        {
            return this.ServiceLocator.GetInstance<IComponentsPublishersReader>()
                        .IsPublishInProgress(this.Location.Main.Trigger, this.Location.Main.PublishHistory);
        }

        public IEnumerable<IComponentPublisher> GetComponentsToPublish()
        {
            return this.ServiceLocator.GetInstance<IComponentsPublishersReader>()
                                      .ReadPublishers(this.Location.Main.PublishHistory, 
                                                      this.Location.Main.Components, 
                                                      this.GetComponents());
        }

        public void Publish(IPublishPayload payload)
        {
            var payloadContent = this.ServiceLocator.GetInstance<IPublishPayloadSerializer>().Serialize(payload);
            
            Location.Main.Trigger.PublishXml.SetBinaryContent(payloadContent);
        }

        InstallerSynchronizer<IQAInstaller, NewQAInstallerEventData> _installersSynchronizer;
        public IEnumerable<IQAInstaller> GetInstallers()
        {
            if (_installersSynchronizer == null)
            {
                _installersSynchronizer = new InstallerSynchronizer<IQAInstaller, NewQAInstallerEventData>(
                                ReadInstallers,
                                installer => new NewQAInstallerEventData(installer, this),
                                ServiceLocator);
            }

            return _installersSynchronizer.CurrentInstallers;
        }

    
        private IEnumerable<IQAInstaller> ReadInstallers()
        {
            var result = new List<IQAInstaller>();
            using (var proxy = DeveloperToolServiceProxyFactory.CreateProxy())
            {
                var response = proxy.GetQAInstallers(new DeveloperToolService.GetQAInstallersRequest() { BranchName = Owner.Version.ToString() });

                foreach (var installer in response.Installers)
                {
                    var installerVersion = new Infra.Types.VersionNumber(installer.Version);
                    result.Add(new QAInstaller(Location.Installers.Installer(installerVersion),
                                               installer.Id,
                                               installerVersion,
                                               this.ServiceLocator));
                }
            }

            return result;
        }

        
    }
}

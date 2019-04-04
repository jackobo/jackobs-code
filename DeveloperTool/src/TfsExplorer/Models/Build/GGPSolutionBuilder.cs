using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Types;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class GGPSolutionBuilder
    {
        public GGPSolutionBuilder(RootBranchVersion ggpBranchName,
                                  BuildType buildType, 
                                  IBuildServices buildServices,
                                  string distributionFolderServerPath)
        {
            _ggpBranchName = ggpBranchName;
            _buildType = buildType;
            _services = buildServices;
            _distributionFolderServerPath = distributionFolderServerPath;

            _logger = buildServices.LoggerFactory.CreateLogger(this.GetType());
        }

        RootBranchVersion _ggpBranchName;
        BuildType _buildType;
        IBuildServices _services;
        ILogger _logger;
        string _distributionFolderServerPath;

        public void Build()
        {
            switch(_buildType)
            {
                case BuildType.Main:
                    BuildQAMain();
                    break;
                case BuildType.ProductionHotfix:
                    BuildProductionHotfix();
                    break;
                case BuildType.QAHotfix:
                    BuildQAHotfix();
                    break;
                default:
                    throw new ApplicationException($"Build Type {_buildType} is not supported");
            }
        }

        private void BuildQAMain()
        {
            var qaMainFolder = GetRootBranchFolder().QA.Main;

            Build(qaMainFolder,
                  qaMainFolder.Components,
                  qaMainFolder.PublishHistory.LatestPublishXml,
                  ReadPublishPayload(qaMainFolder),
                  Optional<GGPDeploymentContent.InstallerCustomizationInfo>.None());
        }

        

        private void BuildProductionHotfix()
        {
            var productionFolder = GetRootBranchFolder().PROD;

            var productionEnvironmentName = ReadTriggerIniProperty(productionFolder.HotfixTrigger.TriggerIni, 
                                                                   AntPropertyNames.ProductionEnvironment);
            var productionEnvironmentFolder = productionFolder.Environment(productionEnvironmentName);

            var installerVersion = VersionNumber.Parse(ReadTriggerIniProperty(productionEnvironmentFolder.HotfixTrigger.TriggerIni, 
                                                                              AntPropertyNames.InstallerVersion));
            var installerFolder = productionEnvironmentFolder.Installers.Installer(installerVersion);

            if (!installerFolder.Exists())
                throw new ApplicationException($"There is no production installer branch at this location {installerFolder.GetServerPath().AsString()}");

            _logger.Info($"Start building hotfix for environment {productionEnvironmentName} and installer {installerVersion}");

            var publishPayload = ReadPublishPayload(installerFolder);

            var customizedInstaller = Optional<GGPDeploymentContent.InstallerCustomizationInfo>.None();
            if(publishPayload.PublishPayload.GenerateCustomizedInstaller)
            {
                customizedInstaller = Optional<GGPDeploymentContent.InstallerCustomizationInfo>.Some(new GGPDeploymentContent.InstallerCustomizationInfo(installerFolder.Name, productionEnvironmentName));
            }
            

            Build(installerFolder,
                  installerFolder.Components,
                  installerFolder.PublishHistory.LatestPublishXml,
                  publishPayload,
                  customizedInstaller);
        }


        private void BuildQAHotfix()
        {
            var qaFolder = GetRootBranchFolder().QA;
            var installerVersion = VersionNumber.Parse(ReadTriggerIniProperty(qaFolder.HotfixTrigger.TriggerIni,
                                                                              AntPropertyNames.InstallerVersion));

            var installerFolder = qaFolder.Installers.Installer(installerVersion);

            if (!installerFolder.Exists())
                throw new ApplicationException($"There is no QA installer branch at this location {installerFolder.GetServerPath().AsString()}");

            _logger.Info($"Start building hotfix for QA installer {installerVersion}");

            var publishPayload = ReadPublishPayload(installerFolder);

            Build(installerFolder,
                  installerFolder.Components,
                  installerFolder.PublishHistory.LatestPublishXml,
                  publishPayload,
                  Optional<GGPDeploymentContent.InstallerCustomizationInfo>.None());

        }



        private StringKeyValueCollection ReadTriggerIniContent(Folders.IFileHolder fileHolder)
        {
            _logger.Info($"Reading {fileHolder.GetServerPath().AsString()}");
            return StringKeyValueCollection.Parse(fileHolder.GetTextContent());
        }


        private string ReadTriggerIniProperty(Folders.IFileHolder fileHolder, string propertyName)
        {
            var content = ReadTriggerIniContent(fileHolder);

            if (!content.Contains(propertyName))
                throw new ApplicationException($"The file {fileHolder.GetServerPath().AsString()} doesn't contain a property named {propertyName}");

            return content[propertyName].Value;
        }

        private void Build(IFolderHolder qaMainFolder, Folders.ComponentsFolder componentsFolder, IFileHolder latestPublishXml, PublishPayloadHolder publishPayloadInfo,
                           Optional<GGPDeploymentContent.InstallerCustomizationInfo> customizedInstaller)
        {
          
            var sourceControlAdapter = _services.CreateSourceControlAdapter();

            _logger.Info($"Execute Get Latest for {qaMainFolder.GetServerPath().AsString()}");
            sourceControlAdapter.GetLatest(qaMainFolder);
            
            var buildContext = new BuildContext(_services,
                                                GetBuildConfiguration(sourceControlAdapter, componentsFolder),
                                                new DeploymentContentBuilder(publishPayloadInfo.PublisherEmailAddress, _ggpBranchName, _buildType, 
                                                                            customizedInstaller),
                                                sourceControlAdapter);
            
            var components = ReadComponentsBuilders(publishPayloadInfo.PublishPayload, componentsFolder);
            
            var actions = GetBuildActionsList(components, publishPayloadInfo.PublishFileContent, latestPublishXml);
            
            foreach (var a in actions)
            {
                a.Execute(buildContext);
            }
        }

        private IRootFolder GetRootBranchFolder()
        {
            return _services.TfsGateway.GetRootFolder(_ggpBranchName);
        }
        
        private IBuildConfiguration GetBuildConfiguration(ISourceControlAdapter sourceControlAdapter, 
                                                          ComponentsFolder componentsFolder)
        {
            var buildOutputFolder = componentsFolder
                                                .Core
                                                .GGPBootstrapper
                                                .ToSourceControlFolder()
                                                .GetLocalPath()
                                                .Subpath("bin")
                                                .Subpath("Release");

            var distributionServerPath = sourceControlAdapter.CreateServerPath(_distributionFolderServerPath)
                                                             .Subpath(_ggpBranchName.ToString());

            var distributionLocalPath = sourceControlAdapter.GetLocalPathFromServerPath(distributionServerPath);
            

            var solutionFile = componentsFolder.GGPGameServerSln.ToSourceControlFile().GetLocalPath();

            return new BuildConfiguration(buildOutputFolder,
                                          distributionLocalPath,
                                          distributionServerPath,
                                          solutionFile);
        }



        private PublishPayloadHolder ReadPublishPayload(ProductionInstallerFolder installerFolder)
        {
            return ReadPublishPayload(installerFolder.Trigger.PublishXml);
        }

        private PublishPayloadHolder ReadPublishPayload(QAInstallerFolder installerFolder)
        {
            return ReadPublishPayload(installerFolder.Trigger.PublishXml);
        }

        private PublishPayloadHolder ReadPublishPayload(QAMainFolder qaMainFolder)
        {
            return ReadPublishPayload(qaMainFolder.Trigger.PublishXml);
        }

        private PublishPayloadHolder ReadPublishPayload(IFileHolder publishXml)
        {
            _logger.Info($"Reading {publishXml.GetServerPath().AsString()}");
            if (!publishXml.Exists())
                throw new ApplicationException($"File {publishXml.GetServerPath().AsString()} doesn't exists!");

            var sourceControlFile = publishXml.ToSourceControlFile();
            var publishFileContent = sourceControlFile.GetContent();
            var publishPayload = _services.CreatePublishPayloadSerializer().Deserialize(sourceControlFile.GetContent());

            var latestChangeSet = sourceControlFile.GetLatestChangeSet();
            
            return new PublishPayloadHolder(publishPayload, 
                                           publishFileContent, 
                                           _services.OperatingSystemServices.AccountManagement.GetAccountInfo(latestChangeSet.CommiterUserName).EmailAddress);
        }


        private class PublishPayloadHolder
        {
            public PublishPayloadHolder(IPublishPayload publishPayload, byte[] publishFileContent, string publisherEmailAddress)
            {
                this.PublishPayload = publishPayload;
                this.PublishFileContent = publishFileContent;
                this.PublisherEmailAddress = publisherEmailAddress;
            }

            public IPublishPayload PublishPayload { get; private set; }
            public byte[] PublishFileContent { get; private set; }
            public string PublisherEmailAddress { get; private set; }
        }

        private IEnumerable<IBuildAction> GetBuildActionsList(
                IEnumerable<IComponentBuilder> components, 
                byte[] publishContent,
                IFileHolder latestPublishXml)
        {
            _logger.Info("Build actions list");
            var buildActions = new List<IBuildAction>();
            
            buildActions.Add(new CreateDeploymentContent(components));
            
            buildActions.Add(new Bm2AsBeginAction());

            foreach (var c in components)
                buildActions.AddRange(c.GetPreCompileActions());

            buildActions.Add(new CompileAction());

            foreach (var c in components)
                buildActions.AddRange(c.GetPostCompileActions());

            foreach (var c in components)
                buildActions.AddRange(c.GetDeployActions());

            foreach (var c in components)
                buildActions.Add(new CreateComponentDescriptionTxtAction(c));

            

#warning a better check-in comment ??
            buildActions.Add(new CheckInAction($"Deliver {_ggpBranchName.ToString()}"));

            buildActions.Add(new WritePublishHistoryAction(latestPublishXml, publishContent));

            buildActions.Add(new Bm2AsEndAction());

            return buildActions;
        }

        private IEnumerable<IComponentBuilder> ReadComponentsBuilders(IPublishPayload publishPayload, ComponentsFolder componentsFolder)
        {
            _logger.Info("Reading components builders");
            return _services.CreateComponentsBuildersReader().GetBuilders(publishPayload, componentsFolder);
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.DeveloperToolService;

namespace Spark.TfsExplorer.Models.Build
{
    public class GGPDeploymentContent
    {

        public GGPDeploymentContent(RootBranchVersion branchName,
                                BuildType buildType,
                                string publisherEmailAddress,
                                Optional<InstallerCustomizationInfo> customizedInstaller,
                                IEnumerable<CoreComponentDeployContent> coreComponents,
                                IEnumerable<GameEngineDeployContent> gameEngines,
                                IEnumerable<GameMathDeployContent> gamesMaths,
                                IEnumerable<GameLimitsDeployContent> gamesLimits)
        {
            _branchName = branchName;
            _buildType = buildType;
            _publisherEmailAddress = publisherEmailAddress;
            _customizedInstaller = customizedInstaller;

            this.CoreComponents = coreComponents;
            this.GameEngines = gameEngines;
            this.GamesMaths = gamesMaths;
            this.GamesLimits = gamesLimits;
        }


        public class InstallerCustomizationInfo
        {
            public InstallerCustomizationInfo(string installerVersion)
                : this(installerVersion, null)
            {

            }

            public InstallerCustomizationInfo(string installerVersion, string productionEnvironmentName)
            {
                this.InstallerVersion = installerVersion;
                this.ProductionEnvironmentName = productionEnvironmentName;
            }

            public string InstallerVersion { get; private set; }
            public string ProductionEnvironmentName { get; private set; }
        }

        string _publisherEmailAddress;
        RootBranchVersion _branchName;
        BuildType _buildType;
        Optional<InstallerCustomizationInfo> _customizedInstaller;
        public DeveloperToolService.BeginBuildRequest CreateBeginBuildRequest()
        {
            var request = new DeveloperToolService.BeginBuildRequest();
            request.Branch = _branchName.ToString();
            request.BuildType = (DeveloperToolService.BuildType)((int)_buildType);
            request.PublisherEmailAddress = _publisherEmailAddress;
            _customizedInstaller.Do(ci =>
            {
                request.CustomizedInstaller = new InstallerCustomizationInfoDTO()
                {
                    InstallerVersion = ci.InstallerVersion,
                    ProductionEnvironmentName = ci.ProductionEnvironmentName
                };
            });
            request.CoreComponents = this.CoreComponents.Select(cc => cc.CreateBeginRequest()).ToArray();
            request.GameEngines = this.GameEngines.Select(ge => ge.CreateBeginRequest()).ToArray();
            
            request.GamesMaths = this.GamesMaths.Select(gm => gm.CreateBeginRequest()).ToArray();
            request.GamesLimits = this.GamesLimits.Select(gl => gl.CreateBeginRequest()).ToArray();

            return request;
        }

        public IEnumerable<CoreComponentDeployContent> CoreComponents { get; private set; }
        public IEnumerable<GameEngineDeployContent> GameEngines { get; private set; }
        public IEnumerable<GameMathDeployContent> GamesMaths { get; private set; }
        public IEnumerable<GameLimitsDeployContent> GamesLimits { get; private set; }

        public abstract class ComponentDeployContent<TBeginRequest>
        {
            public ComponentDeployContent(string componentUniqueId,
                                          string name,
                                          VersionNumber version,
                                          IServerPath projectServerPath,
                                          IEnumerable<DeployableFileDefinition> files,
                                          string componentDescription,
                                          IServerPath distributionServerPath)
            {
                this.ComponentUniqueId = componentUniqueId;
                this.Name = name;
                this.Version = version;
                this.ProjectServerPath = projectServerPath.AsString();
                this.Files = files;
                this.ComponentDescription = componentDescription;
                this.DistributionServerPath = distributionServerPath.AsString();
            }
            

            public string ComponentUniqueId { get; set; }
            public string Name { get; private set; }
            public string ProjectServerPath { get; private set; }
            public VersionNumber Version { get; private set; }
            public IEnumerable<DeployableFileDefinition> Files { get; private set; }

            public string ComponentDescription { get; private set; }

            public string DistributionServerPath { get; private set; }
        }

        

        public class CoreComponentDeployContent : ComponentDeployContent<CoreComponentPublishDTO>
        {
            public CoreComponentDeployContent(string componentUniqueId,
                                                string name, 
                                                 VersionNumber version,
                                                 IServerPath projectServerPath,
                                                 Optional<CoreComponentCustomizationMetaData> customizationMetaData,
                                                 IEnumerable<DeployableFileDefinition> files,
                                                 string componentDescription, 
                                                 IServerPath distributionServerPath)
                : base(componentUniqueId, name, version, projectServerPath, files, componentDescription, distributionServerPath)
            {
                this.CustomizationMetaData = customizationMetaData;
            }



            public CoreComponentPublishDTO CreateBeginRequest()
            {
                return new CoreComponentPublishDTO()
                {
                    ComponentUniqueId = this.ComponentUniqueId,
                    Name = this.Name,
                    Version = this.Version.ToString(),
                    ComponentDescription = this.ComponentDescription,
                    DistributionServerPath = this.DistributionServerPath,
                    ProjectFullServerPath = this.ProjectServerPath,
                    ComponentType = this.CustomizationMetaData.FirstOrDefault()?.ComponentType
                };
            }


            public Optional<CoreComponentCustomizationMetaData> CustomizationMetaData { get; private set; }

         
        }
        
        public class GameEngineDeployContent : ComponentDeployContent<GameEnginePublishDTO>
        {
            public GameEngineDeployContent(string componentUniqueId,
                                            string name,
                                            VersionNumber version,
                                            IServerPath projectServerPath,
                                            IEnumerable<DeployableFileDefinition> files,
                                            string componentDescription,
                                            IServerPath distributionServerPath)
                : base(componentUniqueId, name, version, projectServerPath, files, componentDescription, distributionServerPath)
            {
            }



            public GameEnginePublishDTO CreateBeginRequest()
            {
                return new GameEnginePublishDTO()
                {
                    ComponentUniqueId = this.ComponentUniqueId,
                    Name = this.Name,
                    Version = this.Version.ToString(),
                    ComponentDescription = this.ComponentDescription,
                    DistributionServerPath = this.DistributionServerPath,
                    ProjectFullServerPath = this.ProjectServerPath
                };
            }
        }

        public abstract class GameDeployContent<TBeginRequest> : ComponentDeployContent<TBeginRequest>
        {
            public GameDeployContent(string componentUniqueId,
                                    string name,
                                    GameEngineName engineName,
                                    string engineUniqueId,
                                    VersionNumber version,
                                    IServerPath projectServerPath,
                                    IEnumerable<DeployableFileDefinition> files,
                                    string componentDescription,
                                    IServerPath distributionServerPath)
                : base(componentUniqueId, name, version, projectServerPath, files, componentDescription, distributionServerPath)
            {                
                this.EngineName = engineName;
                this.EngineUniqueId = engineUniqueId;
            }

            public GameEngineName EngineName { get; private set; }
            public string EngineUniqueId { get; private set; }
        }

        public class GameMathDeployContent : GameDeployContent<GameMathPublishDTO>
        {
            public GameMathDeployContent(string componentUniqueId,
                                         string gameName, 
                                         VersionNumber version, 
                                         GameEngineName engineName,
                                         string engineUniqueId,
                                         IServerPath projectServerPath, 
                                         IEnumerable<DeployableFileDefinition> files,
                                         string componentDescription,
                                         IServerPath distributionServerPath) 
                : base(componentUniqueId, gameName, engineName, engineUniqueId, version, projectServerPath, files, componentDescription, distributionServerPath)
            {
            }

            public GameMathPublishDTO CreateBeginRequest()
            {
                return new GameMathPublishDTO()
                {
                    ComponentUniqueId = this.ComponentUniqueId,
                    ComponentDescription = this.ComponentDescription,
                    EngineUniqueId = this.EngineUniqueId,
                    Name = this.Name,
                    Version = this.Version.ToString(),
                    DistributionServerPath = this.DistributionServerPath,
                    ProjectFullServerPath = this.ProjectServerPath
                };
            }
            
        }

        public class GameLimitsDeployContent : GameDeployContent<GameLimitsPublishDTO>
        {
            public GameLimitsDeployContent(string componentUniqueId,
                                            string gameName, 
                                            VersionNumber version, 
                                            GameEngineName engineName,
                                            string engineUniqueId,
                                            IServerPath projectServerPath, 
                                            IEnumerable<DeployableFileDefinition> files,
                                            string componentDescription,
                                            IServerPath distributionServerPath)
                : base(componentUniqueId, gameName, engineName, engineUniqueId, version, projectServerPath, files, componentDescription, distributionServerPath)
            {
            }

            public GameLimitsPublishDTO CreateBeginRequest()
            {
                return new GameLimitsPublishDTO()
                {            
                    ComponentUniqueId = this.ComponentUniqueId,
                    EngineUniqueId = this.EngineUniqueId,
                    Name = this.Name,
                    Version = this.Version.ToString(),       
                    ComponentDescription = this.ComponentDescription,
                    DistributionServerPath = this.DistributionServerPath,
                    ProjectFullServerPath = this.ProjectServerPath
                };
            }
        }

    }


}

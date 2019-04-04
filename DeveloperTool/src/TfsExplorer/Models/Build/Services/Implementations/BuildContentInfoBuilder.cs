using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class DeploymentContentBuilder : IDeploymentContentBuilder
    {
        List<GGPDeploymentContent.CoreComponentDeployContent> _coreComponents = new List<GGPDeploymentContent.CoreComponentDeployContent>();
        List<GGPDeploymentContent.GameEngineDeployContent> _gameEngines = new List<GGPDeploymentContent.GameEngineDeployContent>();
        List<GGPDeploymentContent.GameMathDeployContent> _gamesMaths = new List<GGPDeploymentContent.GameMathDeployContent>();
        List<GGPDeploymentContent.GameLimitsDeployContent> _gamesLimits = new List<GGPDeploymentContent.GameLimitsDeployContent>();

      

        public DeploymentContentBuilder(string publisherEmailAddress, 
                                        RootBranchVersion branchName, 
                                        BuildType buildType,
                                        Optional<GGPDeploymentContent.InstallerCustomizationInfo> customizedInstaller)
        {
            if (string.IsNullOrEmpty(publisherEmailAddress))
                throw new ArgumentNullException(nameof(publisherEmailAddress));

            _publisherEmailAddress = publisherEmailAddress;
            _branchName = branchName;
            _buildType = buildType;
            _customizedInstaller = customizedInstaller;
        }

        string _publisherEmailAddress;
        RootBranchVersion _branchName;
        BuildType _buildType;
        Optional<GGPDeploymentContent.InstallerCustomizationInfo> _customizedInstaller = Optional<GGPDeploymentContent.InstallerCustomizationInfo>.None();
        public void AppendCoreComponent(GGPDeploymentContent.CoreComponentDeployContent coreComponent)
        {
            _coreComponents.Add(coreComponent);
        }
        
        public void AppendGameEngine(GGPDeploymentContent.GameEngineDeployContent gameEngine)
        {
            _gameEngines.Add(gameEngine);   
        }

      

        public void AppendGameLimits(GGPDeploymentContent.GameLimitsDeployContent gameLimits)
        {
            _gamesLimits.Add(gameLimits);
        }

      

        public void AppendGameMath(GGPDeploymentContent.GameMathDeployContent gameMath)
        {
           
            _gamesMaths.Add(gameMath);
        }

        public GGPDeploymentContent Build()
        {
            return new Models.Build.GGPDeploymentContent(_branchName,
                                                     _buildType,
                                                     _publisherEmailAddress,
                                                     _customizedInstaller,
                                                     _coreComponents,
                                                     _gameEngines,
                                                     _gamesMaths,
                                                     _gamesLimits);
        }


       
    }
}

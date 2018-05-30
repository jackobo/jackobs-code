using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service
{
    public class GamesPortalToBuildMachineAdapter : WcfServiceBase, IGamesPortalToBuildMachineAdapter
    {
        public GamesPortalToBuildMachineAdapter()
        {

        }


        public GamesPortalToBuildMachineAdapter(IGamesPortalInternalServices services)
            : base(services)
        {
        }


        private void ForceGameSynchronization(ExplicitForceGameSynchronizationRequest request)
        {
           
        }

        public void StartChillWrapperSynchronization(Entities.GamingComponentCategory componentType, string versionFolder)
        {
            try
            {
                var chillWrapper = ChillWrapperType.FromId(componentType);
                Artifactory.IArtifactoryRepositoryDescriptor repositoryDescriptor = Services.ArtifactorySynchronizationManager.FindRepositoryDescriptor(chillWrapper.Infrastructure, false, chillWrapper.Id);
                Services.ArtifactorySynchronizationManager.ForceGameSynchronization((int)componentType, repositoryDescriptor);
            }
            catch (Exception ex)
            {
                LogException(nameof(ForceGameSynchronization), ex, new {componentType = componentType, VersionFolder = versionFolder } );
                throw;
            }
        }

        public void StartGameSynchronization(string repositoryName, string gamesFolderName, int gameType, string versionFolder)
        {

            try
            {
                var repositoryDescriptor = Services.ArtifactorySynchronizationManager.FindRepositoryDescriptor(repositoryName, gamesFolderName);
                Services.ArtifactorySynchronizationManager.ForceGameSynchronization(gameType, repositoryDescriptor);
            }
            catch (Exception ex)
            {
                LogException(nameof(ForceGameSynchronization), ex, new { repositoryName, gamesFolderName, gameType, versionFolder });
                throw;
            }
        }
    }
}

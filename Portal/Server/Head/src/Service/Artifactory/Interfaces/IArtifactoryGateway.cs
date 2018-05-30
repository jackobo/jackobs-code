using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesPortal.Service.Artifactory
{
    public interface IArtifactoryGateway
    {
        void StartSynchronization();
        void StopSynchronization();
        void ForceSynchronization(string initiatedBy);
        ArtifactoryRepositoryDescriptor GetRepositoryDescriptor(Entities.GameTechnology gameTechnology, Entities.PlatformType platformType, bool isExternal);

        bool SynchronizationInProgress { get;}

        PropertySet[] GetAvailablePropertySets();
    }
}

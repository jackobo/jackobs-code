using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service.Artifactory
{
    public interface IArtifactorySynchronizationManager
    {
        void StartSynchronization();
        void StopSynchronization();
        
        PropertySet[] GetAvailablePropertySets();
        
        void ForceGamesSynchronization(string initiatedBy);
        void ForceGameSynchronization(int mainGameType, IArtifactoryRepositoryDescriptor repositoryDescriptor);
        IComponentSynchronizer CreateGameSynchronizer();
        IArtifactoryRepositoryDescriptor FindRepositoryDescriptor(GameInfrastructureDTO gameInfrastructure, bool isExternal, GamingComponentCategory category);
        IArtifactoryRepositoryDescriptor FindRepositoryDescriptor(string repositoryName, string gamesFolderName);
        IEnumerable<IArtifactoryRepositoryDescriptor> GetAllRepositories();
        bool IsGamesSynchronizationInProgress { get; }
    }
}

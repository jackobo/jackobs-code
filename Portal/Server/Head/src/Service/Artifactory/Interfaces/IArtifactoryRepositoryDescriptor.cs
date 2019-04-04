using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service.Artifactory
{
    public interface IArtifactoryRepositoryDescriptor
    {
        
        //GameTechnology Technology { get; }
        //PlatformType PlatformType { get; }
        IArtifactoryRepository Repository { get; }
        bool IsExternal { get; }

        GameInfrastructureDTO Infrastructure { get; }
        GamingComponentCategory ComponentCategory { get; }
    }
}

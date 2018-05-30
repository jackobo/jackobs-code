using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service.Artifactory
{
    public interface IChillWrapperRepository : IArtifactoryRepository
    {
        GamingComponentCategory ComponentCategory { get; }
    }
}

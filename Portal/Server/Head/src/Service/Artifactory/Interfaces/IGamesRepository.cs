using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.Artifactory
{
    public interface IGamesRepository : IArtifactoryRepository
    {
        
        string GamesFolderPath { get; }
    }
}

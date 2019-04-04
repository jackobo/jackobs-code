using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.Artifactory
{
    public interface IArtifactoryStorage
    {
        ArtifactoryGame GetGame(int gameType);
    }

    public class ArtifactoryGame
    {
        public ArtifactoryGame(IArtifactoryStorage storage)
        {
            this.Storage = storage;
        }

        IArtifactoryStorage Storage { get; set; }


      
    }

    public class ArtifactoryRegulation
    {
        public string Name { get; set; }
    }

    public class ArtifactoryGameVersion
    {
        public VersionNumber Version { get; set; }
        public string FileUrl { get; set; }
    }




}

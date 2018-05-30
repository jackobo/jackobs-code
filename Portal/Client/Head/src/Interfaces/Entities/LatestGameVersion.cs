using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GGPGameServer.ApprovalSystem.Common;

namespace GamesPortal.Client.Interfaces.Entities
{
 
    public class LatestGameVersion
    {
      
        public LatestGameVersion(Guid gameId, string gameName, int mainGameType, bool isExternal, Guid gameVersionId, GameInfrastructure infrastructure, string regulation, string version)
        {
            this.GameId = gameId;
            this.GameName = gameName;
            this.MainGameType = mainGameType;
            this.IsExternal = isExternal;
            this.GameVersionId = gameVersionId;
            this.Infrastructure = infrastructure;
            this.Regulation = RegulationType.GetRegulation(regulation);
            this.Version = new VersionNumber(version);
        }

        
        public Guid GameId { get; set; }
        
        public string GameName { get; set; }
        
        public int MainGameType { get; set; }
        
        public bool IsExternal { get; set; }
        
        public Guid GameVersionId { get; set; }
        
        public GameInfrastructure Infrastructure { get; set; }
        
        public RegulationType Regulation { get; set; }
        
        public VersionNumber Version { get; set; }
    }
}

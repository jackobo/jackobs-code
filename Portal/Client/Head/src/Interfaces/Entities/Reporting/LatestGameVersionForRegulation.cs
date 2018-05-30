using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace GamesPortal.Client.Interfaces.Entities
{
 
    public class LatestGameVersionForRegulation
    {
      
        public LatestGameVersionForRegulation(Guid gameId, string gameName, int mainGameType, bool isExternal, Guid gameVersionId, GameInfrastructure infrastructure, string regulation, string version, DownloadInfo downloadInfo)
        {
            this.GameId = gameId;
            this.GameName = gameName;
            this.MainGameType = mainGameType;
            this.IsExternal = isExternal;
            this.GameVersionId = gameVersionId;
            this.Infrastructure = infrastructure;
            this.Regulation = RegulationType.GetRegulation(regulation);
            this.Version = new VersionNumber(version);
            this.DownloadInfo = downloadInfo;
        }

        
        public Guid GameId { get; set; }
        
        public string GameName { get; set; }
        
        public int MainGameType { get; set; }
        
        public bool IsExternal { get; set; }
        
        public Guid GameVersionId { get; set; }
        
        public GameInfrastructure Infrastructure { get; set; }
        
        public RegulationType Regulation { get; set; }
        
        public VersionNumber Version { get; set; }

        public DownloadInfo DownloadInfo { get; set; }
    }
}

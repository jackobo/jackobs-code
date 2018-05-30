using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class LatestGameVersionForRegulationDTO
    {
        public LatestGameVersionForRegulationDTO()
        {

        }


        public LatestGameVersionForRegulationDTO(Guid gameId, string gameName, int mainGameType, bool isExternal, Guid gameVersionId, GameInfrastructureDTO gameInfrastructure, string regulation, long versionAsLong, DownloadInfoDTO downloadInfo)
        {
            this.GameId = gameId;
            this.GameName = gameName;
            this.MainGameType = mainGameType;
            this.IsExternal = isExternal;
            this.GameVersionId = gameVersionId;
            this.GameInfrastructure = gameInfrastructure;
            this.Regulation = regulation;
            this.Version = new VersionNumber(versionAsLong).ToString();
            this.DownloadInfo = downloadInfo;
        }

        [DataMember]
        public Guid GameId{get;set;}
        [DataMember]
        public string GameName{get;set;}
        [DataMember]
        public int MainGameType{get;set;}
        [DataMember]
        public bool IsExternal{get;set;}
        [DataMember]
        public Guid GameVersionId{get;set;}
        [DataMember]
        public GameInfrastructureDTO GameInfrastructure { get;set;}
        [DataMember]
        public string Regulation{get;set;}
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public DownloadInfoDTO DownloadInfo { get; set; }
    }
}

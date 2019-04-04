using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GGPGameServer.ApprovalSystem.Common;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class LatestGameVersionEntity
    {
        public LatestGameVersionEntity()
        {

        }


        public LatestGameVersionEntity(Guid gameId, string gameName, int mainGameType, bool isExternal, Guid gameVersionId, GameInfrastructureDto gameInfrastructure, string regulation, long versionAsLong)
        {
            this.GameId = gameId;
            this.GameName = gameName;
            this.MainGameType = mainGameType;
            this.IsExternal = isExternal;
            this.GameVersionId = gameVersionId;
            this.GameInfrastructure = gameInfrastructure;
            this.Regulation = regulation;
            this.Version = new VersionNumber(versionAsLong).ToString();
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
        public GameInfrastructureDto GameInfrastructure { get;set;}
        [DataMember]
        public string Regulation{get;set;}
        [DataMember]
        public string Version { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class LatestApprovedGameVersionEntity
    {

        public LatestApprovedGameVersionEntity()
        {

        }

        public LatestApprovedGameVersionEntity(Guid gameID, 
                                                string gameName, 
                                                long? lastVersion,
                                                int mainGameType, 
                                                string regulation, 
                                                string clientType, 
                                                bool isExternal,
                                                GameInfrastructureDto gameInfrastructure,
                                                string qaApprovedVersion, 
                                                Guid? qaApprovedVersionId,
                                                string pmApprovedVersion,
                                                Guid? pmApprovedVersionId)
        {
        
            this.GameId = gameID;
            this.GameName = gameName;
            if (lastVersion != null)
                this.LastVersion = new VersionNumber(lastVersion.Value).ToString();

            this.MainGameType = mainGameType;
            this.Regulation = regulation;
            this.ClientType = clientType;
            this.IsExternal = isExternal;
            this.GameInfrastructure = gameInfrastructure;
            this.QAApprovedVersion = qaApprovedVersion;
            this.QAApprovedVersionId = qaApprovedVersionId;
            this.PMApprovedVersion = pmApprovedVersion;
            this.PMApprovedVersionId = pmApprovedVersionId;
        }


        [DataMember]
        public Guid GameId { get; set; }

        
        [DataMember]
        public string GameName { get; set; }

        
        [DataMember]
        public string LastVersion { get; set; }


        [DataMember]
        public int MainGameType { get; set; }

        [DataMember]
        public string Regulation { get; set; }

        [DataMember]
        public string ClientType { get; set; }

        [DataMember]
        public bool IsExternal { get; set; }

        [DataMember]
        public GameInfrastructureDto GameInfrastructure { get; set; }

        [DataMember]
        public PlatformType PlatformType { get; set; }

        [DataMember]
        public string QAApprovedVersion { get; set; }
        
        [DataMember]
        public Guid? QAApprovedVersionId { get; set; }

        [DataMember]
        public string PMApprovedVersion { get; set; }

        [DataMember]
        public Guid? PMApprovedVersionId { get; set; }
    }
}

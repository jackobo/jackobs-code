using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GamesPortal.Client.Interfaces.Entities
{
  
    
    public class LatestApprovedGameVersion
    {

        public LatestApprovedGameVersion(Guid gameID, 
                                        string gameName, 
                                        int mainGameType,
                                        VersionNumber latestVersion,
                                        RegulationType regulation,
                                        string clientType,
                                        bool isExternal,
                                        GameInfrastructure infrastructure,
                                        VersionNumber qaApprovedVersion,
                                        Guid? qaApprovedVersionId,
                                        VersionNumber pmApprovedVersion,
                                        Guid? pmApprovedVersionId)
        {
           
            this.GameID = gameID;
            this.GameName = gameName;
            this.IsExternal = isExternal;
            this.MainGameType = mainGameType;
            this.LatestVersion = latestVersion;
            this.ClientType = clientType;

            this.Regulation = regulation;
            this.Infrastructure = infrastructure;
            this.QAApprovedVersion = qaApprovedVersion;
            this.QAApprovedVersionId = qaApprovedVersionId;
            this.PMApprovedVersion = pmApprovedVersion;
            this.PMApprovedVersionId = pmApprovedVersionId;
        }

        
        public Guid GameID { get; set; }

        public string GameName { get; set; }
        
        public int MainGameType { get; set; }

        public VersionNumber LatestVersion { get; set; }

        public string ClientType { get; set; }

        public RegulationType Regulation { get; set; }

        public bool IsExternal { get; set; }

        public GameInfrastructure Infrastructure { get; set; }

        public VersionNumber QAApprovedVersion { get; set; }

        public Guid? QAApprovedVersionId { get; set; }

        public VersionNumber PMApprovedVersion { get; set; }

        public Guid? PMApprovedVersionId { get; set; }



        
    }
}

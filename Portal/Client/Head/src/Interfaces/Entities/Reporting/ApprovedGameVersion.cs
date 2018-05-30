using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace GamesPortal.Client.Interfaces.Entities
{
    
    public class ApprovedGameVersion
    {
        public ApprovedGameVersion()
        {

        }

        public ApprovedGameVersion(string gameName,
                                      int mainGameType,
                                      bool isExternal,
                                      GamingComponentCategory category,
                                      VersionNumber version,
                                      GameInfrastructure gameInfra,
                                      RegulationType regulation,
                                      IApproval approvalInfo)
        {
            this.GameName = gameName;
            this.MainGameType = mainGameType;
            this.IsExternal = isExternal;
            this.Category = category;
            this.Version = version;
            this.GameInfra = gameInfra;
            this.Regulation = regulation;
            this.ApprovalInfo = approvalInfo;

        }

        
        public string GameName { get; set; }
        
        public int MainGameType { get; set; }
        
        public bool IsExternal { get; set; }
        
        public GamingComponentCategory Category { get; set; }
        
        public VersionNumber Version { get; set; }
        
        public GameInfrastructure GameInfra { get; set; }
        
        public RegulationType Regulation { get; set; }
        
        public IApproval ApprovalInfo { get; set; }
        
        
    }
}

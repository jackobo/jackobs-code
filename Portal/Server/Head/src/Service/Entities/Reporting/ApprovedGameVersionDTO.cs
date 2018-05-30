using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class ApprovedGameVersionDTO
    {
        public ApprovedGameVersionDTO()
        {

        }

        public ApprovedGameVersionDTO(string gameName,
                                      int mainGameType,
                                      bool isExternal,
                                      GamingComponentCategory category,
                                      string version,
                                      GameInfrastructureDTO gameInfra,
                                      string regulation,
                                      ApprovalInfoDTO approvalInfo)
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

        [DataMember]
        public string GameName { get; set; }
        [DataMember]
        public int MainGameType { get; set; }
        [DataMember]
        public bool IsExternal { get; set; }
        [DataMember]
        public GamingComponentCategory Category { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public GameInfrastructureDTO GameInfra { get; set; }
        [DataMember]
        public string Regulation { get; set; }
        [DataMember]
        public ApprovalInfoDTO ApprovalInfo { get; set; }
        
    }

}

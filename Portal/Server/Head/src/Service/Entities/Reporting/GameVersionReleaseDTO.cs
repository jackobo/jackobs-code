using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class GameVersionReleaseDTO
    {
        public GameVersionReleaseDTO()
        {

        }
        public GameVersionReleaseDTO(Guid gameId,
                                    int mainGameType,
                                    string name,
                                    bool isExternal,
                                    GameInfrastructureDTO gameInfrastructure,
                                    Guid gameVersionId,
                                    long version,
                                    string regulation,
                                    string downloadUrl,
                                    DateTime createdDate,
                                    string createdBy,
                                    string triggeredBy)
        {
            this.GameId = gameId;
            this.MainGameType = mainGameType;
            this.Name = name;
            this.IsExternal = isExternal;
            this.GameInfrastructure = gameInfrastructure;
            this.GameVersionId = gameVersionId;
            this.Version = version;
            this.Regulation = regulation;
            this.DownloadUrl = downloadUrl;
            this.CreatedDate = createdDate;
            this.CreatedBy = createdBy;
            this.TriggeredBy = triggeredBy;
        }

        
        [DataMember]
        public Guid GameId { get; set; }
        [DataMember]
        public int MainGameType { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsExternal { get; set; }
        [DataMember]
        public GameInfrastructureDTO GameInfrastructure { get; set; }
        [DataMember]
        public Guid GameVersionId { get; set; }
        [DataMember]
        public long Version { get; set; }
        [DataMember]
        public string Regulation { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string TriggeredBy { get; set; }
        [DataMember]
        public string DownloadUrl { get; set; }
    }
}

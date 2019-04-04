using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Spark.Infra.Types;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class LatestApprovedGameVersionForRegulationDTO
    {

        public LatestApprovedGameVersionForRegulationDTO()
        {

        }

        public LatestApprovedGameVersionForRegulationDTO(Guid gameID, 
                                                string gameName, 
                                                long? lastVersion,
                                                int mainGameType, 
                                                string regulation, 
                                                bool isExternal,
                                                GameInfrastructureDTO gameInfrastructure,
                                                LatestVersionInfoDTO qaVersionInfo,
                                                LatestVersionInfoDTO pmVersionInfo,
                                                LatestVersionInfoDTO productionVersionInfo,
                                                long? latestQAApprovedVersion)
        {
        
            this.GameId = gameID;
            this.GameName = gameName;
            this.LastVersion = VersionNumber.FromLong(lastVersion)?.ToString();
            this.MainGameType = mainGameType;
            this.Regulation = regulation;
            this.IsExternal = isExternal;
            this.GameInfrastructure = gameInfrastructure;
            this.QAVersionInfo = qaVersionInfo;
            this.PMVersionInfo = pmVersionInfo;
            this.ProductionVersionInfo = productionVersionInfo;
            this.LatestQAApprovedVersion = VersionNumber.FromLong(latestQAApprovedVersion)?.ToString();
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
        public bool IsExternal { get; set; }

        [DataMember]
        public GameInfrastructureDTO GameInfrastructure { get; set; }


        [DataMember]
        public LatestVersionInfoDTO QAVersionInfo { get; set; }


        [DataMember]
        public LatestVersionInfoDTO PMVersionInfo { get; set; }

        [DataMember]
        public LatestVersionInfoDTO ProductionVersionInfo { get; set; }

        [DataMember]
        public string LatestQAApprovedVersion { get; set; }
    }

    [DataContract]
    public class LatestVersionInfoDTO
    {
        public LatestVersionInfoDTO()
        {

        }

        private LatestVersionInfoDTO(Guid versionId, string version, DownloadInfoDTO downloadInfo)
        {
            this.VersionId = VersionId;
            this.Version = version;
            this.DownloadInfo = downloadInfo;
        }
        [DataMember]
        public Guid VersionId { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public DownloadInfoDTO DownloadInfo { get; set; }


        public static LatestVersionInfoDTO CreateOrNull(Guid? versionId, long? version, DownloadInfoDTO downloadInfo)
        {
            if (version == null || versionId == null)
                return null;

            return new LatestVersionInfoDTO(versionId.Value, new VersionNumber(version.Value).ToString(), downloadInfo);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Spark.Infra.Types;

namespace GamesPortal.Client.Interfaces.Entities
{
  
    
    public class LatestApprovedGameVersionForEachRegulation
    {

        public LatestApprovedGameVersionForEachRegulation(Guid gameID, 
                                        string gameName, 
                                        int mainGameType,
                                        VersionNumber latestVersion,
                                        RegulationType regulation,
                                        bool isExternal,
                                        GameInfrastructure infrastructure,
                                        LatestVersionInfo qaVersionInfo,
                                        LatestVersionInfo pmVersionInfo,
                                        LatestVersionInfo productionVersionInfo,
                                        string latestQAApprovedVersion)
        {

            if (qaVersionInfo == null && pmVersionInfo == null)
                throw new ArgumentNullException($"{nameof(qaVersionInfo)} and {nameof(pmVersionInfo)} can't be both null! GameID = {gameID}; Regulation = {regulation}; GameInfrastructure = {infrastructure}");

            this.GameID = gameID;
            this.GameName = gameName;
            this.IsExternal = isExternal;
            this.MainGameType = mainGameType;
            this.LatestVersion = latestVersion;
            this.Regulation = regulation;
            this.Infrastructure = infrastructure;
            this.QAVersionInfo = qaVersionInfo;
            this.PMVersionInfo = pmVersionInfo;
            this.ProductionVersionInfo = productionVersionInfo;
            if (string.IsNullOrEmpty(latestQAApprovedVersion))
                this.LatestQAApprovedVersion = Optional<VersionNumber>.None();
            else
                this.LatestQAApprovedVersion = Optional<VersionNumber>.Some(VersionNumber.Parse(latestQAApprovedVersion));
        }

        
        public Guid GameID { get; set; }

        public string GameName { get; set; }
        
        public int MainGameType { get; set; }

        public VersionNumber LatestVersion { get; set; }
        
        public RegulationType Regulation { get; set; }

        public bool IsExternal { get; set; }

        public GameInfrastructure Infrastructure { get; set; }

        public LatestVersionInfo QAVersionInfo { get; set; }


        public Optional<VersionNumber> LatestQAApprovedVersion { get; private set; } = Optional<VersionNumber>.None();

        public LatestVersionInfo PMVersionInfo { get; set; }

        public LatestVersionInfo ProductionVersionInfo { get; set; }
        public DownloadInfo GetDownloadInfo()
        {
            if (PMVersionInfo?.Version > QAVersionInfo?.Version)
            {
                return PMVersionInfo.DownloadInfo;
            }
            else
            {
                return QAVersionInfo.DownloadInfo;
            }

        }


        public class LatestVersionInfo
        {

            public LatestVersionInfo(Guid versionId, VersionNumber version, DownloadInfo downloadInfo)
            {
                this.VersionId = VersionId;
                this.Version = version;
                this.DownloadInfo = downloadInfo;
            }
            
            public Guid VersionId { get; private set; }

            
            public VersionNumber Version { get; private set; }

            
            public DownloadInfo DownloadInfo { get; private set; }

            

        }

        public bool SameApprovedVersion()
        {
            return QAVersionInfo?.Version == PMVersionInfo?.Version;
        }
    }


    public static class LatestVersionInfoExtension
    {
        public static VersionNumber GetVersionOrNull(this LatestApprovedGameVersionForEachRegulation.LatestVersionInfo latestVersion)
        {
            if (latestVersion == null)
                return null;


            return latestVersion.Version;

        }
    }


    
}

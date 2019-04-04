using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class GameVersionEntity
    {
        public GameVersionEntity()
        {

        }

        public GameVersionEntity(Guid versionId, string version, long versionAsNumber, DateTime createdDate, string createdBy, string triggeredBy, GameInfrastructureDto gameInfrastructure,
            GameVersionApprovalStatusEntity[] approvalStatus,
            GameVersionPropertyEntity[] properties, 
            GameVersionPropertyChangeHistoryEntity[] propertiesChangeHistory)
        {
            this.VersionId = versionId;
            this.Version = version;
            this.VersionAsNumber = versionAsNumber;
            this.CreatedBy = createdBy;
            this.TriggeredBy = triggeredBy;
            this.CreatedDate = createdDate;
            this.GameInfrastructure = gameInfrastructure;
            this.Properties = properties;
            this.ApprovalStatus = approvalStatus;
            this.PropertiesChangeHistory = propertiesChangeHistory;
        }

        [DataMember]
        public Guid VersionId { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public long VersionAsNumber { get; set; }
        [DataMember]
        public DateTime CreatedDate{get;set;}
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string TriggeredBy { get; set; }

        [DataMember]
        public GameInfrastructureDto GameInfrastructure { get; set; }

        [DataMember]
        public PlatformType PlatformType { get; set; }
        [DataMember]
        public GameVersionPropertyEntity[] Properties { get; set; }
        [DataMember]
        public GameVersionApprovalStatusEntity[] ApprovalStatus { get; set; }

        [DataMember]
        public GameVersionPropertyChangeHistoryEntity[] PropertiesChangeHistory { get; set; }
    }

    [DataContract]
    public class GameVersionApprovalStatusEntity
    {
        public GameVersionApprovalStatusEntity()
        {

        }

        public GameVersionApprovalStatusEntity(string clientType, string regulation, string qaStatus, bool pmApproved)
        {
            this.ClientType = clientType;
            this.Regulation = regulation;
            this.QAStatus = qaStatus;
            this.PMApproved = pmApproved;
        }

        [DataMember]
        public string ClientType { get; set; }
        [DataMember]
        public string Regulation { get; set; }
        [DataMember]
        public string QAStatus { get; set; }
        [DataMember]
        public bool PMApproved { get; set; }
    }

    [DataContract]
    public class GameVersionPropertyEntity
    {
        public GameVersionPropertyEntity()
        {

        }

        public GameVersionPropertyEntity(string key, string value, string regulation)
        {
            this.Key = key;
            this.Value = value;
            this.Regulation = regulation;
        }

        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string Regulation { get; set; }
    }


    [DataContract]
    public class GameVersionPropertyChangeHistoryEntity
    {
        public GameVersionPropertyChangeHistoryEntity()
        {

        }


        public GameVersionPropertyChangeHistoryEntity(string propertyKey, string oldValue, string newValue, string regulation, DateTime changeDate, string changedBy, int changeType)
        {
            this.PropertyKey = propertyKey;
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.Regulation = regulation;
            this.ChangeDate = changeDate;
            this.ChangedBy = changedBy;
            this.ChangeType = changeType;
        }

        [DataMember]
        public string PropertyKey { get; set; }
        [DataMember]
        public string OldValue { get; set; }
        [DataMember]
        public string NewValue { get; set; }
        [DataMember]
        public string Regulation { get; set; }
        [DataMember]
        public DateTime ChangeDate { get; set; }
        [DataMember]
        public string ChangedBy { get; set; }
        [DataMember]
        public int ChangeType { get; set; }
    

    }

    [DataContract]
    public class GameVersionDownloadInfoEntity
    {
        public GameVersionDownloadInfoEntity()
        {

        }

        public GameVersionDownloadInfoEntity(Guid gameID, int mainGameType, string gameName, string regulation, Guid gameVersionId, string downloadUri, string fileName, long fileSize, string md5)
        {
            this.GameId = gameID;
            this.MainGameType = mainGameType;
            this.GameName = gameName;
            this.GameVersionId = gameVersionId;
            this.DownloadUri = downloadUri;
            this.FileName = fileName;
            this.FileSize = fileSize;
            this.MD5 = md5;
            this.Regulation = regulation;
        }

        [DataMember]
        public Guid GameId { get; set; }
        [DataMember]
        public int MainGameType { get; set; }
        [DataMember]
        public string GameName { get; set; }
        [DataMember]
        public Guid GameVersionId { get; set; }
        [DataMember]
        public string DownloadUri { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public long FileSize { get; set; }
        [DataMember]
        public string MD5 { get; set; }
        [DataMember]
        public string Regulation { get; set; }
    }
}

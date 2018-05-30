using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GamesPortal.Service.Entities
{
    [DataContract]
    public class GameVersionDTO
    {
        public GameVersionDTO()
        {

        }

        public GameVersionDTO(Guid versionId, 
                                string version, 
                                long versionAsNumber, 
                                DateTime createdDate, 
                                string createdBy, 
                                string triggeredBy, 
                                GameInfrastructureDTO gameInfrastructure,
                                GamingComponentCategory componentCategory,
                                GameVersionRegulationDTO[] regulations,
                                GameVersionPropertyChangeHistoryDTO[] propertiesChangeHistory)
        {
            this.VersionId = versionId;
            this.Version = version;
            this.VersionAsNumber = versionAsNumber;
            this.CreatedDate = createdDate;
            this.CreatedBy = createdBy;
            this.TriggeredBy = triggeredBy;
            this.GameInfrastructure = gameInfrastructure;
            this.ComponentCategory = componentCategory;
            this.Regulations = regulations;
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
        public GameInfrastructureDTO GameInfrastructure { get; set; }
        [DataMember]
        public GamingComponentCategory ComponentCategory { get; set; }

        [DataMember]
        public GameVersionRegulationDTO[] Regulations { get; set; }

        [DataMember]
        public GameVersionPropertyChangeHistoryDTO[] PropertiesChangeHistory { get; set; }
    }

    [DataContract]
    public class GameVersionRegulationLanguageDTO
    {
        public GameVersionRegulationLanguageDTO()
        {

        }

        public GameVersionRegulationLanguageDTO(LanguageDTO language, ApprovalInfoDTO qaApprovalInfo, ProductionUploadInfoDTO productionUploadInfo, bool isMandatory)
        {
            this.Language = language;
            this.QaApprovalInfo = qaApprovalInfo;
            this.ProductionUploadInfo = productionUploadInfo;
            this.IsMandatory = isMandatory;
        }
        
        [DataMember]
        public LanguageDTO Language { get; set; }
        [DataMember]
        public ApprovalInfoDTO QaApprovalInfo { get; set; }
        [DataMember]
        public ProductionUploadInfoDTO ProductionUploadInfo { get; set; }
        [DataMember]
        public bool IsMandatory { get; set; }
    }

    [DataContract]
    public class GameVersionRegulationDTO
    {
        public GameVersionRegulationDTO()
        {

        }

        public GameVersionRegulationDTO(string regulation, 
                                        DownloadInfoDTO downloadInfo, 
                                        ApprovalInfoDTO qaApprovalInfo,
                                        ApprovalInfoDTO pmApprovalInfo,
                                        ProductionUploadInfoDTO productionUploadInfo,
                                        GameVersionRegulationLanguageDTO[] languages)
        {
            this.Regulation = regulation;
            this.DownloadInfo = downloadInfo;
            this.QAApprovalInfo = qaApprovalInfo;
            this.PMApprovalInfo = pmApprovalInfo;
            this.ProductionUploadInfo = productionUploadInfo;
            this.Languages = languages;
        }
        [DataMember]
        public string Regulation { get; set; }
        [DataMember]
        public DownloadInfoDTO DownloadInfo { get; set; }

        [DataMember]
        public ApprovalInfoDTO QAApprovalInfo { get; set; }
        [DataMember]
        public ApprovalInfoDTO PMApprovalInfo { get; set; }
        [DataMember]
        public ProductionUploadInfoDTO ProductionUploadInfo { get; set; }

        [DataMember]
        GameVersionRegulationLanguageDTO[] Languages { get; set; }
    }
    

    [DataContract]
    public class ApprovalInfoDTO
    {
        public ApprovalInfoDTO()
        {

        }

        public ApprovalInfoDTO(DateTime approvalDate, string approvedBy)
        {
            this.ApprovalDate = approvalDate;
            this.ApprovedBy = approvedBy;
        }

        [DataMember]
        public DateTime ApprovalDate { get; set; }
        [DataMember]
        public string ApprovedBy { get; set; }

        internal static ApprovalInfoDTO CreateInstanceOrNull(DateTime? approvalDate, string approvedBy)
        {
            if (approvalDate.HasValue)
                return new ApprovalInfoDTO(approvalDate.Value, approvedBy);
            else
                return null;


        }
    }
    
    [DataContract]
    public class ProductionUploadInfoDTO
    {
        public ProductionUploadInfoDTO()
        {

        }

        public ProductionUploadInfoDTO(DateTime uploadDate)
        {
            this.UploadDate = uploadDate;
        }

        [DataMember]
        public DateTime UploadDate { get; set; }

        internal static ProductionUploadInfoDTO CreateInstanceOrNull(DateTime? uploadDate)
        {
            if (uploadDate.HasValue)
                return new ProductionUploadInfoDTO(uploadDate.Value);
            else
                return null;


        }

    }

    [DataContract]
    public class GameVersionPropertyChangeHistoryDTO
    {
        public GameVersionPropertyChangeHistoryDTO()
        {

        }


        public GameVersionPropertyChangeHistoryDTO(string propertyKey, string oldValue, string newValue, string regulation, DateTime changeDate, string changedBy, int changeType)
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
    public class DownloadInfoDTO
    {
        public DownloadInfoDTO()
        {

        }

        private DownloadInfoDTO(string uri, string fileName, long fileSize, string md5)
        {
            this.Uri = uri;
            this.FileName = fileName;
            this.FileSize = fileSize;
            this.MD5 = md5;
        }

     
        [DataMember]
        public string Uri { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public long FileSize { get; set; }
        [DataMember]
        public string MD5 { get; set; }


        public static DownloadInfoDTO CreateOrNull(string uri, string fileName, long? fileSize, string md5)
        {
            if (string.IsNullOrEmpty(uri))
                return null;

            return new DownloadInfoDTO(uri, fileName, fileSize.Value, md5);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service.DataAccessLayer
{
    public interface IComponentRecord<TComponentVersion>
      where TComponentVersion : class
    {
        int ComponentType { get; }

        EntitySet<TComponentVersion> Versions { get; }
        bool IsExternal { get; }
    }

    public interface IComponentVersionRecord<TComponentVersionProperty, TComponentVersionLanguage, TComponentVersionRegulation, TComponentVersionPropertyHistory>
        where TComponentVersionProperty : class
        where TComponentVersionLanguage : class
        where TComponentVersionRegulation : class
        where TComponentVersionPropertyHistory : class
    {
        string CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        Guid Component_ID { get; set; }
        System.Data.Linq.EntitySet<TComponentVersionRegulation> ComponentVersionRegulation { get; }
        Guid ComponentVersion_ID { get; set; }
        System.Data.Linq.EntitySet<TComponentVersionProperty> ComponentVersionProperties { get; }
        System.Data.Linq.EntitySet<TComponentVersionPropertyHistory> ComponentVersionPropertyHistories { get; }
        System.Data.Linq.EntitySet<TComponentVersionLanguage> ComponentVersionLanguages { get; }
        int Technology { get; set; }
        string TriggeredBy { get; set; }
        long VersionAsLong { get; set; }
        string VersionFolder { get; set; }
        int PlatformType { get; set; }
    }

    public interface IComponentVersionLanguageRecord
    {
        Guid Row_ID { get; set; }
        Guid ComponentVersion_ID { get; set; }
        string Regulation { get; set; }
        string ArtifactoryLanguage { get; set; }
        string Language { get; set; }
        string LanguageHash { get; set; }
        DateTime? ProductionUploadDate { get; set; }
        DateTime? QAApprovalDate { get; set; }
        string QAApprovalUser { get; set; }

    }

    public interface IComponentVersionPropertyRecord
    {
        string ChangedBy { get; set; }
        Guid ComponentVersion_ID { get; set; }
        bool IsUserAdded { get; set; }
        DateTime? LastChange { get; set; }
        string PropertyKey { get; set; }
        string PropertyName { get; set; }
        string PropertySet { get; set; }
        string PropertyValue { get; set; }
        string Regulation { get; set; }
        Guid Row_ID { get; set; }
    }

    public interface IComponentVersionRegulationRecord
    {

        string DownloadUri { get; set; }
        string FileName { get; set; }
        long? FileSize { get; set; }
        Guid ComponentVersion_ID { get; set; }
        string MD5 { get; set; }
        string Regulation { get; set; }
        Guid Row_ID { get; set; }
        string SHA1 { get; set; }
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Services;
using Spark.Infra.Types;

namespace GamesPortal.Client.Interfaces.Entities
{
    public class GameVersion
    {
        public GameVersion()
        {
        }
        public Guid Id { get; set; }
        public GameInfrastructure Infrastructure { get; set; }
        public VersionNumber Version { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public string TriggeredBy { get; set; }

        public GamingComponentCategory Category { get; set; }

        public GameVersionRegulation[] Regulations { get; set; }

        public GameVersionPropertyChangedHistory[] PropertiesChangeHistory { get; set; }
        

        public override string ToString()
        {
            return this.Version.ToString();
        }

        private bool IsWrapper()
        {
            return this.Category == GamingComponentCategory.Wrapper;
        }

        public IEnumerable<RegulationType> GetSupportedRegulations()
        {
            return this.Regulations.Select(x => x.RegulationType).Distinct().ToArray();
        }

        public bool CanQAApprove(IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            return this.Regulations.Any(reg => this.CanQAApprove(reg.RegulationType, mandatoryLanguagesProvider));

        }

        public bool CanPMApprove(IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            return this.Regulations.Any(reg => this.CanPMApprove(reg.RegulationType, mandatoryLanguagesProvider)); 
        }
        
        public RegulationWithMandatoryLanguages[] GetRegulationsWithMandatoryUnApprovedLanguages()
        {
          
            var result = new List<RegulationWithMandatoryLanguages>();

            
            foreach(var regulation in this.Regulations)
            {
                var languages = this.GetUnApprovedMandatoryLanguages(regulation);
                if (languages.Length > 0)
                    result.Add(new RegulationWithMandatoryLanguages(regulation.RegulationType, languages));
            }

            return result.ToArray();
                                   
        }

        public Language[] GetUnApprovedMandatoryLanguages(RegulationType regulationType)
        {
            var regulation = FindRegulation(regulationType);
            if (regulation.Any())
                return GetUnApprovedMandatoryLanguages(regulation.First());

            return new Language[0];
        }

        private Language[] GetUnApprovedMandatoryLanguages(GameVersionRegulation regulation)
        {
            if (IsWrapper())
            {
                return new Language[0];
            }

            return regulation.GetUnApprovedMandatoryLanguages();
        }



        public Language[] GetUnApprovedLanguages()
        {
            
            return this.Regulations.SelectMany(reg => this.GetUnApprovedLanguages(reg))
                            .Distinct()
                            .ToArray();
        }

        public IEnumerable<Language> GetUnApprovedLanguages(RegulationType regulationType)
        {
            var regulation = FindRegulation(regulationType);
            if (regulation.Any())
                return GetUnApprovedLanguages(regulation.First());

            return new Language[0];
        }

        private IEnumerable<Language> GetUnApprovedLanguages(GameVersionRegulation regulation)
        {
            if (IsWrapper())
            {
                return new Language[0];
            }

            return regulation.GetUnApprovedLanguages();
        }


        public class RegulationWithMandatoryLanguages
        {
            public RegulationWithMandatoryLanguages(RegulationType regulationType, Language[] languages)
            {
                this.RegulationType = regulationType;
                this.Languages = languages;
            }

            public RegulationType RegulationType { get; private set; }
            public Language[] Languages { get; private set; }
        }

        public RegulationWithMandatoryLanguages[] GetMissingMandatoryLanguagesPerRegulations(IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            var result = new List<RegulationWithMandatoryLanguages>();

            foreach(var regulation in this.Regulations)
            {
                var missingLanguages = GetMissingMandatoryLanguages(regulation,  mandatoryLanguagesProvider);
                if (missingLanguages.Any())
                {
                    result.Add(new RegulationWithMandatoryLanguages(regulation.RegulationType, missingLanguages.ToArray()));
                }
            }

            return result.ToArray();
        }

        public IEnumerable<Language> GetMissingMandatoryLanguages(RegulationType regulationType, IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            var regulation = FindRegulation(regulationType);

            if(regulation.Any())
                return GetMissingMandatoryLanguages(regulation.First(), mandatoryLanguagesProvider);

            return new Language[0];
        }

        private IEnumerable<Language> GetMissingMandatoryLanguages(GameVersionRegulation regulation, IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            if (IsWrapper())
            {
                return new Language[0];
            }
            
            return regulation.GetMissingMandatoryLanguages(mandatoryLanguagesProvider);
            
        }

        public GameVersionRegulation[] GetRegulationsThatCanApprovedByQA(IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            return this.Regulations.Where(r => this.CanQAApprove(r.RegulationType, mandatoryLanguagesProvider))
                        .ToArray();
        }

        public bool CanQAApprove(RegulationType regulationType, IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            return this.IsWrapper() || FindRegulation(regulationType).Any(reg => reg.CanQAApprove(this.Category, mandatoryLanguagesProvider));
        }
        

        public bool CanPMApprove(RegulationType regulationType, IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            return IsWrapper() || FindRegulation(regulationType).Any(reg => reg.CanPMApprove(this.Category, mandatoryLanguagesProvider));
        }

        private Optional<GameVersionRegulation> FindRegulation(RegulationType regulationType)
        {
            var regulation = this.Regulations.FirstOrDefault(reg => reg.RegulationType == regulationType);
            if (regulation == null)
                return Optional<GameVersionRegulation>.None();
            else
                return Optional<GameVersionRegulation>.Some(regulation);
        }
    }



    public class GameVersionRegulation
    {
        public GameVersionRegulation()
        {

        }

        public GameVersionRegulation(
                                    RegulationType regulationType,
                                    DownloadInfo downloadInfo,
                                    IGameVersionApprovalInfo approvalInfo,
                                    GameVersionRegulationLanguage[] languages)
        {
            this.RegulationType = regulationType;
            this.DownloadInfo = downloadInfo;
            this.ApprovalInfo = approvalInfo;
            this.Languages = languages;
        }
        
        public RegulationType RegulationType { get; set; }

        public DownloadInfo DownloadInfo { get; set; }

        IGameVersionApprovalInfo ApprovalInfo { get; set; }

        public string ApprovalStatusDescription
        {
            get { return ApprovalInfo.Status; }
        }

        public GameVersionRegulationLanguage[] Languages { get; set; }

        internal Language[] GetUnApprovedMandatoryLanguages()
        {
            return this.Languages.Where(lng => lng.ApprovalInfo.IsMandatory && !lng.ApprovalInfo.IsApproved)
                          .Select(lng => lng.Language)
                          .ToArray();
        }


        internal IEnumerable<Language> GetUnApprovedLanguages()
        {
            return this.Languages.Where(lng => !lng.ApprovalInfo.IsApproved)
                                 .Select(lng => lng.Language)
                                 .ToArray();
        }

        internal IEnumerable<Language> GetMissingMandatoryLanguages(IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            var missingLanguages = new List<Language>();
            foreach (var mandatoryLanguage in mandatoryLanguagesProvider.GetManatoryLanguages(RegulationType))
            {
                if (this.Languages.All(lng => lng.Language != mandatoryLanguage))
                    missingLanguages.Add(mandatoryLanguage);
            }
            
            return missingLanguages;


        }

        public bool CanQAApprove(GamingComponentCategory componentCategory, IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            
            if (!this.ApprovalInfo.CanQAApprove) //it means is already QA approved
                return false;

            return componentCategory == GamingComponentCategory.Wrapper
                    || (!GetUnApprovedMandatoryLanguages().Any()
                        && !GetMissingMandatoryLanguages(mandatoryLanguagesProvider).Any());

        }

        public bool CanPMApprove(GamingComponentCategory componentCategory, IMandatoryLanguagesProvider mandatoryLanguagesProvider)
        {
            if(!this.ApprovalInfo.CanPMApprove) //it means is already PM approved
            {
                return false;
            }
            
            return componentCategory == GamingComponentCategory.Wrapper 
                    || (this.ApprovalInfo.IsQAApproved
                        && !GetUnApprovedMandatoryLanguages().Any()
                        && !GetMissingMandatoryLanguages(mandatoryLanguagesProvider).Any());
            
        }
        
    }

    
    public interface ILanguageApprovalInfo 
    {
        string Status { get; }
        bool IsMandatory { get; }
        bool IsApproved { get; }
    }
    public class GameVersionRegulationLanguage
    {
     
        public GameVersionRegulationLanguage(Language language, ILanguageApprovalInfo approvalInfo)
        {
            this.Language = language;
            this.ApprovalInfo = approvalInfo;
        }
        
        public Language Language { get; }

        public ILanguageApprovalInfo ApprovalInfo { get; }
        
    }
    

    public class GameVersionPropertyChangedHistory
    {

        public GameVersionPropertyChangedHistory(string propertyKey, string oldValue, string newValue, string regulation, DateTime changeDate, string changedBy, ChangeType changeType)
        {
            this.PropertyKey = propertyKey;
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.Regulation = regulation;
            this.ChangeDate = changeDate;
            this.ChangedBy = changedBy;
            this.ChangeType = changeType;
        }

        
        public string PropertyKey { get; set; }
        
        public string OldValue { get; set; }
        
        public string NewValue { get; set; }
        
        public string Regulation { get; set; }
        
        public DateTime ChangeDate { get; set; }
        
        public string ChangedBy { get; set; }

        public ChangeType ChangeType { get; set; }
    }


    public interface IProductionUploadInfo
    {
        bool IsInProduction { get; }
        string Description { get; }
    }

    public interface IApproval
    {
        bool IsApproved { get; }
        DateTime? ApprovalDate { get; }
    }

    public interface IGameVersionApprovalInfo 
    {
        string Status { get; }
        bool CanPMApprove { get; }
        bool CanQAApprove { get; }

        bool IsQAApproved { get; }
    }

    
    
}

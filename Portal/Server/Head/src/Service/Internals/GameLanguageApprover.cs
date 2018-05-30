using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service
{
    public interface IGameLanguageApprover
    {
        GameLanguageApprover_Response Approve(GameLanguageApprover_Request request);
    }

    public class GameLanguageApprover_Response
    {
        public GameLanguageApprover_Response(IEnumerable<Game> games, IEnumerable<GameVersion> gameVersions)
        {
            this.AffectedGames = games;
            this.AffectedGameVersions = gameVersions;
        }

        public IEnumerable<Game> AffectedGames { get; private set; }
        public IEnumerable<GameVersion> AffectedGameVersions { get; private set; }
    }

    public class GameLanguageApprover_Request
    {
        public GameLanguageApprover_Request(DateTime approvalDate, string approvalUser, Guid gameVersionId, Language[] languages)
        {
            this.ApprovalDate = approvalDate;
            this.ApprovalUser = approvalUser;
            this.GameVersionId = gameVersionId;
            this.Languages = languages;
        }

        public Guid GameVersionId { get; private set; }
        public DateTime ApprovalDate { get; private set; }
        public string ApprovalUser { get; private set; }
        public Language[] Languages { get; private set; }
    }

    public class GameLanguageApprover : IGameLanguageApprover
    {

        IGamesPortalDataContext _dbContext;
        public GameLanguageApprover(IGamesPortalDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        

        public GameLanguageApprover_Response Approve(GameLanguageApprover_Request request)
        {

            var affectedGameVersions = ApproveLanguagesPerRegulationAndGetAffectedGameVersions(request)
                                      .GroupBy(gameVersion => gameVersion.GameVersion_ID)
                                      .Select(group => group.First())
                                      .ToArray();

            var affectedGames = affectedGameVersions
                                .GroupBy(gameVersion => gameVersion.Game_ID)
                                .Select(group => group.First().Game)
                                .ToArray();
            

            return new GameLanguageApprover_Response(affectedGames, affectedGameVersions);
        }

        private IEnumerable<GameVersion> ApproveLanguagesPerRegulationAndGetAffectedGameVersions(GameLanguageApprover_Request request)
        {
            var affectedGameVersions = new List<GameVersion>();
            foreach (var langPerRegulation in GetLanguagesPerRegulation(request))
            {
                langPerRegulation.Approve(request)
                                 .AppendAffectedGameVersion(affectedGameVersions);
            }

            return affectedGameVersions;
        }

        private string[] GetLanguagesHashes(Guid gameVersionId, Language[] languages)
        {
            var gameVersionRecord = _dbContext.GetTable<GameVersion>().FirstOrDefault(row => row.GameVersion_ID == gameVersionId);

            if (gameVersionRecord == null)
                throw new ArgumentException($"There is no game version with id {gameVersionId} in the system.");

            return gameVersionRecord.GameVersion_Languages
                                            .Where(row => languages.Select(l => l.Name).Contains(row.Language))
                                            .Select(row => row.LanguageHash)
                                            .ToArray();



        }


        IEnumerable<LanguagesPerRegulation> GetLanguagesPerRegulation(GameLanguageApprover_Request request)
        {
            var languagesPerRegulations = new List<LanguagesPerRegulation>();

            var gameInfrastructure = _dbContext.GetTable<GameVersion>().Where(record => record.GameVersion_ID == request.GameVersionId)
                                                                         .Select(record => new GameInfrastructureDTO(record.Technology, record.PlatformType))
                                                                         .FirstOrDefault();

            if (gameInfrastructure == null)
                throw new ArgumentException($"There is no game version with the ID {request.GameVersionId} in the database");

            var languageHashes = GetLanguagesHashes(request.GameVersionId, request.Languages);

            var gameVersions = _dbContext.GetTable<GameVersion>()
                                             .Where(gameVersion => gameVersion.PlatformType == (int)gameInfrastructure.PlatformType
                                                                   && gameVersion.Technology == (int)gameInfrastructure.GameTechnology
                                                                   && gameVersion.GameVersion_Languages.Any(lng => languageHashes.Contains(lng.LanguageHash)))
                                             .ToArray();
            foreach (var gameVersion in gameVersions)
            {
                foreach (var regulation in gameVersion.GameVersion_Languages
                                                  .Select(lng => lng.Regulation)
                                                  .Distinct())
                {
                    languagesPerRegulations.Add(new LanguagesPerRegulation(gameVersion, regulation));
                }
            }

            return languagesPerRegulations;
        }

      

        

        private class LanguagesPerRegulation
        {
            public LanguagesPerRegulation(GameVersion gameVersion, string regulation)
            {
                GameVersion = gameVersion;
                Regulation = regulation;
                RegulationSpecificLanguageRecords = gameVersion.GameVersion_Languages.Where(lng => lng.Regulation == regulation).ToArray();
                OldApprovedLanguages = ComputeQAApprovedLanguagesPropertyValue(RegulationSpecificLanguageRecords);


            }


            GameVersion GameVersion { get; set; }
            string Regulation { get; set; }
            
            string OldApprovedLanguages { get; set; }
           


            private string ComputeQAApprovedLanguagesPropertyValue(IEnumerable<GameVersion_Language> languageRecords)
            {
                return string.Join(",", languageRecords.Where(lng => lng.QAApprovalDate != null)
                                                       .Select(lng => lng.ArtifactoryLanguage)
                                                       .OrderBy(lng => lng));
            }

            GameVersion_Language[] RegulationSpecificLanguageRecords
            {
                get;set;
            }

            public IApproveResult Approve(GameLanguageApprover_Request request)
            {
                if (UpdateLanguageRecords(request))
                {
                    AddHistoryRecord(request);
                    return new GameVersionAffectedResult(this.GameVersion);
                }

                return new NoGameVersionAffected();

            }

            public interface IApproveResult
            {
                void AppendAffectedGameVersion(IList<GameVersion> affectedGameVersions);
            }


            private class GameVersionAffectedResult : IApproveResult
            {
                public GameVersionAffectedResult(GameVersion gameVersion)
                {
                    _gameVersion = gameVersion;
                }

                GameVersion _gameVersion;
                public void AppendAffectedGameVersion(IList<GameVersion> affectedGameVersions)
                {
                    affectedGameVersions.Add(_gameVersion);
                }
            }


            private class NoGameVersionAffected : IApproveResult
            {
                public void AppendAffectedGameVersion(IList<GameVersion> affectedGameVersions)
                {
                    //nothing to do here
                }
            }


            private void AddHistoryRecord(GameLanguageApprover_Request request)
            {
                
                var historyRecord = new GameVersion_Property_History();
                historyRecord.ChangeDate = request.ApprovalDate;
                historyRecord.ChangedBy = request.ApprovalUser;
                historyRecord.GameVersion_ID = GameVersion.GameVersion_ID;
                historyRecord.OldValue = OldApprovedLanguages;
                historyRecord.NewValue = ComputeQAApprovedLanguagesPropertyValue(RegulationSpecificLanguageRecords);
                historyRecord.PropertyKey = Artifactory.LanguageProperty.Language_QAApproved;
                historyRecord.Regulation = Regulation;
                
                if (string.IsNullOrEmpty(historyRecord.OldValue))
                    historyRecord.ChangeType = (int)RecordChangeType.Added;
                else
                    historyRecord.ChangeType = (int)RecordChangeType.Changed;

                GameVersion.GameVersion_Property_Histories.Add(historyRecord);
            }

            private bool UpdateLanguageRecords(GameLanguageApprover_Request request)
            {
                bool changed = false;
                foreach (var languageRecord in RegulationSpecificLanguageRecords.Where(langRecord => request.Languages.Select(l => l.Name).Contains(langRecord.Language)))
                {
                    if (!languageRecord.QAApprovalDate.HasValue)
                    {
                        languageRecord.QAApprovalDate = request.ApprovalDate;
                        languageRecord.QAApprovalUser = request.ApprovalUser;
                        changed = true;
                    }
                }

                return changed;
            }
        }



        

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service.Synchronizers
{
    public interface IGameLanguageToArtifactorySynchronizer : ISynchronizer
    {
    }

    public class GameLanguageToArtifactorySynchronizer : Synchronizer, IGameLanguageToArtifactorySynchronizer
    {
        public GameLanguageToArtifactorySynchronizer(IGamesPortalInternalServices services)
            : base(services)
        {

        }


        protected override void DoWork()
        {
            Services.GamesLanguageQAApprovalStatusNormalizer().NormalizeApprovalStatusForAllLanguagesWithTheSameHash();

            using (var dbContext = Services.CreateGamesPortalDBDataContext())
            {
                ProcessQueue(dbContext);
            }
        }

        private void ProcessQueue(IGamesPortalDataContext dbContext)
        {
            foreach (var record in ReadGameVersions(dbContext))
            {
                try
                {
                    UpdateArtifactory(record.Item1);

                    DeleteRecordsFromQueue(dbContext, record.Item2);

                    dbContext.SubmitChanges();
                }
                catch(Exception ex)
                {
                    throw new ApplicationException($"Failed to update Artifactory with game languages approval status for Game = {record.Item1.Game.GameName}; GameVersion_ID = {record.Item1.GameVersion_ID}; GameVersion = {record.Item1.VersionFolder}", ex);
                }
            }


        }

        private static void DeleteRecordsFromQueue(IGamesPortalDataContext dbContext, GameVersion_Language_ToArtifactorySyncQueue[] toArtifactorySyncQueueRecords)
        {
            foreach (var queueRecord in toArtifactorySyncQueueRecords)
            {
                dbContext.GetTable<GameVersion_Language_ToArtifactorySyncQueue>().DeleteOnSubmit(queueRecord);
            }
        }

        private void UpdateArtifactory(GameVersion gameVersionRecord)
        {
            foreach (var regulation in GetAvailableRegulations(gameVersionRecord))
            {
               
                var approvedLanguages = GetApprovedLanguagesForRegulation(gameVersionRecord, regulation);
                if (approvedLanguages.Length == 0)
                {
                    DeleteApprovedLanguagesProperty(gameVersionRecord, regulation);
                }
                else
                {
                    UpdateApprovedLanguagesProperty(gameVersionRecord, regulation, approvedLanguages);
                }
                
            }
        }

        private void DeleteApprovedLanguagesProperty(GameVersion gameVersionRecord, string regulation)
        {
            var repositoryDescriptor = GetRepository(gameVersionRecord);

            try
            {
                var requestMessage = new DeleteArtifactPropertiesRequest(gameVersionRecord.Game.MainGameType,
                                                                regulation,
                                                                gameVersionRecord.VersionFolder,
                                                                LanguageProperty.Language_QAApproved);

                repositoryDescriptor.Repository.DeleteArtifactProperties(requestMessage);
            }
            catch(Exception ex)
            {
                throw new ApplicationException($"Failed to delete artifactory property {LanguageProperty.Language_QAApproved} from Repository = {repositoryDescriptor.Repository.RepositoryName}; Folder = {repositoryDescriptor.Repository.GetRootFolderRelativeUrl()}; Regulation = {regulation}; GameType = {gameVersionRecord.Game.MainGameType} Version = {gameVersionRecord.VersionFolder}", ex);
            }
        }


        private void UpdateApprovedLanguagesProperty(GameVersion gameVersionRecord, string regulation, string[] approvedLanguages)
        {
            var repositoryDescriptor = GetRepository(gameVersionRecord);

            try
            {
                var requestMessage = new UpdateArtifactPropertiesRequest(gameVersionRecord.Game.MainGameType,
                                                                regulation,
                                                                gameVersionRecord.VersionFolder,
                                                                LanguageProperty.BuildLanguageQaApprovedProperty(approvedLanguages));


                repositoryDescriptor.Repository.UpdateArtifactProperties(requestMessage);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to updated artifactory property {LanguageProperty.Language_QAApproved} in Repository = {repositoryDescriptor.Repository.RepositoryName}; Folder = {repositoryDescriptor.Repository.GetRootFolderRelativeUrl()}; Regulation = {regulation}; GameType = {gameVersionRecord.Game.MainGameType} Version = {gameVersionRecord.VersionFolder}", ex);
            }
        }
        

        private IArtifactoryRepositoryDescriptor GetRepository(GameVersion gameVersionRecord)
        {
            return Services.ArtifactorySynchronizationManager.FindRepositoryDescriptor(new GameInfrastructureDTO(gameVersionRecord.Technology,
                                                                                                      gameVersionRecord.PlatformType),
                                                                        gameVersionRecord.Game.IsExternal,
                                                                        (GamingComponentCategory)gameVersionRecord.Game.ComponentCategory);
                                              
        }

        private string[] GetApprovedLanguagesForRegulation(GameVersion gameVersionRecord, string regulation)
        {
            return gameVersionRecord.GameVersion_Languages.Where(record => string.Compare(record.Regulation, regulation) == 0
                                                                    && record.QAApprovalDate != null)
                                                   .Select(record => record.ArtifactoryLanguage)
                                                   .OrderBy(language => language)
                                                   .ToArray();
        }

        private IEnumerable<string> GetAvailableRegulations(GameVersion gameVersionRecord)
        {
            return gameVersionRecord.GameVersion_Regulations.Select(record => record.Regulation);
        }
        

        private IEnumerable<Tuple<GameVersion, GameVersion_Language_ToArtifactorySyncQueue[]>> ReadGameVersions(IGamesPortalDataContext dbContext)
        {

            var query = from gameVersionRecord in dbContext.GetTable<GameVersion>()
                        join toArtifactorySyncQueueRecord in dbContext.GetTable<GameVersion_Language_ToArtifactorySyncQueue>()
                        on gameVersionRecord.GameVersion_ID equals toArtifactorySyncQueueRecord.GameVersion_ID
                        select new { gameVersionRecord, toArtifactorySyncQueueRecord };

            return query.ToArray().GroupBy(item => item.gameVersionRecord.GameVersion_ID)
                           .Select(group => new Tuple<GameVersion, GameVersion_Language_ToArtifactorySyncQueue[]>(group.First().gameVersionRecord, 
                                                                                                                  group.Select(x => x.toArtifactorySyncQueueRecord).ToArray()));
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.PubSubEvents;

namespace GamesPortal.Client.Interfaces.Services
{
    public interface IGamesRepository
    {
        Game GetGame(Guid gameId);
        Game[] GetAllGames();
        GameVersion[] GetGameVersions(Guid gameId);
        string[] GetAvailableQAApprovalStates();
        string[] GetAvailablePMApprovalStates();
        void QAApprove(Guid gameVersionID, RegulationType[] regulations);
        void PMApprove(Guid gameVersionID, RegulationType[] regulations);
        void LanguageApprove(Guid gameVersionId, string[] languages);
        IMandatoryLanguagesProvider GetMandatoryLanguagesPerRegulationProvider();
    }


    public interface IMandatoryLanguagesProvider
    {
        IEnumerable<Language> GetManatoryLanguages(RegulationType regulation);
    }

    public interface IGamesRepositorySynchronizer
    {
        Game RefreshGame(Guid gameId);
        void GameRemoved(Guid gameId);
        void ForceSynchronization();
        void ForceGameSynchronization(Game game);
    }

}

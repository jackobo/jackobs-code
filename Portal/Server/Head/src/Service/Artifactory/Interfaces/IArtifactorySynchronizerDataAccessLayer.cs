using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common.Databases;

namespace GamesPortal.Service.Artifactory
{
    public interface IArtifactorySynchronizerDataAccessLayer : IDisposable
    {
        void InsertGame(GamesPortal.Service.DataAccessLayer.Game game);
        GamesPortal.Service.DataAccessLayer.Game GetGame(int gameType, bool isExternal);
        void SubmitChanges();

        string GetGameName(int gameType);
        GameTypeDescriptor[] GetGameTypes(int mainGameType);
        bool HasChanges { get; }
    }
}

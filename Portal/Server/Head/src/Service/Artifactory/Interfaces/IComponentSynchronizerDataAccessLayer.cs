using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Service.DataAccessLayer;
using Spark.Infra.Data.LinqToSql;

namespace GamesPortal.Service.Artifactory
{
    public interface IArtifactorySynchronizerDataAccessLayer : IDisposable
    {
        Game GetGame(int componentId, bool isExternal);
        string GetGameName(int gameType);
        GameTypeDescriptor[] GetGameTypes(int mainGameType);
        void SubmitChanges(Game game);
    }
}

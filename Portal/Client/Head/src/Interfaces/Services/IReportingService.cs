using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark.Wpf.Common;

namespace GamesPortal.Client.Interfaces.Services
{
    public interface IReportingService
    {
        Entities.LatestApprovedGameVersionForEachRegulation[] GetLatestApprovedGameVersionForEachRegulationAndGameType();
        Entities.LatestGameVersionForRegulation[] GetLatestGameVersionForEachRegulation();
        Entities.GameVersionRelease[] GetGameReleases(DateTimeInterval inPeriod);

        Entities.NeverApprovedGame[] GetNeverApprovedGames();

        Entities.ApprovedGameVersion[] GetQAApprovedGamesInPeriod(DateTime startDate, DateTime endDate);

    }
}

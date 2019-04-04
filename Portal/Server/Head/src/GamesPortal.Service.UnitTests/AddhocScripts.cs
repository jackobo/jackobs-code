using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using Spark.Infra.Configurations;
using GamesPortal.Service.DataAccessLayer;
using NSubstitute;
using NUnit.Framework;


namespace GamesPortal.Service
{
    [TestFixture]
    [Explicit]
    public class AddhocScripts
    {
        [Test]
        [Explicit]
        public void FixGamesTabState()
        {
            /*
            var internalServices = Helpers.GamesPortalInternalServicesHelper.Create();

            
            internalServices.ConfigurationReader.ReadSection<ArtifactorySettings>().Returns(new ArtifactorySettings() { BaseUrl = "http://artifactory.888holdings.corp:8081/" });
            
            var restClientFactory = new Artifactory.ArtifactoryRestClientFactory(internalServices);

            var repo = new ArtifactoryGamesRepository("modernGame-local", "Games", restClientFactory);



            using (var dbContext = new DataAccessLayer.GamesPortalDataContext("Data Source=10.20.40.158;Initial Catalog=GamesPortal;Integrated Security=True"))
            {
                var q = from game in dbContext.GetTable<Game>()
                        join gversion in dbContext.GetTable<GameVersion>() on game.Game_ID equals gversion.Game_ID
                        join gvproperty in dbContext.GetTable<GameVersion_Property>() on gversion.GameVersion_ID equals gvproperty.GameVersion_ID
                        where gvproperty.PropertyKey == "GTab.State"
                        select new { game.MainGameType, gvproperty.Regulation, gversion.VersionFolder };

                var records = q.OrderBy(row => row.MainGameType).ThenBy(row => row.Regulation).ThenBy(row => row.VersionFolder).ToArray();

                var length = records.Length;

                for (int i = 0; i < length; i++)
                {

                    Console.WriteLine(string.Format("{0} of {1}", i + 1, records.Length));

                    var record = records[i];
                    var a = repo.GetArtifact(record.MainGameType, record.Regulation, record.VersionFolder);



                    var gamesTabStateProperty = a.Properties.FirstOrDefault(p => p.Key == "GTab.State");

                    if (gamesTabStateProperty == null)
                        continue;


                    repo.UpdateArtifactProperties(new UpdateArtifactPropertiesRequest(record.MainGameType, record.Regulation, record.VersionFolder, new ArtifactoryProperty("GamesTab.State", gamesTabStateProperty.Values)));

                    repo.DeleteArtifactProperties(new DeleteArtifactPropertiesRequest(record.MainGameType, record.Regulation, record.VersionFolder, "GTab.State"));
                    

                }
            }
            */
        }

    }
}

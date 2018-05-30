using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using GamesPortal.Service.Helpers;
using Spark.Infra.Types;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using NSubstitute;

namespace GamesPortal.Service
{
    [TestFixture]
    public class GamesPortalServiceTests
    {
        IGamesPortalInternalServices _internalServices;
        GamesPortalService _gamesPortalService;

        [SetUp]
        public void Setup()
        {
            _internalServices = GamesPortalInternalServicesHelper.Create();
            _gamesPortalService = new GamesPortalService(_internalServices);
        }
        
       

      


       


     

        private GameVersion_Regulation CreateGameVersion_RegulationRecord(Guid gameVersionID, string regulation, string md5 = "md5", string fileName = "file.zip")
        {
            var record = new GameVersion_Regulation();
            record.GameVersion = CreateGameVersionRecord(gameVersionID);
            record.DownloadUri = string.Format("http://localhost/download/{0}/file.zip", regulation);
            record.FileName = fileName;
            record.FileSize = 1024;
            record.GameVersion_ID = gameVersionID;
            record.MD5 = md5;
            record.Regulation = regulation;
            record.SHA1 = "sha1";
            record.GameVersionRegulation_ID = Guid.NewGuid();

            return record;
            
        }

        private GameVersion CreateGameVersionRecord(Guid gameVersionID)
        {

            var gameRecord = new Game();
            gameRecord.Game_ID = Guid.NewGuid();
            gameRecord.GameName = "Elm Street";
            gameRecord.MainGameType = 130017;
            gameRecord.IsExternal = false;

            GameVersion gameVersionRecord = new GameVersion();

            gameVersionRecord.CreatedBy = "florin";
            gameVersionRecord.CreatedDate = DateTime.Today;
            gameVersionRecord.Game_ID = gameRecord.Game_ID;
            gameVersionRecord.Game = gameRecord;
            gameVersionRecord.Game_ID = gameRecord.Game_ID; 
            gameVersionRecord.GameVersion_ID = gameVersionID;
            gameVersionRecord.Technology = 0;
            gameVersionRecord.PlatformType = (int)ArtifactoryHelper.DEFAULT_PLATFORM_TYPE;
            gameVersionRecord.TriggeredBy = "me";
            gameVersionRecord.VersionFolder = "1.0.1.1";
            gameVersionRecord.VersionAsLong = VersionNumber.Parse("1.0.1.1").ToLong();

            return gameVersionRecord;

            

        }
    }
}

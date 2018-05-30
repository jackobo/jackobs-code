using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using GamesPortal.Service.GGPVersioning;
using GamesPortal.Service.Helpers;
using GamesPortal.Service.SDM;
using Spark.Infra.Types;
using NSubstitute;

namespace GamesPortal.Service.Helpers
{
    public static class MockRecordsFactory
    {
        public static Game GameRecord(int mainGameType, params GameTypeDescriptor[] gameTypes)
        {
            return GameRecord(mainGameType, false, gameTypes);
        }

        

        public static Game GameRecord(int mainGameType, bool isExternal, params GameTypeDescriptor[] gameTypes)
        {
            var game = new Game() { Game_ID = Guid.NewGuid(), MainGameType = mainGameType, IsExternal = isExternal, ComponentCategory = (int)GamingComponentCategory.Game };
            game.GameName = gameTypes.Select(gt => gt.GameName).FirstOrDefault();

            foreach (var gt in gameTypes)
            {
                game.GameTypes.Add(new DataAccessLayer.GameType() { Game_ID = game.Game_ID, GameType_ID = gt.GameType, Name = gt.GameName, Operator_ID = (int)gt.OperatorId, Row_ID = Guid.NewGuid() });
            }

            game.ComponentCategory = (int)GamingComponentCategory.Game;

            return game;
        }

        
        public static GameVersion GameVersionRecord(Guid versionId, string version, Guid gameID, GameTechnology technology = GameTechnology.Html5, PlatformType platformType = ArtifactoryHelper.DEFAULT_PLATFORM_TYPE)
        {
            var gameVersion = new GameVersion()
            {
                CreatedBy = "florin",
                CreatedDate = DateTime.Today,
                Game_ID = gameID,
                GameVersion_ID = versionId,
                Technology = (int)technology,
                PlatformType = (int)platformType,
                TriggeredBy = "florin",
                VersionFolder = version,
                VersionAsLong = new VersionNumber(version).ToLong()
            };

            return gameVersion;
        }

        public static GameVersion GameVersionRecord(Guid versionId, string version, Game game, GameTechnology technology = GameTechnology.Html5, PlatformType platformType = ArtifactoryHelper.DEFAULT_PLATFORM_TYPE)
        {
            var gameVersion = new GameVersion()
            {
                CreatedBy = "florin",
                CreatedDate = DateTime.Today,
                Game_ID = game.Game_ID,
                Game = game,
                GameVersion_ID = versionId,
                Technology = (int)technology,
                PlatformType = (int)platformType,
                TriggeredBy = "florin",
                VersionFolder = version,
                VersionAsLong = new VersionNumber(version).ToLong()
            };

            return gameVersion;
        }

        public static GameVersion GameVersionRecord(string version, Guid gameID, GameTechnology technology = GameTechnology.Html5, params KeyValuePair<string, ArtifactoryProperty[]>[] regulationsWithProperties)
        {
            var gameVersion = GameVersionRecord(Guid.NewGuid(), version, gameID, technology);
         

            foreach (var regulationWithProperties in (regulationsWithProperties ?? new KeyValuePair<string, ArtifactoryProperty[]>[0]))
            {
                foreach (var prop in regulationWithProperties.Value)
                {
                    gameVersion.GameVersion_Properties.Add(GameVersionPropertyRecord(gameVersion.GameVersion_ID, prop.Key, prop.ConcatValues(), regulationWithProperties.Key));
                }
            }

            return gameVersion;
        }


        public static GameVersion AddVersionRecord(this Game game, string version)
        {
            var gameVersionRecord = GameVersionRecord(version, game.Game_ID);
            game.GameVersions.Add(gameVersionRecord);
            return gameVersionRecord;
        }

        public static GameVersion_Property GameVersionPropertyRecord(ArtifactoryProperty prop, string regulation = "Gibraltar")
        {
            return GameVersionPropertyRecord(Guid.NewGuid(),
                                         prop.Key,
                                         prop.ConcatValues(),
                                         regulation);
        }

        internal static GameVersion_Language_ToArtifactorySyncQueue GameVersion_Language_ToArtifactorySyncQueueRecord(Guid versionId)
        {
            return new GameVersion_Language_ToArtifactorySyncQueue()
            {
                GameVersion_ID = versionId,
                InsertedTime = DateTime.Now
            };
        }

        public static GameVersion_Property GameVersionPropertyRecord(string key, string value, string regulation = "Gibraltar")
        {
            return GameVersionPropertyRecord(new ArtifactoryProperty(key, value), regulation);
        }



        public static GameVersion_Property GameVersionPropertyRecord(Guid gameVersionId, string propertyKey, string propertyValue, string regulation, string changedBy = "florin")
        {
            return GameVersionPropertyRecord(gameVersionId, new ArtifactoryProperty(propertyKey, propertyValue), regulation, DateTime.Now.AddYears(-1), changedBy);
        }


        public static GameVersion_Property GameVersionPropertyRecord(Guid gameVersionId, string propertyKey, string propertyValue, string regulation, DateTime lastChanged, string changedBy = "florin")
        {

            return GameVersionPropertyRecord(gameVersionId, new ArtifactoryProperty(propertyKey, propertyValue), regulation, lastChanged, changedBy);
        }


        public static GameVersion_Property GameVersionPropertyRecord(Guid gameVersionId, ArtifactoryProperty property, string regulation, DateTime lastChanged, string changedBy = "florin")
        {
            return new GameVersion_Property()
            {
                ChangedBy = changedBy,
                GameVersion_ID = gameVersionId,
                IsUserAdded = false,
                PropertyKey = property.Key,
                PropertyName = property.Name,
                PropertySet = property.SetName,
                PropertyValue = property.ConcatValues(),
                Regulation = regulation,
                Row_ID = Guid.NewGuid(),
                LastChange = lastChanged
            };
        }
        
       
        public static GameTypeDescriptor CreateGameTypeDescriptor(int gameType, string gameName, OperatorEnum operatorId)
        {
            return new GameTypeDescriptor(gameType, operatorId, gameName);
        }

        public static GameTypeDescriptor CreateGameTypeDescriptor(int gameType, string gameName)
        {
            return CreateGameTypeDescriptor(gameType, gameName, OperatorEnum.Operator888);
        }

        internal static Component_GameType Component_GameTypeRecord(int gameType, OperatorEnum operatorID, string componentId = "1234") 
        {

            return new Component_GameType()
            {
                GameType = gameType,
                Operator_ID = (int)operatorID,
                Row_ID = Guid.NewGuid(),
                Component_ID = componentId,
                IsBonusGame = false,
                Component = new Component() { Component_ID = componentId, Name = componentId }
            };
        }

        internal static SDM.GameType SDM_GameTypeRecord(int gameType, string name)
        {
            return new SDM.GameType() { GMT_ID = gameType, GMT_Description = name };
        }
               
        public static GameVersion_Language GameVersion_LanguageRecord(Guid versionId, string language, string artifactoryLanguage, string hash, string regulation = "Gibraltar")
        {
            return new GameVersion_Language()
            {
                ArtifactoryLanguage = artifactoryLanguage,
                GameVersionLanguage_ID = Guid.NewGuid(),
                GameVersion_ID = versionId,
                LanguageHash = hash,
                Language = language,
                Regulation = regulation
            };
        }

        public static GameVersion_Language AddLanguageRecord(this GameVersion gameVersion, string language, string artifactoryLanguage, string hash, string regulation = "Gibraltar")
        {
            var languageRecord = GameVersion_LanguageRecord(gameVersion.GameVersion_ID, language, artifactoryLanguage, hash, regulation);
            gameVersion.GameVersion_Languages.Add(languageRecord);
            return languageRecord;
        }

        public static GameVersion_Language GameVersion_LanguageRecord(Guid versionId, string language, string artifactoryLanguage, string hash, DateTime qaApprovalDate, string regulation = "Gibraltar", string qaApprovalUser = "florin")
        {
            var record = GameVersion_LanguageRecord(versionId, language, artifactoryLanguage, hash, regulation);
            record.QAApprovalDate = qaApprovalDate;
            record.QAApprovalUser = qaApprovalUser;
            return record;
        }


        public static GameVersion_LanguageQAApprovalInfo GameVersion_LanguageApprovalStatusRecord(Guid GameVersionLanguage_ID, DateTime? qaApprovalDate, string  qaApprovalUser = "smaster")
        {
            return new GameVersion_LanguageQAApprovalInfo()
            {
                GameVersionLanguage_ID = GameVersionLanguage_ID,
                QAApprovalDate = qaApprovalDate,
                QAApprovalUser = qaApprovalUser,
            };
        }

      

        internal static GameVersion_Regulation GameVersion_RegulationRecord(Guid gameVersionId, string regulation)
        {
            return new GameVersion_Regulation()
            {
                DownloadUri = "http://localhost",
                FileName = regulation,
                GameVersion_ID = gameVersionId,
                Regulation = regulation,
                GameVersionRegulation_ID = Guid.NewGuid(),
                FileSize = 100,
                MD5 = "123",
                SHA1 = "abc"
            };
        }
    }
    
}

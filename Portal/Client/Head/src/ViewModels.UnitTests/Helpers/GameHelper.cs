using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using Rhino.Mocks;

namespace GamesPortal.Client.ViewModels.Helpers
{
    public static class GameHelper
    {
        public static Game CreateGame(int gameType, string name, params GameTechnology[] supportedTechnologies)
        {
            return CreateGame(Guid.NewGuid(), gameType, name, supportedTechnologies);
        }

        public static Game CreateGame(Guid id, int gameType, string name, params GameTechnology[] supportedTechnologies)
        {
            var gameTypes = new GameType[]
            {
                new GameType(gameType, name, 0)
            };

            var supportedInfrastructure = supportedTechnologies.Select(th => new GameInfrastructure(th, PlatformType.Both)).ToArray();
            var game = new Game(id,
                                name,
                                gameType,
                                false,
                                GamingComponentCategory.Game,
                                supportedInfrastructure,
                                gameTypes,
                                MockRepository.GenerateStub<Interfaces.Services.IGamesRepository>());
            game.Name = name;

            game.SupportedInfrastructures = supportedInfrastructure;

            return game;
        }

        public static Game CreateGame(int gameType, params GameTechnology[] supportedTechnologies)
        {
            return CreateGame(gameType, "The Game", supportedTechnologies);

        }
    }
}

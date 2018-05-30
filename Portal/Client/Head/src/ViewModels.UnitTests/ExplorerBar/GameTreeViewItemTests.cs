using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.ViewModels.Helpers;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    [TestFixture]
    public class GameTreeViewItemTests
    {

        [Test]
        public void Constructor_AddsSupportedTechnologiesItems_AddNewItems()
        {
            var game = CreateGame(130017, GameTechnology.Flash, GameTechnology.Html5);

            var gameTreeViewItem = CreateGameTreeViewItem(game);

            Assert.AreEqual(2, gameTreeViewItem.Items.Count);
        }

        [Test]
        public void Items_WhenANewTechnologyIsAddedToTheGame_AddANewItem()
        {
            var game = CreateGame(130017, GameTechnology.Flash);

            var gameTreeViewItem = CreateGameTreeViewItem(game);

            game.SupportedInfrastructures = new GameInfrastructure[] { new GameInfrastructure(GameTechnology.Flash, PlatformType.PC), new GameInfrastructure(GameTechnology.Html5, PlatformType.Mobile) };

            Assert.AreEqual(2, gameTreeViewItem.Items.Count);
        }


        [Test]
        public void Items_WhenANewTechnologyIsAddedToTheGame_ANewItemWithTheCorrespondingTechnologyIsAdded()
        {
            var game = CreateGame(130017, GameTechnology.Flash);

            var gameTreeViewItem = CreateGameTreeViewItem(game);

            game.SupportedInfrastructures = new GameInfrastructure[] { new GameInfrastructure(GameTechnology.Flash, PlatformType.PC), new GameInfrastructure(GameTechnology.Html5, PlatformType.Mobile) };

            Assert.AreEqual(1, gameTreeViewItem.Items.Cast<GameTechnologyTreeViewItem>().Count(item => item.Infrastructure == new GameInfrastructure(GameTechnology.Html5, PlatformType.Mobile)));
        }


        [Test]
        public void Items_WhenATechnologyIsRemoved_RemoveAnItem()
        {
            var game = CreateGame(130017, GameTechnology.Flash, GameTechnology.Html5);

            var gameTreeViewItem = CreateGameTreeViewItem(game);

            game.SupportedInfrastructures = new GameInfrastructure[] {new GameInfrastructure(GameTechnology.Html5, PlatformType.Mobile) };

            Assert.AreEqual(1, gameTreeViewItem.Items.Count);

        }


        [Test]
        public void Items_WhenATechnologyIsRemoved_RemoveTheCorrectItem()
        {
            var game = CreateGame(130017, GameTechnology.Flash, GameTechnology.Html5);

            game.SupportedInfrastructures = new GameInfrastructure[] { new GameInfrastructure(GameTechnology.Flash, PlatformType.PC), new GameInfrastructure(GameTechnology.Html5, PlatformType.Mobile) };

            var gameTreeViewItem = CreateGameTreeViewItem(game);

            game.SupportedInfrastructures = new GameInfrastructure[] { new GameInfrastructure(GameTechnology.Html5, PlatformType.Mobile) };

            Assert.AreEqual(0, gameTreeViewItem.Items.Cast<GameTechnologyTreeViewItem>().Count(item => item.Infrastructure == new GameInfrastructure(GameTechnology.Flash, PlatformType.PC)));

        }

        [Test]
        public void Items_WhenATechnologyIsRemovedAndIsSelected_MoveTheSelectionToTheNextRemainingTechnology()
        {
            var game = CreateGame(130017, GameTechnology.Flash, GameTechnology.Html5);

            var gameTreeViewItem = CreateGameTreeViewItem(game);
            gameTreeViewItem.Items.Cast<GameTechnologyTreeViewItem>().First(item => item.Infrastructure.GameTechnology == GameTechnology.Flash).IsSelected = true;

            game.SupportedInfrastructures = new GameInfrastructure[] { new GameInfrastructure(GameTechnology.Html5, PlatformType.Mobile) };

            Assert.AreEqual(true, gameTreeViewItem.Items[0].IsSelected);

        }



        [Test]
        public void Items_WhenATechnologyIsRemovedAndIsSelectedAndIsTheLastItem_MoveTheSelectionToThePreviousRemainingTechnology()
        {
            var game = CreateGame(130017, GameTechnology.Flash, GameTechnology.Html5);

            var gameTreeViewItem = CreateGameTreeViewItem(game);
            gameTreeViewItem.Items.Cast<GameTechnologyTreeViewItem>().First(item => item.Infrastructure.GameTechnology == GameTechnology.Html5).IsSelected = true;

            game.SupportedInfrastructures = new GameInfrastructure[] { new GameInfrastructure(GameTechnology.Flash, PlatformType.PC) };

            Assert.AreEqual(true, gameTreeViewItem.Items[0].IsSelected);
        }


        [Test]
        public void Items_WhenTheLastItemIsRemovedAndIsSelected_SelectTheGameTreeViewItem()
        {
            var game = CreateGame(130017, GameTechnology.Html5);

            var gameTreeViewItem = CreateGameTreeViewItem(game);
            gameTreeViewItem.Items[0].IsSelected = true;

            game.SupportedInfrastructures = new GameInfrastructure[0];

            Assert.AreEqual(true, gameTreeViewItem.IsSelected);

        }

        [Test]
        public void Caption_ShouldChangeWhenGameNameIsChanged()
        {
            var game = CreateGame(130017, GameTechnology.Html5);
            var gameTreeViewItem = CreateGameTreeViewItem(game);

            game.Name = "NewGameName";

            Assert.IsTrue(gameTreeViewItem.Caption.Contains("NewGameName"));
        }


        [Test]
        public void Caption_ShouldChangeIfMainGameTypeIsChanged()
        {
            var game = CreateGame(130017, GameTechnology.Html5);
            var gameTreeViewItem = CreateGameTreeViewItem(game);

            game.MainGameType = 130018;

            Assert.IsTrue(gameTreeViewItem.Caption.Contains("130018"));
        }

        private GameTreeViewItem CreateGameTreeViewItem(Game game)
        {

            return new GameTreeViewItem(game, null, MockRepository.GenerateStub<IServiceLocator>());
        }

        private Game CreateGame(int gameType, params GameTechnology[] supportedTechnologies)
        {
            return GameHelper.CreateGame(gameType, supportedTechnologies);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.PubSubEvents;
using GamesPortal.Client.Interfaces.Services;
using GamesPortal.Client.ViewModels.Helpers;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Prism.Events;
using Rhino.Mocks;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    [TestFixture]
    public class GamesTreeViewItemTests 
    {

        [Test]
        public void Constructor_ShouldInitializeTheListOfItems()
        {
            var treeViewItem = CreateItem(CreateGame(1, "A"), CreateGame(2, "B"), CreateGame(3, "C"));
            Assert.AreEqual(3, treeViewItem.Items.Count);
        }

        [TestCase(0, "A")]
        [TestCase(1, "B")]
        [TestCase(2, "C")]
        public void Constructor_ShouldAddItemSortedByGameName(int expectedIndex, string expectedGameName)
        {
            var a = CreateGame(1, "A");
            var b = CreateGame(2, "B");
            var c = CreateGame(2, "C");
            var treeViewItem = CreateItem(c, a, b);


            var gameTreeViewItem = treeViewItem.Items[expectedIndex] as GameTreeViewItem;

            Assert.AreEqual(expectedGameName, gameTreeViewItem.Game.Name);
            

        }

        [Test]
        public void RemoveGameById_GameIsTheOnlyGameInTheList_ItemsShuldBeEmpty()
        {
            
            var container = Helpers.UnityContainerHelper.Create();
            
            var game = CreateGame(1, "A");
            var treeViewItem = CreateItem(game);

            treeViewItem.RemoveGameById(game.Id);

            Assert.AreEqual(0, treeViewItem.Items.Count);
        }


        [Test]
        public void RemoveGameById_IfTheGameIsNotFound_IgnoreIt()
        {

            var container = Helpers.UnityContainerHelper.Create();

            var game = CreateGame(1, "A");
            var treeViewItem = CreateItem(game);

            treeViewItem.RemoveGameById(Guid.NewGuid());

            Assert.AreEqual(1, treeViewItem.Items.Count);
        }

        [Test]
        public void RemoveGameById_GameIsTheOnlyGameInTheListAndIsSelected_SetIsSelectedInTheGamesTreeViewItem()
        {

            var container = Helpers.UnityContainerHelper.Create();

            var game = CreateGame(1, "A");
            var treeViewItem = CreateItem(game);

            treeViewItem.Items[0].IsSelected = true;

            treeViewItem.RemoveGameById(game.Id);

            Assert.AreEqual(true, treeViewItem.IsSelected);
        }


        [TestCase(2, 1)]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        public void RemoveGameById_GameIsSelected_SetsASiblingAsSelected(int selectedItemIndex, int expectedSiblingIndex)
        {

            var games = new List<Game>()
            {
                CreateGame(1, "A"),
                CreateGame(2, "B"),
                CreateGame(3, "C")
            };

            
            var treeViewItem = CreateItem(games.ToArray());

            treeViewItem.Items[selectedItemIndex].IsSelected = true;

            treeViewItem.RemoveGameById(games[selectedItemIndex].Id);

            Assert.AreEqual(true, treeViewItem.Items[expectedSiblingIndex].IsSelected);
        }


        [Test]
        public void AddNewGameById_ShouldCallIGamesRepository_GetGameWithTheCorrectGameId()
        {
            var gamesRepository = MockRepository.GenerateStub<IGamesRepository>();
            var newGame = CreateGame(130017, "ElmS treet");
            Guid? actualGameId = null;
            gamesRepository.Stub(gr => gr.GetGame(Guid.Empty)).IgnoreArguments()
                                          .WhenCalled(invocation => actualGameId = (Guid)invocation.Arguments[0])
                                          .Return(newGame);


            

            var treeViewItem = CreateItem(gamesRepository);

            treeViewItem.AddNewGameById(newGame.Id);

            Assert.AreEqual(newGame.Id, actualGameId.Value);


        }


        [Test]
        public void AddNewGameById_ShouldAddANewItemWithTheCorrectGameTreeViewItem()
        {
            var gamesRepository = MockRepository.GenerateStub<IGamesRepository>();
            var addedGame = CreateGame(130017, "ElmS treet");
            gamesRepository.Stub(gr => gr.GetGame(Guid.Empty))
                                          .IgnoreArguments()
                                          .Return(addedGame);



            var treeViewItem = CreateItem(gamesRepository);

            treeViewItem.AddNewGameById(addedGame.Id);

            Assert.AreEqual(1, treeViewItem.Items.Count);
            Assert.AreEqual(addedGame.Name, ((GameTreeViewItem)treeViewItem.Items[0]).Game.Name);


        }


        [TestCase("A", 0)]
        [TestCase("C", 1)]
        [TestCase("F", 2)]
        public void AddNewGameById_ShouldAddANewInTheSortOrder(string newGameName, int expectedIndex)
        {
            var gamesRepository = MockRepository.GenerateStub<IGamesRepository>();
            var addedGame = CreateGame(2, newGameName);
            gamesRepository.Stub(gr => gr.GetGame(Guid.Empty))
                                          .IgnoreArguments()
                                          .Return(addedGame);



            var treeViewItem = CreateItem(gamesRepository, CreateGame(1, "B"), CreateGame(3, "D"));

            treeViewItem.AddNewGameById(addedGame.Id);

            var gameTreeViewItem = (GameTreeViewItem)treeViewItem.Items[expectedIndex];

            Assert.AreEqual(addedGame.Name, gameTreeViewItem.Game.Name);
        }

        private Game CreateGame(int gameType, string name, bool isExternal = false)
        {
            var g = GameHelper.CreateGame(gameType, name, GameTechnology.Flash, GameTechnology.Html5);
            g.IsExternal = isExternal;
            return g;
        }

        protected GamesTreeViewItemBase CreateItem(params Game[] games)
        {
            return new GamesTreeViewItemBase(games, UnityContainerHelper.Create().Resolve<IServiceLocator>(), false, GamingComponentCategory.Game);
        }


        protected GamesTreeViewItemBase CreateItem(IGamesRepository gamesRepository, params Game[] games)
        {
            var container = UnityContainerHelper.Create();
            container.RegisterInstance<IGamesRepository>(gamesRepository);

            return new GamesTreeViewItemBase(games, container.Resolve<IServiceLocator>(), false, GamingComponentCategory.Game);
        }


        protected GamesTreeViewItemBase CreateItem(IServiceLocator serviceLocator, bool containsExternalGames, params Game[] games)
        {
            return new GamesTreeViewItemBase(games, serviceLocator, containsExternalGames, GamingComponentCategory.Game);
        }
        
    }

}

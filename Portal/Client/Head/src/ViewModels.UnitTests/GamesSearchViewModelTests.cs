using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GamesPortal.Client.ViewModels.ExplorerBar;
using GamesPortal.Client.ViewModels.Helpers;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Prism.Events;
using Rhino.Mocks;

namespace GamesPortal.Client.ViewModels
{
    [TestFixture]
    public class GamesSearchViewModelTests
    {
        
        

        [Test]
        public void Constructor_WhenInitializedFilteredListIsEmpty()
        {
            var gameSearch = CreateGamesSearchViewModel();
            Assert.AreEqual(0, gameSearch.Items.Count);
        }
        
        [Test]
        public void Constructor_WhenInitialized_SearchTextIsEmpty()
        {
            var gameSearch = CreateGamesSearchViewModel();
            Assert.AreEqual("", gameSearch.Filter);
        }


        [TestCase("street")]
        [TestCase("STREET")]
        public void Filter_ShouldBeCaseInsensitive(string criteria)
        {

            var gameSearch = CreateGamesSearchViewModel(CreateTreeViewItem("Elm Street"), CreateTreeViewItem("Millionare Genie"));

            gameSearch.Filter = criteria;
                        
            Assert.AreEqual(1, gameSearch.Items.Count);
        }




        [Test]
        public void Filter_ShouldMatchAllWords()
        {
            var gameSearch = CreateGamesSearchViewModel(CreateTreeViewItem("Elm Road"), CreateTreeViewItem("Elm Street"));

            gameSearch.Filter = "elm street";

            Assert.AreEqual(1, gameSearch.Items.Count);
            Assert.AreEqual("Elm Street", gameSearch.Items[0].Caption);
        }



        [Test]
        public void Filter_WhenIsSet_SetsIsOpenFlagToTrue()
        {
            var gameSearch = CreateGamesSearchViewModel(CreateTreeViewItem("Elm Road"), CreateTreeViewItem("Elm Street"));

            gameSearch.Filter = "elm street";

            Assert.AreEqual(true, gameSearch.IsOpen);
        }

       
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Filter_IfSetToNullOrEmpty_ReturnNoRecords(string criteria)
        {
            var gameSearch = CreateGamesSearchViewModel(CreateTreeViewItem("Elm Road"), CreateTreeViewItem("Elm Street"));

            gameSearch.Filter = "elm street";
            gameSearch.Filter = criteria;
            
            Assert.AreEqual(0, gameSearch.Items.Count);
        }

        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Filter_IfSetToNullOrEmpty_TurnIsOpenFlagToFalse(string criteria)
        {
            var gameSearch = CreateGamesSearchViewModel(CreateTreeViewItem("Elm Road"), CreateTreeViewItem("Elm Street"));

            gameSearch.Filter = "elm street";
            gameSearch.Filter = criteria;

            Assert.AreEqual(false, gameSearch.IsOpen);
        }


        [Test]
        public void Filter_GoToCommandOfTheSelectedItemIsExecuted_CloseTheSearch()
        {
            var gameSearch = CreateGamesSearchViewModel(CreateTreeViewItem("Elm Road"), CreateTreeViewItem("Elm Street"));

            gameSearch.Filter = "elm street";

            Assert.AreEqual(true, gameSearch.IsOpen);

            gameSearch.Items[0].GoToCommand.Execute(null);

            Assert.AreEqual(false, gameSearch.IsOpen);
        }

        private static TreeViewItem CreateTreeViewItem(string caption)
        {

            var serviceLocator = UnityContainerHelper.Create().Resolve<IServiceLocator>();

            return new TreeViewItem(serviceLocator) { Caption = caption };
        }
        

        private GamesSearchViewModel CreateGamesSearchViewModel(params TreeViewItem[] items)
        {
            return new GamesSearchViewModel(items);
        }

        
    }
}

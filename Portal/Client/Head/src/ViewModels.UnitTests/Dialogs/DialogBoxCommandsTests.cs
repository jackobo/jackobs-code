using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Prism.Logging;
using Rhino.Mocks;
using Spark.Wpf.Common.UIServices;

namespace GamesPortal.Client.ViewModels.Dialogs
{
    [TestFixture]
    public class DialogBoxCommandsTests
    {
        [Test]
        public void Constructor_SetsTheDialogViewModel()
        {
            var viewModel = new object();
            var dialogCommands = new DialogBoxCommands(viewModel);

            Assert.IsTrue(object.ReferenceEquals(viewModel, dialogCommands.DialogViewModel));
        }

    }

    
}

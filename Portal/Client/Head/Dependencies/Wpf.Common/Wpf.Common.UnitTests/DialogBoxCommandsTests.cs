using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spark.Wpf.Common.UIServices;

namespace Spark.Wpf.Common
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

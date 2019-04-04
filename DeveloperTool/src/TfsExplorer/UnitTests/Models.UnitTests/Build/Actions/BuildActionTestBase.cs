using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Spark.Infra.Logging;
using Spark.Infra.Windows;

namespace Spark.TfsExplorer.Models.Build.Actions
{

    public class BuildActionTestBase
    {
        protected IBuildContext BuildContext { get; set; }
        protected ILogger Logger { get; set; }

        [SetUp]
        public void Setup()
        {
            BuildContext = Substitute.For<IBuildContext>();
            
            BuildContext.FileSystemAdapter.Returns(Substitute.For<IFileSystemAdapter>());
            BuildContext.Logger.Returns(Substitute.For<ILogger>());
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    [TestFixture]
    public class QaMainBranchExplorerBarItemTests
    {

        IQaBranch _qaBranch;

        [SetUp]
        public void Setup()
        {
            _qaBranch = Substitute.For<IQaBranch>();
        }

        private QaMainBranchExplorerBarItem CreateQAMainBranchItem()
        {
            return new QaMainBranchExplorerBarItem(_qaBranch, Substitute.For<IExplorerBarItem>(), Substitute.For<IServiceLocator>());
        }
    }
}

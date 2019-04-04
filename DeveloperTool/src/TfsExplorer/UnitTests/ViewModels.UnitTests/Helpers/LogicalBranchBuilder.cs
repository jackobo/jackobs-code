using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.ViewModels.Helpers
{
    public class LogicalBranchBuilder
    {
        private int  _majorVersion = 3;


        private LogicalBranchBuilder()
        {

        }

        public static LogicalBranchBuilder LogicalBranch()
        {
            return new LogicalBranchBuilder();
        }

        public LogicalBranchBuilder WithName(int majorVersion)
        {
            _majorVersion = majorVersion;

            return this;
        }
        
        public IRootBranch Build()
        {
            var logicalBranch = Substitute.For<IRootBranch>();
            logicalBranch.Version.Returns(new RootBranchVersion(_majorVersion));
            return logicalBranch;
        }
    }
}

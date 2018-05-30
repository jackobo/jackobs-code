using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    public class LogicalBranchComponentFactory : ILogicalBranchComponentFactory
    {
        public LogicalBranchComponentFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        IServiceLocator _serviceLocator;
        public IRootBranch CreateRootBranch(Folders.RootBranchFolder logicalBranchFolder)
        {
            return new RootBranch(logicalBranchFolder, _serviceLocator);
        }

        public IQaBranch CreateQaBranch(Folders.QAFolder qaBranchFolder, IRootBranch owner)
        {
            return new QaBranch(qaBranchFolder, owner, _serviceLocator);
        }

        public IDevBranch CreateDevBranch(Folders.DevFolder devBranchFolder, IRootBranch owner)
        {
            return new DevBranch(devBranchFolder, owner, _serviceLocator);
        }

        public IFeatureBranch CreateFeatureBranch(Folders.FeatureFolder folder, IMainBranch owner)
        {
            return new FeatureBranch(folder, owner, _serviceLocator);
        }


    }
}

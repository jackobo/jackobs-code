using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    public interface ILogicalBranchComponentFactory
    {
        IRootBranch CreateRootBranch(Folders.RootBranchFolder rootBranchFolder);
        IQaBranch CreateQaBranch(Folders.QAFolder qaBranchFolder, IRootBranch owner);
        IDevBranch CreateDevBranch(Folders.DevFolder devBranchFolder, IRootBranch owner);
        IFeatureBranch CreateFeatureBranch(Folders.FeatureFolder folder, IMainBranch owner);

    }
}

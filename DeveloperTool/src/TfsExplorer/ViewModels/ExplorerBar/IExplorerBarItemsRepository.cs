using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public interface IExplorerBarItemsRepository
    {
        IExplorerBarItem CreateRootBranchItem(IRootBranch rootBranch, IExplorerBar explorerBar);
        IExplorerBarItem CreateQABranchItem(IQaBranch qaBranch);
        IExplorerBarItem CreateMainQaBranchItem(IQaBranch qaBranch);
        IExplorerBarItem CreateDevBranchItem(IDevBranch devBranch);
        IExplorerBarItem CreateQAFeaturesBranchesItem(IQaBranch qaBranch);
        IExplorerBarItem CreateQAFeatureBranchItem(IFeatureBranch featureBranch);
        IExplorerBarItem CreateQAInstallerItem(IQAInstaller installer);
        IExplorerBarItem CreateProductionInstallerItem(IProductionInstaller installer);
        IExplorerBarItem CreateProductionInstallersItem(IProductionEnvironment environment);
        IExplorerBarItem CreateDevMainBranchItem(IDevBranch devBranch);
        IExplorerBarItem CreateProductionEnvironment(IProductionEnvironment environment);
        IExplorerBarItem CreateQAInstallersItem(IQaBranch qaBranch);
        IExplorerBarItem CreateDevFeatureBrancheItem(IFeatureBranch featureBranch);
        IExplorerBarItem CreateDevFeaturesBranchesItem(IDevBranch devBranch);
        IExplorerBarItem CreateProductionBranchItem(IProductionBranch productionBranch);
    }
}

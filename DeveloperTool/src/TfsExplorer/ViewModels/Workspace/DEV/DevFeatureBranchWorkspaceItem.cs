using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class DevFeatureBranchWorkspaceItem : FeatureBranchWorkspaceItem
    {
        public DevFeatureBranchWorkspaceItem(IFeatureBranch featureBranch, IServiceLocator serviceLocator) 
            : base(featureBranch, serviceLocator)
        {
            
        }

        
    }
}

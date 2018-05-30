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
    public class QABranchWorkspaceItem : WorkspaceItemBase
    {
        public QABranchWorkspaceItem(IQaBranch qaBranch, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            _logicalBranch = qaBranch;
        }

        IQaBranch _logicalBranch;
        
        public override string Title
        {
            get
            {
                return "QA";
            }
        }
    }
}

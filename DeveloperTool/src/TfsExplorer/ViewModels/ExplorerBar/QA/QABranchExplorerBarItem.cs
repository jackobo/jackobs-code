using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class QABranchExplorerBarItem : EnvironmentBranchExplorerBarItem<QABranchWorkspaceItem, IQaBranch>
    {
        public QABranchExplorerBarItem(IQaBranch qaBranch, IExplorerBarItem parent, IServiceLocator serviceLocator)
            : base(qaBranch, parent, serviceLocator)
        {

            this.Items.Add(ItemsFactory.CreateQAInstallersItem(qaBranch));

            
            
        }

        public override string Caption
        {
            get
            {
                return "QA";
            }
        }

        protected override IExplorerBarItem CreateMainExplorerBarItem(IQaBranch qaBranch)
        {
            return ItemsFactory.CreateMainQaBranchItem(qaBranch);
        }
        
        protected override IExplorerBarItem CreateFaturesBranchesExplorerBarItem(IQaBranch qaBranch)
        {
            return ItemsFactory.CreateQAFeaturesBranchesItem(MainBranch);
        }

        protected override QABranchWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new QABranchWorkspaceItem(MainBranch, ServiceLocator);
        }
        
    }
}

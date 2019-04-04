using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class QAInstallersWorkspaceItem : WorkspaceItemBase
    {
        public QAInstallersWorkspaceItem(IServiceLocator serviceLocator) : base(serviceLocator)
        {
        }

        public override string Title
        {
            get
            {
                return "QA Installers";
            }
        }
    }
}

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
    public class QAInstallerWorkspaceItem : InstallerWorkspaceItem<IQAInstaller>
    {
        public QAInstallerWorkspaceItem(IQAInstaller installer, IServiceLocator serviceLocator) 
            : base(installer,  serviceLocator)
        { 
        }


        protected override bool AllowCustomizedInstallerGeneration
        {
            get
            {
                return false;
            }
        }

    }
}

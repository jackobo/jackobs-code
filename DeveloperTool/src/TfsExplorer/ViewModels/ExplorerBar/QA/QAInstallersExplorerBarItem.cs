using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.ExplorerBar
{
    public class QAInstallersExplorerBarItem : NavigationAwareItem<QAInstallersWorkspaceItem>
    {
        public QAInstallersExplorerBarItem(IQaBranch qaBranch, IExplorerBarItem parent, IServiceLocator serviceLocator) : base(parent, serviceLocator)
        {
            _qaBranch = qaBranch;
            if(!UseLazyLoading)
            {
                foreach(var item in GetLazyLoadedItems())
                {
                    this.Items.Add(item);
                }
            }
           
        }

        protected override bool UseLazyLoading
        {
            get
            {
                return true;
            }
        }

        protected override IEnumerable<IExplorerBarItem> GetLazyLoadedItems()
        {
           
            var items = _qaBranch.GetInstallers().OrderByDescending(i => i.Version)
                                                 .Select(i => ItemsFactory.CreateQAInstallerItem(i))
                                                 .ToList();
            
            SubscribeToEvent<NewQAInstallerEventData>(InstallerAddedHandler);
            
            return items;
        }


        IQaBranch _qaBranch;
        private void InstallerAddedHandler(NewQAInstallerEventData eventData)
        {
            if(_qaBranch.Equals(eventData.QABranch))
            {
                this.Items.Insert(0, ItemsFactory.CreateQAInstallerItem(eventData.Installer));
            }
        }

        public override string Caption
        {
            get
            {
                return "Installers";
            }
        }

        protected override QAInstallersWorkspaceItem CreateWorkspaceItemViewModel()
        {
            return new QAInstallersWorkspaceItem(this.ServiceLocator);
        }
    }
}

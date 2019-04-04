using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;

namespace Spark.Wpf.Common.ViewModels
{
    public abstract class WorkspaceItemBase : ServicedViewModelBase, IWorkspaceItem
    {
        public WorkspaceItemBase(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {

        }

        public abstract string Title { get; }


        public virtual IContextCommand[] Actions
        {
            get { return new IContextCommand[0]; }
        }

        private Optional<ISideBarItem> CurrentSideBarItem { get; set; } = Optional<ISideBarItem>.None();

        protected virtual TSideBarItem ShowSideBarItem<TSideBarItem>(TSideBarItem sideBarItem) where TSideBarItem : ISideBarItem
        {
            this.ExecuteOnUIThread(() =>
            {
                ServiceLocator.GetInstance<ISidebar>().Navigate(sideBarItem);
                CurrentSideBarItem = Optional<ISideBarItem>.Some(sideBarItem);
            });

            return sideBarItem;
        }

        public override void Deactivate()
        {
            CurrentSideBarItem.Do(sideBarItem =>
            {
                sideBarItem.Deactivate();
                
            });

            base.Deactivate();
        }

    }
}

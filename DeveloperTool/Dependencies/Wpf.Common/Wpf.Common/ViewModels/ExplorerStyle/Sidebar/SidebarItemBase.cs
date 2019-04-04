using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace Spark.Wpf.Common.ViewModels
{
    public abstract class SidebarItemBase : ServicedViewModelBase, ISideBarItem
    {
        public SidebarItemBase(IServiceLocator serviceLocator) 
            : base(serviceLocator)
        {
        }

        public override void Deactivate()
        {
            this.ExecuteOnUIThread(() => this.ServiceLocator.GetInstance<ISidebar>().Hide());
            base.Deactivate();
        }
    }





}

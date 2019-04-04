using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.ViewModels.DynamicLayout;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace LayoutTool.ViewModels
{
    public class TriggersExplorerBarItem : TreeViewItem<TriggerViewModelCollection>
    {
        public TriggersExplorerBarItem(TriggerViewModelCollection triggers, IServiceLocator serviceLocator)
            : base(triggers, serviceLocator)
        {
            this.IsExpanded = true;
            this.Items = new SynchronizedTreeViewItemCollection<TriggerViewModel>(triggers, viewModel => new TriggerExplorerBarItem(viewModel, this, ServiceLocator));
        }

        public override string Caption
        {
            get
            {
                return "Dynamic Layouts";
            }
        }
        
    }
}

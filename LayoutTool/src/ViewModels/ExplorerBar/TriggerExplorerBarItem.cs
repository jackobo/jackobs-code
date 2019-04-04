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
    public class TriggerExplorerBarItem : TreeViewItem<TriggerViewModel>
    {
        public TriggerExplorerBarItem(TriggerViewModel trigger, TreeViewItem parent, IServiceLocator serviceLocator)
            : base(trigger, parent, serviceLocator)
        {
            trigger.PropertyChanged += Trigger_PropertyChanged;
        }

        public override string Caption
        {
            get
            {
                return ViewModel.Name; 
            }
        }

        private void Trigger_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(TriggerViewModel.Name))
            {
                OnPropertyChanged(nameof(Caption));
            }
        }
    }
}

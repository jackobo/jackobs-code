using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace LayoutTool.ViewModels
{
    public class MyAccountExplorerBarItem : TreeViewItem<MyAccountViewModel>
    {
        public MyAccountExplorerBarItem(MyAccountViewModel myAccountViewModel, IServiceLocator serviceLocator)
            : base(myAccountViewModel, serviceLocator)
        {
            
            
        }

        public override string Caption
        {
            get
            {
                return "My Account Menu";
            }
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class FlashGamesTreeViewItem : TreeViewItem
    {
        public FlashGamesTreeViewItem(TreeViewItem parent)
            : base(parent)
        {
        }

        Models.Client.ClientProduct Product
        {
            get
            {
                return this.ExplorerBar.GetWorkArea().GetProduct<Models.Client.ClientProduct>();
            }
        }

        public override string Caption
        {
            get { return "Flash games"; }
        }

        public override System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return ResourcesProvider.CreateBitmapImageSource("Flash24x24.png");
            }
        }

      
        protected override IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea)
        {
            return new FlashGamesWorkAreaItem(this.ExplorerBar.GetWorkArea());
        }

       
    }
}

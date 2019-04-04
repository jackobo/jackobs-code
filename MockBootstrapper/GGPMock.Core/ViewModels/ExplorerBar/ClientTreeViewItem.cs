using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class ClientTreeViewItem : TreeViewItem
    {
        public ClientTreeViewItem(Models.Client.ClientProduct product, IExplorerBar explorerBar)
            : base(explorerBar)
        {
            this.Product = product;
            this.Items.Add(new FlashGamesTreeViewItem(this));
            this.Items.Add(new Html5GamesTreeViewItem(this));
            this.IsExpanded = true;
        }

        Models.Client.ClientProduct Product { get; set; }

        public override string Caption
        {
            get { return this.Product.Name; }
        }

        protected override IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea)
        {
            return null;
        }
       

        public override System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return ResourcesProvider.CreateBitmapImageSource("Client24x24.png");
            }
        }
    }
}

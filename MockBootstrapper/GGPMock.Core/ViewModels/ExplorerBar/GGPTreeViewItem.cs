using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class GGPTreeViewItem : TreeViewItem
    {
        public GGPTreeViewItem(Models.GGP.GGPProduct product, IExplorerBar explorerBar)
            : base(explorerBar)
        {
            this.Product = product;
            this.Items.Add(new GGPLogTreeViewItem(this));
            this.Items.Add(new AvailableGamesTreeViewItem(this));
            this.Items.Add(new OpenSessionsTreeViewItem(this));
            this.Items.Add(new HistoryRecordsTreeViewItem(this));
            this.IsExpanded = true;
        }


        Models.GGP.GGPProduct Product { get; set; }

        public override string Caption
        {
            get { return this.Product.Name; }
        }

        protected override IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea)
        {
            return new GGPWorkAreaItem(workArea);
        }

        

        public override System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return ResourcesProvider.CreateBitmapImageSource("GGP.png");
            }
        }
    }
}

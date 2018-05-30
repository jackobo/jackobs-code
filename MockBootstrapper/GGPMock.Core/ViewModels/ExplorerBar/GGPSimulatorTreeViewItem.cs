using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class GGPSimulatorTreeViewItem : TreeViewItem
    {
        public GGPSimulatorTreeViewItem(Models.GGPSimulator.GGPSimulatorProduct product, IExplorerBar explorerBar)
            : base(explorerBar)
        {
            this.Product = product;
        }

        Models.GGPSimulator.GGPSimulatorProduct Product { get; set; }

        public override string Caption
        {
            get { return this.Product.Name; }
        }

       
        protected override IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea)
        {
            return new GGPSimulatorWorkAreaItem(this.ExplorerBar.GetWorkArea()); 
        }

        public override System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return ResourcesProvider.CreateBitmapImageSource("GGPSimulator.gif");
            }
        }
    }
}

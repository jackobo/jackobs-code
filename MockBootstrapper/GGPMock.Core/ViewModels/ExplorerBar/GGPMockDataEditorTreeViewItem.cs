using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class GGPMockDataEditorTreeViewItem : TreeViewItem
    {
        public GGPMockDataEditorTreeViewItem(IExplorerBar explorerBar) : base(explorerBar)
        {

        }
        public override string Caption
        {
            get
            {
                return "GGP Mock data";
            }
        }

        protected override IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea)
        {
            return new GGPMockDataEditorWorkAreaItem(workArea);
        }


        public override System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return ResourcesProvider.CreateBitmapImageSource("GGPMockData24x24.png");
            }
        }

    }
}

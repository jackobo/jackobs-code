using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class GGPLogTreeViewItem : TreeViewItem
    {
        public GGPLogTreeViewItem(TreeViewItem parent)
            : base(parent)
        {

        }

        public override string Caption
        {
            get
            {
                return "GGP Logs";
            }
        }

        GGPLogWorkAreaItem _workArea = null;
        protected override IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea)
        {
            if (_workArea == null)
                _workArea = new GGPLogWorkAreaItem(workArea);

            return _workArea;
        }

        public override System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return ResourcesProvider.CreateBitmapImageSource("Logger.24x24.png");
            }
        }
    }
}

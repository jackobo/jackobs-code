using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class OpenSessionsTreeViewItem : TreeViewItem
    {
        public OpenSessionsTreeViewItem(TreeViewItem parent)
            : base(parent)
        {

        }

        public override string Caption
        {
            get
            {
                return "My open sessions";
            }
        }


        protected override IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea)
        {
            return new OpenSessionsWorkAreaItem(workArea);
        }

        public override System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return ResourcesProvider.CreateBitmapImageSource("OpenSign24.x24.png");
            }
        }
    }
}

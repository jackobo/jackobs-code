using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class HistoryRecordsTreeViewItem : TreeViewItem
    {
        public HistoryRecordsTreeViewItem(TreeViewItem parent)
            : base(parent)
        {

        }
        public override string Caption
        {
            get
            {
                return "History records";
            }
        }

        protected override IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea)
        {
            return new HistoryRecordsWorkAreaItem(workArea);
        }

        public override System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return ResourcesProvider.CreateBitmapImageSource("History24x24.png");
            }
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class Html5GamesTreeViewItem : TreeViewItem
    {
        public Html5GamesTreeViewItem(TreeViewItem parent)
            : base(parent)
        {

        }
        public override string Caption
        {
            get { return "HTML5 Games"; }
        }

        public override System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return ResourcesProvider.CreateBitmapImageSource("HTML5.png");
            }
        }


        protected override IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea)
        {
            return new Html5GamesWorkAreaItem(this.ExplorerBar.GetWorkArea());
        }
    }
}

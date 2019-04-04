using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class AvailableGamesTreeViewItem : TreeViewItem
    {
        public AvailableGamesTreeViewItem(TreeViewItem parent)
            : base(parent)
        {

        }

        public override string Caption
        {
            get
            {
                return "Available games";
            }
        }




        
        protected override IWorkAreaItemViewModel CreateWorkAreaItem(IWorkArea workArea)
        {
            return new AvailableGamesViewModel(workArea);
        }


        public override IWorkAreaItemViewModel WorkAreaItem
        {
            get
            {
                if (Models.GGPMockDataManager.Singleton.MockState == Models.GGPMockState.Disconnected)
                    return CreateWorkAreaItem(this.ExplorerBar.GetWorkArea());
                else
                    return base.WorkAreaItem;
            }
        }

        public override System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return ResourcesProvider.CreateBitmapImageSource("Games.png");
            }
        }
    }
}

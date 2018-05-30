using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels
{
    public interface IExplorerBar : IViewModel
    {
        IExplorerBarItem CurrentItem { get; set; }
        ObservableCollection<IExplorerBarItem> Items { get; }
        void SelectFirstItem();
        void ExpandAll();
        void CollapseAll();
        IExplorerBar GetCheckedItemsAsExplorerBar();
        void CheckAll();
        void UncheckAll();
        bool AllowItemsCheck { get; set; }
        bool AllowSearching { get; set; }
    }
}

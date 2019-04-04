using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.Wpf.Common.ViewModels
{
    public interface IExplorerBarItem : IViewModel, IDisposable
    {
        string Caption { get; }
        
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
        bool? IsChecked { get; set; }

        bool AllowItemCheck { get; set; }
        bool IsAnyDescendantSelected { get; }
        void EnsureExpanded();
        ObservableCollection<IExplorerBarItem> Items { get; }

        IExplorerBarItem[] GetAccessorsList();

        IExplorerBar ExplorerBar { get; }
        string FullPath { get; }

        Optional<T> As<T>() where T : class;
        void CollapseAll();
        void ExpandAll();
        IEnumerable<IExplorerBarItem> Search(string[] words);
    }
}

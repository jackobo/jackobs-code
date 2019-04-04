using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Helpers
{
    public class ExplorerBarItemBuilder
    {
        private string _caption = string.Empty;
        private ObservableCollection<IExplorerBarItem> _items = new ObservableCollection<IExplorerBarItem>();
        private ExplorerBarItemBuilder()
        {

        }


        public static ExplorerBarItemBuilder ExplorerBarItem()
        {
            return new ExplorerBarItemBuilder();
        }

        public ExplorerBarItemBuilder WithCaption(string caption)
        {
            _caption = caption;
            return this;
        }

        public IExplorerBarItem Build()
        {
            var item = Substitute.For<IExplorerBarItem>();
            item.Caption.Returns(_caption);
            item.Items.Returns(_items);
            return item;
        }

    }
}

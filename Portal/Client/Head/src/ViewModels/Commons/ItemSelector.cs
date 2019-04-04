using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels
{
    public class ItemSelector<TItem> : ViewModelBase
    {
        public ItemSelector(TItem item, bool selected = true)
        {
            this.Item = item;
            this.Selected = selected;
        }


        private bool _selected;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                SetProperty(ref _selected, value);
            }
        }

        public TItem Item { get; private set; }

    }
}

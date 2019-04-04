using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels
{
    public class ItemSelectorViewModel<TItem> : ViewModelBase
    {
        public ItemSelectorViewModel(TItem item, bool defaultSelected = false)
        {
            this.Item = item;
        }

        public TItem Item { get; private set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                SetProperty(ref _isSelected, value);
            }
        }
        
    }
}

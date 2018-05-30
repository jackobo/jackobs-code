using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout.ValueEditors
{
        
    public class MultiSelectValueEditor : ViewModelBase, IConditionValueEditor
    {
        public MultiSelectValueEditor(IDialogServices dialogServices)
        {
            _dialogServices = dialogServices;
            SelectCommand = new Command(Select);
            
        }

        IDialogServices _dialogServices;

        public void AddItem(object originalItem, string id, string name, bool isSelected)
        {
            var item = new Item(id, name, isSelected, originalItem);
            Items.Add(item);
        }
                

        public ConditionValue[] GetValues()
        {
            return this.Items.Where(item => item.IsSelected)
                      .Select(item => new ConditionValue(item.Id.ToString()))
                      .ToArray();
        }



        private void Select()
        {
            var dialog = new MultiSelectValueDialog(this);
            _dialogServices.ShowOkCancelDialogBox(dialog);
            
        }

        public ICommand SelectCommand { get; private set; }
        


        private ObservableCollectionExtended<Item> _items = new ObservableCollectionExtended<Item>();

        public ObservableCollectionExtended<Item> Items
        {
            get
            {
                return _items;
            }
            set
            {
                SetProperty(ref _items, value);
                OnPropertyChanged(nameof(Description));
            }
        }

        public override string ToString()
        {
            return Description;
        }

        public string Description
        {
            get
            {
                return string.Join(", ", GetSelectedItems().Select(item => item.Name));
            }
        }


        IEnumerable<Item> GetSelectedItems()
        {
            return this.Items.Where(item => item.IsSelected);
        }

        public IConditionValueEditor Clone()
        {
            var clone = new MultiSelectValueEditor(_dialogServices);

            foreach(var item in Items)
            {
                clone.AddItem(item.OriginalItem, item.Id, item.Name, item.IsSelected);
            }

            return clone;
        }

        public object[] GetPositiveTestValues(EquationType equationType)
        {
            return GetSelectedItems().Select(item => item.OriginalItem).ToArray();
        }

        public object GetNegativeTestValue(EquationType equationType, IEnumerable<object> positiveValues)
        {
            var itemsToExclude = new List<object>();
            itemsToExclude.AddRange(positiveValues);
            
            var itemsToChoseFrom = Items.Where(item => !itemsToExclude.Contains(item.OriginalItem)).ToArray();

            if (itemsToChoseFrom.Length == 0)
                return null;
            
            return itemsToChoseFrom.First().OriginalItem;



        }

        public class Item : ViewModelBase
        {
            public Item(string id, string name, bool isSelected, object originalItem)
            {
                Id = id;
                Name = name;
                IsSelected = isSelected;
                OriginalItem = originalItem;
            }

            public object OriginalItem { get; private set; }

            public string Id { get; private set; }
            public string Name { get; private set; }

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
            

            public override string ToString()
            {
                return this.Name;
            }


            
          

        }
    }
    
}

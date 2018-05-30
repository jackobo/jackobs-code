using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels
{
    public class SynchronizedTreeViewItemCollection<TSourceItem> : TreeViewItemCollection
    {

        Func<TSourceItem, TreeViewItem> _treeViewItemFactory;
        Dictionary<TSourceItem, TreeViewItem> _sourceItemToTreeItemMapping = new Dictionary<TSourceItem, TreeViewItem>();
        public SynchronizedTreeViewItemCollection(ObservableCollection<TSourceItem> observedCollection, Func<TSourceItem, TreeViewItem> treeViewItemFactory)
        {
            this.ObservedCollection = observedCollection;
            _treeViewItemFactory = treeViewItemFactory;

            foreach (var item in observedCollection)
            {
                var treeViewItem = _treeViewItemFactory(item);
                this.Items.Add(treeViewItem);
                _sourceItemToTreeItemMapping.Add(item, treeViewItem);
            }

            this.ObservedCollection.CollectionChanged += ObservedCollection_CollectionChanged;


        }

        private void ObservedCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
#warning I must implement the replace behavior   
            if (e.NewItems != null)
            {
                var startingIndex = e.NewStartingIndex;
                foreach (TSourceItem sourceItem in e.NewItems)
                {
                    var destinationItem = _treeViewItemFactory(sourceItem);
                    this.Insert(startingIndex, destinationItem);
                    _sourceItemToTreeItemMapping.Add(sourceItem, destinationItem);
                    startingIndex++;
                }
            }

            if (e.OldItems != null)
            {
                foreach (TSourceItem removedItem in e.OldItems)
                {
                    this.Remove(_sourceItemToTreeItemMapping[removedItem]);
                    _sourceItemToTreeItemMapping.Remove(removedItem);
                }
            }
        }

        ObservableCollection<TSourceItem> ObservedCollection { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout
{
    public class TriggerViewModelCollection : ObservableCollectionExtended<TriggerViewModel>, IViewModel, IPlayerStatusFriendlyNameProvider
    {
        public TriggerViewModelCollection(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            AddDynamicLayoutHandler = new AddDynamicLayoutHandler<TriggerViewModel>(this,
                                                                                      playerStatus => new TriggerViewModel(playerStatus.Name, playerStatus, _serviceLocator),
                                                                                      this);
        }

        IServiceLocator _serviceLocator;

        public event EventHandler Changed;

        private void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public AddDynamicLayoutHandler<TriggerViewModel> AddDynamicLayoutHandler { get; private set; }

        public string GetFriendlyName(PlayerStatusType playerStatusType)
        {
            foreach(var trigger in this)
            {
                if (trigger.PlayerStatus.Id == playerStatusType.Id)
                    return trigger.Name;
            }

            return playerStatusType.Name;
        }

        protected override void InsertItem(int index, TriggerViewModel item)
        {
            item.PropertyChanged += Trigger_PropertyChanged;

            base.InsertItem(index, item);
        }

        protected override void ClearItems()
        {
            foreach(var item in this)
            {
                item.PropertyChanged -= Trigger_PropertyChanged;
            }
            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            this[index].PropertyChanged -= Trigger_PropertyChanged;
            base.RemoveItem(index);
        }

        private void Trigger_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TriggerViewModel.Name))
                OnChanged();
        }

       
    }
}

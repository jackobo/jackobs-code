using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.NotificationArea
{
    public interface IInformationArea : INotificationCategory
    {
        void AddMessage(string message);
    }

    public interface IInformationAreaItem
    {
        string Message { get; }
    }

    public class MessageInformationAreaItem : IInformationAreaItem
    {
        public MessageInformationAreaItem(string message)
        {
            this.Message = message;
        }
        #region IInformationAreaItem Members

        public string Message
        {
            get;
            private set;
        }

        #endregion
    }

    public class InformationArea : ServicedViewModelBase, IInformationArea
    {
        public InformationArea(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.Items = new ObservableCollection<IInformationAreaItem>();
            this.Items.CollectionChanged += Items_CollectionChanged;
            this.ClearCommand = new Command(Clear, () => this.Items.Count > 0);
            SubscribeToEvent<Interfaces.PubSubEvents.FullGamesSynchronizationFinishedEventData>(FullGamesSynchronizationFinishedHandler);
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ((Command)this.ClearCommand).RaiseCanExecuteChanged();
        }

        private void FullGamesSynchronizationFinishedHandler(Interfaces.PubSubEvents.FullGamesSynchronizationFinishedEventData obj)
        {
            this.AddMessage(string.Format("Artifactory games synchronization initiated by {0} finished at {1}", obj.InitiatedBy, obj.SynchronizationTime.ToString()));
        }


        public ICommand ClearCommand { get; private set; }

        private void Clear()
        {
            this.Items.Clear();
        }

        #region IInformationArea Members

        public void AddMessage(string message)
        {
            Items.Insert(0, new MessageInformationAreaItem(message));
        }


        public ObservableCollection<IInformationAreaItem> Items { get; private set; }

        #endregion

        #region INotificationCategory Members

        public string Caption
        {
            get { return "Informations"; }
        }

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

        #endregion
    }
}

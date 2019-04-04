using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Mvvm;
using Spark.Infra.Types;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged
    {

    }

    public class ViewModelBase : BindableBase, IViewModel, IActivationAware, IDisposable
    {
        public ViewModelBase()
        {
            
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ViewModelBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
        }


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (GlobalNotificationsEnabled && !GetPropertiesExcludedFromGlobalNotification().Contains(propertyName))
            {
                GlobalNotificationsManager.RaisePropertyChanged(this, propertyName);
            }
            
        }

        protected virtual bool GlobalNotificationsEnabled
        {
            get { return true; }
        }

        protected virtual  List<string> GetPropertiesExcludedFromGlobalNotification()
        {
            return new List<string>();
        }
        
       

        


        bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            private set
            {
                SetProperty(ref _isActive, value);
            }
        }

        public virtual void Activate()
        {
            this.IsActive = true;
        }


        public virtual void Deactivate()
        {
            this.IsActive = false;
        }
        
    }

    public class ServicedViewModelBase : ViewModelBase
    {
        public ServicedViewModelBase(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException(nameof(serviceLocator));

            this.ServiceLocator = serviceLocator;
        }

        protected IServiceLocator ServiceLocator { get; private set; }

        Dictionary<ISubscriptionToken, Type> _eventSubscriptions = new Dictionary<ISubscriptionToken, Type>();

        protected void SubscribeToEvent<TEventData>(Action<TEventData> handler, Predicate<TEventData> filter = null)
        {

            _eventSubscriptions.Add(ServiceLocator.GetInstance<IPubSubMediator>().Subscribe(handler, filter),
                                    typeof(TEventData));
            

        }

        protected virtual void ExecuteOnUIThread(Action action)
        {
            this.ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(action);
        }

        public override void Activate()
        {
            this.ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(() => base.Activate());
            
        }

        public override void Deactivate()
        {
            this.ExecuteOnUIThread(() => base.Deactivate());
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                foreach (var subscription in _eventSubscriptions)
                {
                    ServiceLocator.GetInstance<IPubSubMediator>().Unsubscribe(subscription.Key, subscription.Value);
                }
            }
        }
        

        protected void PublishEvent<TEventData>(TEventData eventData)
        {
            ServiceLocator.GetInstance<IPubSubMediator>().Publish(eventData);
        }


        public void StartBusyAction(Action action, string busyActionDescription = "")
        {
            StartBusyAction(action, busyActionDescription, null);
        }


        public void StartBusyAction(Action action, string busyActionDescription, Action onFinish)
        {
            this.BusyActionDescription = busyActionDescription;
            this.IsBusy = true;
            ServiceLocator.GetInstance<IApplicationServices>().StartNewParallelTask(() =>
            {
                try
                {
                    action();
                }
                finally
                {
                    ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(() => IsBusy = false);
                    onFinish?.Invoke();
                }
            });
        }


        private string _busyActionDescription;
        public string BusyActionDescription
        {
            get
            {
                return _busyActionDescription;
            }

            set
            {
                SetProperty(ref _busyActionDescription, value);
            }
        }


        private bool _isBusy = false;

        public virtual bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                SetProperty(ref _isBusy, value);
            }
        }
    }
}

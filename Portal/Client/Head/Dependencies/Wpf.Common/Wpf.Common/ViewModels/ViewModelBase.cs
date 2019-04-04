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
        
        protected virtual void Dispose(bool disposing)
        {
        }


        public event EventHandler Activated;
        
        public virtual void Activate()
        {
            Activated?.Invoke(this, EventArgs.Empty);
        }



    }

    public class ServicedViewModelBase : ViewModelBase
    {
        public ServicedViewModelBase(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        protected IServiceLocator ServiceLocator { get; private set; }

        Dictionary<SubscriptionToken, Type> _eventSubscriptions = new Dictionary<SubscriptionToken, Type>();

        protected void SubscribeToEvent<TEventData>(Action<TEventData> handler, Predicate<TEventData> filter = null)
        {

            var ev = GetEventOrNull<TEventData>();

            if (ev != null)
            {
                _eventSubscriptions.Add(ev.Subscribe(handler, ThreadOption.UIThread, false, filter),
                                        typeof(PubSubEvent<TEventData>));
            }
        }




        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                foreach (var subscription in _eventSubscriptions)
                {
                    var eventBase = GetEventOrNull(subscription.Value);
                    if (eventBase != null)
                    {
                        eventBase.Unsubscribe(subscription.Key);
                    }
                }
            }
        }


        private PubSubEvent<TEventData> GetEventOrNull<TEventData>()
        {
            var eventAggregator = ServiceLocator.GetInstance<IEventAggregator>();

            if (eventAggregator == null)
                return null;
            else
                return eventAggregator.GetEvent<PubSubEvent<TEventData>>();

        }


        private EventBase GetEventOrNull(Type eventType)
        {
            var eventAggregator = ServiceLocator.GetInstance<IEventAggregator>();

            if (eventAggregator == null)
                return null;


            var methodInfo = eventAggregator.GetType().GetMethod("GetEvent");

            if (methodInfo == null)
                return null;

            return methodInfo.MakeGenericMethod(eventType).Invoke(eventAggregator, new object[0]) as EventBase;
        }


        protected void PublishEvent<TEventData>(TEventData eventData)
        {
            var eventToPublish = GetEventOrNull<TEventData>();

            if (eventToPublish != null)
            {
                eventToPublish.Publish(eventData);
            }
        }

    }
}

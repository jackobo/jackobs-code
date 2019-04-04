using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Spark.Wpf.Common.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
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
             

        protected override List<string> GetPropertiesExcludedFromGlobalNotification()
        {
            var properties = base.GetPropertiesExcludedFromGlobalNotification();

            properties.Add(nameof(IsBusy));

            return properties;

        }

        public void StartBusyAction(Action action)
        {
            IsBusy = true;
            
            ServiceLocator.GetInstance<IApplicationServices>().StartNewParallelTask(() =>
            {
                try
                {
                    action();
                }
                finally
                {
                    ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(() => IsBusy = false);
                }
            });
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

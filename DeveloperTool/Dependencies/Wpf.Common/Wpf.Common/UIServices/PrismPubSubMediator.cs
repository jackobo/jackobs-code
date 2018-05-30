using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Spark.Infra.Types;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.UIServices
{
    internal class PrismPubSubMediator : IPubSubMediator
    {
        public PrismPubSubMediator(IEventAggregator eventAgregator)
        {
            _eventAggregator = eventAgregator;
        }

        IEventAggregator _eventAggregator;
        public void Publish<TPayload>(TPayload payload)
        {
            _eventAggregator.GetEvent<PubSubEvent<TPayload>>().Publish(payload);
        }

        public ISubscriptionToken Subscribe<TPayload>(Action<TPayload> action, Predicate<TPayload> filter = null)
        {
            return new SubscriptionTokenWrapper<SubscriptionToken>(
                        _eventAggregator.GetEvent<PubSubEvent<TPayload>>().Subscribe(action, ThreadOption.UIThread, false, filter)
                        );
        }

        public void Unsubscribe<TPayload>(ISubscriptionToken subscriptionToken)
        {
        
            _eventAggregator.GetEvent<PubSubEvent<TPayload>>().Unsubscribe(ExtractTokenFromWrapper(subscriptionToken));
        }

        private SubscriptionToken ExtractTokenFromWrapper(ISubscriptionToken subscriptionToken)
        {
            var tokenWrapper = subscriptionToken as SubscriptionTokenWrapper<SubscriptionToken>;
            if (tokenWrapper == null)
                throw new ArgumentException($"{nameof(subscriptionToken)} must be of type {typeof(SubscriptionTokenWrapper<SubscriptionToken>).FullName}");

            return tokenWrapper.Token;
        }

        public void Unsubscribe(ISubscriptionToken subscriptionToken, Type payloadType)
        {
            GetEvent(payloadType).Unsubscribe(ExtractTokenFromWrapper(subscriptionToken));
        }

        private EventBase GetEvent(Type payloadData)
        {
            var eventType = typeof(PubSubEvent<>).MakeGenericType(payloadData);
            var methodInfo = _eventAggregator.GetType().GetMethod(nameof(IEventAggregator.GetEvent));
            return methodInfo.MakeGenericMethod(eventType).Invoke(_eventAggregator, new object[0]) as EventBase;
        }
    }
}

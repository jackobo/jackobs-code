using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.TestHelpers
{
    public class MockPubSubMediator : IPubSubMediator
    {

        Dictionary<Type, List<ISubscription>> _subscriptions = new Dictionary<Type, List<ISubscription>>();
        public void Publish<TPayload>(TPayload payload)
        {
            foreach(var s in GetSubscriptionForPayLoad(typeof(TPayload)))
            {
                s.Notify(payload);
            }
        }


        private List<ISubscription> GetSubscriptionForPayLoad(Type payloadType)
        {
            List<ISubscription> subscriptions;
            if (_subscriptions.TryGetValue(payloadType, out subscriptions))
                return subscriptions;
            else
                return new List<ISubscription>();

        }

        public ISubscriptionToken Subscribe<TPayload>(Action<TPayload> action, Predicate<TPayload> filter = null)
        {
            if (!_subscriptions.ContainsKey(typeof(TPayload)))
                _subscriptions.Add(typeof(TPayload), new List<ISubscription>());

            var subscription = new Subscription<TPayload>(action, filter);
            _subscriptions[typeof(TPayload)].Add(subscription);

            return subscription.Token;


        }

        public void Unsubscribe(ISubscriptionToken token, Type payloadType)
        {
            foreach(var subscription in GetSubscriptionForPayLoad(payloadType).Where(s => s.Token.Equals(token)).ToArray())
            {
                _subscriptions[payloadType].Remove(subscription);
            }
        }

        public void Unsubscribe<TPayload>(ISubscriptionToken subscriptionToken)
        {
            Unsubscribe(subscriptionToken, typeof(TPayload));
        }


        private interface ISubscription
        {
            void Notify(object payload);
            ISubscriptionToken Token { get; }
        }

        private class Subscription<TPayload> : ISubscription
        {

            public Subscription(Action<TPayload> action, Predicate<TPayload> filter)
            {
                _action = action;
                _filter = filter;
                Token = new MockSubscriptionToken();
            }

            public ISubscriptionToken Token { get; private set; }

            Action<TPayload> _action;
            Predicate<TPayload> _filter;

            public void Notify(object payload)
            {
                var thePayload = (TPayload)payload;
                if (_filter != null)
                {
                    if (_filter(thePayload))
                        _action(thePayload);
                }
                else
                {
                    _action(thePayload);
                }
            }
        }


        private class MockSubscriptionToken : ISubscriptionToken
        {
            Guid _id = Guid.NewGuid();

            public override bool Equals(object obj)
            {
                var theOther = obj as MockSubscriptionToken;
                if (theOther == null)
                    return false;

                return this._id == theOther._id;
            }

            public override int GetHashCode()
            {
                return _id.GetHashCode();
            }

            public override string ToString()
            {
                return _id.ToString();
            }
        }
    }
}

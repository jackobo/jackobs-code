using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace Spark.Wpf.Common.Interfaces.UI
{
    public interface IPubSubMediator
    {
        ISubscriptionToken Subscribe<TPayload>(Action<TPayload> action, Predicate<TPayload> filter = null);
        void Unsubscribe<TPayload>(ISubscriptionToken subscriptionToken);
        void Unsubscribe(ISubscriptionToken token, Type payloadType);
        void Publish<TPayload>(TPayload payload);
    }


    public interface ISubscriptionToken
    {

    }

    internal sealed class SubscriptionTokenWrapper<TToken> : ISubscriptionToken
    {
        public SubscriptionTokenWrapper(TToken token)
        {
            Token = token;
        }

        public TToken Token { get; private set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as SubscriptionTokenWrapper<TToken>;
            if (theOther == null)
                return false;

            return this.Token.Equals(theOther.Token);
        }

        public override int GetHashCode()
        {
            return this.Token.GetHashCode();
        }

        public override string ToString()
        {
            return this.Token.ToString();
        }
    }


}

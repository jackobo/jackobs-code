using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public abstract class ComponentPublisherViewModel<TPublisher> : ViewModelBase, IComponentPublisherViewModel
        where TPublisher : IComponentPublisher
    {
        public ComponentPublisherViewModel(TPublisher publisher)
        {
            this.Publisher = publisher;
            this.NextVersion = NextVersionProviderViewModel.From(publisher.GetNextVersionProvider());
        }

        protected TPublisher Publisher { get; private set; }
        public abstract string Name { get; }

        public INextVersionProviderViewModel NextVersion { get; private set; }
        
        public abstract void Append(IPublishPayloadBuilder publishPayloadBuilder);
    }
}

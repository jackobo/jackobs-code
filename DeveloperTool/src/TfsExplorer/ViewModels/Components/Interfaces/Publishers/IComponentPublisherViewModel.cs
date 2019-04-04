using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public interface IComponentPublisherViewModel : IViewModel
    {
        string Name { get; }
        void Append(IPublishPayloadBuilder publishPayloadBuilder);
        INextVersionProviderViewModel NextVersion { get; }
    }
}

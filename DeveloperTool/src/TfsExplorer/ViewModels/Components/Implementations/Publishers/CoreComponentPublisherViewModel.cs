using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public class CoreComponentPublisherViewModel : ComponentPublisherViewModel<ICoreComponentPublisher>, ICoreComponentPublisherViewModel
    {
        public CoreComponentPublisherViewModel(ICoreComponentPublisher publisher)
            : base(publisher)
        {
        }
        
        public override string Name
        {
            get
            {
                return Publisher.Name;
            }
        }
        
        public override void Append(IPublishPayloadBuilder publishPayloadBuilder)
        {
            publishPayloadBuilder.AddCoreComponent(this.Name, NextVersion.SelectedVersion);
        }
    }
}

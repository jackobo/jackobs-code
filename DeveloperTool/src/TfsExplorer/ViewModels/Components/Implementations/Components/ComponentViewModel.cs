using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public abstract class ComponentViewModel<TComponent> : ViewModelBase, IComponentViewModel
        where TComponent : ILogicalComponent
    {
        public ComponentViewModel(TComponent component)
        {
            Component = component;
        }

        protected TComponent Component { get; private set; }

        public string Name
        {
            get
            {

                return this.Component.Name;
            }
        }


      
        public virtual string Version
        {
            get
            {
                var publishers = this.Component.As<ISupportPublishing>()
                                               .SelectMany(sp => sp.GetPublishers())
                                               .ToArray();
                
                return string.Join(Environment.NewLine, ExtractVersions(publishers));
            }
        }

        public virtual ComponentMetaDataItem[] MetaData
        {
            get
            {
                
                return this.Component.As<ISupportPublishing>()
                              .SelectMany(sp => sp.GetPublishers().SelectMany(p => p.GetMetadata()))
                              .Select(item => new ComponentMetaDataItem(item.Name, item.Value))
                              .ToArray();
            }
        }

        private IEnumerable<string> ExtractVersions(IEnumerable<IComponentPublisher> publishers)
        {
            return publishers.SelectMany(p => p.GetCurrentVersion().Select(v => v.ToString()))
                             .ToList();
        }
        
        public Optional<ILogicalComponent> GetComponent()
        {
            return Optional<ILogicalComponent>.Some(this.Component);
        }
    }
    
}

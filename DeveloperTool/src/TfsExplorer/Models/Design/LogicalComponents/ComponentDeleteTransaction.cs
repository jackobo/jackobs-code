using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    internal class ComponentDeleteTransaction<TLocation> : Interfaces.IComponentDeleteTransaction
        where TLocation : Folders.IFolderHolder
    {
        public ComponentDeleteTransaction(ILogicalComponent component,
                                         TLocation location,
                                         IServiceLocator serviceLocator,
                                         params IComponentUniqueId[] uniqueIDs)
        {
            this.Component = component;
            this.Location = location;
            this.ServiceLocator = serviceLocator;
            _uniqueIDs = uniqueIDs;
        }

        IComponentUniqueId[] _uniqueIDs;
        ILogicalComponent Component { get; set; }
        protected TLocation Location { get; private set; }
        protected IServiceLocator ServiceLocator { get; private set; }

        public virtual ILocalPath LocalPath
        {
            get
            {
                return this.Location.ToSourceControlFolder().GetLocalPath();
            }
        }

        public void Commit()
        {
            var pubSubMediator = ServiceLocator.GetInstance<IPubSubMediator>();
            pubSubMediator.Publish(new Interfaces.Events.ComponentDeletedEventData(this.Component));
        }

        public IEnumerable<IComponentUniqueId> GetUniqueIDs()
        {
            return _uniqueIDs;
        }
    }

    internal class GameEngineDeleteTransaction : ComponentDeleteTransaction<Folders.EngineFolder>
    {
        public GameEngineDeleteTransaction(GameEngineComponent gameEngine, EngineFolder location, IServiceLocator serviceLocator, IComponentUniqueId uniqueId) 
            : base(gameEngine, location, serviceLocator, uniqueId)
        {
        }

        public override ILocalPath LocalPath
        {
            get
            {
                return this.Location.Parent.ToSourceControlFolder().GetLocalPath();
            }
        }
    }

}

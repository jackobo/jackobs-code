using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.Models.Folders;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    internal class ComponentRenameTransaction<TLocation> : Interfaces.IComponentRenameTransaction
        where TLocation : Folders.IFolderHolder
    {
        
        public ComponentRenameTransaction(
            ILogicalComponent component,
            TLocation oldLocation, 
            TLocation newLocation, 
            IServiceLocator serviceLocator)
        {
            this.Component = component;
            OldLocation = oldLocation;
            NewLocation = newLocation;
            _serviceLocator = serviceLocator;
        }

        IServiceLocator _serviceLocator;
        ILogicalComponent Component { get; set; }


        protected TLocation NewLocation { get; private set; }
        protected TLocation OldLocation { get; private set; }

        

        public virtual IServerPath OldServerPath
        {
            get
            {
                return OldLocation.GetServerPath();
            }
        }

        public virtual IServerPath NewServerPath
        {
            get
            {
                return NewLocation.GetServerPath();
            }
        }

        

        public void Commit()
        {
            var pubSubMediator = _serviceLocator.GetInstance<IPubSubMediator>();
            pubSubMediator.Publish(new ComponentRelocatedEventData<TLocation>(OldLocation, NewLocation));
            pubSubMediator.Publish(new ComponentRenamedEventData(this.Component));
        }
    }

    internal class GameEngineRenameTransaction : ComponentRenameTransaction<Folders.EngineFolder>
    {
        public GameEngineRenameTransaction(GameEngineComponent gameEngine,
                                            EngineFolder oldLocation, 
                                            EngineFolder newLocation, 
                                            IServiceLocator serviceLocator) 
            : base(gameEngine, oldLocation, newLocation, serviceLocator)
        {
        }

        public override IServerPath NewServerPath
        {
            get
            {
                return this.NewLocation.Parent.GetServerPath();
            }
        }

        public override IServerPath OldServerPath
        {
            get
            {
                return this.OldLocation.Parent.GetServerPath();
            }
        }
    }
}

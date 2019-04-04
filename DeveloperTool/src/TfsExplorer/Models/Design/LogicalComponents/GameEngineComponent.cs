using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.Publish;
using Spark.TfsExplorer.Models.TFS;

namespace Spark.TfsExplorer.Models.Design
{
    public class GameEngineComponent : LogicalComponent<Folders.EngineFolder>, IGameEngineComponent
    {
        public GameEngineComponent(Folders.EngineFolder location, Optional<IComponentsVersionsProvider> componentsVersionsProvider, IServiceLocator serviceLocator)
            : base(location, componentsVersionsProvider, serviceLocator)
        {
            this.ComponentUniqueId = new FileBasedComponentUniqueID(() => this.Location.ComponentUniqueIdTxt);
        }

        public GameEngineComponent(Folders.EngineFolder location, 
                                   IComponentUniqueId uniqueId,
                                   Optional<IComponentsVersionsProvider> componentsVersionsProvider, 
                                   IServiceLocator serviceLocator)
          : base(location, componentsVersionsProvider, serviceLocator)
        {
            this.ComponentUniqueId = uniqueId;
        }

        public override IComponentDeleteTransaction CreateDeleteTransaction()
        {
            return new GameEngineDeleteTransaction(this, this.Location, this.ServiceLocator, this.ComponentUniqueId);
        }

        public override string Name
        {
            get
            {
                return this.Location.Parent.Name;
            }
        }

        public override IComponentRenameTransaction CreateRenameTransaction(string newName)
        {
            return new GameEngineRenameTransaction( 
                        this,
                        this.Location, 
                        this.Location.Parent.Parent.GameEngine(newName).Engine, 
                        this.ServiceLocator);

            
        }
        
        IComponentUniqueId ComponentUniqueId { get; set; }
        
        public GameEngineName EngineName
        {
            get { return new GameEngineName(this.Name); }
        }
        

        protected override void AcceptCommandVisitor(ILogicalComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
        
        protected override IFolderWithBranchingSupport GetTargetFolder(ComponentsFolder targetComponentsFolder)
        {
            return targetComponentsFolder.EnginesAndGames.GameEngine(this.Name).Engine;
        }

        public override IEnumerable<IComponentPublisher> GetPublishers()
        {
            return new IComponentPublisher[] { new GameEnginePublisher(this, this.VersionsProvider.First(), this.ComponentUniqueId) };
        }
    }
}

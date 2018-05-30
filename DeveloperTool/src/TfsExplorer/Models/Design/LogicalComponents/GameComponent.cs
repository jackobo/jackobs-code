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
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    public class GameComponent : LogicalComponent<Folders.GameFolder>, IGameComponent
    {
        public GameComponent(Folders.GameFolder location, Optional<IComponentsVersionsProvider> componentsVersionsProvider, IServiceLocator serviceLocator)
            : base(location, componentsVersionsProvider, serviceLocator)
        {
            Init(new FileBasedComponentUniqueID(() => this.Location.Math.ComponentUniqueIdTxt),
                 new FileBasedComponentUniqueID(() => this.Location.Limits.ComponentUniqueIdTxt));
        }

        public GameComponent(Folders.GameFolder location, IComponentUniqueId mathUniqueId, IComponentUniqueId limitsUniqueId, Optional<IComponentsVersionsProvider> componentsVersionsProvider, IServiceLocator serviceLocator)
                : base(location, componentsVersionsProvider, serviceLocator)
        {
            Init(mathUniqueId, limitsUniqueId);
        }

        ISubscriptionToken _gameEngineRelocationSubscription;

        private void Init(IComponentUniqueId mathUniqueId, IComponentUniqueId limitsUniqueId)
        {
            this.MathUniqueID = mathUniqueId;
            this.LimitsUniqueID = limitsUniqueId;
            _gameEngineRelocationSubscription = ServiceLocator.GetInstance<IPubSubMediator>()
                                                    .Subscribe<ComponentRelocatedEventData<Folders.EngineFolder>>(GameEngineRelocatedEventHandler);
        }

        private void GameEngineRelocatedEventHandler(ComponentRelocatedEventData<EngineFolder> eventData)
        {
            if(this.Location.Parent.Parent.Engine.Equals(eventData.OldLocation))
            {
                this.Location = eventData.NewLocation.Parent.Games.Game(this.Name);
            }
        }


        protected override void Dispose(bool disposing)
        {
            this.ServiceLocator.GetInstance<IPubSubMediator>().Unsubscribe<ComponentRelocatedEventData<EngineFolder>>(_gameEngineRelocationSubscription);
            base.Dispose(disposing);
        }

        
        public override string Name
        {
            get
            {
                return this.Location.Name;
            }
        }

        public override IComponentRenameTransaction CreateRenameTransaction(string newName)
        {
            return new ComponentRenameTransaction<Folders.GameFolder>(this, 
                                                                      this.Location,
                                                                      this.Location.Parent.Game(newName),
                                                                      this.ServiceLocator);
        }

        public override IComponentDeleteTransaction CreateDeleteTransaction()
        {
            return new ComponentDeleteTransaction<Folders.GameFolder>(this, this.Location, this.ServiceLocator, this.MathUniqueID, this.LimitsUniqueID);
        }

        public GameEngineName EngineName
        {
            get
            {
                return new GameEngineName(Location.Parent.Parent.Name);
            }
        }


        IComponentUniqueId LimitsUniqueID { get; set; }
        IComponentUniqueId MathUniqueID { get; set; }

        
        protected override void AcceptCommandVisitor(ILogicalComponentVisitor visitor)
        {
            visitor.Visit(this);
        }


        protected override IFolderWithBranchingSupport GetTargetFolder(ComponentsFolder targetComponentsFolder)
        {
            return GetTargetGameFolder(targetComponentsFolder);
        }


        private Folders.GameFolder GetTargetGameFolder(ComponentsFolder targetComponentsFolder)
        {
            return targetComponentsFolder.EnginesAndGames.GameEngine(Location.Parent.Parent.Name)
                                                        .Games
                                                        .Game(this.Name);
        }

        public override bool SameAs(ILogicalComponent component)
        {
            if (!base.SameAs(component))
                return false;

            var theOther = component as GameComponent;

            return this.EngineName == theOther.EngineName;

        }


        public override void Branch(ComponentsFolder targetComponentsFolder)
        {
            var targetGameFolder = GetTargetGameFolder(targetComponentsFolder);

            if(targetGameFolder.Math.Exists())
            {
                throw new InvalidOperationException($"Target branch folder {targetGameFolder.Math.GetServerPath()} already exists!");
            }

            if (targetGameFolder.Limits.Exists())
            {
                throw new InvalidOperationException($"Target branch folder {targetGameFolder.Limits.GetServerPath()} already exists!");
            }

            
            Location.Math.Branch(targetGameFolder.Math);

            if(Location.Limits.Exists())
                Location.Limits.Branch(targetGameFolder.Limits);
        }

        protected override IEnumerable<IMergeableChangeSet> GetMergeChangeSets(ComponentsFolder targetComponentsFolder)
        {
            try
            {
                var result = new List<IMergeableChangeSet>();

                var targetGameFolder = GetTargetGameFolder(targetComponentsFolder);


                if (targetGameFolder.Math.Exists() && Location.Math.Exists())
                    result.AddRange(Location.Math.GetMergeChangeSets(targetGameFolder.Math));

                if (targetGameFolder.Limits.Exists() && Location.Math.Exists())
                    result.AddRange(Location.Limits.GetMergeChangeSets(targetGameFolder.Limits));

                return result;
            }
            catch(Exception ex)
            {
                throw new ApplicationException($"Failed to get Merge Sets for game {this.Name}; SourceBranch = {this.Location}; TargetBranch = {GetTargetGameFolder(targetComponentsFolder)}", ex);
            }
        }


        public override MergeResult Merge(ComponentsFolder targetComponentsFolder)
        {
            var targetGameFolder = GetTargetGameFolder(targetComponentsFolder);

            var mergeResult = new MergeResult(0);
            
            if (targetGameFolder.Math.Exists() && Location.Math.Exists())
                mergeResult = mergeResult.Combine(Location.Math.Merge(targetGameFolder.Math));

            if (targetGameFolder.Limits.Exists() && Location.Math.Exists())
                mergeResult = mergeResult.Combine(Location.Limits.Merge(targetGameFolder.Limits));

            return mergeResult;
        }

        public override IEnumerable<IComponentPublisher> GetPublishers()
        {
            return new IComponentPublisher[]
            {
                GameMathPublisher(),
                GameLimitsPublisher()
            };
        }

        public override IEnumerable<IComponentPublisher> GetPublishers(IChangeSet sinceThisChangeSet)
        {
            return GetMathPublisher(sinceThisChangeSet)
                    .Union(GetLimitsPublisher(sinceThisChangeSet))
                    .ToArray();
        }


        private Optional<IComponentPublisher> GetMathPublisher(IChangeSet sinceThisChangeSet)
        {
            if (Location.Math.ToSourceControlFolder().QueryHistory(sinceThisChangeSet).Any())
                return Optional<IComponentPublisher>.Some(GameMathPublisher());
            else
                return Optional<IComponentPublisher>.None();
        }

        private Optional<IComponentPublisher> GetLimitsPublisher(IChangeSet sinceThisChangeSet)
        {
           
            var limitsSSCFolder = Location.Limits.ToSourceControlFolder();

            if (limitsSSCFolder.QueryHistory(sinceThisChangeSet).Any())
            {
                return Optional<IComponentPublisher>.Some(new GameLimitsPublisher(this, this.VersionsProvider.First(), LimitsUniqueID));
            }
            else
            {
                return Optional<IComponentPublisher>.None();
            }
        }

        private IGameMathPublisher GameMathPublisher()
        {
            return new GameMathPublisher(this, this.VersionsProvider.First(), MathUniqueID);
        }

        private  IGameLimitsPublisher GameLimitsPublisher()
        {
            
            return new GameLimitsPublisher(this, this.VersionsProvider.First(), LimitsUniqueID);
            
        }

    
    }
}

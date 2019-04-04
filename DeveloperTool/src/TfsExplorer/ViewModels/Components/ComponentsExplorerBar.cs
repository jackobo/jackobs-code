using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public class ComponentsExplorerBar : StandardExplorerBar
    {
        public ComponentsExplorerBar(IEnumerable<ILogicalComponent> components,
                                     IServiceLocator serviceLocator)
            : this(components, serviceLocator, null)
        {
            
        }
        
        public ComponentsExplorerBar(IEnumerable<ILogicalComponent> components, 
                                     IServiceLocator serviceLocator,
                                     IComponentRenameDeleteHandler renameDeleteHandler)
            : base(serviceLocator)
        {
            var componentsClassification = new ComponentsHierarchyBuilder(components).Build();

            var optionalRenameDeleteHandler = Optional<IComponentRenameDeleteHandler>.None();

            if (renameDeleteHandler != null)
                optionalRenameDeleteHandler = Optional<IComponentRenameDeleteHandler>.Some(renameDeleteHandler);

            if (componentsClassification.CoreComponents.Any())
                this.Items.Add(new CoreComponentsTreeItem(componentsClassification.CoreComponents, this, serviceLocator, optionalRenameDeleteHandler) { IsExpanded = true });

            if(componentsClassification.GameEngines.Any())
                this.Items.Add(new GameEnginesTreeItem(componentsClassification.GameEngines, this, serviceLocator, optionalRenameDeleteHandler) { IsExpanded = true });
        }
    }



    public interface IComponentsExplorerBarItem : IExplorerBarItem
    {
        string Description { get; }
        string Version { get; }
        
        ComponentMetaDataItem[] MetaData { get; }
        
    }

    public abstract class ComponentsExplorerBarItem<TComponentViewModel> : ExplorerBarItem,
                                                                           IComponentsExplorerBarItem,
                                                                           ILogicalComponentHolder
        where TComponentViewModel : IComponentViewModel
    {
      
        public ComponentsExplorerBarItem(TComponentViewModel componentViewModel, 
                                         IExplorerBarItem parent, 
                                         IServiceLocator serviceLocator, 
                                         Optional<IComponentRenameDeleteHandler> renameDeleteHandler) 
            : base(parent, serviceLocator)
        {
            this.ComponentViewModel = componentViewModel;
            _renameDeleteHanlder = renameDeleteHandler;
            SubscribeToEvent<ComponentRenamedEventData>(ComponentRenamedEventHandler);
            SubscribeToEvent<ComponentDeletedEventData>(ComponentDeletedEventHandler);
        }

        private void ComponentDeletedEventHandler(ComponentDeletedEventData eventData)
        {
            this.As<ILogicalComponentHolder>().Do(componentHolder =>
            {
                componentHolder.GetComponent().Do(component =>
                {
                    if (component.Equals(eventData.Component))
                    {
                        this.Delete();
                    }
                });
            });
        }
        public string Description
        {
            get { return $"{Caption} [{Version}]"; }
        }

        private void ComponentRenamedEventHandler(ComponentRenamedEventData eventData)
        {
            this.As<ILogicalComponentHolder>().Do(componentHolder =>
            {
                componentHolder.GetComponent().Do(component =>
                {
                    if (component.Equals(eventData.Component))
                        OnPropertyChanged(nameof(Caption));
                });
            });
            
        }

        Optional<IComponentRenameDeleteHandler> _renameDeleteHanlder;
        

        protected TComponentViewModel ComponentViewModel { get; private set; }

        public override string Caption
        {
            get
            {
                return this.ComponentViewModel.Name;
            }
        }

        public string Version
        {
            get { return this.ComponentViewModel.Version; }
        }

        public Optional<ILogicalComponent> GetComponent()
        {
            return this.ComponentViewModel.GetComponent();
        }

        public override IContextCommand[] Actions
        {
            get
            {
                var actions = new List<IContextCommand>();
                
                _renameDeleteHanlder.Do(handler =>
                {
                    this.As<ILogicalComponentHolder>().Do(componentHolder =>
                    {
                        foreach(var component in componentHolder.GetComponent())
                        {
                            if(component.AllowRename)
                            {
                                actions.Add(new Actions.RenameComponentAction(handler, component));
                            }

                            if(component.AllowDelete)
                            {
                                actions.Add(new Actions.DeleteComponentAction(handler, component));
                            }
                        }    
                    });
                    
                });

                return actions.ToArray();
            }
        }

        public ComponentMetaDataItem[] MetaData
        {
            get
            {
                return this.ComponentViewModel.MetaData;
            }
        }
    }


    public class CoreComponentsTreeItem : ExplorerBarItem, IComponentsExplorerBarItem
    {
        public CoreComponentsTreeItem(IEnumerable<ICoreComponentViewModel> coreComponents, IExplorerBar explorerBar, IServiceLocator serviceLocator, Optional<IComponentRenameDeleteHandler> renamingService) 
            : base(explorerBar, serviceLocator)
        {
            foreach(var coreComponent in coreComponents.OrderBy(cc => cc.Name))
            {
                this.Items.Add(new CoreComponentTreeItem(coreComponent, this, serviceLocator, renamingService));
            }
        }

        public override string Caption
        {
            get
            {
                return "Core components";
            }
        }

        public string Description
        {
            get
            {
                return this.Caption;
            }
        }

        public ComponentMetaDataItem[] MetaData
        {
            get
            {
                return new ComponentMetaDataItem[0];
            }
        }

        public string Version
        {
            get { return string.Empty; }
        }
    }


    public class CoreComponentTreeItem : ComponentsExplorerBarItem<ICoreComponentViewModel>
    {
        public CoreComponentTreeItem(ICoreComponentViewModel coreComponent, IExplorerBarItem parent, IServiceLocator serviceLocator, Optional<IComponentRenameDeleteHandler> renamingService) 
            : base(coreComponent, parent, serviceLocator, renamingService)
        {
            
        }


    }


    public class GameEnginesTreeItem : ExplorerBarItem, IComponentsExplorerBarItem
    {
        public GameEnginesTreeItem(IEnumerable<IGameEngineViewModel> gameEngines, IExplorerBar explorerBar, IServiceLocator serviceLocator, Optional<IComponentRenameDeleteHandler> renamingService) 
            : base(explorerBar, serviceLocator)
        {
            foreach(var gengine in gameEngines.OrderBy(engine => engine.Name))
            {
                this.Items.Add(new GameEngineTreeItem(gengine, this, serviceLocator, renamingService));
            }
        }

        public override string Caption
        {
            get
            {
                return "Engines & Games";
            }
        }

        public string Description
        {
            get
            {
                return this.Caption;
            }
        }

        public ComponentMetaDataItem[] MetaData
        {
            get
            {
                return new ComponentMetaDataItem[0];
            }
        }

        public string Version
        {
            get { return string.Empty; }
        }

    }

    public class GameEngineTreeItem : ComponentsExplorerBarItem<IGameEngineViewModel>
    {
        public GameEngineTreeItem(IGameEngineViewModel gameEngine, IExplorerBarItem parent, IServiceLocator serviceLocator, Optional<IComponentRenameDeleteHandler> renamingService) 
            : base(gameEngine, parent, serviceLocator, renamingService)
        {
            
            foreach(var game in gameEngine.Games.OrderBy(g => g.Name))
            {
                this.Items.Add(new GameTreeItem(game, this, serviceLocator, renamingService));
            }
        }

        
    }

    public class GameTreeItem : ComponentsExplorerBarItem<IGameViewModel>
    {
        public GameTreeItem(IGameViewModel game, IExplorerBarItem parent, IServiceLocator serviceLocator, Optional<IComponentRenameDeleteHandler> renameHandler) 
            : base(game, parent, serviceLocator, renameHandler)
        {
            game.Math.Do(math =>
            {
                this.Items.Add(new GameMathTreeItem(game.Name, math, this, serviceLocator));
            });

            game.Limits.Do(limits =>
            {
                this.Items.Add(new GameLimitsTreeItem(game.Name, limits, this, serviceLocator));
            });    
        }       
    }

    public abstract class GamePartTreeItem<TViewModel> : ExplorerBarItem, IComponentsExplorerBarItem
        where TViewModel : IGamePartViewModel
    {
        public GamePartTreeItem(string gameName, TViewModel viewModel, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(parent, serviceLocator)
        {
            GameName = gameName;
            ViewModel = viewModel;
            this.AllowItemCheck = false;
        }

        protected string GameName;
        protected TViewModel ViewModel { get; private set; }

        public string Description
        {
            get
            {
                return $"{GameName} - {Caption} [{Version}]";
            }
        }


        public string Version
        {
            get
            {
                return this.ViewModel.Version;
            }
        }

        public override bool? IsChecked
        {
            get
            {
                return false;
            }

            set
            {

            }
        }

        public ComponentMetaDataItem[] MetaData
        {
            get
            {
                return this.ViewModel.MetaData;
            }
        }
    }

    public class GameMathTreeItem : GamePartTreeItem<IGameMathViewModel>
    {
        public GameMathTreeItem(string gameName, IGameMathViewModel  math, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(gameName, math, parent, serviceLocator)
        {
        }
        
        public override string Caption
        {
            get
            {
                return "Math";
            }
        }

       
    }

    public class GameLimitsTreeItem : GamePartTreeItem<IGameLimitsViewModel>
    {
        public GameLimitsTreeItem(string gameName, IGameLimitsViewModel limits, IExplorerBarItem parent, IServiceLocator serviceLocator)
            : base(gameName, limits, parent, serviceLocator)
        {
        }

        public override string Caption
        {
            get
            {
                return "Limits";
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public class ComponentsPublishersExplorerBar : StandardExplorerBar
    {
        public ComponentsPublishersExplorerBar(IEnumerable<IComponentPublisher> publishers, IServiceLocator serviceLocator) 
            : base(serviceLocator)
        {
            
            
            this.Items.Add(new ComponentsPublishersExplorerBarHeader(this, serviceLocator));

            var publishersClassification = new ComponentsPublishersClassificationBuilder(publishers).Build();

            
            if(publishersClassification.CoreComponentsPublishers.Any())
            {
                this.Items.Add(new CoreComponentsPublishersTreeViewItem(publishersClassification.CoreComponentsPublishers, this, serviceLocator));
            }

            if (publishersClassification.GameEnginesPublishers.Any())
            {
                this.Items.Add(new EnginesAndGamesPublishersTreeViewItem(publishersClassification.GameEnginesPublishers, this, serviceLocator));
            }
        }
    }


    public interface IComponentPublisherExplorerBarItem : IExplorerBarItem
    {
        INextVersionProviderViewModel NextVersion { get; }
    }
    

    public class ComponentsPublishersExplorerBarHeader : ExplorerBarItem
    {
        public ComponentsPublishersExplorerBarHeader(IExplorerBar explorerBar, IServiceLocator serviceLocator) : base(explorerBar, serviceLocator)
        {
        }

        public override string Caption
        {
            get
            {
                return "COMPONENTS";
            }
        }

        
    }

    

    public class CoreComponentsPublishersTreeViewItem : ExplorerBarItem, IComponentPublisherExplorerBarItem
    {
        public CoreComponentsPublishersTreeViewItem(IEnumerable<ICoreComponentPublisherViewModel> coreComponentsPublishers, IExplorerBar explorerBar, IServiceLocator serviceLocator) : base(explorerBar, serviceLocator)
        {
            
            foreach (var component in coreComponentsPublishers)
                this.Items.Add(new CoreComponentPublisherTreeViewItem(component, this, serviceLocator));

        }

        public override string Caption
        {
            get
            {
                return "Core components";
            }
        }

        public INextVersionProviderViewModel NextVersion { get; private set; } = new VoidNextVersionsHolderViewModel();
    }


    public class EnginesAndGamesPublishersTreeViewItem : ExplorerBarItem, IComponentPublisherExplorerBarItem
    {
        public EnginesAndGamesPublishersTreeViewItem(IEnumerable<IGameEnginePublisherViewModel> gameEnginesPublishers, IExplorerBar explorerBar, IServiceLocator serviceLocator)
            : base(explorerBar, serviceLocator)
        {
            foreach (var gameEngine in gameEnginesPublishers)
            {
                this.Items.Add(new GameEnginePublisherTreeViewItem(gameEngine, this, serviceLocator));
            }
        }

        public override string Caption
        {
            get
            {
                return "Engines & Games";
            }
        }

        public INextVersionProviderViewModel NextVersion { get; private set; } = new VoidNextVersionsHolderViewModel();


    }

    public abstract class ComponentPublisherTreeViewItem<TPublisherViewModel> : ExplorerBarItem, IComponentPublisherViewModel, IComponentPublisherExplorerBarItem
        where TPublisherViewModel : IComponentPublisherViewModel
    {
        public ComponentPublisherTreeViewItem(TPublisherViewModel publisher, IExplorerBarItem parent, IServiceLocator serviceLocator)
            : base(parent, serviceLocator)
        {
            this.Publisher = publisher;
        }

        public override string Caption
        {
            get
            {
                return this.Publisher.Name;
            }
        }

        string IComponentPublisherViewModel.Name
        {
            get { return this.Publisher.Name; }
        }

        protected TPublisherViewModel Publisher { get; private set; }

        public INextVersionProviderViewModel NextVersion
        {
            get { return this.Publisher.NextVersion; }
        }

        public virtual void Append(IPublishPayloadBuilder publishPayloadBuilder)
        {
            this.Publisher.Append(publishPayloadBuilder);
        }
    }

    public class CoreComponentPublisherTreeViewItem : ComponentPublisherTreeViewItem<ICoreComponentPublisherViewModel>
    {
        public CoreComponentPublisherTreeViewItem(ICoreComponentPublisherViewModel publisher, 
                                                IExplorerBarItem parent, 
                                                IServiceLocator serviceLocator) 
            : base(publisher, parent, serviceLocator)
        {
        }
        
       
    }


    public class GameEnginePublisherTreeViewItem : ComponentPublisherTreeViewItem<IGameEnginePublisherViewModel>
    {
        public GameEnginePublisherTreeViewItem(IGameEnginePublisherViewModel publisher, 
                                             IExplorerBarItem parent, 
                                             IServiceLocator serviceLocator) 
            : base(publisher, parent, serviceLocator)
        {
            foreach(var game in publisher.Games)
            {
                this.Items.Add(new GamePublisherTreeViewItem(game, this, serviceLocator));
            }
        }
    }

    public class GamePublisherTreeViewItem : ComponentPublisherTreeViewItem<IGamePublisherViewModel>
    {
        public GamePublisherTreeViewItem(IGamePublisherViewModel publisher, IExplorerBarItem parent, IServiceLocator serviceLocator) 
            : base(publisher, parent, serviceLocator)
        {
            this.IsExpanded = true;

            publisher.MathPublisher.Do(p =>
            {
                this.Items.Add(new GameMathPublisherTreeViewItem(p, this, serviceLocator));
            });


            publisher.LimitsPublisher.Do(p =>
            {
                this.Items.Add(new GameLimitsPublisherTreeViewItem(p, this, serviceLocator));
            });

      
        }
       
    }



    public class GameMathPublisherTreeViewItem : ComponentPublisherTreeViewItem<IGameMathPublisherViewModel>
    {
        public GameMathPublisherTreeViewItem(IGameMathPublisherViewModel mathPublisher, IExplorerBarItem parent, IServiceLocator serviceLocator)
            : base(mathPublisher, parent, serviceLocator)
        {
        }

    }

    public class GameLimitsPublisherTreeViewItem : ComponentPublisherTreeViewItem<IGameLimitsPublisherViewModel>
    {
        public GameLimitsPublisherTreeViewItem(IGameLimitsPublisherViewModel limitsPublisher, IExplorerBarItem parent, IServiceLocator serviceLocator)
            : base(limitsPublisher, parent, serviceLocator)
        {
        }
      
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.ViewModels
{
    public class GameViewModel : ComponentViewModel<IGameComponent>, IGameViewModel, IComponentPublisherVisitor
    {
        public GameViewModel(IGameComponent component) 
            : base(component)
        {
            component.As<ISupportPublishing>()
                .Do(sp =>
                {
                    foreach(var publisher in sp.GetPublishers())
                    {
                        publisher.AcceptCommandVisitor(() => this);
                    }
                });
        }

        public override string Version
        {
            get
            {
                return string.Empty;
            }
        }

        public override ComponentMetaDataItem[] MetaData
        {
            get
            {
                var metaData = new List<ComponentMetaDataItem>();

                Math.Do(math =>
                {
                    metaData.AddRange(math.MetaData.Select(item => new ComponentMetaDataItem("Math - " + item.Name, item.Value)));
                });

                Limits.Do(limits =>
                {
                    metaData.AddRange(limits.MetaData.Select(item => new ComponentMetaDataItem("Limits - " + item.Name, item.Value)));
                });

                return metaData.ToArray();
            }
        }

        public Optional<IGameMathViewModel> Math { get; private set; } = Optional<IGameMathViewModel>.None();
        public Optional<IGameLimitsViewModel> Limits { get; private set; } = Optional<IGameLimitsViewModel>.None();

        


        void IComponentPublisherVisitor.Visit(ICoreComponentPublisher coreComponentPublisher)
        {
            throw new NotSupportedException();
        }

        void IComponentPublisherVisitor.Visit(IGameEnginePublisher gameEnginePublisher)
        {
            throw new NotSupportedException();
        }

        void IComponentPublisherVisitor.Visit(IGameMathPublisher gameMathPublisher)
        {
            Math = Optional<IGameMathViewModel>.Some(new GameMathViewModel(gameMathPublisher));
        }

        void IComponentPublisherVisitor.Visit(IGameLimitsPublisher gameLimistPublisher)
        {
            Limits = Optional<IGameLimitsViewModel>.Some(new GameLimitsViewModel(gameLimistPublisher));
        }
    }

    public class GameMathViewModel : IGameMathViewModel
    {
        public GameMathViewModel(IGameMathPublisher publisher)
        {
            _publisher = publisher;
        }
        IGameMathPublisher _publisher;
        public ComponentMetaDataItem[] MetaData
        {
            get
            {
                return _publisher.GetMetadata().Select(item => new ComponentMetaDataItem(item.Name, item.Value)).ToArray();
            }
        }

        public string Version
        {
            get
            {
                return _publisher.GetCurrentVersion().Select(v => v.ToString()).FirstOrDefault();
            }
        }


    }

    public class GameLimitsViewModel : IGameLimitsViewModel
    {
        public GameLimitsViewModel(IGameLimitsPublisher publisher)
        {
            _publisher = publisher;
        }
        IGameLimitsPublisher _publisher;
        public ComponentMetaDataItem[] MetaData
        {
            get
            {
                return _publisher.GetMetadata().Select(item => new ComponentMetaDataItem(item.Name, item.Value)).ToArray();
            }
        }

        public string Version
        {
            get
            {
                return _publisher.GetCurrentVersion().Select(v => v.ToString()).FirstOrDefault();
            }
        }


    }
}

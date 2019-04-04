using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public class GameEnginePublisherViewModel : ComponentPublisherViewModel<IGameEnginePublisher>, IGameEnginePublisherViewModel
    {
        public GameEnginePublisherViewModel(IGameEnginePublisher gameEnginePublisher, IEnumerable<IGamePublisherViewModel> gamesPublishers)
            : base(gameEnginePublisher)
        {
            
            this.Games = gamesPublishers;
        }

        
        public IEnumerable<IGamePublisherViewModel> Games { get; private set; }

        public override string Name
        {
            get
            {
                return Publisher.EngineName.ToString();
            }
        }
        
        public override void Append(IPublishPayloadBuilder publishPayloadBuilder)
        {
            publishPayloadBuilder.AddGameEngine(Publisher.EngineName, this.NextVersion.SelectedVersion);
        }
    }
}

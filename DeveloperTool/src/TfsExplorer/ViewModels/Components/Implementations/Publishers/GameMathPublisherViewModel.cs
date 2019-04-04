using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public class GameMathPublisherViewModel : ComponentPublisherViewModel<IGameMathPublisher>, IGameMathPublisherViewModel
    {
        public GameMathPublisherViewModel(IGameMathPublisher mathPublisher)
            : base(mathPublisher)
        {
        }
        
        public override string Name
        {
            get
            {
                return "Math";
            }
        }
        
        public override void Append(IPublishPayloadBuilder publishPayloadBuilder)
        {
            publishPayloadBuilder.AddGameMath(Publisher.GameName, Publisher.EngineName, this.NextVersion.SelectedVersion);
        }
    }
}

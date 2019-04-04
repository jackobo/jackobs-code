using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public class GameLimitsPublisherViewModel : ComponentPublisherViewModel<IGameLimitsPublisher>, IGameLimitsPublisherViewModel
    {
        public GameLimitsPublisherViewModel(IGameLimitsPublisher limitsPublisher)
            : base(limitsPublisher)
        {
           
        }
        
        public override string Name
        {
            get
            {
                return "Limits";
            }
        }

        public override void Append(IPublishPayloadBuilder publishPayloadBuilder)
        {
            publishPayloadBuilder.AddGameLimits(Publisher.GameName, Publisher.EngineName, NextVersion.SelectedVersion);
        }
    }

}

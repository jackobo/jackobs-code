using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public class GamePublisherViewModel : ViewModelBase, IGamePublisherViewModel
    {
        public GamePublisherViewModel(string gameName,
                                      Optional<IGameMathPublisherViewModel> mathPublisher,
                                      Optional<IGameLimitsPublisherViewModel> limitsPublisher)
        {
            this.Name = gameName;
            MathPublisher = mathPublisher;
            LimitsPublisher = limitsPublisher;   
        }


        public Optional<IGameMathPublisherViewModel> MathPublisher { get; private set; }
        public Optional<IGameLimitsPublisherViewModel> LimitsPublisher { get; private set; }

        public string Name
        {
            get; private set;
        }

        public INextVersionProviderViewModel NextVersion { get; private set; } = new VoidNextVersionsHolderViewModel();


        public void Append(IPublishPayloadBuilder publishPayloadBuilder)
        {
            this.MathPublisher.Do(mp =>
            {
                mp.Append(publishPayloadBuilder);
            });

            this.LimitsPublisher.Do(lp =>
            {
                lp.Append(publishPayloadBuilder);
            });

        }
    }
}

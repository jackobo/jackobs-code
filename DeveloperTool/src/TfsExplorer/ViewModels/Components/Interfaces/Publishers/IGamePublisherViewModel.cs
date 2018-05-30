using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.ViewModels
{
    public interface IGamePublisherViewModel : IComponentPublisherViewModel
    {
        Optional<IGameMathPublisherViewModel> MathPublisher { get; }
        Optional<IGameLimitsPublisherViewModel> LimitsPublisher { get; }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.ViewModels
{
    public interface IGameViewModel : IComponentViewModel
    {
        Optional<IGameMathViewModel> Math { get; }
        Optional<IGameLimitsViewModel> Limits { get; }
    }

    public interface IGamePartViewModel
    {
        string Version { get; }
        ComponentMetaDataItem[] MetaData { get; }
    }

    public interface IGameLimitsViewModel : IGamePartViewModel
    {
    }

    public interface IGameMathViewModel : IGamePartViewModel
    {
    
    }

}

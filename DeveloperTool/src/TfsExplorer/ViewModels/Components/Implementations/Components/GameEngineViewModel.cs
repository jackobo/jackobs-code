using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels
{
    public class GameEngineViewModel : ComponentViewModel<IGameEngineComponent>, IGameEngineViewModel
    {
        public GameEngineViewModel(IGameEngineComponent gameEngine, IEnumerable<IGameViewModel> games)
            : base(gameEngine)
        {
            this.Games = new ObservableCollection<IGameViewModel>(games);
        }

        public ObservableCollection<IGameViewModel> Games { get; private set; }

     
    }

    public class GameEnginePlaceholderViewModel : ViewModelBase, IGameEngineViewModel
    {
        public GameEnginePlaceholderViewModel(GameEngineName name, IEnumerable<IGameViewModel> games)
        {
            this.Name = name.ToString();
            this.Games = new ObservableCollection<IGameViewModel>(games);
        }
        public ObservableCollection<IGameViewModel> Games
        {
            get;private set;
        }

        public ComponentMetaDataItem[] MetaData
        {
            get
            {
                return new ComponentMetaDataItem[0];
            }
        }

        public string Name
        {
            get;private set;
        }

        public string Version { get { return string.Empty; } }
        

        public Optional<ILogicalComponent> GetComponent()
        {
            return Optional<ILogicalComponent>.None();
        }

        public Optional<VersionNumber> GetNextVersion()
        {
            return Optional<VersionNumber>.None();
        }
    }
}

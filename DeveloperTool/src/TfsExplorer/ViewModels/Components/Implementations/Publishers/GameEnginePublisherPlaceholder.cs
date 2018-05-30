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
    public class GameEnginePublisherPlaceholder : ViewModelBase, IGameEnginePublisherViewModel
    {
        public GameEnginePublisherPlaceholder(GameEngineName gameEngineName, IEnumerable<IGamePublisherViewModel> games)
        {
            _gameEngineName = gameEngineName;
            this.Games = games;
        }

        GameEngineName _gameEngineName;


        public string Name
        {
            get
            {
                return _gameEngineName.ToString();
            }
        }

        public IEnumerable<IGamePublisherViewModel> Games { get; private set; }

        public INextVersionProviderViewModel NextVersion { get; private set; } = new VoidNextVersionsHolderViewModel();
        
        public void Append(IPublishPayloadBuilder publishPayloadBuilder)
        {
            //nothing do to here
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class AvailableGameViewModelCollection : FilterableCollectionViewModel<AvailableGameViewModel>
    {
        public AvailableGameViewModelCollection(params string[] filtarableProperties)
            : base(filtarableProperties)
        {
            this.ShowGameOwnersCommand = new Command<AvailableGameViewModel>(ShowGameOwners);
        }


        public AvailableGameViewModelCollection(IEnumerable<AvailableGameViewModel> collection, params string[] filtarableProperties)
            : base(collection, filtarableProperties)
        {
            this.ShowGameOwnersCommand = new Command<AvailableGameViewModel>(ShowGameOwners);
        }

        public ICommand ShowGameOwnersCommand { get; private set; }

        private void ShowGameOwners(AvailableGameViewModel game)
        {
            CurrentGame = game;
            IsOpen = true;
        }


        protected override List<string> GetPropertiesExcludedFromGlobalNotification()
        {
            var properties = base.GetPropertiesExcludedFromGlobalNotification();

            properties.Add(nameof(CurrentGame));
            properties.Add(nameof(IsOpen));

            return properties;
        }

        private AvailableGameViewModel _currentGame;

        public AvailableGameViewModel CurrentGame
        {
            get { return _currentGame; }
            set
            {
                SetProperty(ref _currentGame, value);
            }
        }

        bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                SetProperty(ref _isOpen, value);
            }
        }
    }
}

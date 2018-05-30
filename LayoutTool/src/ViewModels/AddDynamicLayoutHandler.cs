using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LayoutTool.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class AddDynamicLayoutHandler<TLayout> : ViewModelBase
        where TLayout : ISupportDynamicLayout<TLayout>
    {
        public AddDynamicLayoutHandler(ObservableCollection<TLayout> layoutCollection, 
                                       Func<PlayerStatusTypeViewModel, TLayout> createNewLayout, 
                                       IPlayerStatusFriendlyNameProvider playerStatusFriendlyNameProvider)
        {
            _layoutCollection = layoutCollection;
            _createNewLayout = createNewLayout;
            _playerStatusFriendlyNameProvider = playerStatusFriendlyNameProvider;

            _layoutCollection.CollectionChanged += LayoutCollection_CollectionChanged;

        }

        
        ObservableCollection<TLayout> _layoutCollection;
        Func<PlayerStatusTypeViewModel, TLayout> _createNewLayout;
        IPlayerStatusFriendlyNameProvider _playerStatusFriendlyNameProvider;

        private void LayoutCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Options));
        }

        
        public AddDynamicLayoutMenuOption[] Options
        {
            get
            {
                var result = new List<AddDynamicLayoutMenuOption>();
                var usedLayouts = _layoutCollection.Select(l => l.PlayerStatus).ToArray();

                foreach(var playerStatus in PlayerStatusType.DynamicLayouts.Select(ps => new PlayerStatusTypeViewModel(ps, _playerStatusFriendlyNameProvider))
                                                            .Where(ps => !usedLayouts.Contains(ps)))
                {
                    var menuOption = new AddDynamicLayoutMenuOption(playerStatus);
                    
                    foreach(var existingLayout in _layoutCollection)
                    {
                        menuOption.Commands.Add(new CloneLayoutCommand(playerStatus, existingLayout, _layoutCollection));
                    }

                    menuOption.Commands.Add(new CreateFromScratchCommand(playerStatus, _createNewLayout, _layoutCollection));

                    result.Add(menuOption);
                }

                return result.ToArray();
            }
        }

        public class AddDynamicLayoutMenuOption : ViewModelBase
        {
            public AddDynamicLayoutMenuOption(PlayerStatusTypeViewModel playerStatus)
            {
                _playerStatus = playerStatus;
                _playerStatus.PropertyChanged += PlayerStatus_PropertyChanged;
                Commands = new List<CreateLayoutCommand>();
            }


            public string Caption
            {
                get { return _playerStatus.ToString(); }
            }

            PlayerStatusTypeViewModel _playerStatus;

            private void PlayerStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                OnPropertyChanged(nameof(Caption));
            }

            public List<CreateLayoutCommand> Commands { get; private set; }
        }


        public abstract class CreateLayoutCommand : ViewModelBase, ICommand
        {
            public abstract string Caption { get; }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public abstract void Execute(object parameter);
        }


        private class CreateFromScratchCommand : CreateLayoutCommand
        {
            public CreateFromScratchCommand(PlayerStatusTypeViewModel playerStatus, Func<PlayerStatusTypeViewModel, TLayout> createNewLayout, ObservableCollection<TLayout> layoutCollection)
            {
                _playerStatus = playerStatus;
                _createNewLayout = createNewLayout;
                _layoutCollection = layoutCollection;
            }

            Func<PlayerStatusTypeViewModel, TLayout> _createNewLayout;
            ObservableCollection<TLayout> _layoutCollection;
            PlayerStatusTypeViewModel _playerStatus;

            public override string Caption
            {
                get
                {
                    return "Create from scratch";
                }
            }

            public override void Execute(object parameter)
            {
                var newLayout = _createNewLayout(_playerStatus);
                _layoutCollection.Add(newLayout);

                if(newLayout is IActivationAware)
                {
                    ((IActivationAware)newLayout).Activate();
                }
            }
        }

        private class CloneLayoutCommand : CreateLayoutCommand
        {
            public CloneLayoutCommand(PlayerStatusTypeViewModel newPlayerStatus, TLayout layout, ObservableCollection<TLayout> layoutCollection)
            {
                _newPlayerStatus = newPlayerStatus;
                _layout = layout;
                _layoutCollection = layoutCollection;

                _layout.PropertyChanged += _layout_PropertyChanged;
                
            }

            private void _layout_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if(e.PropertyName == nameof(ISupportDynamicLayout<TLayout>.PlayerStatus))
                {
                    OnPropertyChanged(nameof(Caption));
                }
            }

            public override string Caption
            {
                get { return "Copy from " + _layout.PlayerStatus.ToString(); }
            }

            TLayout _layout;
            ObservableCollection<TLayout> _layoutCollection;
            PlayerStatusTypeViewModel _newPlayerStatus;



            public override void Execute(object parameter)
            {
                var newLayout = _layout.Clone(_newPlayerStatus);

                _layoutCollection.Add(newLayout);

                if (newLayout is IActivationAware)
                {
                    ((IActivationAware)newLayout).Activate();
                }
            }
        }

    }

   
}

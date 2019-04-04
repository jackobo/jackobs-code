using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels
{
    public class ClientTypesSelectorViewModel : ViewModelBase
    {
        public ClientTypesSelectorViewModel(ClientType[] clientTypes)
        {
            this.SelectionHandlers = clientTypes.SelectMany(ct => ct.Regulations.Select(r => new ClientToRegulationSelectionHandler(ct, r))).ToArray();

            if (this.SelectionHandlers.Length == 1)
                this.SelectionHandlers[0].Selected = true;


            this.ViewByClientType = new ViewByClientTypeStrategy(SelectionHandlers);
            this.ViewByRegulation = new ViewByRegulationStrategy(SelectionHandlers);

            this.CurrentViewMode = this.ViewByClientType;

            this.ViewByClientTypeCommand = new Command(() => this.CurrentViewMode = this.ViewByClientType);
            this.ViewByRegulationCommand = new Command(() => this.CurrentViewMode = this.ViewByRegulation);

        }

        void CurrentViewMode_SelectionChanged(object sender, EventArgs e)
        {
            OnSelectionChanged();
        }

        
        public event EventHandler SelectionChanged;

        private void OnSelectionChanged()
        {
            var ev = SelectionChanged;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }

        

        public ClientType[] GetSelectedClientTypes()
        {
            return SelectionHandlers.Where(m => m.Selected).GroupBy(m => m.ClientType).Select(g => new ClientType(g.Key.Name, g.Select(item => item.Regulation).ToArray())).ToArray();
        }


        public ICommand ViewByClientTypeCommand { get; private set; }
        public ICommand ViewByRegulationCommand { get; private set; }


       


        ClientToRegulationSelectionHandler[] SelectionHandlers { get; set; }

        public ViewByClientTypeStrategy ViewByClientType { get; private set; }
        public ViewByRegulationStrategy ViewByRegulation { get; private set; }


        private ViewByStrategyBase _currentViewMode;

        public ViewByStrategyBase CurrentViewMode
        {
            get { return _currentViewMode; }
            private set
            {
                if (_currentViewMode != null)
                    _currentViewMode.SelectionChanged -= CurrentViewMode_SelectionChanged;

                SetProperty(ref _currentViewMode, value);

                if (_currentViewMode != null)
                    _currentViewMode.SelectionChanged += CurrentViewMode_SelectionChanged;
            }
        }


        public abstract class ViewByStrategyBase : ViewModelBase
        {
            public ViewByStrategyBase()
            {
                this.SelectAllCommand = new Command(SelectAll);
                this.UnselectAllCommand = new Command(UnselectAll);
            }

            ParentItemViewModel[] _items;
            public ParentItemViewModel[] Items
            {
                get { return _items; }
                protected set
                {
                    _items = value;

                    SubscribeToItemsSelectionChangedEvent();
                }
            }

            private void SubscribeToItemsSelectionChangedEvent()
            {
                foreach (var item in _items)
                {
                    item.SelectionChanged += Item_SelectionChanged;
                }
            }

            private void UnsubscribeFromItemsSelectionChangedEvent()
            {
                foreach (var item in _items)
                {
                    item.SelectionChanged -= Item_SelectionChanged;
                }
            }

            void Item_SelectionChanged(object sender, EventArgs e)
            {
                OnSelectionChanged();
            }

            public event EventHandler SelectionChanged;

            private void OnSelectionChanged()
            {
                var ev = SelectionChanged;
                if (ev != null)
                    ev(this, EventArgs.Empty);
            }


            public ICommand SelectAllCommand { get; private set; }
            public ICommand UnselectAllCommand { get; private set; }

            private void SelectAll()
            {
                ChangeSelection(item => item.SelectAllCommand);
            }

            private void UnselectAll()
            {
                ChangeSelection(item => item.UnselectAllCommand);
            }


            private void ChangeSelection(Func<ParentItemViewModel, ICommand> commandSelector)
            {
                UnsubscribeFromItemsSelectionChangedEvent();

                try
                {
                    foreach (var item in this.Items)
                    {
                        commandSelector(item).Execute(null);
                    }
                    
                    OnSelectionChanged();

                }
                finally
                {
                    SubscribeToItemsSelectionChangedEvent();
                }
            }

        }

        public class ViewByClientTypeStrategy : ViewByStrategyBase
        {
            public ViewByClientTypeStrategy(ClientToRegulationSelectionHandler[] selectionHandlers)
            {
                Items = selectionHandlers.GroupBy(handler => handler.ClientType)
                                         .Select(group => new ParentItemViewModel(group.Key.Name,
                                                                                  group.OrderBy(x => x.Regulation.Name)
                                                                                       .Select(item => new ChildItemViewModel(item.Regulation.Name, item))
                                                                                       .ToArray()))
                                         .OrderBy(item => item.Name)
                                         .ToArray();
            }





        }

        public class ViewByRegulationStrategy : ViewByStrategyBase
        {
            public ViewByRegulationStrategy(ClientToRegulationSelectionHandler[] selectionHandlers)
            {
                Items = selectionHandlers.GroupBy(handler => handler.Regulation)
                                         .Select(group => new ParentItemViewModel(group.Key.Name, group.OrderBy(item => item.ClientType.Name)
                                                                                                      .Select(item => new ChildItemViewModel(item.ClientType.Name, item))
                                                                                                      .ToArray()))
                                         .OrderBy(item => item.Name)
                                         .ToArray();
            }

        }

        public class ParentItemViewModel : ViewModelBase
        {


            public ParentItemViewModel(string name, params ChildItemViewModel[] children)
            {
                this.Name = name;
                this.Children = children;
                this.SelectAllCommand = new Command(SelectAll);
                this.UnselectAllCommand = new Command(UnselectAll);

                foreach (var ch in this.Children)
                {
                    ch.PropertyChanged += Child_PropertyChanged;
                }

            }

            void Child_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                OnSelectionChanged();
            }


            public string Name
            {
                get;
                private set;
            }



            public ChildItemViewModel[] Children
            {
                get;
                private set;
            }

            public event EventHandler SelectionChanged;

            private void OnSelectionChanged()
            {
                var ev = SelectionChanged;
                if (ev != null)
                    ev(this, EventArgs.Empty);
            }

            public ICommand SelectAllCommand { get; private set; }
            public ICommand UnselectAllCommand { get; private set; }

            private void SelectAll()
            {
                ChangeSelectionState(true);
            }

            private void UnselectAll()
            {
                ChangeSelectionState(false);
            }

            private void ChangeSelectionState(bool selected)
            {
                foreach (var ch in this.Children)
                {
                    ch.PropertyChanged -= Child_PropertyChanged;
                    ch.Selected = selected;
                    ch.PropertyChanged += Child_PropertyChanged;
                }

                OnSelectionChanged();
            }
        }

        public class ChildItemViewModel : ViewModelBase
        {
            public ChildItemViewModel(string name, ClientToRegulationSelectionHandler selectionHandler)
            {
                this.Name = name;
                this.SelectionHandler = selectionHandler;
            }

            private ClientToRegulationSelectionHandler SelectionHandler { get; set; }

            public string Name
            {
                get;
                private set;
            }



            public bool Selected
            {
                get { return SelectionHandler.Selected; }
                set
                {
                    SelectionHandler.Selected = value;
                    OnPropertyChanged(() => Selected);
                }
            }
        }

        public class ClientToRegulationSelectionHandler 
        {
            public ClientToRegulationSelectionHandler(ClientType clientType, RegulationType regulation)
            {
                this.ClientType = clientType;
                this.Regulation = regulation;
            }
            public ClientType ClientType { get; private set; }
            public RegulationType Regulation { get; private set; }

            bool _selected;

            public bool Selected
            {
                get { return _selected; }
                set
                {
                    //SetProperty(ref _selected, value);
                    _selected = value;
                }
            }
            

        }

    }
}

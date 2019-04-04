using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Spark.Wpf.Common;
using Spark.Wpf.Common.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    
    public interface ISkinDesigner
    {
        SkinDefinitionContext BuildSkinDefinitionContext();
        void NavigateToWorkspace<TViewModel>(TViewModel viewModel) where TViewModel : IViewModel;
    }



    public class SkinDesignerViewModel : ViewModelBase, ISkinDesigner, INavigationAware, IConfirmNavigationRequest, IApplicationExitHandler
    {

        public SkinDesignerViewModel(IServiceLocator serviceLocator,
                                    AvailableGameViewModelCollection existingClientGames,
                                    AvailableGameViewModelCollection allOtherGames,
                                    FilterableCollectionViewModel<ArenaFilterViewModel> availableFilters,
                                    SkinDefinitionViewModel skinDefinition,
                                    ExplorerBarViewModel explorerBar,
                                    IEnumerable<ErrorMessageViewModel> buildErrors,
                                    SkinIndentity sourceSkin,
                                    SkinIndentity destinationSkin)
        {
            serviceLocator.GetInstance<IUnityContainer>().RegisterInstance<ISkinDesigner>(this);

            ServiceLocator = serviceLocator;
            AvailableGames = existingClientGames;
            NewGames = allOtherGames;
            AvailableFilters = availableFilters;
            SkinDefinition = skinDefinition;
            ExplorerBar = explorerBar;
            _buildErrors = buildErrors;
            SourceSkin = sourceSkin;
            DestinationSkin = destinationSkin;

            CreateCommands();

            InitMockupViewModel();

            RegisterGlobalNotificationHandlers();


            _validator = new SkinValidator(this,
                                           ServiceLocator.GetInstance<IApplicationServices>());

            RegisterApplicationExitHandler();
            
        }


        SkinValidator _validator;

        IEnumerable<ErrorMessageViewModel> _buildErrors;
        ErrorListViewModel _errorList = new ErrorListViewModel();

        public string SourceSkinDescription
        {
            get
            {
                return "Source: " + SourceSkin.ToString();
            }           
        }

        public string DestinationSkinDescription
        {
            get
            {
                return "Target: " + DestinationSkin.ToString();
            }
        }

        public ErrorListViewModel ErrorList
        {
            get { return _errorList; }
            set
            {
                if (SetProperty(ref _errorList, value))
                {
                    if (_errorList != null && _buildErrors != null)
                    {
                        foreach (var buildError in _buildErrors)
                            _errorList.Add(buildError);
                    }
                }
            }
        }


        protected override List<string> GetPropertiesExcludedFromGlobalNotification()
        {
            var properties = base.GetPropertiesExcludedFromGlobalNotification();

            properties.Add(nameof(ErrorList));

            return properties;
        }

        private void CreateCommands()
        {
            SelectSkinCommand = new Command(SelectSkin);

            Commands = new DesignerCommand[]
            {
                new DesignerCommand("Mockup View", ViewMockup),
                new DesignerCommand("Report View", ViewReport),
                new DesignerCommand("Save to file", () => SaveToFile()),
                new DesignerCommand("Publish to TFS", Publish)
            };
        }

        private void ViewReport()
        {
            NavigateAndUnselectCurrentItem(new SkinReportsViewModel(SkinDefinition, ServiceLocator));
        }

        public DesignerCommand[] Commands { get; private set; } 

        public class DesignerCommand : Command
        {
            public DesignerCommand(string caption, Action action)
                : base(action)
            {
                Caption = caption;
            }

            public string Caption { get; private set; }
        }

        IServiceLocator ServiceLocator { get; set; }


        IRegionManager RegionManager
        {
            get { return this.ServiceLocator.GetInstance<IRegionManager>(); }
        }

        public ICommand SelectSkinCommand { get; private set; }

        private void SelectSkin()
        {
            this.RegionManager.NavigateToMainContent(this.ServiceLocator.TryResolve<SkinToDesignSelectorViewModel>());
        }
        
        private string _savedToFile;
        private OkCancelDialogBoxResult SaveToFile()
        {

            var serializedContent = ServiceLocator.GetInstance<ISkinDefinitionSerializer>()
                                                  .Serialize(BuildSkinDefinitionContext());


            var fileName = _savedToFile;

            if(string.IsNullOrEmpty(fileName))
            {
                fileName = DestinationSkin.ToString().Replace(" ", "_").Replace("|", "").Replace(":", "");
            }

            
            var saveFileResponse = ServiceLocator.GetInstance<IDialogServices>()
                                                 .SaveFile("Save layout", Encoding.UTF8.GetBytes(serializedContent), "Layout file (*.lyt)|*.lyt", fileName);

            if (saveFileResponse.Response == OkCancelDialogBoxResult.Ok)
            {
                _savedToFile = saveFileResponse.FileName;
                _isDirty = false;
            }

            return saveFileResponse.Response;
        }

        

        private void ViewMockup()
        {
            NavigateAndUnselectCurrentItem(MockupViewModel);
        }

     
        private void RegisterGlobalNotificationHandlers()
        {
            _globalPropertyChangedHandler = new PropertyChangedEventHandler(GlobalNotificationsManager_PropertyChanged);
            _globalCollectionChangedHandler = new NotifyCollectionChangedEventHandler(GlobalNotificationsManager_CollectionChanged);
            GlobalNotificationsManager.PropertyChanged += _globalPropertyChangedHandler;
            GlobalNotificationsManager.CollectionChanged += _globalCollectionChangedHandler;
        }


        private void UnregisterGlobalNotificationHandlers()
        {
            GlobalNotificationsManager.PropertyChanged -= _globalPropertyChangedHandler;
            GlobalNotificationsManager.CollectionChanged -= _globalCollectionChangedHandler;
        }

        internal IEnumerable<AvailableGameViewModel> GetAllUsedGames()
        {
            return SkinDefinition.Arenas.SelectMany(a => a.Layouts.SelectMany(l => l.Games.Select(g => g.ConvertToAvailableGame())))
                                                                    .Distinct()
                                                                    .ToList();
        }

        
        


        public void NavigateToWorkspace<TViewModel>(TViewModel viewModel)
            where TViewModel : IViewModel
        {
            ServiceLocator.GetInstance<IRegionManager>().NavigateToViewModel<TViewModel>(RegionNames.SkinDesignerWorkspace, viewModel);
            
        }

        private void NavigateAndUnselectCurrentItem<TViewModel>(TViewModel viewModel)
            where TViewModel : IViewModel
        {
            NavigateToWorkspace(viewModel);

            var selectedTreeItem = this.ExplorerBar.GetSelectedItem();

            if (selectedTreeItem != null)
                selectedTreeItem.IsSelected = false;
        }


        private void InitMockupViewModel()
        {
            MockupViewModel = new MockupViewModel(ServiceLocator, DestinationSkin, SkinDefinition.Triggers);
            RegisterFilesOverrideProvider();
        }

        private void RegisterFilesOverrideProvider()
        {
            ServiceLocator.GetInstance<IFiddlerServices>().RegisterFilesOverrideProvider(MockupViewModel);
        }

        private void UnregisterFiddlerOverrideProvider()
        {
            if (MockupViewModel != null)
            {
                ServiceLocator.GetInstance<IFiddlerServices>().UnregisterFilesOverrideProvider(MockupViewModel);
            }
        }

        public MockupViewModel MockupViewModel { get; private set; }
        
        
        private void Publish()
        {
            var skinSelector = new SkinDefinitionBuilderSelectorViewModel(ServiceLocator.GetAllInstances<ISkinDefinitionBuilderViewModel>().Where(b => b.CanPublish).ToArray());
            skinSelector.LoadFrom(DestinationSkin);
            NavigateAndUnselectCurrentItem(new SkinPublisherViewModel(skinSelector, ServiceLocator));
        }

        

        public ArenaCollectionViewModel AvailableArenas
        {
            get { return SkinDefinition.Arenas; }
        }

        public ObservableCollection<MyAccountItemViewModel> AvailableMyAccountItems
        {
            get { return SkinDefinition.MyAccount.AllMyAccountItems; }
        }

        private ExplorerBarViewModel _explorerBar;

        public ExplorerBarViewModel ExplorerBar
        {
            get { return _explorerBar; }
            set
            {
                SetProperty(ref _explorerBar, value);
            }
        }



        private SkinDefinitionViewModel _skinDefinition;

        public SkinDefinitionViewModel SkinDefinition
        {
            get
            {
                return _skinDefinition;
            }
            set
            {
                SetProperty(ref _skinDefinition, value);
            }
        }


        public AvailableGameViewModelCollection AvailableGames { get; private set; }

        public AvailableGameViewModelCollection NewGames { get; private set; }


        public FilterableCollectionViewModel<ArenaFilterViewModel> AvailableFilters { get; private set; }
        

        private SkinIndentity SourceSkin { get; set; }

        public SkinIndentity DestinationSkin { get; set; }

        public SkinDefinitionContext BuildSkinDefinitionContext()
        {
            var skinDefintionContext = new SkinDefinitionContext();
            skinDefintionContext.Publisher = $"{Environment.UserDomainName}\\{Environment.UserName}";
            skinDefintionContext.AvailableArenaTypes = new ArenaTypeCollection(this.AvailableArenas.Select(a => new ArenaType(a.Type, a.Name)));
            skinDefintionContext.AvailableFilters = new FilterCollection(this.AvailableFilters.GetOriginalItems().Select(f => new Filter(f.Label, f.Name, f.Attributes)));
            skinDefintionContext.AvailableGames = new GameCollection(this.AvailableGames.GetOriginalItems().Select(g => new Game(g.GameType, g.Name)));
            skinDefintionContext.SourceSkin = this.SourceSkin;
            skinDefintionContext.DestinationSkin = this.DestinationSkin;
            skinDefintionContext.SkinDefinition = BuildSkinDefinition();
            skinDefintionContext.Errors = GetErrors();




            return skinDefintionContext;


        }

        private ErrorMessageCollection GetErrors()
        {
            var errors = new ErrorMessageCollection();

            foreach(var err in ErrorList)
            {
                errors.Add(new ErrorMessage(err.SourceName, err.Severity, err.Message));
            }

            return errors;
        }

        private SkinDefinition BuildSkinDefinition()
        {
            var skinDefinition = new SkinDefinition();

            
            

            skinDefinition.SkinContent = new Interfaces.Entities.SkinContent();

            foreach (var arenaViewModel in SkinDefinition.Arenas)
            {
                skinDefinition.SkinContent.Arenas.Add(BuildArena(arenaViewModel));
            }


            foreach (var lobbyViewModel in SkinDefinition.Lobbies)
            {
                skinDefinition.SkinContent.Lobbies.Add(BuildLobby(lobbyViewModel));
            }


            foreach (var gamesGroupViewModel in SkinDefinition.TopGames)
            {
                skinDefinition.SkinContent.TopGames.Add(BuildGameGroupLayout(gamesGroupViewModel));
            }

            foreach (var gamesGroupViewModel in SkinDefinition.VipGames)
            {
                skinDefinition.SkinContent.VipTopGames.Add(BuildGameGroupLayout(gamesGroupViewModel));
            }

            foreach (var myAccountItemViewModel in SkinDefinition.MyAccount.Lobby)
            {
                skinDefinition.SkinContent.MyAccountLobby.Add(BuildMyAccountMenu(myAccountItemViewModel));
            }

            foreach (var myAccountItemViewModel in SkinDefinition.MyAccount.History)
            {
                skinDefinition.SkinContent.MyAccountHistory.Add(BuildMyAccountMenu(myAccountItemViewModel));
            }


            foreach (var triggerViewModel in SkinDefinition.Triggers)
            {
                skinDefinition.SkinContent.Triggers.Add(BuildTrigger(triggerViewModel));
            }

            return skinDefinition;
        }

        private Trigger BuildTrigger(DynamicLayout.TriggerViewModel triggerViewModel)
        {
            var trigger = new Trigger(triggerViewModel.PlayerStatus.Name, triggerViewModel.PlayerStatus.Priority);
            var action = new TriggerAction(triggerViewModel.PlayerStatus.ActionName);

            foreach (var conditionViewModel in triggerViewModel.Conditions)
            {
                var condition = new Condition(conditionViewModel.Field.UpdateType,
                                             conditionViewModel.Field.ComputeFieldName(conditionViewModel.ValueEditor),
                                             conditionViewModel.EquationType.Id);

                foreach (var v in conditionViewModel.ValueEditor.GetValues())
                {
                    condition.Values.Add(v);
                }

                action.Conditions.Add(condition);
            }

            trigger.Actions.Add(action);

            return trigger;
        }

        private MyAccountItem BuildMyAccountMenu(MyAccountItemViewModel myAccountItemViewModel)
        {
            return new MyAccountItem(myAccountItemViewModel.Id, myAccountItemViewModel.Name, myAccountItemViewModel.Attributes);
        }

        private GameGroupLayout BuildGameGroupLayout(GameGroupLayoutViewModel gamesGroupViewModel)
        {
            var gamesGroupLayout = new GameGroupLayout(gamesGroupViewModel.PlayerStatus.Id);

            foreach (var game in gamesGroupViewModel.Games)
            {
                gamesGroupLayout.Games.Add(new Game(game.GameType, game.Name));
            }


            return gamesGroupLayout;
        }

        private Lobby BuildLobby(LobbyViewModel lobbyViewModel)
        {
            var lobby = new Lobby(lobbyViewModel.FavoritesSize, lobbyViewModel.PlayerStatus.Id);
            
            foreach (var lobbyItemViewModel in lobbyViewModel.Items)
            {
                
                lobby.Items.Add(new LobbyItem(lobbyItemViewModel.Id, lobbyItemViewModel.ShouldShowJackpot(lobbyViewModel.PlayerStatus)));
            }

            return lobby;
        }

        private Arena BuildArena(ArenaViewModel arenaViewModel)
        {
            var arena = new Arena(arenaViewModel.Type, arenaViewModel.Name, arenaViewModel.IsNewGamesArena);

            foreach (var layoutViewModel in arenaViewModel.Layouts)
            {
                var layout = new ArenaLayout(layoutViewModel.PlayerStatus.Id, layoutViewModel.Attributes);

                layout.DataGridInfo = layoutViewModel.DataGridInfo;

                foreach (var filterViewModel in layoutViewModel.Filters)
                {
                    layout.FilteringInfo.Add(new Filter(filterViewModel.Label, filterViewModel.Name, filterViewModel.Attributes));
                }

                foreach (var alsoPlayGameViewModel in layoutViewModel.AlsoPlayingGames)
                {
                    layout.AlsoPlayingGames.Add(new Game(alsoPlayGameViewModel.GameType, alsoPlayGameViewModel.Name));
                }

                foreach (var gameViewModel in layoutViewModel.Games.Select(g => g.ConvertToAvailableGame()))
                {
                    layout.Games.Add(gameViewModel.BuildArenaGame());
                }

                arena.Layouts.Add(layout);
            }

            return arena;
        }



        private PropertyChangedEventHandler _globalPropertyChangedHandler;
        private NotifyCollectionChangedEventHandler _globalCollectionChangedHandler;

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            UnregisterGlobalNotificationHandlers();
            UnregisterFiddlerOverrideProvider();
            UnregisterApplicationExitHandler();
        }


        private void RegisterApplicationExitHandler()
        {
            ServiceLocator.GetInstance<IApplicationServices>().RegisterApplicationExitHandler(this);
        }

        private void UnregisterApplicationExitHandler()
        {
            ServiceLocator.GetInstance<IApplicationServices>().UnregisterApplicationExitHandler(this);
        }

                

        private void GlobalNotificationsManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            MakeDirty();
            ResetMockup();
            _validator.Validate();
        }

     
        private void GlobalNotificationsManager_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!object.ReferenceEquals(sender, ErrorList))
            {
                MakeDirty();
                ResetMockup();
                _validator.Validate();
            }
        }

        private void ResetMockup()
        {
            if (MockupViewModel != null)
            {
                MockupViewModel.Reset();
            }
        }

        private bool _isDirty = false;

        private void MakeDirty()
        {
            _isDirty = true;
        }


        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (SaveToFileWithConfirmation() == MessageBoxResponse.OK)
                continuationCallback(true);
            else
                continuationCallback(false);
        }

        MessageBoxResponse SaveToFileWithConfirmation()
        {
            if (_isDirty == false)
            {
                return MessageBoxResponse.OK;
            }

            var response = ServiceLocator.GetInstance<IMessageBox>().ShowYesNoCancelMessage("Save changes to file ?");
            if (response == Spark.Wpf.Common.Interfaces.UI.MessageBoxResponse.Cancel)
            {
                return MessageBoxResponse.Cancel;
            }
            else
            {
                if (response == Spark.Wpf.Common.Interfaces.UI.MessageBoxResponse.Yes)
                {
                    if (SaveToFile() == OkCancelDialogBoxResult.Cancel)
                    {
                        return MessageBoxResponse.Cancel;
                    }
                }
                return MessageBoxResponse.OK;
            }
        }

        bool IApplicationExitHandler.CanExit()
        {
            return SaveToFileWithConfirmation() != MessageBoxResponse.Cancel;
        }
        
    }
}

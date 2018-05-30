using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GGPMockBootstrapper.GGPMockDataProvider;
using GGPMockBootstrapper.Models;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.ViewModels
{
    public class MainViewModel : ViewModelBase, IExplorerBar, IWorkArea
    {
        Product[] _productModels = null;

        public GGPMockDataProvider.AvailablePlayer[] AvailablePlayers
        {
            get
            {
                return Models.GGPMockDataManager.Singleton.AvailablePlayers;
            }
        }

        public GGPMockDataProvider.AvailablePlayer CurrentSelectedPlayer
        {
            get { return Models.GGPMockDataManager.Singleton.CurrentSelectedPlayer; }
            set
            {
                Models.GGPMockDataManager.Singleton.CurrentSelectedPlayer = value;
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentSelectedPlayer));
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentPlayerData));
            }
        }

        public PlayerData CurrentPlayerData
        {
            get { return Models.GGPMockDataManager.Singleton.MockData; }
        }

        public ActionViewModel AddNewUserAction { get; private set; }

        private void AddNewUser()
        {
            UIServices.GetCustomUIService<IDialogShowService>().ShowOkCancelDialog(new AddNewUserDialog());
        }

        public ActionViewModel SelectCurrentUserAction { get; private set; }

        private void SelectCurrentUser()
        {
            var selectUserDialog = new SelectUserDialog();
            selectUserDialog.Init();
            UIServices.GetCustomUIService<IDialogShowService>().ShowOkCancelDialog(selectUserDialog);
        }

        private MainViewModel(GGPGameServer.ApprovalSystem.Common.IUserInterfaceServices uiServices)
        {
            this.UIServices = uiServices;

            this.InstallationContext =  new InstallationContext(uiServices);
            this.GamesInformationProvider = new GamesInformationProvider();
            
            _productModels =  new Product[]
                                            {
                                                new Models.GGP.GGPProduct(InstallationContext),
                                                new Models.FlashPolicyServer.FlashPolicyServerProduct(InstallationContext),
                                                new Models.GGPSimulator.GGPSimulatorProduct(InstallationContext),
                                                new Models.Client.ClientProduct(InstallationContext, GamesInformationProvider),
                                                new Models.MainProxy.MainProxyProduct(InstallationContext)
                                            };
            LoadProductsTreeView();

            this.AddNewUserAction = new ActionViewModel("Add new user", new Command(AddNewUser));
            this.SelectCurrentUserAction = new ActionViewModel("Selected current user", new Command(SelectCurrentUser));

            GGPMockDataManager.Singleton.PropertyChanged += GGPMockDataManager_PropertyChanged;
        }

        void GGPMockDataManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GGPMockDataManager.Singleton.GetPropertyName(x => x.AvailablePlayers))
            {
                OnPropertyChanged(this.GetPropertyName(x => x.AvailablePlayers));
            }
            else if (e.PropertyName == GGPMockDataManager.Singleton.GetPropertyName(x => x.CurrentSelectedPlayer))
            {
                OnPropertyChanged(this.GetPropertyName(x => x.CurrentSelectedPlayer));
            }
            else if (e.PropertyName == GGPMockDataManager.Singleton.GetPropertyName(x => x.MockData))
            {
                OnPropertyChanged(this.GetPropertyName(x => x.CurrentPlayerData));
            }
        }

        private void LoadProductsTreeView()
        {
            this.Products = new ObservableCollection<TreeViewItem>();
            this.Products.Add(new GGPMockDataEditorTreeViewItem(this));
            this.Products.Add(new GGPSimulatorTreeViewItem((Models.GGPSimulator.GGPSimulatorProduct)_productModels[2], this));
            this.Products.Add(new GGPTreeViewItem((Models.GGP.GGPProduct)_productModels[0], this));
            this.Products.Add(new ClientTreeViewItem((Models.Client.ClientProduct)_productModels[3], this));
            
            this.Products.First().IsSelected = true;
        }

        public InstallationProgressViewModel CreateInstallationProgressViewModel()
        {
            var actions = _productModels.SelectMany(p => p.GetInstallActions()).ToArray();
            return new InstallationProgressViewModel(actions, this.InstallationContext);
        }


        public static void Init(GGPGameServer.ApprovalSystem.Common.IUserInterfaceServices uiServices)
        {
            _singletone = new MainViewModel(uiServices);
        }

        static MainViewModel _singletone;
        public static MainViewModel Singletone
        {
            get
            {
                return _singletone;
            }
        }

     
        IInstalationContext InstallationContext { get; set; }

        public ObservableCollection<TreeViewItem> Products { get; private set; }
        
        #region IExplorerBar Members

        TreeViewItem _selectedItem;
        public TreeViewItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SelectedItem));
            }
        }

        #endregion

        #region IWorkArea Members

        public TProduct GetProduct<TProduct>() where TProduct : Product
        {
            return (TProduct) (object)_productModels.Where(p => p.GetType() == typeof(TProduct)).FirstOrDefault();
        }

        public IEnvironmentServices EnvironmentServices
        {
            get 
            {
                return this.InstallationContext.EnvironmentServices;
            }
        }


        #endregion

        #region IExplorerBar Members


        public IWorkArea GetWorkArea()
        {
            return this;
        }

        #endregion


        #region IWorkArea Members


        public IGamesInformationProvider GamesInformationProvider
        {
            get;
            private set;
        }

        #endregion

        internal void AppExit()
        {
            GGPMockDataManager.Singleton.Disconnect();
        }
    }
}

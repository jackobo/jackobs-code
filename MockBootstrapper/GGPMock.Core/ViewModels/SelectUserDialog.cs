using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Odbc;
using System.Linq;
using System.Windows.Forms;
using GGPGameServer.ApprovalSystem.Common;
using GGPGameServer.Installer.ViewModels;
using GGPMockBootstrapper.GGPMockDataProvider;

namespace GGPMockBootstrapper.ViewModels
{
    public class SelectUserDialog : OkCancelDialogViewModel
    {
        public List<PlayerData> PlayersInfo { get; set; }

        private ObservableCollection<PlayerData> _visiblePlayersInfo;
        public ObservableCollection<PlayerData> VisiblePlayersInfo
        {
            get
            {
                return _visiblePlayersInfo;
            }
            set
            {
                _visiblePlayersInfo = value;
                OnPropertyChanged(this.GetPropertyName(t => t.VisiblePlayersInfo));
            }
        }

        private string _searchTemplate;

        public string SearchTemplate
        {
            get { return _searchTemplate; }
            set
            {
                _searchTemplate = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SearchTemplate));
                ApplySearchTemplate();
            }
        }

        public SelectUserDialog()
        {
            Title = "Select User";

            DeleteAction = new ActionViewModel("Remove", new Command<PlayerData>(DeleteUser), ResourcesProvider.CreateBitmapImageSource("Excluded.png"));
            SelectAction = new ActionViewModel("Select", new Command<PlayerData>(SelectUser), ResourcesProvider.CreateBitmapImageSource("Save24x24.png"));

            Models.GGPMockDataManager.Singleton.PropertyChanged += Singleton_PropertyChanged;
        }

        void Singleton_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Models.GGPMockDataManager.Singleton.GetPropertyName(t => t.AvailablePlayers))
            {
                Init();
                OnPropertyChanged(this.GetPropertyName(t => t.SearchTemplate));
                ApplySearchTemplate();
            }
        }

        private void ApplySearchTemplate()
        {
            if (!string.IsNullOrEmpty(SearchTemplate))
            {
                VisiblePlayersInfo =
                    new ObservableCollection<PlayerData>(
                        GenericSearchTemplate<PlayerData>.GetMatchingElements(PlayersInfo.ToList(),
                            new List<string> { "CID", "UserName" }, SearchTemplate));
            }
            else
            {
                VisiblePlayersInfo = new ObservableCollection<PlayerData>(PlayersInfo);
            }
        }

        public ActionViewModel SelectAction { get; set; }
        private void SelectUser(PlayerData playerData)
        {
            var currentSelectedPlayer = Models.GGPMockDataManager.Singleton.AvailablePlayers.First(item => item.CID == playerData.CID);
            if (currentSelectedPlayer == null)
            {
                UIServices.ShowMessage("Failed to select player. The player might have been deleted by someone else");
                Init();
            }
            else
            {
                Models.GGPMockDataManager.Singleton.CurrentSelectedPlayer = currentSelectedPlayer;
                Close();    
            }
        }

        public ActionViewModel DeleteAction { get; set; }
        private void DeleteUser(PlayerData playerData)
        {
            if (!Models.GGPMockDataManager.Singleton.DeletePlayer(playerData.CID))
            {
                UIServices.ShowMessage("Failed to delete user");
            }
        }

        public void Init()
        {
            PlayersInfo = new List<PlayerData>();
            foreach (var availablePlayer in Models.GGPMockDataManager.Singleton.AvailablePlayers)
            {
                PlayersInfo.Add(Models.GGPMockDataManager.Singleton.GetPlayerData(availablePlayer.CID));
            }

            VisiblePlayersInfo = new ObservableCollection<PlayerData>(PlayersInfo);
        }
    }
}
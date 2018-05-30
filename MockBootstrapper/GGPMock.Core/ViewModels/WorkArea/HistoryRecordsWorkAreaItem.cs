using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GGPMockBootstrapper.Models;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.ViewModels
{
    public class HistoryRecordsWorkAreaItem : WorkAreaItemBase, IDisposable
    {

        public HistoryRecordsWorkAreaItem(IWorkArea workArea)
            : base(workArea)
        {
            Models.GGPMockDataManager.Singleton.PropertyChanged += Singleton_PropertyChanged;
            this.History = new ObservableCollection<GameHistoryRecordViewModel>();
            this.ClearAction = new ActionViewModel("Remove all", new Command(Clear), ResourcesProvider.CreateBitmapImageSource("Clear24x24.png"));
            this.SaveHistoryAction = new ActionViewModel("Save history", new Command(SaveHistory), ResourcesProvider.CreateBitmapImageSource("Save24x24.png"));
            this.LoadHistoryAction = new ActionViewModel("Load saved history", new Command(LoadHistory), ResourcesProvider.CreateBitmapImageSource("OpenFile24x24.png"));
            RefreshRecords();
        }

       


        public ActionViewModel ClearAction { get; private set; }
        public ActionViewModel SaveHistoryAction { get; private set; }
        public ActionViewModel LoadHistoryAction { get; private set; }

        private void SaveHistory()
        {
            Models.GGPMockDataManager.Singleton.SaveHistory();
            UIServices.ShowMessage("History saved!");
        }


        private Models.Client.ClientProduct ClientProduct
        {
            get
            {
                return this.WorkArea.GetProduct<Models.Client.ClientProduct>();
            }
        }

        private void LoadHistory()
        {
            if (Models.GGPMockDataManager.Singleton.CanLoadSavedHistory)
            {
                Models.GGPMockDataManager.Singleton.LoadHistory();
            }
            else
            {
                UIServices.ShowMessage("There are no saved history records to load from!");
            }
            
        }

        private void Clear()
        {
            if (MessageBoxResponse.Yes == UIServices.ShowMessage("Are you sure you want to remove all history records?", "Clear history confirmation", MessageBoxButtonType.YesNo))
            {
                Models.GGPMockDataManager.Singleton.ClearHistory();
            }
        }



        public string GGPHistoryHandlerRelativeUrl
        {
            get
            {
                return this.WorkArea.GetProduct<Models.GGP.GGPProduct>().GetGGPHistoryHandlerRelativeUrl();
            }
        }

        void Singleton_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Models.GGPMockDataManager.Singleton.GetPropertyName(t => t.HistoryRecords))
            {
                UIServices.InvokeOnMainThread(RefreshRecords);
            }
        }

             

        private void RefreshRecords()
        {
            this.History.Clear();

            

            foreach (var recordViewModel in Models.GGPMockDataManager.Singleton.HistoryRecords.Select(r => new GameHistoryRecordViewModel(r.Game.GameType, r, this.WorkArea)))
            {
                this.History.Add(recordViewModel);
            }    
        }


        public ObservableCollection<GameHistoryRecordViewModel> History { get; private set; }



        #region IDisposable Members

        public void Dispose()
        {
            Models.GGPMockDataManager.Singleton.PropertyChanged -= Singleton_PropertyChanged;
        }

        #endregion
    }

    public class GameHistoryRecordViewModel : ViewModelBase
    {
        public GameHistoryRecordViewModel(int gameType, MockHistoryRecordsProvider.HistoryRecord record, IWorkArea workArea)
        {
            
            this.GameType = gameType;
            this.Record = record;
            this.WorkArea = workArea;
            
            this.ShowHistoryAction = new ActionViewModel("Show history", new Command(ShowHistory));
            this.RemoveAction = new ActionViewModel("Remove", new Command(RemoveRecord), ResourcesProvider.CreateBitmapImageSource("Excluded.png"));
        }

      

        public ActionViewModel ShowHistoryAction { get; private set; }
        public ActionViewModel RemoveAction { get; private set; }

        private void ShowHistory()
        {
            var html5GameTypes = ClientProduct.SupportedHtml5GameTypes.Select(g => g.GameType).ToArray();

            if (!html5GameTypes.Contains(this.GameType))
            {
                UIServices.ShowMessage("History is supported only for HTML5 games!");
                return;
            }

            var url = ClientProduct.GetHtml5HistoryUrl(this.GameType, this.GameID, this.GameCurrencyCode, this.Record.SimplifiedHistoryData.FreePlayCurrentAmount, this.Record.SimplifiedHistoryData.FreePlayWinningAmount);

            Models.InstalledBrowsersProvider.OpeneWithDefaultBrowser(url);
        }

        private void RemoveRecord()
        {
            Models.GGPMockDataManager.Singleton.RemoveHistoryRecord(this.GameID);
        }

        IWorkArea WorkArea { get; set; }

        private Models.Client.ClientProduct ClientProduct
        {
            get
            {
                return this.WorkArea.GetProduct<Models.Client.ClientProduct>();
            }
        }

        MockHistoryRecordsProvider.HistoryRecord Record { get; set; }

        public int GameType { get; private set; }

        IGamesInformationProvider GamesInformationProvider
        {
            get { return this.WorkArea.GamesInformationProvider; }
        }

        public string GameName
        {
            get
            {
                var gameInfo = GamesInformationProvider.GetGameInfoOrNull(this.GameType);

                if (gameInfo == null)
                {
                    return "N/A";
                }
                else
                {
                    return gameInfo.FriendlyName;
                }
            }
        }

        public long CID
        {
            get { return Record.Game.CustomerID; }
        }

        public long GameID
        {
            get { return Record.Game.GameID; }
        }

        public string GameCurrencyCode
        {
            get
            {
                return Record.Game.GameCurrencyCode;
            }
        }


        public long RoundsCount
        {
            get
            {
                return Record.Game.RoundsCount;
            }
        }


        public DateTime TimeStamp
        {
            get
            {
                return Record.DateAndTime;
            }
        }

    }
}

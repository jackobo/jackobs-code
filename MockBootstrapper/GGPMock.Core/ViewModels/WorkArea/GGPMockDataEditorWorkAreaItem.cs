using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;
using GGPMockBootstrapper.GGPMockDataProvider;
using GGPMockBootstrapper.Views;

namespace GGPMockBootstrapper.ViewModels
{
    public class GGPMockDataEditorWorkAreaItem : WorkAreaItemBase
    {
        public GGPMockDataEditorWorkAreaItem(IWorkArea workArea)
            : base(workArea)
        {
            this.UpdateGGPMockData = new ActionViewModel("Send this mock data to the GGP", new Command(SendGGPMockData));

            this.AddNewFreePlayAction = new ActionViewModel("Add new free play voucher", new Command(AddNewFreePlay));

            Models.GGPMockDataManager.Singleton.PropertyChanged += Singleton_PropertyChanged;
            FreePlay.CollectionChanged += FreePlay_CollectionChanged;
        }

        void FreePlay_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateFreePlayMockDataInTheDataProvider();
        }

        internal void UpdateFreePlayMockDataInTheDataProvider()
        {
            Models.GGPMockDataManager.Singleton.PropertyChanged -= Singleton_PropertyChanged;
            try
            {
                this.MockData.FreePlay = this.FreePlay.Select(fp => fp.FreePlayData).ToArray();
            }
            finally
            {
                Models.GGPMockDataManager.Singleton.PropertyChanged += Singleton_PropertyChanged;
            }
        }


        Models.IGamesInformationProvider GamesInformationProvider
        {
            get
            {
                return this.WorkArea.GamesInformationProvider;
            }
        }

        
        void Singleton_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Models.GGPMockDataManager.Singleton.GetPropertyName(t => t.MockData))
            {
                OnPropertyChanged(this.GetPropertyName(t => t.MockData));
                UpdateFreePlayCollection();
            }
            else if (e.PropertyName == Models.GGPMockDataManager.Singleton.GetPropertyName(t => t.MockState))
            {
                OnPropertyChanged(this.GetPropertyName(t => t.MockState));
            }
            else if (e.PropertyName == Models.GGPMockDataManager.Singleton.GetPropertyName(t => t.MockStateDescription))
            {
                OnPropertyChanged(this.GetPropertyName(t => t.MockStateDescription));
            }
            else if (e.PropertyName == Models.GGPMockDataManager.Singleton.GetPropertyName(t => t.AvailablePlayers))
            {
                OnPropertyChanged(this.GetPropertyName(t => t.AvailablePlayers));
            }
            else if (e.PropertyName == Models.GGPMockDataManager.Singleton.GetPropertyName(t => t.CurrentSelectedPlayer))
            {
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentSelectedPlayer));
            }
        }

        private void UpdateFreePlayCollection()
        {
            FreePlay.CollectionChanged -= FreePlay_CollectionChanged;
            try
            {
                FreePlay.Clear();

                foreach (var f in this.MockData.FreePlay)
                {
                    FreePlay.Add(new FreePlayMockDataViewModel(f, this));
                }
            }
            finally
            {
                FreePlay.CollectionChanged += FreePlay_CollectionChanged;
            }
        }

        ObservableCollection<FreePlayMockDataViewModel> _freePlay = new ObservableCollection<FreePlayMockDataViewModel>();

        public ObservableCollection<FreePlayMockDataViewModel> FreePlay
        {
            get { return _freePlay; }
            set
            {
                
                _freePlay = value;
                OnPropertyChanged(this.GetPropertyName(t => t.FreePlay));
            }
        }

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
            }

    
        }

        public GGPMockDataProvider.PlayerData MockData
        {
            get
            {

                if (Models.GGPMockDataManager.Singleton.MockData == null)
                {
                    return new GGPMockDataProvider.PlayerData();
                }
                else
                {
                    return Models.GGPMockDataManager.Singleton.MockData;
                }
            }
        }


        public Models.GGPMockState MockState
        {
            get
            {
                return Models.GGPMockDataManager.Singleton.MockState;
            }
        }


        public string MockStateDescription
        {
            get
            {
                return Models.GGPMockDataManager.Singleton.MockStateDescription;
            }
        }

       
        public GGPMockDataProvider.RegulationTypeMock[] SupportedRegulations
        {
            get { return Models.GGPMockDataManager.Singleton.SupportedRegulations; }
        }


        public ActionViewModel UpdateGGPMockData { get; private set; }

        private void SendGGPMockData()
        {
            Models.GGPMockDataManager.Singleton.SendMockData();

            UIServices.ShowMessage("Data was sent!");
        }

        public ActionViewModel AddNewFreePlayAction { get; private set; }

        private void AddNewFreePlay()
        {
            this.FreePlay.Add(new FreePlayMockDataViewModel(new FreePlayMockData() {FreePlayType = 2, Balance = 15000, MaxBetAmount = 15000, FreePlayState = FreePlayStateMock.Open, FreePlayID = 1, ExpirationDate = DateTime.Now.AddMonths(1)}, this));
        }


    
    }

    public class FreePlayMockDataViewModel : ViewModelBase
    {
        public FreePlayMockDataViewModel(FreePlayMockData freePlayData, GGPMockDataEditorWorkAreaItem parentWorkArea)
        {
            _freePlayData = freePlayData;
            this.ParentWorkArea = parentWorkArea;
            this.DeleteAction = new ActionViewModel("Remove", new Command(Delete), ResourcesProvider.CreateBitmapImageSource("Excluded.png"));
        }

        GGPMockDataEditorWorkAreaItem ParentWorkArea { get; set; }

        FreePlayMockData _freePlayData;

        public FreePlayMockData FreePlayData
        {
            get { return _freePlayData; }
        }


        public ActionViewModel DeleteAction { get; private set; }
        private void Delete()
        {
            ParentWorkArea.FreePlay.Remove(this);
        }

        public FreePlayStateMock FreePlayState
        {
            get { return _freePlayData.FreePlayState; }
            set
            {
                _freePlayData.FreePlayState = value;
                OnPropertyChanged(this.GetPropertyName(t => t.FreePlayState));
            }
        }

        public FreePlayTypeMock FreePlayType
        {
            get
            {
                return FreePlayTypes.FirstOrDefault(fpt => fpt.Id == _freePlayData.FreePlayType);
            }
            set
            {

                if (value == null)
                    _freePlayData.FreePlayType = 0;
                else
                    _freePlayData.FreePlayType = value.Id;

                OnPropertyChanged(this.GetPropertyName(t => t.FreePlayType));
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            ParentWorkArea.UpdateFreePlayMockDataInTheDataProvider();
        }


        public GGPMockDataProvider.FreePlayStateMock[] FreePlayStates
        {
            get { return Models.GGPMockDataManager.Singleton.FreePlayStates; }
        }


        public GGPMockDataProvider.FreePlayTypeMock[] FreePlayTypes
        {
            get { return Models.GGPMockDataManager.Singleton.FreePlayTypes; }
        }

        public long Balance
        {
            get { return _freePlayData.Balance; }
            set
            {
                _freePlayData.Balance = value;
                OnPropertyChanged(this.GetPropertyName(t => t.FreePlayType));
            }
        }

        public long MaxBetAmount
        {
            get { return _freePlayData.MaxBetAmount; }
            set
            {
                _freePlayData.MaxBetAmount = value;
                OnPropertyChanged(this.GetPropertyName(t => t.MaxBetAmount));
            }
        }

        public long? MaxWinningCap
        {
            get { return _freePlayData.MaxWinningCap; }
            set
            {
                _freePlayData.MaxWinningCap = value;
                OnPropertyChanged(this.GetPropertyName(t => t.MaxWinningCap));
            }
        }


        public long TotalWinnings
        {
            get { return _freePlayData.TotalWinnings; }
            set
            {
                _freePlayData.TotalWinnings = value;
                OnPropertyChanged(this.GetPropertyName(t => t.TotalWinnings));
            }
        }


        public string GameTypes
        {
            get
            {
                if (_freePlayData.GamesTypes.IsNullOrEmpty())
                    return string.Empty;


                return string.Join(",", _freePlayData.GamesTypes.Select(g => g.ToString()));
            }
            set
            {
                List<int> finalGames = new List<int>();


                foreach (var gt in StringToInt32Array(value))
                {
                    if (this.ParentWorkArea.WorkArea.GamesInformationProvider.GetGameInfoOrNull(gt) != null)
                        finalGames.Add(gt);
                }

                _freePlayData.GamesTypes = finalGames.ToArray();
                OnPropertyChanged(this.GetPropertyName(t => t.GameTypes));
            }
        }


        public string AlternativeGames
        {
            get
            {

               
                if (_freePlayData.AlternativeGames.IsNullOrEmpty())
                    return string.Empty;

                return string.Join(",", _freePlayData.AlternativeGames.Select(g => g.ToString()));
            }
            set
            {
                var finalGames = new List<int>();
                foreach (var gt in StringToInt32Array(value))
                {
                    if (this.ParentWorkArea.WorkArea.GamesInformationProvider.GetGameInfoOrNull(gt) != null)
                        finalGames.Add(gt);
                }

                _freePlayData.AlternativeGames = finalGames.ToArray();

                OnPropertyChanged(this.GetPropertyName(t => t.AlternativeGames));
            }
        }

        public string ExpirationDate
        {
            get { return FreePlayData.ExpirationDate == null ? null : FreePlayData.ExpirationDate.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    FreePlayData.ExpirationDate = null;
                }
                else
                {
                    DateTime temp;
                    if (DateTime.TryParse(value, out temp))
                    {
                        FreePlayData.ExpirationDate = temp;
                    }
                }
                OnPropertyChanged(this.GetPropertyName(t => t.ExpirationDate));
            }
        }

        public string FreePlayID
        {
            get { return FreePlayData.FreePlayID == null ? null: FreePlayData.FreePlayID.ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    FreePlayData.FreePlayID = null;
                }
                else
                {
                    FreePlayData.FreePlayID = long.Parse(value);
                    OnPropertyChanged(this.GetPropertyName(t => t.FreePlayID));
                }
            }
        }

        private int[] StringToInt32Array(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new int[0];
            }

            var items = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var result = new List<int>();

            foreach (var item in items)
            {
                int val;

                if (int.TryParse(item.Trim(), out val))
                    result.Add(val);

            }

            return result.ToArray();
        }
    }
}

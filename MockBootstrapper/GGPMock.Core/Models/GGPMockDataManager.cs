using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using GGPGameServer.ApprovalSystem.Common;
using GGPMockBootstrapper.GGPMockDataProvider;

namespace GGPMockBootstrapper.Models
{
    public class GGPMockDataManager : GGPMockDataProvider.IGGPMockSupportServiceCallback, INotifyPropertyChanged
    {
        static GGPMockDataManager()
        {
            Singleton = new GGPMockDataManager();
        }




        public GGPMockDataManager()
        {
            Thread t1 = new Thread(new ThreadStart(() => Connect()));
            t1.IsBackground = true;
            t1.Start();

            Thread t2 = new Thread(new ThreadStart(() => ReadOpenSessionsLoop()));
            t2.IsBackground = true;
            t2.Start();


            Thread t3 = new Thread(new ThreadStart(() => ReadHistoryRecordsLoop()));
            t3.IsBackground = true;
            t3.Start();
        }


        Guid? PlayersListChangesSubscriptionID { get; set; }

        WcfClientProxySafeRelease<GGPMockDataProvider.GGPMockSupportServiceClient, GGPMockDataProvider.IGGPMockSupportService> _ggpMockSupportServiceSafeRelease;

        private bool IsServiceProxyValid()
        {
            return _ggpMockSupportServiceSafeRelease != null && _ggpMockSupportServiceSafeRelease.Client.State != System.ServiceModel.CommunicationState.Faulted;
        }

        private void Connect()
        {
            while (true)
            {
                try
                {
                    if (!IsServiceProxyValid())
                    {
                        var oldSafeRelease = _ggpMockSupportServiceSafeRelease;
                        _ggpMockSupportServiceSafeRelease = CreateGGPMockSupportServiceClientSafeRelease();

                        this.AvailablePlayers = _ggpMockSupportServiceSafeRelease.Client.GetAvailablePlayers().OrderBy(p => p.Name).ToArray();
                        this.CurrentSelectedPlayer = this.AvailablePlayers.FirstOrDefault();

                        PlayersListChangesSubscriptionID = _ggpMockSupportServiceSafeRelease.Client.SubscribeForPlayersListChanges();

                        if (oldSafeRelease != null)
                            oldSafeRelease.Dispose();
                        
                        this.Languages = _ggpMockSupportServiceSafeRelease.Client.GetLanguages();
                        
                        var mockData = _ggpMockSupportServiceSafeRelease.Client.GetPlayerData(CurrentSelectedPlayer.CID);
                        this.SupportedRegulations = mockData.SupportedRegulations;
                        this.FreePlayStates = mockData.FreePlayStates;
                        this.FreePlayTypes = mockData.FreePlayTypes;
                        this.MockData = mockData.MockData;
                    }

                    if (_ggpMockSupportServiceSafeRelease.Client.IsAlive())
                    {
                        this.MockState = GGPMockState.Connected;
                        this.MockStateDescription = null;
                    }
                    else
                    {
                        this.MockState = GGPMockState.Disconnected;
                        this.MockStateDescription = "GGPMock service endpoint is not available";
                    }
                    
                }
                catch (Exception ex)
                {
                    this.MockState = GGPMockState.Disconnected;
                    MockStateDescription = ex.Message;

                    if (_ggpMockSupportServiceSafeRelease != null)
                    {
                        _ggpMockSupportServiceSafeRelease.Dispose();
                        _ggpMockSupportServiceSafeRelease = null;
                    }

                }

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            
        }

        public void ReadHistoryRecordsLoop()
        {
            while (true)
            {
                try
                {
                    ReadHistoryRecords();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        private void ReadHistoryRecords()
        {
            if (MockData != null && MockState == GGPMockState.Connected)
            {
                using (var safeRelease = CreateMockHistoryRecordsProviderSafeRelease())
                {
                    var newRecords = safeRelease.Chanel.GetAllRecords();
                    

                    var newHistoryRecords = newRecords ?? new MockHistoryRecordsProvider.HistoryRecord[0];

                    if (!AreEquals(_historyRecords, newHistoryRecords))
                    {
                        this.HistoryRecords = newRecords;
                    }

                }
            }
        }

        IWcfSafeRelease<MockHistoryRecordsProvider.IMockHistoryRecordsProvider> CreateMockHistoryRecordsProviderSafeRelease()
        {
            return new WcfClientProxySafeRelease<MockHistoryRecordsProvider.MockHistoryRecordsProviderClient, MockHistoryRecordsProvider.IMockHistoryRecordsProvider>(new MockHistoryRecordsProvider.MockHistoryRecordsProviderClient());
        }

        private bool AreEquals(MockHistoryRecordsProvider.HistoryRecord[] arr1, MockHistoryRecordsProvider.HistoryRecord[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;

            return (from r1 in arr1
                    join r2 in arr2 on r1.Game.GameID equals r2.Game.GameID
                    select r1).Count() == arr1.Length;
        }
        
        private MockHistoryRecordsProvider.HistoryRecord[] _historyRecords = new MockHistoryRecordsProvider.HistoryRecord[0];

        public MockHistoryRecordsProvider.HistoryRecord[] HistoryRecords
        {
            get
            {
                if (_historyRecords == null)
                    ReadHistoryRecords();


                return _historyRecords ?? new MockHistoryRecordsProvider.HistoryRecord[0];
            }
            private set
            {
                _historyRecords = value;
                OnPropertyChanged(this.GetPropertyName(t => t.HistoryRecords));
            }
        }

        public void ReadOpenSessionsLoop()
        {
            while (true)
            {
                try
                {
                    ReadOpenSessions();   
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        public void SaveOpenSessionsToFile()
        {
            if (_openSessions == null)
                return;

            PersistedSessions persistedSessions = new PersistedSessions(_openSessions.Select(s => new PersistableSessionObject() { SessionID = s.PlayerSessionID, SessionObjectJSON = s.SessionObject }).ToList());
            
            persistedSessions.SerializeObjectToFile(OpenSessionXmlFile);

            OnPropertyChanged(this.GetPropertyName(t => CanLoadSavedSessions));

        }

        private string SavedHistoryXmlFile
        {
            get { return Path.Combine(ApplicationFolders.AppData, "history.xml"); }
        }


        private string OpenSessionXmlFile
        {
            get { return Path.Combine(ApplicationFolders.AppData, "opensessions.xml"); }
        }

        public bool CanLoadSavedSessions
        {
            get
            {
                return File.Exists(OpenSessionXmlFile);
            }
        }

        public void LoadOpenSessionsFromFile()
        {

            if (!CanLoadSavedSessions)
                return;


            using (var safeRelease = CreateOpenSessionServiceSafeRelease())
            {
                var persistedSessions = UtilExtensions.DeserializeObjectFromFile<PersistedSessions>(OpenSessionXmlFile);
                
                foreach (var openSession in persistedSessions.Sessions)
                {
                    safeRelease.Chanel.LoadDisconnection(new OpenSessionProviderService.LoadDisconnectionRequest() { PlayerSessionID = openSession.SessionID, SessionObject = openSession.SessionObjectJSON });
                }
            }

            ReadOpenSessions();
            OnPropertyChanged(this.GetPropertyName(t => t.OpenSessions));
        }


     

        

        private void ReadOpenSessions()
        {
            if (MockData != null && MockState == GGPMockState.Connected)
            {
                using (var wcfSafeRelease = CreateOpenSessionServiceSafeRelease())
                {
                    var get888OpenGamesResponse = wcfSafeRelease.Chanel.GetOpenSessions(new OpenSessionProviderService.GetOpenSessionRequest() { CID = MockData.CID, OperatorID = 0 });
                    var getBingoOpenGamesResponse = wcfSafeRelease.Chanel.GetOpenSessions(new OpenSessionProviderService.GetOpenSessionRequest() { CID = MockData.CID, OperatorID = 1 });


                    var new888OpenSessions = get888OpenGamesResponse ?? new OpenSessionProviderService.GetOpenSessionResponse[0];
                    var newBingoOpenSessions = getBingoOpenGamesResponse ?? new OpenSessionProviderService.GetOpenSessionResponse[0];
                    var newOpenSessions = new888OpenSessions.Concat(newBingoOpenSessions).ToArray();

                    if (!AreEquals(_openSessions, newOpenSessions))
                    {
                        this.OpenSessions = newOpenSessions;
                    }
                }
            }
        }

        private IWcfSafeRelease<OpenSessionProviderService.IGGPMockOpenSessionProviderService> CreateOpenSessionServiceSafeRelease()
        {
            return new WcfClientProxySafeRelease<OpenSessionProviderService.GGPMockOpenSessionProviderServiceClient, OpenSessionProviderService.IGGPMockOpenSessionProviderService>(new OpenSessionProviderService.GGPMockOpenSessionProviderServiceClient());
        }

        private bool AreEquals(OpenSessionProviderService.GetOpenSessionResponse[] arr1, OpenSessionProviderService.GetOpenSessionResponse[] arr2)
        {
            arr1 = arr1 ?? new OpenSessionProviderService.GetOpenSessionResponse[0];
            arr2 = arr2 ?? new OpenSessionProviderService.GetOpenSessionResponse[0];

            if (arr1.Length != arr2.Length)
                return false;

            return arr1.Length == (from a in arr1
                                    join b in arr2 on a.PlayerSessionID equals b.PlayerSessionID
                                    where 0 == string.Compare(a.GameString, b.GameString) && 0 == string.Compare(a.SubGameString, b.SubGameString)
                                         select a).Count();
            
            
        }

        private bool AreEquals(OpenSessionProviderService.GetOpenSessionResponse s1, OpenSessionProviderService.GetOpenSessionResponse s2)
        {
            return 0 == string.Compare(s1.SessionObject, s2.SessionObject);
        }

        private OpenSessionProviderService.GetOpenSessionResponse[] _openSessions = null;

        public OpenSessionProviderService.GetOpenSessionResponse[] OpenSessions
        {
            get
            {
                if (_openSessions == null)
                    ReadOpenSessions();

                return (_openSessions ?? new OpenSessionProviderService.GetOpenSessionResponse[0]);
            }
            private set
            {
                _openSessions = value;
                OnPropertyChanged(this.GetPropertyName(t => t.OpenSessions));
            }
        }


        private WcfClientProxySafeRelease<GGPMockDataProvider.GGPMockSupportServiceClient, GGPMockDataProvider.IGGPMockSupportService> CreateGGPMockSupportServiceClientSafeRelease()
        {
            return new WcfClientProxySafeRelease<GGPMockDataProvider.GGPMockSupportServiceClient, GGPMockDataProvider.IGGPMockSupportService>(
                        new GGPMockDataProvider.GGPMockSupportServiceClient(new System.ServiceModel.InstanceContext(this), "NetNamedPipeBinding_IGGPMockSupportService"));
        }


        public static GGPMockDataManager Singleton { get; private set; }

   
        public void OnGGPMockDataChanged(GGPMockDataProvider.PlayerData playerData)
        {
            MockData = playerData;
        }


        private GGPMockState _mockState = GGPMockState.Disconnected;

        public GGPMockState MockState
        {
            get { return _mockState; }
            set
            {
                if (_mockState == value)
                    return;

                _mockState = value;
                OnPropertyChanged(this.GetPropertyName(t => t.MockState));
            }
        }

        string _mockStateDescription;

        public string MockStateDescription
        {
            get { return _mockStateDescription; }
            set
            {
                if (_mockStateDescription == value)
                    return;

                _mockStateDescription = value;
                OnPropertyChanged(this.GetPropertyName(t => t.MockStateDescription));
            }
        }


        private GGPMockDataProvider.PlayerData _mockData;

        public GGPMockDataProvider.PlayerData MockData
        {
            get { return _mockData; }
            set
            {
                UnsubscribeToMockDataPropertyChanged();
                _mockData = value;
                OnPropertyChanged(this.GetPropertyName(t => t.MockData));
                SubscribeToMockDataPropertyChanged();
            }
        }

        private void SubscribeToMockDataPropertyChanged()
        {
            if (_mockData != null)
            {
                _mockData.PropertyChanged += _mockData_PropertyChanged;
            }
        }

        private void UnsubscribeToMockDataPropertyChanged()
        {
            if (_mockData != null)
            {
                _mockData.PropertyChanged -= _mockData_PropertyChanged;
            }
        }

     


        void _mockData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.MockData.GetPropertyName(t => t.Regulation))
            {
                this.MockData.BankrollCurrency = this.MockData.Regulation.DefaultCurrency;
            }
        }


        private GGPMockDataProvider.RegulationTypeMock[] _supportedRegulations;

        public GGPMockDataProvider.RegulationTypeMock[] SupportedRegulations
        {
            get { return _supportedRegulations; }
            set
            {
                _supportedRegulations = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SupportedRegulations));
            }
        }


        private GGPMockDataProvider.FreePlayStateMock[] _freePlayStates;

        public GGPMockDataProvider.FreePlayStateMock[] FreePlayStates
        {
            get { return _freePlayStates; }
            set
            {
                _freePlayStates = value;
                OnPropertyChanged(this.GetPropertyName(t => t.FreePlayStates));
            }
        }

        GGPMockDataProvider.FreePlayTypeMock[] _freePlayTypes;
        public GGPMockDataProvider.FreePlayTypeMock[] FreePlayTypes
        {
            get { return _freePlayTypes; }
            set
            {
                _freePlayTypes = value;
                OnPropertyChanged(this.GetPropertyName(t => t.FreePlayTypes));
            }
        }


        private GGPMockDataProvider.LanguageMock[] _languages = new GGPMockDataProvider.LanguageMock[0];

        public GGPMockDataProvider.LanguageMock[] Languages
        {
            get { return _languages; }
            set
            {
                _languages = value;
                OnPropertyChanged(this.GetPropertyName(t => t.Languages));
            }
        }

      
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                           {
                               var ev = PropertyChanged;

                               if (ev != null)
                                   ev(this, new PropertyChangedEventArgs(propertyName));
                           }));
            }
            else
            {
                var ev = PropertyChanged;

                if (ev != null)
                    ev(this, new PropertyChangedEventArgs(propertyName));
            }

        }

        public void SendMockData()
        {
            if (this.MockState == GGPMockState.Disconnected)
            {
                throw new InvalidOperationException("GGP Mock endpoint is not available");
            }

            _ggpMockSupportServiceSafeRelease.Client.UpdateMockData(this.MockData);
        }

        public void RemoveSession(long playerSessionID)
        {
            using (var proxy = new ToolService.SupportServiceClient())
            {
                proxy.ForceCloseSession(playerSessionID);
            }

            ReadOpenSessions();
            OnPropertyChanged(this.GetPropertyName(t => t.OpenSessions));
        }

        public void ClearHistory()
        {
            using (var safeReleasse = CreateMockHistoryRecordsProviderSafeRelease())
            {
                safeReleasse.Chanel.ClearAllRecords();
            }

            this.HistoryRecords = new MockHistoryRecordsProvider.HistoryRecord[0];

        }

        public void SaveHistory()
        {
            if (_historyRecords == null)
                return;

            PersistedHistory history = new PersistedHistory(_historyRecords);

            history.SerializeObjectToFile(SavedHistoryXmlFile);
            OnPropertyChanged(this.GetPropertyName(t => t.CanLoadSavedHistory));

        }

        public void LoadHistory()
        {
            if (!CanLoadSavedHistory)
                return;

            var persistedSessions = UtilExtensions.DeserializeObjectFromFile<PersistedHistory>(SavedHistoryXmlFile);

            using (var safeRelease = CreateMockHistoryRecordsProviderSafeRelease())
            {
                safeRelease.Chanel.LoadRecords(persistedSessions.Records.ToArray());
            }

            ReadHistoryRecords();
            OnPropertyChanged(this.GetPropertyName(t => t.HistoryRecords));
        }

        public bool CanLoadSavedHistory
        {
            get
            {
                return File.Exists(SavedHistoryXmlFile);
            }
        }

        public void RemoveHistoryRecord(long gameID)
        {
            using (var safeRelease = CreateMockHistoryRecordsProviderSafeRelease())
            {
                safeRelease.Chanel.RemoveRecord(gameID);
            }

            ReadHistoryRecords();
            OnPropertyChanged(this.GetPropertyName(t => t.HistoryRecords));

        }

        #region IGGPMockSupportServiceCallback Members


        private GGPMockDataProvider.AvailablePlayer[] _availablePlayers;

        public GGPMockDataProvider.AvailablePlayer[] AvailablePlayers
        {
            get { return _availablePlayers; }
            set
            {
                _availablePlayers = value;
                OnPropertyChanged(this.GetPropertyName(t => t.AvailablePlayers));
            }
        }

        private GGPMockDataProvider.AvailablePlayer _currentSelectedPlayer;


        public GGPMockDataProvider.AvailablePlayer CurrentSelectedPlayer
        {
            get { return _currentSelectedPlayer; }
            set
            {

                if (_currentSelectedPlayer != null && IsServiceProxyValid())
                {
                    using (var proxy = CreateGGPMockSupportServiceClientSafeRelease())
                    {
                        proxy.Client.UnsubscribeFromPlayerChanges(_currentSelectedPlayer.CID, _currentSelectedPlayer.SubscriptionID.Value);
                    }
                    _currentSelectedPlayer.SubscriptionID = null;
                }

                _currentSelectedPlayer = value;


                if (_currentSelectedPlayer != null)
                {
                    _ggpMockSupportServiceSafeRelease = CreateGGPMockSupportServiceClientSafeRelease();
                    _currentSelectedPlayer.SubscriptionID = _ggpMockSupportServiceSafeRelease.Client.SubscribeForPlayerChanges(_currentSelectedPlayer.CID);
                    MockData = _ggpMockSupportServiceSafeRelease.Client.GetPlayerData(_currentSelectedPlayer.CID).MockData;    
                }

                OnPropertyChanged(this.GetPropertyName(t => t.CurrentSelectedPlayer));
                
            }
        }

        public void OnAvailablePlayersChanged(GGPMockDataProvider.AvailablePlayer[] availablePlayers)
        {
            this.AvailablePlayers = availablePlayers;
        }

        #endregion

        public void Disconnect()
        {
            if (PlayersListChangesSubscriptionID != null && IsServiceProxyValid())
            {
                _ggpMockSupportServiceSafeRelease.Client.UnsubscribeFromPlayersListChanges(PlayersListChangesSubscriptionID.Value);
            }

        }

        public PlayerData GetPlayerData(int cid)
        {
            return _ggpMockSupportServiceSafeRelease.Client.GetPlayerData(cid).MockData;    
        }

        public void CreateNewUser(string userName)
        {
            var newPlayer = _ggpMockSupportServiceSafeRelease.Client.CreatePlayerFromUserName(userName);
            this.CurrentSelectedPlayer = new GGPMockDataProvider.AvailablePlayer(newPlayer.CID, newPlayer.UserName);  
        }

        public void CreateNewUser(int cid, string userName)
        {
            var newPlayer = _ggpMockSupportServiceSafeRelease.Client.CreatePlayerFromCidAndUserName(cid, userName);
            this.CurrentSelectedPlayer = new GGPMockDataProvider.AvailablePlayer(newPlayer.CID, newPlayer.UserName);
        }

        public bool DeletePlayer(int cid)
        {
            if (CurrentSelectedPlayer.CID == cid)
            {
                var nextAvailablePlayer = AvailablePlayers.FirstOrDefault(item => item.CID != cid);
                if (nextAvailablePlayer != null)
                {
                    CurrentSelectedPlayer = nextAvailablePlayer;
                }
            }
            return _ggpMockSupportServiceSafeRelease.Client.DeletePlayer(cid);
        }
    }

    public class PersistedSessions
    {
        public PersistedSessions()
            : this(new PersistableSessionObject[0])
        {

        }

        public PersistedSessions(IEnumerable<PersistableSessionObject> sessions)
        {
            this.Sessions = new List<PersistableSessionObject>(sessions);
        }

        public List<PersistableSessionObject> Sessions { get; set; }
    }


    public class PersistedHistory
    {
        public PersistedHistory()
            : this(new MockHistoryRecordsProvider.HistoryRecord[0])
        {

        }

        public PersistedHistory(IEnumerable<MockHistoryRecordsProvider.HistoryRecord> records)
        {
            this.Records = new List<MockHistoryRecordsProvider.HistoryRecord>(records);
        }

        public List<MockHistoryRecordsProvider.HistoryRecord> Records { get; set; }
    }



    public class PersistableSessionObject
    {
        public long SessionID { get; set; }
        public string SessionObjectJSON { get; set; }
    }

    public enum GGPMockState
    {
        Disconnected,
        Connected
    }


    public interface ILoggerAppender
    {
        void Append(GGPMockLoggerService.GGPMockLoggerMessage message);
    }

}



namespace GGPMockBootstrapper.GGPMockDataProvider
{
    public partial class PlayerData
    {
        public long GetItalyGameLimits()
        {
            if (this.Regulation.Id != RegulationTypeMock.Italy)
                return -1;

            if (this.ItalyRegulationData == null)
                return -1;

            return this.ItalyRegulationData.CurrentGameLimit;


        }

        internal string GetSpainGameLimitsOperatorXml()
        {
            if (this.Regulation.Id != RegulationTypeMock.Spain)
            {
                return string.Empty;
            }

            return string.Format(@"<RequestedGameLimit>{0}</RequestedGameLimit>
                                    <RequestedTimeLimit>{1}</RequestedTimeLimit>
                                    <RestrictionPeriod>{2}</RestrictionPeriod>
                                    <IntervalReminderInMinutes>{3}</IntervalReminderInMinutes>",
                             this.SpainRegulationData.GameLimit,
                             this.SpainRegulationData.RequestedTimeLimit,
                             this.SpainRegulationData.RestrictionPeriod,
                             this.SpainRegulationData.IntervalReminderInMinutes);

        }
    }

    public partial class AvailablePlayer
    {
        public AvailablePlayer()
        {

        }

        public AvailablePlayer(int cid, string name)
        {
            this.CID = cid;
            this.Name = name;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Name) || this.Name == this.CID.ToString())
                return this.CID.ToString();


            return string.Format("{0} [CID: {1}]", this.Name, this.CID);

        }

        public override bool Equals(object obj)
        {
            var theOther = obj as AvailablePlayer;

            if (theOther == null)
                return false;

            return this.CID == theOther.CID;
        }

        public override int GetHashCode()
        {
            return this.CID.GetHashCode();
        }



        public Guid? SubscriptionID { get; set; }
    }


    public partial class FreePlayStateMock
    {
        public FreePlayStateMock()
        {

        }

        public FreePlayStateMock(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }


        public override bool Equals(object obj)
        {
            var theOther = obj as FreePlayStateMock;

            if (theOther == null)
                return false;

            return theOther.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public static readonly FreePlayStateMock Open = new FreePlayStateMock(0, "Open");
        public static readonly FreePlayStateMock MinGameBetReached = new FreePlayStateMock(1, "Free play game minimum bet was reached");
        public static readonly FreePlayStateMock Closed = new FreePlayStateMock(2, "Free play is closed");
        public static readonly FreePlayStateMock MaxWinningCapReached = new FreePlayStateMock(3, "Free play reached max winning cap");

        
    }


    public partial class FreePlayTypeMock
    {
        public override bool Equals(object obj)
        {
            var theOther = obj as FreePlayTypeMock;

            if (theOther == null)
                return false;

            return theOther.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }


    }


  

    public partial class RegulationTypeMock
    {
        public override bool Equals(object obj)
        {
            var theOther = obj as RegulationTypeMock;

            if (theOther == null)
                return false;

            return theOther.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name + " [" + this.Id + "]";
        }

        public static readonly int Italy = 1;
        public static readonly int Spain = 2;
    }


    public partial class LanguageMock
    {
        public override bool Equals(object obj)
        {
            var theOther = obj as LanguageMock;

            if (theOther == null)
                return false;

            return theOther.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
    
}

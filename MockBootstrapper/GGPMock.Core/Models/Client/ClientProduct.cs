using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GGPMockBootstrapper.GGPMockDataProvider;
using GGPGameServer.ApprovalSystem.Common;
using Microsoft.Web.Administration;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using GGPMockBootstrapper.Models.GGP;

namespace GGPMockBootstrapper.Models.Client
{
    public class ClientProduct : Product, Models.ISwfFilesProvider
    {
        private IGamesInformationProvider GamesInformationProvider;

        public ClientProduct(IInstalationContext installationContext, IGamesInformationProvider gamesInformationProvider)
            : base("Client", installationContext)
        {
            GamesInformationProvider = gamesInformationProvider;
        }

        public override IInstallAction[] GetInstallActions()
        {
            if (IISDetection.IsInstalled)
            {
                return new IInstallAction[]
                {
                    new ThinClientInstallAction(this),
                    new Html5ClientInstallAction(this),
                    new OpenWebsitePortInTheFirewallAction()
                };
            }
            else
            {
                return new IInstallAction[0];
            }
        }


        public string ThinClientForMockAppName
        {
            get { return ThinClientInstallAction.AppName; }
        }

        public string Html5GamesForMockAppName
        {
            get
            {
                return Html5ClientInstallAction.AppName;
            }
        }


        protected override string GetContentSectionName()
        {
            return "Client";
        }

        public InstalledGame[] SupportedFlashGameTypes
        {
            get
            {
                using (var iisManager = new Microsoft.Web.Administration.ServerManager())
                {
                    var app = iisManager.FindApplication(this.ThinClientForMockAppName);
                    if (app == null)
                        return new InstalledGame[0];


                    var gamesFolder = Path.Combine(app.GetPhysicalPath(), "versionX", "games");
                    InitFlashGamesFolderMonitor(gamesFolder);


                    return ReadGamesTypes(gamesFolder);
                }
            }
        }




        FileSystemWatcher _flashGamesFolderMonitor = null;
        private void InitFlashGamesFolderMonitor(string gamesFolder)
        {

            if (_flashGamesFolderMonitor == null)
            {
                _flashGamesFolderMonitor = new FileSystemWatcher(gamesFolder);
                _flashGamesFolderMonitor.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName;
                _flashGamesFolderMonitor.InternalBufferSize = 65536;

                _flashGamesFolderMonitor.IncludeSubdirectories = true;
                StartListenForFlashGamesChanges();
                _flashGamesFolderMonitor.EnableRaisingEvents = true;
            }
        }

        private void StartListenForFlashGamesChanges()
        {
            _flashGamesFolderMonitor.Changed += FlashGamesFolderMonitorEventsHandler;
            _flashGamesFolderMonitor.Created += FlashGamesFolderMonitorEventsHandler;
            _flashGamesFolderMonitor.Deleted += FlashGamesFolderMonitorEventsHandler;
            _flashGamesFolderMonitor.Renamed += FlashGamesFolderMonitorEventsHandler;
        }

        private void StopListenForFlashGamesChanges()
        {
            _flashGamesFolderMonitor.Changed -= FlashGamesFolderMonitorEventsHandler;
            _flashGamesFolderMonitor.Created -= FlashGamesFolderMonitorEventsHandler;
            _flashGamesFolderMonitor.Deleted -= FlashGamesFolderMonitorEventsHandler;
            _flashGamesFolderMonitor.Renamed -= FlashGamesFolderMonitorEventsHandler;
        }


        #region ISwfFilesProvider Members

        public SwfFile[] GetSwfFiles(int forGameType)
        {
            using (var iisManager = new Microsoft.Web.Administration.ServerManager())
            {
                var app = iisManager.FindApplication(this.ThinClientForMockAppName);
                if (app == null)
                    return new SwfFile[0];

                var mediaFolder = Path.Combine(app.GetPhysicalPath(), "versionX", "Games", forGameType.ToString(), "Media");

                if (!Directory.Exists(mediaFolder))
                    return new SwfFile[0];

                var swfFiles = Directory.EnumerateFiles(mediaFolder, "*.swf")
                                       .Select(f => new SwfFile(f))
                                       .ToArray();


                string selectedSwfName = GetSelectedSwfFile(forGameType, app.GetPhysicalPath());
                foreach (var swf in swfFiles)
                {
                    swf.IsSelected = (0 == string.Compare(swf.Name, selectedSwfName, true));
                }


                return swfFiles;
            }

        }


        private string GetSelectedSwfFile(int forGameType, string appPath)
        {
            var casino888GamesSettingsXml = Path.Combine(appPath, "versionX", "brand", GetOperatorNameForGameType(forGameType), "settings.xml");

            var casino888GamesSettingsXmlDocument = XDocument.Load(casino888GamesSettingsXml);
            var gamesElement = casino888GamesSettingsXmlDocument.Root.Element("games");
            var gameElement = gamesElement.Elements("game")
                                          .FirstOrDefault(g => g.Element("id") != null && g.Element("id").Value == forGameType.ToString());

            if (gameElement == null)
                return null;


            var gameNameElement = gameElement.Element("gameName");

            if (gameNameElement == null)
                return null;

            return gameNameElement.Value;

        }

        public void UpdateSelectedSwf(int gameType, SwfFile swf)
        {
            using (var iisManager = new Microsoft.Web.Administration.ServerManager())
            {
                var app = iisManager.FindApplication(this.ThinClientForMockAppName);
                if (app == null)
                    return;

                UpdateSettingsXmlAndGetSelected(gameType, swf, app.GetPhysicalPath());
            }
        }

        private int GetOperatorIdForGameType(int gameType)
        {
            return IsBingoOperatorForGameType(gameType) ? 1 : 0;
        }

        private bool IsBingoOperatorForGameType(int gameType)
        {
            var gameInfo = GamesInformationProvider.GetGameInfoOrNull(gameType);
            if (gameInfo != null)
            {
                return gameInfo.OperatorId == 1;
            }

            return false;
        }

        private string GetOperatorNameForGameType(int gameType)
        {
            return IsBingoOperatorForGameType(gameType) ? "888Bingo" : "888Games";
        }

        private void UpdateSettingsXmlAndGetSelected(int gameType, SwfFile swf, string appPath)
        {
            var casino888GamesSettingsXml = Path.Combine(appPath, "versionX", "brand", GetOperatorNameForGameType(gameType), "settings.xml");

            var casino888GamesSettingsXmlDocument = XDocument.Load(casino888GamesSettingsXml);
            var gamesElement = casino888GamesSettingsXmlDocument.Root.Element("games");
            var gameElement = gamesElement.Elements("game")
                                          .FirstOrDefault(g => g.Element("id") != null && g.Element("id").Value == gameType.ToString());


            bool shouldSave = false;
            if (gameElement == null)
            {
                gameElement = new XElement("game");
                gameElement.Add(new XElement("id", gameType.ToString()));
                gameElement.Add(new XElement("help"));
                gamesElement.Add(gameElement);
                shouldSave = true;
            }


            var gameNameElement = gameElement.Element("gameName");
            if (swf != null)
            {
                if (gameNameElement == null)
                {
                    gameNameElement = new XElement("gameName", swf.Name);
                    gameElement.Add(gameNameElement);
                    shouldSave = true;
                }
                else if (gameNameElement.Value != swf.Name)
                {
                    gameNameElement.Value = swf.Name;
                    shouldSave = true;
                }

            }

            var folderGameElement = gameElement.Element("folderGame");
            if (folderGameElement != null)
            {
                folderGameElement.Value = gameType.ToString();
            }
            else
            {
                gameElement.Add(new XElement("folderGame", gameType.ToString()));
            }

            if (shouldSave)
                casino888GamesSettingsXmlDocument.Save(casino888GamesSettingsXml);
        }

        #endregion

        void FlashGamesFolderMonitorEventsHandler(object sender, FileSystemEventArgs e)
        {
            OnPropertyChanged(this.GetPropertyName(t => t.SupportedFlashGameTypes));
        }

        void FlashGamesFolderMonitorEventsHandler(object sender, RenamedEventArgs e)
        {
            OnPropertyChanged(this.GetPropertyName(t => t.SupportedFlashGameTypes));
        }



        FileSystemWatcher _html5GamesFolderMonitor = null;
        private void InitHtml5GamesFolderMonitor(string gamesFolder)
        {

            if (_html5GamesFolderMonitor == null)
            {
                _html5GamesFolderMonitor = new FileSystemWatcher(gamesFolder);
                _html5GamesFolderMonitor.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName;
                _html5GamesFolderMonitor.InternalBufferSize = 65536;

                _html5GamesFolderMonitor.IncludeSubdirectories = true;
                StartListenForHtml5GamesChanges();
                _html5GamesFolderMonitor.EnableRaisingEvents = true;
            }
        }

        private void StartListenForHtml5GamesChanges()
        {
            _html5GamesFolderMonitor.Changed += Html5GamesFolderMonitorEventsHandler;
            _html5GamesFolderMonitor.Created += Html5GamesFolderMonitorEventsHandler;
            _html5GamesFolderMonitor.Deleted += Html5GamesFolderMonitorEventsHandler;
            _html5GamesFolderMonitor.Renamed += Html5GamesFolderMonitorEventsHandler;
        }

        private void StopListenForHtml5GamesChanges()
        {
            _html5GamesFolderMonitor.Changed -= Html5GamesFolderMonitorEventsHandler;
            _html5GamesFolderMonitor.Created -= Html5GamesFolderMonitorEventsHandler;
            _html5GamesFolderMonitor.Deleted -= Html5GamesFolderMonitorEventsHandler;
            _html5GamesFolderMonitor.Renamed -= Html5GamesFolderMonitorEventsHandler;
        }

        void Html5GamesFolderMonitorEventsHandler(object sender, FileSystemEventArgs e)
        {
            OnPropertyChanged(this.GetPropertyName(t => t.SupportedHtml5GameTypes));
        }

        void Html5GamesFolderMonitorEventsHandler(object sender, RenamedEventArgs e)
        {
            OnPropertyChanged(this.GetPropertyName(t => t.SupportedHtml5GameTypes));
        }

        public InstalledGame[] SupportedHtml5GameTypes
        {
            get
            {
                using (var iisManager = new Microsoft.Web.Administration.ServerManager())
                {

                    var app = iisManager.FindApplication(this.Html5GamesForMockAppName);
                    if (app == null)
                        return new InstalledGame[0];
                    
                    InitHtml5GamesFolderMonitor(app.GetPhysicalPath());

                    return CustomHtmlGamesRepository.Games.Select(g => new InstalledGame(g.GameType, g.BaseUrl) { IsCustomGame = true })
                            .Union(ReadGamesTypes(app.GetPhysicalPath()))
                            .ToArray();

                }
            }
        }

        private InstalledGame[] ReadGamesTypes(string fromFolder)
        {
            var gameTypes = new List<InstalledGame>();

            foreach (var dir in System.IO.Directory.EnumerateDirectories(fromFolder).Select(d => new DirectoryInfo(d)))
            {
                int gt;

                if (int.TryParse(dir.Name, out gt))
                    gameTypes.Add(new InstalledGame(gt, dir.FullName));
            }

            return gameTypes.ToArray();
        }

        public string GetHtml5GameUrl(OpenGameParameters parameters)
        {
            var mockData = GetMockData();


            if (mockData == null)
                throw new ApplicationException("GGP Mock is not available!");

            var gameData = new Html5GameData(mockData.CID.ToString());
            gameData.gameName = parameters.GameName;
            gameData.gametype = parameters.GameTypeId;
            gameData.operatorid = GetOperatorIdForGameType(parameters.GameTypeId);
            gameData.operatorxml = BuildOperatorData(parameters, gameData.operatorid);
            gameData.balance = mockData.RealMoneyBalance;
            gameData.regulationTypeID = mockData.Regulation.Id;
            gameData.gamecurrencycode = parameters.Currency;
            gameData.lang = parameters.Language.Iso3;
            gameData.jointype = (int)parameters.JoinType;

            string baseUrl = string.Empty;

            var theGame = this.SupportedHtml5GameTypes.FirstOrDefault(g => g.GameType == parameters.GameTypeId && 0 == string.Compare(g.PhysicalPath, parameters.GamePhisicalPath, true));

            if (theGame.IsCustomGame)
                baseUrl = theGame.PhysicalPath;
            else
                baseUrl = string.Format("http://{0}:{1}/Html5GamesForGGPMock/{2}/Game/",
                                    GetMachineHostName(),
                                    GetApplicationWebSitePort(Html5GamesForMockAppName),
                                    parameters.GameTypeId);


            return string.Format("{0}?gameData={1}",
                                    baseUrl,
                                    System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(gameData, Newtonsoft.Json.Formatting.None)));
        }

        private object BuildOperatorData(OpenGameParameters parameters, int operatorId)
        {
            var mockData = GetMockData();

            switch (operatorId)
            {
                case 0:
                {
                    var operatorData = new _888ClientDataWrapper();
                    operatorData._888ClientData.IsFreePlay = parameters.IsFreePlay ? 1 : 0;
                    operatorData._888ClientData.BrandID = (parameters.Brand == null ? 0 : parameters.Brand.BrandId);

                    if (mockData.Regulation.Id == RegulationTypeMock.Italy)
                    {
                        operatorData._888ClientData.GameLimits = mockData.ItalyRegulationData.CurrentGameLimit;
                    }
                    else if (mockData.Regulation.Id == RegulationTypeMock.Spain)
                    {
                        operatorData._888ClientData.RequestedGameLimit = mockData.SpainRegulationData.GameLimit;
                        operatorData._888ClientData.RequestedTimeLimit = mockData.SpainRegulationData.RequestedTimeLimit;
                        operatorData._888ClientData.RestrictionPeriod = mockData.SpainRegulationData.RestrictionPeriod;
                        operatorData._888ClientData.IntervalReminderInMinutes =
                            mockData.SpainRegulationData.IntervalReminderInMinutes;
                    }

                    return operatorData;
                }
                case 1:

                    return new _BIPDataWrapper
                    {
                        BIPData = new BIPData(6, mockData.CID, mockData.CID.ToString(), parameters.IsFreePlay,
                            mockData.RealMoneyBalance, mockData.RealMoneyBalance, parameters.GameTypeId)
                    };
            }

            return null;
        }

        private CustomHtmlGamesRepository _customHtmlGamesRepository;

        private CustomHtmlGamesRepository CustomHtmlGamesRepository
        {
            get
            {
                if (_customHtmlGamesRepository == null)
                {
                    _customHtmlGamesRepository = LoadCustomHtmlGames();
                }
                
                return _customHtmlGamesRepository;
                
            }
        }

        private CustomHtmlGamesRepository LoadCustomHtmlGames()
        {
            if (!File.Exists(CustomHtml5GamesXmlFile))
            {
                return new CustomHtmlGamesRepository();
            }

            return UtilExtensions.DeserializeObjectFromFile<CustomHtmlGamesRepository>(CustomHtml5GamesXmlFile);
        }


        public void AddCustomHtml5Game(CustomHtml5Game newGame)
        {
            if (this.CustomHtmlGamesRepository.Games.Any(g => g.GameType == newGame.GameType))
                throw new ArgumentException(string.Format("Game {0} already exists", newGame.GameType));

            this.CustomHtmlGamesRepository.Games.Add(newGame);
            this.CustomHtmlGamesRepository.SerializeObjectToFile(CustomHtml5GamesXmlFile);
            OnPropertyChanged(this.GetPropertyName(t => t.SupportedHtml5GameTypes));
        }

        public void RemoveCustomHtml5Game(int gameTypeId)
        {
            var gameToRemove = this.CustomHtmlGamesRepository.Games.FirstOrDefault(g => g.GameType == gameTypeId);

            if (gameToRemove == null)
                throw new ArgumentException(string.Format("Game {0} doesn't exists", gameTypeId));

            this.CustomHtmlGamesRepository.Games.Remove(gameToRemove);
            this.CustomHtmlGamesRepository.SerializeObjectToFile(CustomHtml5GamesXmlFile);
            OnPropertyChanged(this.GetPropertyName(t => t.SupportedHtml5GameTypes));
        }



        private string CustomHtml5GamesXmlFile
        {
            get { return Path.Combine(ApplicationFolders.AppData, "MyHtml5Games.xml"); }
        }


        public string GetHtml5HistoryUrl(int gameType, long gameID, string currencyCode, long? fpCurrentAmount, long? fpWinningAmount)
        {
            Html5HistoryData historyData = new Html5HistoryData();
            historyData.gamecurrencycode = currencyCode;
            historyData.history = new Html5HistoryData.HistoryInfo(gameID);

            if (fpCurrentAmount != null)
            {
                historyData.freeplay = new Html5HistoryData.FreeplayInfo() { freeplayCurrentAmount = fpCurrentAmount.Value, freeplayWinningAmount = fpWinningAmount ?? 0 };
            }

            var customGame = this.CustomHtmlGamesRepository.Games.FirstOrDefault(g => g.GameType == gameType);



            if (customGame == null)
            {

                return string.Format("http://localhost:{0}/Html5GamesForGGPMock/{1}/Game/?gameData={2}",
                              GetApplicationWebSitePort(Html5GamesForMockAppName),
                              gameType,
                              System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(historyData, Newtonsoft.Json.Formatting.None)));
            }
            else
            {
                return string.Format("{0}?gameData={1}",
                           customGame.BaseUrl,
                           System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(historyData, Newtonsoft.Json.Formatting.None)));
            }


        }



        private int GetApplicationWebSitePort(string applicationName)
        {
            return ServerManagerExtensionMethods.GetApplicationHttpPort(applicationName);
        }


        private class Html5HistoryData
        {
            public Html5HistoryData()
            {
                this.jointype = 99;
                this.balance = 0;
                this.lang = "eng";
            }

            public int jointype { get; set; }
            public int integration { get; set; }
            public long balance { get; set; }
            public string lang { get; set; }
            public string gamecurrencycode { get; set; }
            public HistoryInfo history { get; set; }
            public FreeplayInfo freeplay { get; set; }


            public class HistoryInfo
            {
                public HistoryInfo()
                {
                    this.token = "123456";
                }
                public HistoryInfo(long gameID)
                {
                    this.token = "123456";
                    this.gameID = gameID;
                }

                public string token { get; set; }
                public long gameID { get; set; }

            }


            public class FreeplayInfo
            {
                public long freeplayCurrentAmount { get; set; }
                public long freeplayWinningAmount { get; set; }
            }


        }

        private class Html5GameData
        {
            public Html5GameData(string token)
            {
                this.integration = 1;
                this.gameId = 710;
                this.token = token;
                this.url = "/mobile/default.aspx";
                this.jointype = 1;
                this.operatorid = 0;
                this.lang = "eng";
                this.gamecurrencycode = "EUR";
                this.GameProviderType = 3;
                this.isHybrid = false;
            }
            public int integration { get; set; }
            public int gameId { get; set; }
            public string token { get; set; }
            public string url { get; set; }
            public int jointype { get; set; }
            public int operatorid { get; set; }
            public string lang { get; set; }
            public int gametype { get; set; }
            public string gamecurrencycode { get; set; }
            public long balance { get; set; }
            public object operatorxml { get; set; }
            public string gameName { get; set; }
            public int GameProviderType { get; set; }
            public int regulationTypeID { get; set; }
            public bool isHybrid { get; set; }
        }

        public class _888ClientDataWrapper
        {
            public _888ClientDataWrapper()
            {
                _888ClientData = new _888ClientData();
            }
            public _888ClientData _888ClientData { get; set; }
        }

        public class _888ClientData
        {
            public _888ClientData()
            {
                this.ClientVersion = "Touch-0-EN-0-1.0-0-0";
                this.ClientPlatform = 700;
                this.ClientURL = "url";
                this.ProductPackage = 37;
                this.ClientType = 14;
                this.GameLimits = -1;

                this.RequestedGameLimit = -1;
                this.RequestedTimeLimit = 0;
                this.RestrictionPeriod = 0;
                this.IntervalReminderInMinutes = 0;

                this.EnableOperatorData = true;

            }
            public string ClientVersion { get; set; }
            public int ClientPlatform { get; set; }
            public string ClientURL { get; set; }
            public int BrandID { get; set; }
            public int SubBrandID { get; set; }
            public int ProductPackage { get; set; }
            public int ClientType { get; set; }
            public long GameLimits { get; set; }
            public bool EnableOperatorData { get; set; }
            public int IsFreePlay { get; set; }

            public long RequestedGameLimit { get; set; }
            public long? RequestedTimeLimit { get; set; }
            public long? RestrictionPeriod { get; set; }
            public int? IntervalReminderInMinutes { get; set; }

        }


        public class _BIPDataWrapper
        {
            public _BIPDataWrapper()
            {
                BIPData = new BIPData();
            }
            public BIPData BIPData { get; set; }
        }

        public class BIPData
        {
            public int NetworkID { get; set; }
            public int PlayerID { get; set; }
            public string Token { get; set; }
            public bool IsFreePlay { get; set; }
            public long BankrollCurrency { get; set; }
            public long BankrollInGameCurrency { get; set; }
            public long GameID { get; set; }

            public BIPData()
            {
                
            }

            public BIPData(int networkID, int playerID, string token, bool isFreePlay, long bankrollCurrency, long bankrollInGameCurrency, long gameID)
            {
                NetworkID = networkID;
                PlayerID = playerID;
                Token = token;
                IsFreePlay = isFreePlay;
                BankrollCurrency = bankrollCurrency;
                BankrollInGameCurrency = bankrollInGameCurrency;
                GameID = gameID;
            }
        }

        public string GetFlashGameURL(OpenGameParameters parameters)
        {
            
            var mockData = GetMockData();

            if (parameters.IsFreePlay && (mockData.FreePlay == null || mockData.FreePlay.Length == 0))
            {
                throw new ValidationException("You don't have any free play voucher defined!");
            }

            string url = string.Format("http://localhost:{0}/{1}/ThinClient.html?realMoney={2}&platformID={3}&soundEnabled={4}&Token={5}&GameType={6}&GameGroupID={7}&GameCurrency={8}&lang={9}&PlayerAlias={10}&OperatorID={11}&OperatorXml={12}&JoinType={13}&debug={14}&balance={15}&BrandName={16}&width={17}&height={18}&RegulationMode={19}&showBottomBar=true",
                                 GetApplicationWebSitePort(ThinClientForMockAppName),
                                 this.ThinClientForMockAppName,
                                 1, //realMoney
                                 1, //platformID
                                 parameters.SoundEnabled ? 1 : 0, //soundEnabled
                                 mockData.CID.ToString(),
                                 parameters.GameTypeId,
                                 0, //GameGroupID
                                 parameters.Currency,
                                 parameters.Language.Iso2,
                                 "alias",
                                 GetOperatorIdForGameType(parameters.GameTypeId), //operatorid
                                 System.Web.HttpUtility.UrlEncode(GetOperatorDataXml(parameters.IsFreePlay, mockData, parameters.JoinType, parameters.GameTypeId)),
                                 (int)parameters.JoinType, //JoinType
                                 "true", //debug
                                 parameters.IsFreePlay ? mockData.FreePlay.First().Balance : mockData.RealMoneyBalance,
                                 GetOperatorNameForGameType(parameters.GameTypeId), //BrandName
                                 980,
                                 602,
                                 mockData.Regulation.Id);

            PrepareFlashWrapperLanguageFolders(parameters.Language);

            return url;


        }

        private void PrepareFlashWrapperLanguageFolders(LanguageMock languageMock)
        {
            using (var iisManager = new Microsoft.Web.Administration.ServerManager())
            {

                var app = iisManager.FindApplication(this.ThinClientForMockAppName);
                if (app == null)
                    return;

                var wrapperFolder = Path.Combine(app.GetPhysicalPath(), "versionX", "wrapper");

                var languageFolder = Path.Combine(wrapperFolder, languageMock.Iso2);

                if (Directory.Exists(languageFolder))
                    return;


                this.InstallationContext.EnvironmentServices.FileSystem.CopyFolderContent(Path.Combine(wrapperFolder, "en"), languageFolder);


            }
        }



        private string GetOperatorDataXml(bool isFreePlay, PlayerData mockData, JoinTypeEnum joinType, int gameType)
        {

            if (joinType == JoinTypeEnum.Regular)
            {
                if (IsBingoOperatorForGameType(gameType))
                {
                    return string.Format(@"<BIPData><NetworkID>{0}</NetworkID><PlayerID>{1}</PlayerID><Token>{2}</Token><IsFreePlay>{3}</IsFreePlay><BankrollCurrency>{4}</BankrollCurrency><BankrollInGameCurrency>{5}</BankrollInGameCurrency><GameID>{6}</GameID></BIPData>"
                                       , 6
                                       , mockData.CID
                                       , mockData.CID
                                       , isFreePlay ? 1 : 0
                                       , mockData.BankrollCurrency
                                       , mockData.RealMoneyBalance
                                       , gameType);
                }
                return string.Format(@"<_888ClientData><CID>{0}</CID><SubBrandID>8</SubBrandID><BrandID>8</BrandID><ProductPackage>5</ProductPackage><ClientVersion>Touch-0-EN-0-1.0-0-0</ClientVersion><EnableOperatorData>true</EnableOperatorData><IsFreePlay>{1}</IsFreePlay><GameLimits>{2}</GameLimits>{3}</_888ClientData>"
                                       , mockData.CID
                                       , isFreePlay ? 1 : 0
                                       , mockData.GetItalyGameLimits()
                                       , mockData.GetSpainGameLimitsOperatorXml());
            }
            else
            {
                return string.Format(@"<AnonymousData><BankrollAmount>{0}</BankrollAmount></AnonymousData>",
                                       mockData.RealMoneyBalance);
            }

        }






        public void GenerateMobileDeviceHelperPage(string gameName, string url)
        {
            using (var iisManager = new ServerManager())
            {


                var homeDirectory = iisManager.GetRootHomeDirectory(this.Html5GamesForMockAppName);


                var htmlContent = string.Format("<html><head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no\"><h1>GGP Mock - Mobile device lunch page</h1></head>	<body>	<a href=\"{0}\">{1}</a>	</body></html>", url, gameName);

                File.WriteAllText(Path.Combine(homeDirectory, "mock.html"), htmlContent);

            }
        }



        public void CreateFlashGameFromArtifactoryZip(int gameType, string gameZipFile, IEnvironmentServices environmentServices)
        {
            string flashGamesFolder;
            using (var iisManager = new Microsoft.Web.Administration.ServerManager())
            {
                var app = iisManager.FindApplication(this.ThinClientForMockAppName);
                flashGamesFolder = Path.Combine(app.GetPhysicalPath(), "VersionX", "Games");
            }

            try
            {
                StopListenForFlashGamesChanges();
                var gameTypeFolder = Path.Combine(flashGamesFolder, gameType.ToString());
                environmentServices.FileSystem.DeleteFolder(gameTypeFolder);

                environmentServices.UnzipFile(gameZipFile, flashGamesFolder);

                environmentServices.FileSystem.DeleteFile(gameZipFile);

                OnPropertyChanged(this.GetPropertyName(t => t.SupportedFlashGameTypes));
            }
            finally
            {
                StartListenForFlashGamesChanges();
            }
        }

        internal void CreateHtml5GameFromArtifactoryZip(int gameType, string gameZipFile, string chillZipFile, IEnvironmentServices environmentServices, string ggpHttpHandlerUrl, string historyHandlerUrl)
        {
            string html5GamesFolder = null;
            using (var iisManager = new Microsoft.Web.Administration.ServerManager())
            {

                var app = iisManager.FindApplication(this.Html5GamesForMockAppName);
                if (app == null)
                    return;

                html5GamesFolder = app.GetPhysicalPath();
            }



            try
            {
                StopListenForHtml5GamesChanges();
                
                var gameTypeTempFolder = Path.Combine(html5GamesFolder, gameType.ToString() + "_temp");
                environmentServices.FileSystem.DeleteFolder(gameTypeTempFolder);

                environmentServices.UnzipFile(gameZipFile, gameTypeTempFolder);


                var gameTypeFolder = Path.Combine(html5GamesFolder, gameType.ToString());
                environmentServices.FileSystem.DeleteFolder(gameTypeFolder);

                var gameFolder = Path.Combine(gameTypeFolder, "Game");

                environmentServices.FileSystem.CopyFolderContent(FindFolderContainingIndexHtmlFile(gameTypeTempFolder), gameFolder);

                environmentServices.FileSystem.DeleteFolder(gameTypeTempFolder);

                environmentServices.UnzipFile(chillZipFile, gameTypeFolder);


                var chillConfigFile = Path.Combine(gameTypeFolder, "chill", "resources", "config", "config.js");

                var chillConfigContent = File.ReadAllText(chillConfigFile);

                chillConfigContent = ReplaceBetween(chillConfigContent, "\"realServer\"", "',", string.Format(": '{0}", ggpHttpHandlerUrl));
                chillConfigContent = ReplaceBetween(chillConfigContent, "\"realServer\"", "\",", string.Format(": \"{0}", ggpHttpHandlerUrl));
                chillConfigContent = ReplaceBetween(chillConfigContent, "\"demoServer\"", "',", string.Format(": '{0}", ggpHttpHandlerUrl));
                chillConfigContent = ReplaceBetween(chillConfigContent, "\"demoServer\"", "\",", string.Format(": \"{0}", ggpHttpHandlerUrl));


                chillConfigContent = ReplaceBetween(chillConfigContent,
                                                    "\"url\"",
                                                    "/GGPHistoryHandler/',", string.Format(": '{0}", historyHandlerUrl.Replace(string.Format("/{0}/", HistoryHandlerInstallAction.GGPHistoryHandlerAppName), "")));

                chillConfigContent = ReplaceBetween(chillConfigContent,
                                                    "\"url\"",
                                                    "/GGPHistoryHandler/\",", string.Format(": \"{0}", historyHandlerUrl.Replace(string.Format("/{0}/", HistoryHandlerInstallAction.GGPHistoryHandlerAppName), "")));

                chillConfigContent = ReplaceBetween(chillConfigContent,
                                                    "\"urlDemo\"",
                                                    "/GGPHistoryHandler/',", string.Format(": '{0}", historyHandlerUrl.Replace(string.Format("/{0}/", HistoryHandlerInstallAction.GGPHistoryHandlerAppName), "")));

                chillConfigContent = ReplaceBetween(chillConfigContent,
                                                    "\"urlDemo\"",
                                                    "/GGPHistoryHandler/\",", string.Format(": \"{0}", historyHandlerUrl.Replace(string.Format("/{0}/", HistoryHandlerInstallAction.GGPHistoryHandlerAppName), "")));



                environmentServices.FileSystem.DeleteFile(chillConfigFile);
                environmentServices.FileSystem.WriteAllText(chillConfigFile, chillConfigContent);




                environmentServices.FileSystem.DeleteFile(gameZipFile);
                environmentServices.FileSystem.DeleteFile(chillZipFile);

                OnPropertyChanged(this.GetPropertyName(t => t.SupportedHtml5GameTypes));
            }
            finally
            {
                StartListenForHtml5GamesChanges();
            }
            
            
        }

        private string FindFolderContainingIndexHtmlFile(string folder)
        {
            if(Directory.EnumerateFiles(folder, "index.html").Any())
            {
                return folder;
            }

            foreach(var subfolder in Directory.EnumerateDirectories(folder))
            {
                var result = FindFolderContainingIndexHtmlFile(subfolder);

                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            return null;

        }

        private string ReplaceBetween(string text, string start, string end, string replaceValue)
        {
            var startEscaped = Regex.Escape(start);
            var endEscaped = Regex.Escape(end);

            return Regex.Replace(text, startEscaped + @"(.*?)" + endEscaped, start  + replaceValue + end);

        }


        
    }



}

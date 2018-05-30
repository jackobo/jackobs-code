using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;
using GamesPortal.Client.ViewModels.NotificationArea;

using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class LatestApprovedGameVersionWorkspaceItem : WorkspaceItem
    {
        public LatestApprovedGameVersionWorkspaceItem(IServiceLocator serviceLocator, 
                                                      GameTechnology gameTechnology)
            : base(serviceLocator)
        {
            this.GameTechnology = gameTechnology;

            RefreshOriginalRecords();
            
            InitFilterOptions();

            InitDownloadOptions();
            
            RefreshCurrentRecords();

#warning I should subscribe to an event for games approval and autorefresh when something is approved
            //SubscribeToEvent<Interfaces.PubSubEvents.GameSynchronizationFinishedEventData>(GamesSynchronizationFinishedHandler);
        }


        public override string Title
        {
            get { return $"Latest approved version for {GameTechnology} games"; }
        }

        private void InitDownloadOptions()
        {
            this.DownloadOptions = new GamesDownloadOptionsViewModel(DownloadVersions, this.ServiceLocator);
        }

        private void RefreshOriginalRecords()
        {
            OriginalRecords = this.ServiceLocator.GetInstance<IReportingService>()
                             .GetLatestApprovedGameVersionForEachRegulationAndGameType()
                             .Where(item => item.Infrastructure.GameTechnology == this.GameTechnology)
                             .ToList();
        }

        private void GamesSynchronizationFinishedHandler(Interfaces.PubSubEvents.GameSynchronizationFinishedEventData obj)
        {
            RefreshOriginalRecords();
            RefreshCurrentRecords();
        }

        private void RefreshCurrentRecords()
        {

            this.Records = SetIsMJC(GetFilteredRecords()
                                      .Select(record => new ReportItem(record.GameName,
                                                                    record.MainGameType,
                                                                    record.Infrastructure,
                                                                    record.LatestVersion,
                                                                    record.LatestQAApprovedVersion,
                                                                    record.Regulation,
                                                                    record.QAVersionInfo?.Version,
                                                                    record.PMVersionInfo?.Version,
                                                                    record.ProductionVersionInfo?.Version))

                                    .ToList());
        }

        private List<ReportItem> SetIsMJC(List<ReportItem> reportItems)
        {
            foreach(var gameTypeGroup in reportItems.GroupBy(item => new { item.GameType, item.Platform, item.LatestQAApprovedVersion}))
            {
                bool isMJC = !string.IsNullOrEmpty(gameTypeGroup.Key.LatestQAApprovedVersion)
                             && gameTypeGroup.All(r => r.QAVersion == gameTypeGroup.Key.LatestQAApprovedVersion);

                foreach(var regulation in gameTypeGroup)
                {
                    regulation.IsMJC = isMJC ? "Yes" : "No";
                }
            }

            return reportItems;
        }

        private IEnumerable<LatestApprovedGameVersionForEachRegulation> GetFilteredRecords()
        {
            var records = OriginalRecords;

            var selectedRegulations = this.FilterOptions.RegulationsSelector.SelectedRegulations;
            var selectedInfrastructures = this.FilterOptions.InfrastructuresSelector.SelectedInfrastructures;
            records = records.Where(item => selectedRegulations.Contains(item.Regulation)
                                            && selectedInfrastructures.Contains(item.Infrastructure));
            if (this.FilterOptions.OnlyInternalGames)
                records = records.Where(item => item.IsExternal == false);
            else if (this.FilterOptions.OnlyExternalGames)
                records = records.Where(item => item.IsExternal == true);

            return records.ToList();
        }

        
        private void InitFilterOptions()
        {
            var regulations = OriginalRecords.Where(item => item.Regulation != null)
                                                    .Select(item => item.Regulation)
                                                    .Distinct()
                                                    .ToArray();
            var gameInfrastructures = OriginalRecords.Select(item => item.Infrastructure).Distinct().ToArray();

            this.FilterOptions = new GamesReportFilterOptionsViewModel(regulations, gameInfrastructures);

            this.FilterOptions.FilterChanged += FilterOptions_FilterChanged;
        }

        private void DownloadVersions()
        {
            
            var backgroundActionsArea = this.ServiceLocator.GetInstance<IBackgroundActionsArea>();

            string rootDownloadFolder = this.DownloadOptions.DestinationFolder;
            

            foreach (var versionToDownload in this.GetFilteredRecords().OrderBy(rec => rec.GameName))
            {

                DownloadInfo downloadInfo = versionToDownload.GetDownloadInfo();
                
                var zipFileName = Path.Combine(rootDownloadFolder, "Downloads", versionToDownload.Infrastructure.ToString(), versionToDownload.Regulation.Name, versionToDownload.GameName + "_" + downloadInfo.FileName);
                var unzipFolder = Path.Combine(rootDownloadFolder, "Unzip", versionToDownload.Infrastructure.ToString(), versionToDownload.Regulation.Name, versionToDownload.MainGameType.ToString());

                backgroundActionsArea.AddAction(new DownloadAndUnzipFileAction(new DownloadFileAction(this.ServiceLocator, new Uri(downloadInfo.Uri), zipFileName),
                                                                               new UnzipFileAction(zipFileName, unzipFolder, this.ServiceLocator)));
                                                                                
            }
            
        }


        private bool _showQAColumn = true;

        public bool ShowQAColumn
        {
            get { return _showQAColumn; }
            set
            {
                if (SetProperty(ref _showQAColumn, value))
                    OnPropertyChanged(nameof(ReportParameters));
            }
        }


        private bool _showPMColumn = true;
        public bool ShowPMColumn
        {
            get { return _showPMColumn; }
            set
            {
                if (SetProperty(ref _showPMColumn, value))
                    OnPropertyChanged(nameof(ReportParameters));
            }
        }

        private bool _showPRODColumn = true; 
        public bool ShowPRODColumn      
        {
            get { return _showPRODColumn; }
            set
            {
                if (SetProperty(ref _showPRODColumn, value))
                    OnPropertyChanged(nameof(ReportParameters));
            }
        }


        public ReportViewerParameter[] ReportParameters
        {
            get
            {
                var parameters = new List<ReportViewerParameter>();

                parameters.Add(new ReportViewerParameter("HideQA", (!this.ShowQAColumn).ToString()));
                parameters.Add(new ReportViewerParameter("HidePM", (!this.ShowPMColumn).ToString()));
                parameters.Add(new ReportViewerParameter("HidePROD", (!this.ShowPRODColumn).ToString()));

                return parameters.ToArray();
            }
        }

        void FilterOptions_FilterChanged(object sender, EventArgs e)
        {
            RefreshCurrentRecords();
        }
                
        public GamesReportFilterOptionsViewModel FilterOptions { get; private set; }

        public GamesDownloadOptionsViewModel DownloadOptions { get; private set; }
        

        GameTechnology _platformType;
        public GameTechnology GameTechnology
        {
            get { return _platformType; }
            set
            {
                _platformType = value;
                OnPropertyChanged(() => GameTechnology);
            }

        }


        IEnumerable<LatestApprovedGameVersionForEachRegulation> OriginalRecords { get; set; }

        IEnumerable<ReportItem> _records;
        public IEnumerable<ReportItem> Records
        {
            get { return _records; }
            set 
            {
                SetProperty(ref _records,  value); 
            }
        }

      
        public class ReportItem
        {
            public ReportItem(string gameName, int gameType, GameInfrastructure infrastructure, VersionNumber latestVersion, Optional<VersionNumber> latestQaApprovedVersion, RegulationType regulation, VersionNumber qaVersion, VersionNumber pmVersion, VersionNumber productionVersion)
            {
                this.GameName = gameName;
                this.GameType = gameType;
                this.Platform = infrastructure.PlatformType.ToDescription();
                this.Regulation = regulation?.Name;
                this.LatestVersion = latestVersion?.ToString();
                latestQaApprovedVersion.Do(v => this.LatestQAApprovedVersion = v.ToString());
                this.QAVersion = qaVersion?.ToString();
                this.PMVersion = pmVersion?.ToString();
                this.ProductionVersion = productionVersion?.ToString();
            }

            public string IsMJC { get; set; }
            
            public string GameName { get; set; }
            public int GameType { get; set; }
            public string Regulation { get; set; }
            public string LatestVersion { get; set; }

            public string LatestQAApprovedVersion { get; set; }


            public string QAVersion { get; private set; }

            public string PMVersion { get; private set; }

            public string ProductionVersion { get; private set; }

            public string Platform { get; private set; }
        }
    }


  

    
}

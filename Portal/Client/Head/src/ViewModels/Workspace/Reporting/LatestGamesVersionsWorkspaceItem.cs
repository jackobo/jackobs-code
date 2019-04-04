using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using GamesPortal.Client.Interfaces.Services;
using System.IO;
using GamesPortal.Client.ViewModels.NotificationArea;
using Spark.Wpf.Common.ViewModels;
using System.ComponentModel;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class LatestGamesVersionsWorkspaceItem : WorkspaceItem
    {
        public LatestGamesVersionsWorkspaceItem(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {

            this.DownloadOptions = new GamesDownloadOptionsViewModel(Download, this.ServiceLocator);
            RefreshOriginalRecords();
            InitFilter();
            RefreshCurrentRecords();

            SubscribeToEvent<Interfaces.PubSubEvents.GameSynchronizationFinishedEventData>(GameSynchronizationFinishedEventHandler);           
        }

        private void InitFilter()
        {
            var regulations = this.OriginalRecords.Select(r => r.Regulation).Distinct().ToArray();
            var infrastructures = this.OriginalRecords.Select(r => r.Infrastructure).Distinct().ToArray();
            this.FilterOptions = new GamesReportFilterOptionsViewModel(regulations, infrastructures);
            this.FilterOptions.FilterChanged += FilterOptions_FilterChanged;
        }

        private void FilterOptions_FilterChanged(object sender, EventArgs e)
        {
            RefreshCurrentRecords();
        }
        
        private void GameSynchronizationFinishedEventHandler(Interfaces.PubSubEvents.GameSynchronizationFinishedEventData obj)
        {
            RefreshOriginalRecords();
            InitFilter();
            RefreshCurrentRecords();
        }

        public override string Title
        {
            get
            {
                return "Latest version for each game";
            }
        }
        
        private void Download()
        {
            

            var backgroundActionsArea = this.ServiceLocator.GetInstance<IBackgroundActionsArea>();

            string rootDownloadFolder = this.DownloadOptions.DestinationFolder;


            foreach (var versionToDownload in this.CurrentRecords.OrderBy(rec => rec.GameName))
            {

                var zipFileName = Path.Combine(rootDownloadFolder, "Downloads", versionToDownload.Infrastructure.ToString(), versionToDownload.Regulation.Name, versionToDownload.GameName + "_" + versionToDownload.DownloadInfo.FileName);
                var unzipFolder = Path.Combine(rootDownloadFolder, "Unzip", versionToDownload.Infrastructure.ToString(), versionToDownload.Regulation.Name, versionToDownload.MainGameType.ToString());

                backgroundActionsArea.AddAction(new DownloadAndUnzipFileAction(new DownloadFileAction(this.ServiceLocator, new Uri(versionToDownload.DownloadInfo.Uri), zipFileName),
                                                                               new UnzipFileAction(zipFileName, unzipFolder, this.ServiceLocator)));

            }


        }

        private GamesDownloadOptionsViewModel _downloadOptions;

        public GamesDownloadOptionsViewModel DownloadOptions
        {
            get { return _downloadOptions; }
            set
            {
                SetProperty(ref _downloadOptions,  value);

            }
        }

        private GamesReportFilterOptionsViewModel _filterOptions;

        public GamesReportFilterOptionsViewModel FilterOptions
        {
            get { return _filterOptions; }
            private set
            {
                SetProperty(ref _filterOptions,  value);
            }
        }


        private void RefreshOriginalRecords()
        {
            this.OriginalRecords = this.ServiceLocator.GetInstance<IReportingService>().GetLatestGameVersionForEachRegulation();
        }
        
        private void RefreshCurrentRecords()
        {
            
            var query = OriginalRecords.Where(rec => FilterOptions.RegulationsSelector.SelectedRegulations.Contains(rec.Regulation)
                                                     && this.FilterOptions.InfrastructuresSelector.SelectedInfrastructures.Contains(rec.Infrastructure));

            if (this.FilterOptions.OnlyInternalGames)
            {
                query = query.Where(item => item.IsExternal == false);
            }
            else if (this.FilterOptions.OnlyExternalGames)
            {
                query = query.Where(item => item.IsExternal == true);
            }

            query = query.OrderBy(item => item.IsExternal).ThenBy(item => item.Infrastructure.GameTechnology).ThenBy(item => item.GameName).ThenBy(item => item.Regulation);

            this.CurrentRecords = query.ToList();
        }

        private IEnumerable<Interfaces.Entities.LatestGameVersionForRegulation> OriginalRecords { get; set; }

        private IEnumerable<Interfaces.Entities.LatestGameVersionForRegulation> _currentRecords;

        public IEnumerable<Interfaces.Entities.LatestGameVersionForRegulation> CurrentRecords
        {
            get { return _currentRecords; }
            set
            {
                SetProperty(ref _currentRecords,  value);
            }
        }
        
        
    }


}

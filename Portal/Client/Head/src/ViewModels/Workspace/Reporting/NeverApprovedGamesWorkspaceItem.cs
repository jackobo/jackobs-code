using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class NeverApprovedGamesWorkspaceItem : WorkspaceItem
    {
        public NeverApprovedGamesWorkspaceItem(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            RefreshRecords();

            SubscribeToEvent<Interfaces.PubSubEvents.GameSynchronizationFinishedEventData>(GameSynchronizationFinishedEventHandler);
        }

        private void GameSynchronizationFinishedEventHandler(Interfaces.PubSubEvents.GameSynchronizationFinishedEventData obj)
        {
            RefreshRecords();
        }

        private void RefreshRecords()
        {
            this.Records = this.ServiceLocator.GetInstance<Interfaces.Services.IReportingService>().GetNeverApprovedGames()
                                                                                    .OrderBy(item => item.GameInfrastructure.GameTechnology)
                                                                                    .OrderBy(item => item.GameInfrastructure.PlatformType)
                                                                                    .ThenBy(item => item.GameName)
                                                                                    .Select(item => new NeverApprovedGameViewMode(item))
                                                                                    .ToList();
        }


        private IEnumerable<NeverApprovedGameViewMode> _records;

        public IEnumerable<NeverApprovedGameViewMode> Records
        {
            get { return _records; }
            set
            {
                SetProperty(ref _records, value);
            }
        }


        public override string Title
        {
            get
            {
                return "Never approved games";
            }
        }


        public class NeverApprovedGameViewMode
        {
            public NeverApprovedGameViewMode(NeverApprovedGame record)
            {
                this.Record = record;
            }

            NeverApprovedGame Record { get; set; }


            public string GameTechnology
            {
                get { return this.Record.GameInfrastructure.ToString(); }
            }

            public string GameName
            {
                get { return this.Record.GameName; }
            }

            public int MainGameType
            {
                get { return this.Record.MainGameType; }
            }


            public string LatestVersion
            {
                get
                {
                    return this.Record.LatestVersion.ToString();   
                }

            }
            
        }
    }
}

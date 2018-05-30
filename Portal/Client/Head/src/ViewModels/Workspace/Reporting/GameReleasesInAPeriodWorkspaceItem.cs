using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;

using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Exceptions;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class GameReleasesInAPeriodWorkspaceItem : WorkspaceItem
    {
        public GameReleasesInAPeriodWorkspaceItem(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.ToDate = DateTime.Now;
            this.FromDate = ToDate.AddDays(-7).Date;
            SearchCommand = new Command(Search);
        }

        public override string Title
        {
            get
            {
                return "Games version releases in a specific period";
            }
        }

        private DateTime _fromDate;
        public DateTime FromDate
        {
            get
            {
                return _fromDate;
            }

            set
            {
                SetProperty(ref _fromDate, value);
            }
        }

        private DateTime _toDate;
        public DateTime ToDate
        {
            get
            {
                return _toDate;
            }

            set
            {
                SetProperty(ref _toDate, value);
            }
        }

        
        public ICommand SearchCommand { get; private set; }

        private void Search()
        {
            if(this.FromDate >= this.ToDate)
            {
                throw new ValidationException("Start date must be less than the End date");
            }

            this.GamesReleases = ServiceLocator.GetInstance<IReportingService>()
                                               .GetGameReleases(new Spark.Wpf.Common.DateTimeInterval(this.FromDate, this.ToDate))
                                               .OrderBy(item => item.Name)
                                               .ToArray();

            this.Summary = this.GamesReleases.GroupBy(item => new { item.GameId, item.MainGameType, item.Name, item.GameInfrastructure })
                                             .Select(group => new GameReleaseSummary(group.Key.MainGameType, group.Key.Name, group.Key.GameInfrastructure, group.Count()))
                                             .OrderBy(group => group.Name)
                                             .ToArray();

            if(this.GamesReleases.Length == 0)
            {
                ServiceLocator.GetInstance<IMessageBox>().ShowMessage("There are no games releases in the period you specified!");
            }

            OnPropertyChanged(nameof(TotalNumberOfVersions));
        }


        public int TotalNumberOfVersions
        {
            get
            {
                return this.Summary.Sum(item => item.VersionsCount);
            }
        }

        public Interfaces.Entities.GameVersionRelease[] _gamesReleases;

        public Interfaces.Entities.GameVersionRelease[] GamesReleases
        {
            get { return _gamesReleases; }
            private set { SetProperty(ref _gamesReleases, value); }
        }


        private GameReleaseSummary[] _summary;

        public GameReleaseSummary[] Summary
        {
            get { return _summary; }
            set { SetProperty(ref _summary , value); }
        }




        public class GameReleaseSummary
        {
            public GameReleaseSummary(int mainGameType, string name, GameInfrastructure gameInfrastructure, int versionsCount)
            {
                this.MainGameType = mainGameType;
                this.Name = name;
                this.GameInfrastructure = gameInfrastructure;
                this.VersionsCount = versionsCount;
            }

            public int MainGameType { get; private set; }
            public string Name { get; private set; }
            public GameInfrastructure GameInfrastructure { get; private set; }
            public int VersionsCount { get; private set; }
        }
    }
}

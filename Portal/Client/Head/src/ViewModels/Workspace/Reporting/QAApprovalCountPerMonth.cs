using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class QAApprovalCountPerMonth : WorkspaceItem
    {
        public QAApprovalCountPerMonth(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.RefreshCommand = new Command(Refresh);
            SetDefaults();
            Refresh();
        }



        public override string Title
        {
            get
            {
                return "QA Approval Count";
            }
        }

        private void SetDefaults()
        {
            var startDate = DateTime.Today.AddMonths(-12);

            this.StartMonth = Month.Get(startDate.Month);
            this.StartYear = startDate.Year;

            this.EndMonth = Month.Get(DateTime.Today.Month);
            this.EndYear = DateTime.Today.Year;
        }

        public Month[] AllMonths
        {
            get { return Month.All; }
        }

        private Month _startMonth;

        public Month StartMonth
        {
            get { return _startMonth; }
            set { SetProperty(ref _startMonth, value); }
        }

        private int _startYear;

        public int StartYear
        {
            get { return _startYear; }
            set { SetProperty(ref _startYear, value); }
        }

        private Month _endMonth;

        public Month EndMonth
        {
            get { return _endMonth; }
            set { SetProperty(ref _endMonth, value); }
        }

        private int _endYear;

        public int EndYear
        {
            get { return _endYear; }
            set { SetProperty(ref _endYear, value); }
        }

        public ICommand RefreshCommand { get; private set; }

        private void Refresh()
        {
            var startDate = new DateTime(this.StartYear, this.StartMonth.Id, 1);
            var endDate = new DateTime(this.EndYear, this.EndMonth.Id, 1).AddMonths(1).AddDays(-1);

            this.Records = this.ServiceLocator.GetInstance<IReportingService>()
                                              .GetQAApprovedGamesInPeriod(startDate, endDate)
                                              .OrderBy(item => item.GameName)
                                              .ThenBy(item => item.ApprovalInfo.ApprovalDate)
                                              .Select(item => new ReportItem(item))
                                              .ToArray();
        }


        ReportItem[] _records;
        public ReportItem[] Records
        {
            get { return _records; }
            set
            {
                SetProperty(ref _records, value);
            }
        }


        public class ReportItem
        {
            public ReportItem(ApprovedGameVersion approvedGameVersion)
            {
                _approvedGameVersion = approvedGameVersion;
            }

            ApprovedGameVersion _approvedGameVersion;
            public string GameName
            {
                get { return _approvedGameVersion.GameName; }
            }
            public int MainGameType
            {
                get { return _approvedGameVersion.MainGameType; }
            }

            public DateTime QAApprovalDate
            {
                get { return _approvedGameVersion.ApprovalInfo.ApprovalDate.Value; }
            }

            public long MonthCode
            {
                get
                {
                    return QAApprovalDate.Year * 1000 + QAApprovalDate.Month;
                }
            }

            public string MonthName
            {
                get
                {
                    return QAApprovalDate.ToString("MMM", CultureInfo.InvariantCulture) + " " + QAApprovalDate.Year;
                }
            }
            

            public string GameInfrastructure
            {
                get { return _approvedGameVersion.GameInfra.ToString(); }
            }

            public string Version
            {
                get { return _approvedGameVersion.Version.ToString(); }
            }

            public string Regulation
            {
                get { return _approvedGameVersion.Regulation.ToString(); }
            }
        }
    }
}

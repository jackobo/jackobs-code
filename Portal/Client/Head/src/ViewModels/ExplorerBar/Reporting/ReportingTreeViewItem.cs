using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class ReportingTreeViewItem : TreeViewItem
    {
        public ReportingTreeViewItem(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            
            this.Caption = "Reports";

            this.Items.Add(new LatestApprovedGameVersionTreeViewItem(this, serviceLocator, "HTML5 games - latest approved version", GameTechnology.Html5));

            this.Items.Add(new LatestApprovedGameVersionTreeViewItem(this, serviceLocator, "Flash games - latest approved version", GameTechnology.Flash));
            
            //this.Items.Add(new LatestApprovedGameVersionTreeViewItem(this, serviceLocator, "HTML5 PC games - latest approved version", new GameInfrastructure(GameTechnology.Html5, PlatformType.PC)));
            //this.Items.Add(new LatestApprovedGameVersionTreeViewItem(this, serviceLocator, "HTML5 PC & Mobile games - latest approved version", new GameInfrastructure(GameTechnology.Html5, PlatformType.Both)));
            this.Items.Add(new LatestGamesVersionsTreeViewItem(this, serviceLocator));
            this.Items.Add(new NeverApprovedGamesTreeViewItem(this, serviceLocator));
            this.Items.Add(new GameReleasesInAPeriodTreeViewItem(this, serviceLocator));
            this.Items.Add(new QAApprovalCountPerMonthTreeViewItem(this, serviceLocator));

        }


    }
}

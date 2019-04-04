using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class LatestApprovedGameVersionTreeViewItem : TreeViewItem
    {
        public LatestApprovedGameVersionTreeViewItem(TreeViewItem parent, 
                                                      IServiceLocator serviceLocator, 
                                                      string caption, 
                                                      GameTechnology technology)
            : base(parent, serviceLocator)
        {
            
            this.Caption = caption;
            this.GameTechnology = technology;
        }

        public GameTechnology GameTechnology { get; private set;}

        public override IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator)
        {
            return new Workspace.LatestApprovedGameVersionWorkspaceItem(serviceLocator, GameTechnology);   
        }
    }
}

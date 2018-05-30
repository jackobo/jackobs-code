using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels.ExplorerBar
{
    public class ChillWrapperTreeViewItem : GameTreeViewItem
    {
        public ChillWrapperTreeViewItem(Game game, TreeViewItem parent, IServiceLocator serviceLocator)
            : base(game, parent, serviceLocator)
        {

        }

        protected override void UpdateCaption()
        {
            this.Caption = this.Game.Name;
        }

        public override IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator)
        {
            return new Workspace.ChillWrapperWorkspaceItem(this.Game, this.Game.SupportedInfrastructures.First(), this.ServiceLocator);
            
        }

        public bool IsChill
        {
            get
            {
                return this.Game.Category == GamingComponentCategory.Chill;
            }
        }

        protected override void CreateSupportedTechnologies()
        {
            //ignore
        }

        protected override void RefreshTechnologies()
        {
            //ignore
        }
    }
}

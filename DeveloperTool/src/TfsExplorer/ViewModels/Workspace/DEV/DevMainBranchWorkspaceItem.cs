using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class DevMainBranchWorkspaceItem : MainBranchWorkspaceItem<IDevBranch>
    {
        public DevMainBranchWorkspaceItem(IDevBranch devBranch, IServiceLocator serviceLocator) 
            : base(devBranch, serviceLocator)
        {
        }
        
        public override string Title
        {
            get
            {
                return "DEV Main Branch";
            }
        }

        public override void MergeToPairEnvironmentMain(Action<IMergeBuilderViewModel> onDone)
        {
            var sideBar = ShowSideBarItem(new MergeBuilderViewModel("Merge TO QA", () => this.MainBranch.GetMergeSetsToQA(),
                                   this.ServiceLocator));

            onDone(sideBar);
        }



        IContextCommand[] _actions = null;

        public override IContextCommand[] Actions
        {
            get
            {
                if (_actions == null)
                {
                    var actions = new List<IContextCommand>(base.Actions);
                    actions.Add(new Actions.CreateFeatureBranchAction(this));
                    actions.Add(new Actions.MainMergeAction("Merge To QA", this, this.ServiceLocator.GetInstance<IApplicationServices>()));

                    _actions = actions.ToArray();
                }

                return _actions;
            }
        }

    }
}

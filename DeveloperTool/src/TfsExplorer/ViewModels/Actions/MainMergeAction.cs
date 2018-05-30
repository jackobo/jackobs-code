using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Actions
{
    public class MainMergeAction : MergeAction
    {
        public MainMergeAction(string caption, IMainBranchWorkspaceItem mainBranchWorkspaceItem, IApplicationServices appServices)
            : base(appServices)
        {
            _caption = caption;
            this.MainBranchWorkspaceItem = mainBranchWorkspaceItem;
        }

        string _caption;
        public override string Caption
        {
            get
            {
                return _caption;
            }
        }

        protected IMainBranchWorkspaceItem MainBranchWorkspaceItem { get; private set; }

        protected override void StartMerge(Action<IMergeBuilderViewModel> onDone)
        {
            MainBranchWorkspaceItem.MergeToPairEnvironmentMain(onDone);
        }
    }
}

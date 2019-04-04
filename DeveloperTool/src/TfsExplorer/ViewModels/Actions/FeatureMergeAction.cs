using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Actions
{
    public abstract class FeatureMergeAction : MergeAction
    {
        public FeatureMergeAction(IFeatureBranchWorkspaceItem featureBranchWorkspaceItem, IApplicationServices appServices)
            : base(appServices)
        {
            FeatureBranchWorkspaceItem = featureBranchWorkspaceItem;
        }


        protected IFeatureBranchWorkspaceItem FeatureBranchWorkspaceItem { get; private set; }
      
    }

    public class FeatureMergeToMainAction : FeatureMergeAction
    {
        public FeatureMergeToMainAction(IFeatureBranchWorkspaceItem featureBranchWorkspaceItem, IApplicationServices appServices) 
            : base(featureBranchWorkspaceItem, appServices)
        {
        }

        public override string Caption
        {
            get { return "Merge TO Main"; }
        }


        protected override void StartMerge(Action<IMergeBuilderViewModel> onDone)
        {
             FeatureBranchWorkspaceItem.MergeToMain(onDone);
        }
    }

    public class FeatureMergeFromMainAction : FeatureMergeAction
    {
        public FeatureMergeFromMainAction(IFeatureBranchWorkspaceItem featureBranchWorkspaceItem, IApplicationServices appServices) 
            : base(featureBranchWorkspaceItem, appServices)
        {
        }

        public override string Caption
        {
            get
            {
                return "Merge FROM Main";
            }
        }

        protected override void StartMerge(Action<IMergeBuilderViewModel> onDone)
        {
            this.FeatureBranchWorkspaceItem.MergeFromMain(onDone);
        }
    }
}

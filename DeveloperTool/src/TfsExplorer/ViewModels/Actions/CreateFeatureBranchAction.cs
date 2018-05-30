using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Actions
{
    public class CreateFeatureBranchAction : CustomContextCommand
    {
        public CreateFeatureBranchAction(IMainBranchWorkspaceItem mainBranchWorkspaceItem)
        {
            _mainBranchWorkspaceItem = mainBranchWorkspaceItem;
        }

        IMainBranchWorkspaceItem _mainBranchWorkspaceItem;

        public override string Caption
        {
            get
            {
                return "Create feature branch";
            }
        }

        Optional<IFeatureBranchBuilderViewModel> _featureBranchBuilder = Optional<IFeatureBranchBuilderViewModel>.None();
        public override void Execute(object parameter)
        {
            _featureBranchBuilder.Do(builder => builder.PropertyChanged -= Builder_PropertyChanged);
            _featureBranchBuilder = Optional<IFeatureBranchBuilderViewModel>.Some(_mainBranchWorkspaceItem.StartNewFeatureBranch());
            _featureBranchBuilder.Do(builder => builder.PropertyChanged += Builder_PropertyChanged);
            RaiseCanExecuteChanged();
        }

        private void Builder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object parameter)
        {
            return !_featureBranchBuilder.Any(builder => builder.IsActive);
        }
    }
}

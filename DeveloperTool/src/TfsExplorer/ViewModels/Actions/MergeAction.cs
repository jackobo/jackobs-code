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
    public abstract class MergeAction : CustomContextCommand
    {
        public MergeAction(IApplicationServices appServices)
        {
            _appServices = appServices;
        }
        IApplicationServices _appServices;
        Optional<IMergeBuilderViewModel> _mergeBuilder = Optional<IMergeBuilderViewModel>.None();

        public override void Execute(object parameter)
        {
            UsubscribeFromMergeBuilderPropertyChanged();
            StartMerge(mergeBuilder =>
            {
                _mergeBuilder = Optional<IMergeBuilderViewModel>.Some(mergeBuilder);
                SubscribeToMergeBuilderPropertyChanged();
                RaiseCanExecuteChanged();
                
            });
           
          
        }

        protected abstract void StartMerge(Action<IMergeBuilderViewModel> onDone);


        private void SubscribeToMergeBuilderPropertyChanged()
        {
            _mergeBuilder.Do(builder => builder.PropertyChanged += Builder_PropertyChanged);
        }

        private void UsubscribeFromMergeBuilderPropertyChanged()
        {
            _mergeBuilder.Do(builder => builder.PropertyChanged -= Builder_PropertyChanged);
        }

        private void Builder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        public override void RaiseCanExecuteChanged()
        {
            ExecuteOnUIThread(base.RaiseCanExecuteChanged);
        }

        protected void ExecuteOnUIThread(Action action)
        {
            _appServices.ExecuteOnUIThread(action);
        }

        public override bool CanExecute(object parameter)
        {
            return !_mergeBuilder.Any(builder => builder.IsActive);
        }
    }
}

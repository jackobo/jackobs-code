using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Exceptions;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public abstract class RenameDeleteComponentViewModel : SidebarItemBase, ILogicalComponentVisitor
    {
        
        public RenameDeleteComponentViewModel(IMainBranch mainBranch,
                                        ILogicalComponent component,
                                        IServiceLocator serviceLocator,
                                        Action<bool> setInProgressStatus)
            : base(serviceLocator)
        {
            MainBranch = mainBranch;
            Component = component;
            _setInProgressStatus = setInProgressStatus;

            this.OkCommand = new Command(OkAction,
                                        () => !this.IsBusy && CanDoWork(),
                                        this);
            this.CancelCommand = new Command(Deactivate, 
                                             () => !this.IsBusy, 
                                             this);

            StartBusyAction(() => ScanForSimilarComponents(mainBranch), 
                            $"Searching for components with name {Component.Name}");
        }

        protected IMainBranch MainBranch { get; private set; }
        protected ILogicalComponent Component { get; private set; }
        
        private void OkAction()
        {
            this.ExecuteOnUIThread(() => _setInProgressStatus(true));
            StartBusyAction(() =>
            {
                try
                {
                    DoWork();
                    this.ExecuteOnUIThread(this.Deactivate);
                }
                finally
                {
                    this.ExecuteOnUIThread(() => _setInProgressStatus(false));
                    
                }
            },
            GetActionDescription());

        }

        protected abstract void DoWork();
        protected abstract bool CanDoWork();
        protected abstract string GetActionDescription();

        Action<bool> _setInProgressStatus;


        private void ScanForSimilarComponents(IMainBranch mainBranch)
        {
            this.SimilarComponents = mainBranch.ScanForSimilarComponents(Component);
            _locations.Clear();
            foreach (var similarComponent in SimilarComponents)
            {
                similarComponent.AcceptCommandVisitor(() => this);
            }

            this.ExecuteOnUIThread(() => OnPropertyChanged(nameof(Locations)));
        }

        protected IEnumerable<ILogicalComponent> SimilarComponents { get; private set; } = new ILogicalComponent[0];
        List<IServerPath> _locations = new List<IServerPath>();
        public string[] Locations
        {
            get
            {
                return _locations.Select(l => l.AsString()).OrderBy(l => l).ToArray();
            }
        }


        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        void ILogicalComponentVisitor.Visit(IGameComponent game)
        {

        }

        void ILogicalComponentVisitor.Visit(IServerPath location)
        {
            _locations.Add(location);
        }

        void ILogicalComponentVisitor.Visit(IGameEngineComponent gameEngine)
        {

        }

        void ILogicalComponentVisitor.Visit(ICoreComponent coreComponent)
        {

        }
    }
}

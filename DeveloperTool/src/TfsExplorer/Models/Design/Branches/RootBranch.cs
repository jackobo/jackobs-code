using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Logging;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    public class RootBranch : IRootBranchExtended
    {
        
        public RootBranch(Folders.RootBranchFolder location, IServiceLocator serviceLocator)
        {
            if (!location.QA.Exists())
                throw new ArgumentException($"There is no QA branch in {location.Name}");
            
            _location = location;
            _serviceLocator = serviceLocator;
            _mainDevBranchBuilder = _serviceLocator.GetInstance<IMainDevBranchBuilder>();
            _productionBranch = new ProductionBranch(_location.PROD, this, _serviceLocator);
        }

        private bool _branchInProgress = false;
        public bool CanBranch
        {
            get
            {
                return !_branchInProgress &&
                       !_serviceLocator.GetInstance<IComponentsRepository>()
                                       .GetRootBranches()
                                       .Any(b => b.Version.MajorVersion > this.Version.MajorVersion);
            }
        }

        public void Branch(Action<ProgressCallbackData> progressCallback)
        {
            if (!CanBranch)
                throw new InvalidOperationException($"You can't branch from {this.Version} because it is not the latest branch!");

            _branchInProgress = true;
            try
            {
                _serviceLocator.GetInstance<IRootBranchGenerator>()
                               .CreateRootBranch(this, progressCallback);
            }
            finally
            {
                _branchInProgress = false;
            }
        }
        
        IServiceLocator _serviceLocator;
        Folders.RootBranchFolder _location;
        Folders.IRootFolder IRootBranchExtended.Location
        {
            get { return _location; }
        }

        public RootBranchVersion Version
        {
            get
            {
                return RootBranchVersion.Parse(_location.Name);
            }
        }

        IMainDevBranchBuilder _mainDevBranchBuilder;

        private ILogger Logger
        {
            get
            {
                return _serviceLocator.GetInstance<ILoggerFactory>()
                                      .CreateLogger(this.GetType());
            }
        }

        public void CreateDevelopmentBranch(Action<ProgressCallbackData> onProgress = null, Action<Exception> onError = null)
        {
            

            try
            {
                BuildDevBranch(onProgress);
            }
            catch(Exception exception)
            {
                try
                {
                    onError(exception);
                }
                catch(Exception ex)
                {
                    Logger.Exception($"{nameof(CreateDevelopmentBranch)}; LogicalBranch: {_location.Name}; onError handler failed!", ex);
                }
                throw;
            }
           
            SendCreateDevBranchNotification();

        }

        private IPubSubMediator PubSubMediator
        {
            get { return _serviceLocator.GetInstance<IPubSubMediator>(); }
        }

        private void SendCreateDevBranchNotification()
        {
            PubSubMediator.Publish(new CreateDevBranchFinishEventData(this));
        }

        public void DeleteComponents(IEnumerable<ILogicalComponent> sameComponents)
        {
            new ComponentsRenameDeleteExecutor(_serviceLocator.GetInstance<TFS.ITfsGateway>()).DeleteComponents(sameComponents, this.Version);
        }

        public void RenameComponents(IEnumerable<ILogicalComponent> sameComponents, string newName)
        {
            new ComponentsRenameDeleteExecutor(_serviceLocator.GetInstance<TFS.ITfsGateway>()).RenameComponents(sameComponents, newName);
        }

        

        private void BuildDevBranch(Action<ProgressCallbackData> onProgress = null)
        {
            GetMainDevBranchBuilder().Build(_location, 
                                            GetQaBranch().GetComponents(),
                                            onProgress);
        }


        IMainDevBranchBuilder GetMainDevBranchBuilder()
        {
            return _serviceLocator.GetInstance<IMainDevBranchBuilder>();   
        }

       

       
        public bool CanCreateDevBranch()
        {
            return _mainDevBranchBuilder.CanBuild(_location);
            
        }


        public Optional<IDevBranch> GetDevBranch()
        {
            if(_mainDevBranchBuilder.CanBuild(_location))
            {
                return Optional<IDevBranch>.None();
            }

            return Optional<IDevBranch>.Some(_serviceLocator.GetInstance<ILogicalBranchComponentFactory>().CreateDevBranch(_location.DEV, this));
        }
        
        public IQaBranch GetQaBranch()
        {
            return _serviceLocator.GetInstance<ILogicalBranchComponentFactory>().CreateQaBranch(_location.QA, this);

        }

        public override bool Equals(object obj)
        {
            var theOther = obj as RootBranch;
            if (theOther == null)
                return false;

            return this._location.Equals(theOther._location);
        }

        public override int GetHashCode()
        {
            return _location.GetHashCode();
        }

        public override string ToString()
        {
            return _location.ToString();
        }

        IProductionBranch _productionBranch;
        public IProductionBranch GetProductionBranch()
        {

            return _productionBranch;
        }

        public IEnumerable<ILogicalComponent> ScanForSimilarComponents(ILogicalComponent component)
        {
            
            var allComponents = GetQaBranch().GetComponents()
                                .Union(GetQaBranch().GetFeatureBranches().SelectMany(fb => fb.GetComponents()))
                                .Union(GetDevBranch().SelectMany(devBranch => devBranch.GetComponents()))
                                .Union(GetDevBranch().SelectMany(devBranch => devBranch.GetFeatureBranches().SelectMany(fb => fb.GetComponents())));

            var similarComponents = new List<ILogicalComponent>();

            foreach (var c in allComponents)
            {
                if (c.SameAs(component))
                    similarComponents.Add(c);
            }
            
            return similarComponents;

        }
    }
}

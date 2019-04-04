using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.Publish;
using Spark.TfsExplorer.Models.TFS;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    public abstract class LogicalComponent<TLocation> : ILogicalComponent, ISupportBranching, ISupportPublishing, IDisposable
        where TLocation : Folders.IFolderWithBranchingSupport
    {
        public LogicalComponent(TLocation location, 
                                Optional<IComponentsVersionsProvider> componentsVersionsProvider,
                                IServiceLocator serviceLocator)
        {
            this.Location = location;
            this.VersionsProvider = componentsVersionsProvider;
            this.ServiceLocator = serviceLocator;
            _relocateSubscription = serviceLocator.GetInstance<IPubSubMediator>().Subscribe<ComponentRelocatedEventData<TLocation>>(ComponentRelocatedEventHandler);
        }

        ISubscriptionToken _relocateSubscription;
        

        private void ComponentRelocatedEventHandler(ComponentRelocatedEventData<TLocation> eventData)
        {
            if (this.Location.Equals(eventData.OldLocation))
            {
                this.Location = eventData.NewLocation;
            }
        }
        
        protected IServiceLocator ServiceLocator { get; private set; }
        protected Optional<IComponentsVersionsProvider> VersionsProvider { get; private set; } = Optional<IComponentsVersionsProvider>.None();

        public abstract string Name { get; }
        protected TLocation Location { get; set; }

        public virtual void AcceptCommandVisitor(Func<ILogicalComponentVisitor> visitorFactory)
        {
            var visitor = visitorFactory();
            
            AcceptCommandVisitor(visitor);

            visitor.Visit(this.Location.GetServerPath());
        }

        public virtual bool AllowRename
        {
            get { return true; }
        }

        public virtual bool AllowDelete
        {
            get { return true; }
        }
        protected abstract void AcceptCommandVisitor(ILogicalComponentVisitor visitor);

        public virtual T AcceptQueryVisitor<T>(Func<ILogicalComponentVisitor<T>> visitorFactory)
        {
            var visitor = visitorFactory();
            AcceptCommandVisitor(visitor);
            return visitor.ProduceResult();
        }


        public virtual Optional<T> As<T>()
        {
            if (typeof(T).Equals(typeof(ISupportPublishing)) && !this.VersionsProvider.Any())
            {
                return Optional<T>.None();
            }

            if (typeof(T).IsAssignableFrom(this.GetType()))
            {           
                return Optional<T>.Some((T)(object)this);
            }

            return Optional<T>.None();
        }

        public virtual void Branch(Folders.ComponentsFolder targetComponentsFolder)
        {
            var targetFolder = GetTargetFolder(targetComponentsFolder);
            if(targetFolder.Exists())
            {
                throw new InvalidOperationException($"Target branch folder {targetFolder.Name} already exists!");
            }

            Location.Branch(targetFolder);
        }

        protected abstract IFolderWithBranchingSupport GetTargetFolder(Folders.ComponentsFolder targetComponentsFolder);

        public virtual Optional<IMergeSet> GetMergeSet(ComponentsFolder targetComponentsFolder)
        {

            try
            {
                if (!GetTargetFolder(targetComponentsFolder).Exists())
                    return Optional<IMergeSet>.Some(new NewComponentMergeSet(this, targetComponentsFolder));

                var changeSets = GetMergeChangeSets(targetComponentsFolder);

                if (changeSets.Any())
                    return Optional<IMergeSet>.Some(new ExistingComponentMergeSet(this, changeSets, targetComponentsFolder));

                return Optional<IMergeSet>.None();
            }
            catch(Exception ex)
            {
                throw new ApplicationException($"Failed to get Merge Sets for component {this.Name}; SourceBranch = {this.Location}; TargetBranch = {GetTargetFolder(targetComponentsFolder)}", ex);
            }
        }


        protected virtual IEnumerable<IMergeableChangeSet> GetMergeChangeSets(ComponentsFolder targetComponentsFolder)
        {
            return Location.GetMergeChangeSets(GetTargetFolder(targetComponentsFolder));
        }

        public virtual MergeResult Merge(ComponentsFolder targetComponentsFolder)
        {
            return Location.Merge(GetTargetFolder(targetComponentsFolder));
        }
        
        public virtual bool SameAs(ILogicalComponent component)
        {
            if (component == null)
                return false;

            return this.GetType() == component.GetType()
                    && this.Name == component.Name; 
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            var theOther = obj as LogicalComponent<TLocation>;

            return this.Location.Equals(theOther.Location);
        }

        public override int GetHashCode()
        {
            return this.Location.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public abstract IEnumerable<IComponentPublisher> GetPublishers();

        public virtual IEnumerable<IComponentPublisher> GetPublishers(IChangeSet sinceThisChangeSet)
        {
            if (this.Location.ToSourceControlFolder().QueryHistory(sinceThisChangeSet).Any())
                return GetPublishers();
            else
                return new IComponentPublisher[0];
        }

        public abstract IComponentRenameTransaction CreateRenameTransaction(string newName);
        public abstract IComponentDeleteTransaction CreateDeleteTransaction();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LogicalComponent()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.ServiceLocator.GetInstance<IPubSubMediator>().Unsubscribe< ComponentRelocatedEventData<TLocation>>(_relocateSubscription);
        }
    }
}

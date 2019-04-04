using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    public abstract class EnvironmentBranch<TLocation> : IMainBranch
        where TLocation : Folders.EnvironmentFolder
    {
        public EnvironmentBranch(TLocation location, IRootBranch owner,  IServiceLocator serviceLocator)
        {
            this.Location = location;
            this.ServiceLocator = serviceLocator;
            this.Owner = owner;
        }

        protected IRootBranch Owner { get; private set; }
        protected TLocation Location { get; private set; }
        protected IServiceLocator ServiceLocator { get; private set; }


        protected abstract IComponentsReader ComponentsReader { get; }
        
        
        IPubSubMediator PubSubMediator
        {
            get { return ServiceLocator.GetInstance<IPubSubMediator>(); }
        }

        public void CreateFeatureBranch(string name, IEnumerable<ILogicalComponent> components, Action<ProgressCallbackData> progressCallback)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "You must provide a name for the feature branch");

            if (components == null)
                throw new ArgumentNullException(nameof(components), "A feature branch must have at least one component");

            if (!components.Any())
                throw new ArgumentException(nameof(components), "A feature branch must have at least one component");

            Folders.FeatureFolder featureFolder = Location.Features.Feature(name).Create();

            new BranchBuilder(featureFolder, components, GetMainFolder(), progressCallback).Build();

            var featureBranch = ServiceLocator.GetInstance<ILogicalBranchComponentFactory>().CreateFeatureBranch(featureFolder, this);

            PubSubMediator.Publish(new Interfaces.Events.NewFeatureBranchEventData(featureBranch, this));
        }

        public IEnumerable<ILogicalComponent> GetComponents()
        {
            return ComponentsReader.ReadComponents(GetMainFolder().Components);
        }

        protected abstract Folders.IBranchFolder GetMainFolder();

        public IEnumerable<IFeatureBranch> GetFeatureBranches()
        {
            return ServiceLocator.GetInstance<IFeaturesBranchesReader>().GetFeatureBranches(Location, this);
        }

        public IEnumerable<ILogicalComponent> ScanForSimilarComponents(ILogicalComponent component)
        {
            return this.Owner.ScanForSimilarComponents(component);
        }
        public void RenameComponents(IEnumerable<ILogicalComponent> sameComponents, string newName)
        {
            this.Owner.RenameComponents(sameComponents, newName);
        }
        public void DeleteComponents(IEnumerable<ILogicalComponent> sameComponents)
        {
            this.Owner.DeleteComponents(sameComponents);
        }

    }
}

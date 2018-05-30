using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    public class FeatureBranch : IFeatureBranch
    {
        public FeatureBranch(Folders.FeatureFolder folder, IMainBranch owner, IServiceLocator serviceLocator)
        {
            _featureFolder = folder;
            this.Owner = owner;
            _serviceLocator = serviceLocator;
        }

        Folders.FeatureFolder _featureFolder;
        IComponentsReader ComponentsReader
        {
            get
            {
                return _serviceLocator.GetInstance<IComponentsReaderFactory>().FeatureBranchComponentsReader();
            }
        }
        
        
        IServiceLocator _serviceLocator;

        public string Name
        {
            get { return _featureFolder.Name; }
        }

        IMainBranch Owner { get; set; }

        public IEnumerable<ILogicalComponent> GetMissingComponentsFromMain()
        {
            var featureComponents = GetComponents();

            return Owner.GetComponents().Where(mainComponent => !featureComponents.Any(c => c.SameAs(mainComponent)))
                                            .ToArray();

        }

        public IEnumerable<ILogicalComponent> GetComponents()
        {
            return ComponentsReader.ReadComponents(_featureFolder.Components);
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as FeatureBranch;

            if (theOther == null)
                return false;

            return this._featureFolder.Equals(theOther._featureFolder);
        }

        public override int GetHashCode()
        {
            return _featureFolder.GetHashCode();
        }

        public override string ToString()
        {
            return _featureFolder.ToString();
        }

        public IEnumerable<IMergeSet> GetMergeSetsToMain()
        {
            return _serviceLocator.GetInstance<IMergeSetsReader>().ReadMergeSets(GetComponents(),
                                                                          _featureFolder.Parent.Parent.GetMain().Components);
            
        }

        public IEnumerable<IMergeSet> GetMergeSetsFromMain()
        {
            return _serviceLocator.GetInstance<IMergeSetsReader>().ReadMergeSets(GetMatchingMainComponents(),
                                                                                _featureFolder.Components);
        }

        private ILogicalComponent[] GetMatchingMainComponents()
        {
            var featureComponents = GetComponents();
            var matchingComponentsWithTheMain = Owner.GetComponents().Where(mainComponent => featureComponents.Any(c => c.SameAs(mainComponent)))
                                                                     .ToArray();
            return matchingComponentsWithTheMain;
        }

        public void AddMissingComponents(ILogicalComponent[] components, Action<ProgressCallbackData> progressCallBack)
        {
            
            for(int i =0; i <components.Length; i++)
            {
                var component = components[i];

                progressCallBack?.Invoke(ProgressCallbackData.Create(i,
                                                                    components.Length,
                                                                    "Branching " + component.Name));
                
                component.As<ISupportBranching>().Do(c =>
                {

                    c.Branch(_featureFolder.Components);
                });

                progressCallBack?.Invoke(ProgressCallbackData.Create(i + 1,
                                                                   components.Length,
                                                                   "Branching " + component.Name));
            }

            _serviceLocator.GetInstance<IPubSubMediator>().Publish(new FeatureBranchUpdateEventData(this));
        }

       
    }
}

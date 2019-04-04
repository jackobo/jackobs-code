using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;


namespace Spark.TfsExplorer.Models.Design
{
    internal class NewComponentMergeSet : IMergeSet
    {
        public NewComponentMergeSet(ILogicalComponent sourceComponent, Folders.ComponentsFolder targetComponentsFolder)
        {
            this.SourceComponent = sourceComponent;
            _targetComponentsFolder = targetComponentsFolder;
        }

        public bool IsNew
        {
            get { return true; }
        }

        Folders.ComponentsFolder _targetComponentsFolder;

        public ILogicalComponent SourceComponent { get; private set; }

        public IEnumerable<IMergeableChangeSet> ChangeSets
        {
            get { return new IMergeableChangeSet[0]; }
        }

        public MergeResult Merge()
        {
            this.SourceComponent.As<ISupportBranching>().Do(c => c.Branch(_targetComponentsFolder));
            return MergeResult.Empty();
        }
    }

    internal class ExistingComponentMergeSet : IMergeSet
    {
        public ExistingComponentMergeSet(ILogicalComponent sourceComponent, 
                                         IEnumerable<IMergeableChangeSet> changeSets, 
                                         Folders.ComponentsFolder targetComponentsFolder)
        {
            this.SourceComponent = sourceComponent;
            this.ChangeSets = changeSets;
            _targetComponentsFolder = targetComponentsFolder;
        }

        public bool IsNew
        {
            get { return false; }
        }
            
        Folders.ComponentsFolder _targetComponentsFolder;
        public ILogicalComponent SourceComponent { get; private set; }

        public IEnumerable<IMergeableChangeSet> ChangeSets
        {
            get; private set;
        }

        public MergeResult Merge()
        {
            var mergeResult = MergeResult.Empty();

            this.SourceComponent.As<ISupportBranching>().Do(c => mergeResult = c.Merge(_targetComponentsFolder));

            return mergeResult;
        }
    }
}

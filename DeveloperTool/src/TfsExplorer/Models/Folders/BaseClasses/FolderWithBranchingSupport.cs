using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    public abstract class ChildFolderWithBranchingSupport<TConcreteFolder, TParent> : ChildFolderHolder<TConcreteFolder, TParent>, IFolderWithBranchingSupport
        where TConcreteFolder : IFolderWithBranchingSupport
        where TParent : IFolderHolder
       
    {
        public ChildFolderWithBranchingSupport(string name, TParent parent) : base(name, parent)
        {
        }

        public void Branch(IFolderWithBranchingSupport targetFolder)
        {

            if (!this.Exists())
                throw new InvalidOperationException($"You cannnot branch a folder that doesn't exists: '{this.GetServerPath().AsString()}'");

            ToSourceControlFolder().Branch(targetFolder.GetServerPath());
        }

        public IEnumerable<IMergeableChangeSet> GetMergeChangeSets(IFolderWithBranchingSupport targetFolder)
        {
            if (!this.Exists())
                throw new InvalidOperationException($"You cannnot merge from a folder that doesn't exists: '{this.GetServerPath().AsString()}'");

            return ToSourceControlFolder().GetMergeChangeSets(targetFolder.GetServerPath());
        }


        public MergeResult Merge(IFolderWithBranchingSupport targetFolder)
        {
            if (!this.Exists())
                throw new InvalidOperationException($"You cannnot merge from a folder that doesn't exists: '{this.GetServerPath().AsString()}'");


            return ToSourceControlFolder().Merge(targetFolder.GetServerPath());


        }

    }
}

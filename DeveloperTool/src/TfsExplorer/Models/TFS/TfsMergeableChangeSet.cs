using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.TFS
{
    internal class TfsChangeSet : IChangeSet
    {
        protected Changeset _changeSet;
        public TfsChangeSet(Changeset changeSet)
        {
            _changeSet = changeSet;
        }

        public int Id
        {
            get { return _changeSet.ChangesetId; }
        }

        public string Comments
        {
            get { return _changeSet.Comment; }
        }


        public string CommiterDisplayName
        {
            get
            {
                return _changeSet.OwnerDisplayName;
            }
        }

        public string CommiterUserName
        {
            get
            {
                return _changeSet.Committer;
            }
        }

        public DateTime Date
        {
            get { return _changeSet.CreationDate; }
        }

        internal Changeset GetRawChangeset()
        {
            return _changeSet;
        }
    }

    internal class TfsMergeableChangeSet : TfsChangeSet, IMergeableChangeSet
    {
        
        public TfsMergeableChangeSet(Changeset changeSet, TfsFolder sourceFolder, IServerPath targetForMerge)
            : base(changeSet)
        {
            
            _sourceFolder = sourceFolder;
            _targetForMerge = targetForMerge;
        }

        TfsFolder _sourceFolder;
        IServerPath _targetForMerge;
        

        public MergeResult Merge()
        {
            return _sourceFolder.MergeChangeSet(_changeSet.ChangesetId, _targetForMerge);
        }
    }
}

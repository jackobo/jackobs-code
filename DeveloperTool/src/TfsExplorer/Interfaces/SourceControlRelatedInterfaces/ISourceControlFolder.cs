using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Interfaces
{
    public interface ISourceControlFolder
    {
        string Name { get; }
        Optional<ISourceControlFolder> TryGetSubfolder(string name);
        ISourceControlFolder GetSubfolder(string name);
        IEnumerable<ISourceControlFolder> GetSubfolders();
        ISourceControlFolder CreateSubfolder(string name);
        void Branch(IServerPath targetFolder);
        IEnumerable<IMergeableChangeSet> GetMergeChangeSets(IServerPath targetFolder);
        MergeResult Merge(IServerPath targetFolder);
        Optional<ISourceControlFile> TryGetFile(string fileName);
        IEnumerable<IChangeSet> QueryHistory(IChangeSet sinceThisChangeSet);
        IChangeSet GetLatestChangeSet();
        ISourceControlFile CreateFile(string fileName, byte[] content);
        ILocalPath GetLocalPath();
        void GetLatest();
    }

    public interface ISourceControlFile
    {
        void Branch(IServerPath targetFolder);
        IChangeSet GetLatestChangeSet();
        void UpdateContent(byte[] content);
        byte[] GetContent();
        ILocalPath GetLocalPath();
    }

    public class MergeResult
    {
        public MergeResult(int numberOfConflicts)
        {
            this.NumberOfConflicts = numberOfConflicts;
        }
        public int NumberOfConflicts { get; private set; }

        public MergeResult Combine(MergeResult other)
        {
            return new MergeResult(this.NumberOfConflicts + other.NumberOfConflicts);
        }

        public static MergeResult Empty()
        {
            return new MergeResult(0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Interfaces
{
    public interface IRootBranch
    {
        RootBranchVersion Version { get; }
        bool CanBranch { get; }
        void Branch(Action<ProgressCallbackData> progressCallback);
        IQaBranch GetQaBranch();
        bool CanCreateDevBranch();
        Optional<IDevBranch> GetDevBranch();
        void CreateDevelopmentBranch(Action<ProgressCallbackData> onProgress = null, Action<Exception> onError = null);
        IProductionBranch GetProductionBranch();
        IEnumerable<ILogicalComponent> ScanForSimilarComponents(ILogicalComponent component);
        void RenameComponents(IEnumerable<ILogicalComponent> sameComponents, string newName);
        void DeleteComponents(IEnumerable<ILogicalComponent> sameComponents);
    }

    public sealed class RootBranchVersion : IComparable<RootBranchVersion>, IComparable
    {
        public RootBranchVersion(int majorVersion)
        {
            this.MajorVersion = majorVersion;
        }

        public static RootBranchVersion Parse(string v)
        {
            var version = VersionNumber.Parse(v.Replace("x", "0").Replace("X", "0"));
            return new RootBranchVersion(version.Major);
        }

        public static Optional<RootBranchVersion> TryParse(string v)
        {
            try
            {
                return Optional<RootBranchVersion>.Some(Parse(v));
            }
            catch
            {
                return Optional<RootBranchVersion>.None();
            }
        }

        public VersionNumber GetFirstVersion()
        {
            return new VersionNumber(this.MajorVersion, 0, 0, 0);
        }

        public static RootBranchVersion operator +(RootBranchVersion r, int increment)
        {
            if (r == null)
                throw new ArgumentNullException();

            return new RootBranchVersion(r.MajorVersion + increment);
        }

        public static RootBranchVersion operator ++(RootBranchVersion r)
        {
            if (r == null)
                throw new ArgumentNullException();

            return r + 1;
        }

       

        public static RootBranchVersion operator -(RootBranchVersion r, int decrement)
        {
            if (r == null)
                throw new ArgumentNullException();

            return new RootBranchVersion(r.MajorVersion - decrement);
        }

        public static RootBranchVersion operator --(RootBranchVersion r)
        {
            if (r == null)
                throw new ArgumentNullException();

            return r - 1;
        }

        public int MajorVersion { get; private set; }

        public int CompareTo(RootBranchVersion other)
        {
            if (other == null)
                return 1;

            return this.MajorVersion.CompareTo(other.MajorVersion);
        }


        int IComparable.CompareTo(object obj)
        {
            return this.CompareTo(obj as RootBranchVersion);
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as RootBranchVersion;

            if (theOther == null)
                return false;

            return this.MajorVersion == theOther.MajorVersion;
        }

        public override int GetHashCode()
        {
            return this.MajorVersion.GetHashCode();
        }

        public override string ToString()
        {
            return this.MajorVersion + ".x";
        }
    }
}

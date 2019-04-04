using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public abstract class MainFolder<TParentFolder> : BranchFolder<MainFolder<TParentFolder>, TParentFolder>
        where TParentFolder : EnvironmentFolder
    {
        public MainFolder(TParentFolder parent)
            : base("Main", parent)
        {
        }
    }

    public class QAMainFolder : MainFolder<QAFolder>
    {
        public QAMainFolder(QAFolder parent) : base(parent)
        {
        }

        public PublishHistoryFolder PublishHistory
        {
            get { return new PublishHistoryFolder(this); }
        }


    }

    public class DevMainFolder : MainFolder<DevFolder>
    {
        public DevMainFolder(DevFolder parent) : base(parent)
        {
        }
    }
}

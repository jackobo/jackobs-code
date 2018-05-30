using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public interface IInstallerFolder : IBranchFolder
    {
        TriggerFolder Trigger { get; }
        PublishHistoryFolder PublishHistory { get; }
    }
}

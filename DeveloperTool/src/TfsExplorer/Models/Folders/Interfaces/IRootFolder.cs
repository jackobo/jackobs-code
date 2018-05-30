using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    public interface IRootFolder : IFolderHolder
    {
        QAFolder QA { get; }
        DevFolder DEV { get; }

        ProdFolder PROD { get; }
    }
}

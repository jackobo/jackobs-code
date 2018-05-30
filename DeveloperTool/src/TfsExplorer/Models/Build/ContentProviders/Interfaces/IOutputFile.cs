using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IOutputFile
    {
        ILocalPath ResolveBuildOutputPath(ILocalPath basePath);
        ILocalPath ResolveDistributionPath(ILocalPath basePath);

        DeployableFileDefinition GetDeployableFileDefinition();
    }
}

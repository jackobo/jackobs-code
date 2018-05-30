using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IComponentBuilder
    {
        void AppendContent(IBuildContext builderContext);
        IEnumerable<IBuildAction> GetPreCompileActions();
        IEnumerable<IBuildAction> GetPostCompileActions();
        IEnumerable<IBuildAction> GetDeployActions();
        string GetComponentDescriptionTxtContent();
        ILocalPath ResolveDistributionLocalPath(ILocalPath basePath);
    }

   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IComponentsBuildersReader
    {
        IEnumerable<IComponentBuilder> GetBuilders(IPublishPayload publishPayload, Folders.ComponentsFolder componentsFolder);
    }
}

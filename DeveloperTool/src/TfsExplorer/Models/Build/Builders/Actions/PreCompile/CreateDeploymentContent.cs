using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public class CreateDeploymentContent : IBuildAction
    {
        public CreateDeploymentContent(IEnumerable<IComponentBuilder> components)
        {
            _components = components;
        }

        IEnumerable<IComponentBuilder> _components;
        public void Execute(IBuildContext buildContext)
        {
            foreach (var c in _components)
            {
                c.AppendContent(buildContext);
            }
        }
    }
}

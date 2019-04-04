using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.ViewModels
{
    public class ComponentsHierarchy
    {
      
        public ComponentsHierarchy(IEnumerable<ICoreComponentViewModel> coreComponents, IEnumerable<IGameEngineViewModel> gameEngines)
        {
            CoreComponents = coreComponents.ToArray();
            GameEngines = gameEngines.ToArray();
        }

        public ICoreComponentViewModel[] CoreComponents { get; private set; }
        public IGameEngineViewModel[] GameEngines { get; private set; }

       
    }
}

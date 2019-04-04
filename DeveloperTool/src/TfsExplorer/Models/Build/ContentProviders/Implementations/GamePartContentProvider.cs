using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public abstract class GamePartContentProvider<TLocation> 
        where TLocation : Folders.IFolderHolder
    {
        public GamePartContentProvider(TLocation location, IComponentUniqueIdBuilder componentUniqueIdBuilder, IComponentUniqueIdBuilder engineUniqueIdBuilder)
        {
            this.Location = location;
            this.ComponentUniqueIdBuilder = componentUniqueIdBuilder;
            this.EngineUniqueIdBuilder = engineUniqueIdBuilder;
        }

        public TLocation Location { get; private set; }
        public IComponentUniqueIdBuilder ComponentUniqueIdBuilder { get; private set; }
        public IComponentUniqueIdBuilder EngineUniqueIdBuilder { get; private set; }

        public IServerPath GetProjectPath()
        {
            return this.Location.GetServerPath();
        }
    }
}

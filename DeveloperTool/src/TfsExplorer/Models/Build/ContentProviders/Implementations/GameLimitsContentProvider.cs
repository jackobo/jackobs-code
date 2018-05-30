using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class GameLimitsContentProvider : GamePartContentProvider<GameLimitsFolder>,  IGameLimitsContentProvider
    {
        public GameLimitsContentProvider(GameLimitsFolder limitsFolder, IComponentUniqueIdBuilder engineUniqueIdBuilder)
            : base(limitsFolder, new ComponentUniqueIdBuilder(limitsFolder.ComponentUniqueIdTxt), engineUniqueIdBuilder)
        {
           
        }

        

        public string Name
        {
            get
            {
                return this.Location.Parent.Name;
            }
        }
      

        public IEnumerable<IGameOutputFile> LimitsFiles
        {
            get
            {
                return Directory.EnumerateFiles(Location.ToSourceControlFolder().GetLocalPath().AsString(), "*.limits")
                                .Select(f => new GameLimitsOutputFile(Location.Parent.Name, Location.Parent.Parent.Parent, new LocalPath(f)))
                                .ToArray();
            }
        }

        IEnumerable<IOutputFile> IComponentContentProvider.OutputFiles
        {
            get
            {
                return LimitsFiles;
            }
        }

      
    }
}

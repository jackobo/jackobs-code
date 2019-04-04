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
    public class GameMathContentProvider : GamePartContentProvider<GameMathFolder>, IGameMathContentProvider
    {
        public GameMathContentProvider(GameMathFolder mathFolder, IComponentUniqueIdBuilder engineUniqueIdBuilder)
            : base(mathFolder, new ComponentUniqueIdBuilder(mathFolder.ComponentUniqueIdTxt), engineUniqueIdBuilder)
        {
            
        }
        
        public string Name
        {
            get
            {
                return Location.Parent.Name;
            }
        }

        

        public IEnumerable<IGameOutputFile> MathFiles
        {
            get
            {
                return Directory.EnumerateFiles(Location.ToSourceControlFolder().GetLocalPath().AsString(), "*.xml")
                                .Select(f => new GameMathOutputFile(Location.Parent.Name, Location.Parent.Parent.Parent, new LocalPath(f)))
                                .ToArray();
            }
        }
        
        IEnumerable<IOutputFile> IComponentContentProvider.OutputFiles
        {
            get
            {
                return MathFiles;
            }
        }
    }
}

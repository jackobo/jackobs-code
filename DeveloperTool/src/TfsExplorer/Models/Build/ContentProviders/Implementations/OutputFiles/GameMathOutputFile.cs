using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class GameMathOutputFile : GameOutputFile
    {
        public GameMathOutputFile(string gameName, GameEngineFolder engineFolder, ILocalPath sourceFile)
            : base(gameName, engineFolder, sourceFile)
        {
        }

      
    }
}

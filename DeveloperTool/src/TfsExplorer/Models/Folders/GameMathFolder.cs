using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class GameMathFolder : ChildFolderWithBranchingSupport<GameMathFolder, GameFolder>
    {
        public GameMathFolder(GameFolder parent) 
            : base(WellKnownName, parent)
        {
        }

        public static readonly string WellKnownName = "Math";

        public ComponentUniqueIdTxt<GameMathFolder> ComponentUniqueIdTxt
        {
            get { return new ComponentUniqueIdTxt<GameMathFolder>(this); }
        }
    }
}

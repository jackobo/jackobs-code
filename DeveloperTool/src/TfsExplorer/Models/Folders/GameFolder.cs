using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    public class GameFolder : ChildFolderWithBranchingSupport<GameFolder, GamesFolder>
    {
        public GameFolder(string name, GamesFolder parent)
            : base(name, parent)
        {
        }

        public GameMathFolder Math
        {
            get { return new GameMathFolder(this); }
        }

        public GameLimitsFolder Limits
        {
            get { return new GameLimitsFolder(this); }
        }

    }
}

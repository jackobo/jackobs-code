using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class GameLimitsFolder : ChildFolderWithBranchingSupport<GameLimitsFolder, GameFolder>
    {
        public GameLimitsFolder(GameFolder parent) 
            : base(WellKnownName, parent)
        {
        }

        public static readonly string WellKnownName = "Limits";

        public ComponentUniqueIdTxt<GameLimitsFolder> ComponentUniqueIdTxt
        {
            get { return new ComponentUniqueIdTxt<GameLimitsFolder>(this); }
        }
    }
}

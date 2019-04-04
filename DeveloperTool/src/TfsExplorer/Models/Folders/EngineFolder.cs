using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    public class EngineFolder : ChildFolderWithBranchingSupport<EngineFolder, GameEngineFolder>
    {
        public EngineFolder(GameEngineFolder parent)
            : base(WellKnownName, parent)
        {
        }

        public static readonly string WellKnownName = "Engine";

        public ComponentUniqueIdTxt<EngineFolder> ComponentUniqueIdTxt
        {
            get { return new ComponentUniqueIdTxt<EngineFolder>(this); }
        }
    }
}

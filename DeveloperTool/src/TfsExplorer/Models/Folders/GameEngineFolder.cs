using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class GameEngineFolder : ChildFolderHolder<GameEngineFolder, EnginesAndGamesFolder>
    {
        public GameEngineFolder(string name, EnginesAndGamesFolder parent)
            : base(name, parent)
        {
        }

        public EngineFolder Engine
        {
            get { return new EngineFolder(this); }
        }

        public GamesFolder Games
        {
            get { return new GamesFolder(this); }
        }
    }
}

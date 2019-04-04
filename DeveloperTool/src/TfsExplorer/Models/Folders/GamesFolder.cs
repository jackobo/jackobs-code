using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class GamesFolder : ChildFolderHolder<GamesFolder, GameEngineFolder>
    {
        public GamesFolder(GameEngineFolder parent)
            : base(WellKnownName, parent)
        {
        }

        public GameFolder Game(string name)
        {
            return new GameFolder(name, this);
        }

        public IEnumerable<GameFolder> AllGames
        {
            get
            {
                if (!Exists())
                    return new GameFolder[0];

                return ToSourceControlFolder().GetSubfolders().Select(f => Game(f.Name));
            }
        }

        public static readonly string WellKnownName = "Games";
    }
}

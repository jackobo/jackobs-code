using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class EnginesAndGamesFolder : ChildFolderHolder<EnginesAndGamesFolder, ComponentsFolder>
    {
        public EnginesAndGamesFolder(ComponentsFolder components)
            : base(WellKnownName, components)
        {

        }

        public GameEngineFolder GameEngine(string name)
        {
            return new GameEngineFolder(name, this);
        }

        public IEnumerable<GameEngineFolder> AllGameEngines
        {
            get
            {
                if (!Exists())
                    return new GameEngineFolder[0];

                return ToSourceControlFolder().GetSubfolders().Select(f => GameEngine(f.Name)).ToList();
            }

        }

        public static readonly string WellKnownName = "GameEngines";
    }
}

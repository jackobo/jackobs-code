using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class GameContentProvider : IGameContentProvider
    {
        public GameContentProvider(GameFolder location, IComponentUniqueIdBuilder engineUniqueIdBuilder)
        {
            this.Location = location;

            this.MathContent = new GameMathContentProvider(location.Math, engineUniqueIdBuilder);
            
            this.LimitsContent = new GameLimitsContentProvider(location.Limits, engineUniqueIdBuilder);
        }


        GameFolder Location { get; set; }

        public string Name
        {
            get { return Location.Name; }
        }

        public IGameMathContentProvider MathContent { get; }
        public IGameLimitsContentProvider LimitsContent { get; }
    }
}

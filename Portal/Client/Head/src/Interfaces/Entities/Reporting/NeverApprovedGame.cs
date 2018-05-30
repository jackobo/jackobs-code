using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace GamesPortal.Client.Interfaces.Entities
{
    public class NeverApprovedGame
    {
        public NeverApprovedGame()
        {
        }

        public NeverApprovedGame(string gameName, int mainGameType, GameInfrastructure gameInfrastructure, VersionNumber latestVersion)
        {
            this.GameName = gameName;
            this.MainGameType = mainGameType;
            this.GameInfrastructure = gameInfrastructure;
            this.LatestVersion = latestVersion;
        }
                
        public string GameName { get; set; }
        
        public int MainGameType { get; set; }
        
        public GameInfrastructure GameInfrastructure { get; set; }
        
        public VersionNumber LatestVersion { get; set; }
    }

}

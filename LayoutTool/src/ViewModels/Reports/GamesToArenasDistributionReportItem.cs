using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels.Reports
{
    public class GamesToArenasDistributionReportItem
    {
        public GamesToArenasDistributionReportItem(string gameGroup, int gameType, string gameName, bool isNew, bool isVip, 
                                                   string alsoPlaying, bool isInTopGames, bool isInVipTopGames, bool isInLobby,
                                                   int arenaType, string arenaName, string arenaLayoutName)
        {
            this.GameGroup = string.IsNullOrEmpty(gameGroup) ? "?" : gameGroup;
            this.GameType = gameType;
            this.GameName = gameName;
            this.IsNew = isNew ? "Yes" : "";
            this.IsVip = isVip ? "Yes" : "";
            this.AlsoPlaying = alsoPlaying;
            this.IsInLobby = isInLobby ? "Yes" : "";
            this.IsInTopGames = isInTopGames ? "Yes" : "";
            this.IsInVipTopGames = isInVipTopGames ? "Yes" : "";
            this.ArenaType = arenaType;
            this.ArenaName = arenaName;
            this.ArenaLayoutName = arenaLayoutName;
            
        }

        public string GameGroup { get; private set; }

        public int GameType { get; private set; }
        public string GameName { get; set; }
        public string IsNew { get; set; }
        public string IsVip { get; set; }
        public int ArenaType { get; set; }
        public string ArenaName { get; set; }
        public string ArenaLayoutName { get; set; }
        public string AlsoPlaying{ get; set; }

        public string IsInTopGames { get; set; }
        public string IsInVipTopGames { get; set; }
        public string IsInLobby{ get; set; }
    }
}

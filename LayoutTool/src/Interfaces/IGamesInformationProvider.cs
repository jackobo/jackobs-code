using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public interface IGamesInformationProvider
    {
        GameInfo[] GetGamesInfo(int brandId);
    }
    

    public class GameInfo
    {
      
        public GameInfo(int gameType, string name, string gameGroup, bool? isApproved, string vendorName, int[] jackpotIds)
        {
            this.GameType = gameType;
            this.Name = name;
            this.GameGroup = gameGroup;
            this.IsApproved = isApproved;
            this.VendorName = vendorName;
            this.JackpotIds = jackpotIds;
        }

        
        public int GameType { get; private set; }

        
        public string Name { get; private set; }

        public string GameGroup { get; private set; }
        public bool? IsApproved { get; private set; }
        public string VendorName { get; private set; }

        public int[] JackpotIds { get; private set; }
    }
}
